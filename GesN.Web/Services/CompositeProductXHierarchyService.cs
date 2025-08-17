using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Production;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    /// <summary>
    /// Serviço para gerenciar relações CompositeProductXHierarchy
    /// </summary>
    public class CompositeProductXHierarchyService : ICompositeProductXHierarchyService
    {
        private readonly ICompositeProductXHierarchyRepository _relationRepository;
        private readonly IProductComponentHierarchyRepository _hierarchyRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CompositeProductXHierarchyService> _logger;

        public CompositeProductXHierarchyService(
            ICompositeProductXHierarchyRepository relationRepository,
            IProductComponentHierarchyRepository hierarchyRepository,
            IProductRepository productRepository,
            ILogger<CompositeProductXHierarchyService> logger)
        {
            _relationRepository = relationRepository;
            _hierarchyRepository = hierarchyRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        #region CRUD Operations

        public async Task<int> CreateRelationAsync(CreateCompositeProductXHierarchyViewModel viewModel, string userId)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(viewModel.ProductComponentHierarchyId))
                    throw new ArgumentException("Hierarquia de componentes é obrigatória");

                if (string.IsNullOrEmpty(viewModel.ProductId))
                    throw new ArgumentException("Produto é obrigatório");

                // Verificar se já existe uma relação entre o produto e a hierarquia
                var existingRelation = await _relationRepository.RelationExistsAsync(
                    viewModel.ProductId, viewModel.ProductComponentHierarchyId);

                if (existingRelation)
                    throw new InvalidOperationException("Já existe uma relação entre este produto e hierarquia");

                // Verificar se a ordem de montagem já está sendo usada
                var orderExists = await _relationRepository.AssemblyOrderExistsAsync(
                    viewModel.ProductId, viewModel.AssemblyOrder);

                if (orderExists)
                    throw new InvalidOperationException($"A ordem de montagem {viewModel.AssemblyOrder} já está sendo usada neste produto");

                // Converter ViewModel para Entity
                var relation = viewModel.ToEntity();

                var relationId = await _relationRepository.CreateAsync(relation);

                return relationId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar relação: {ex.Message}", ex);
            }
        }

        public async Task<CompositeProductXHierarchyDetailsViewModel?> GetRelationByIdAsync(int id)
        {
            try
            {
                var relation = await _relationRepository.GetByIdAsync(id);
                if (relation == null) return null;

                return new CompositeProductXHierarchyDetailsViewModel
                {
                    Id = relation.Id,
                    ProductComponentHierarchyId = relation.ProductComponentHierarchyId,
                    ProductId = relation.ProductId,
                    HierarchyName = relation.ProductComponentHierarchy?.Name ?? "",
                    ProductName = relation.Product?.Name ?? "",
                    MinQuantity = relation.MinQuantity,
                    MaxQuantity = relation.MaxQuantity,
                    IsOptional = relation.IsOptional,
                    AssemblyOrder = relation.AssemblyOrder,
                    Notes = relation.Notes
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar relação por ID: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateRelationAsync(EditCompositeProductXHierarchyViewModel viewModel, string userId)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(viewModel.ProductComponentHierarchyId))
                    throw new ArgumentException("Hierarquia de componentes é obrigatória");

                if (string.IsNullOrEmpty(viewModel.ProductId))
                    throw new ArgumentException("Produto é obrigatório");

                // Verificar se a relação existe
                var existingRelation = await _relationRepository.GetByIdAsync(viewModel.Id);
                if (existingRelation == null)
                    throw new InvalidOperationException("Relação não encontrada");

                // Verificar se já existe outra relação entre o produto e a hierarquia (excluindo a atual)
                var existingProductHierarchyRelation = await _relationRepository.GetByProductAndHierarchyAsync(
                    viewModel.ProductId, viewModel.ProductComponentHierarchyId);

                if (existingProductHierarchyRelation != null && existingProductHierarchyRelation.Id != viewModel.Id)
                    throw new InvalidOperationException("Já existe outra relação entre este produto e hierarquia");

                // Verificar se a ordem de montagem já está sendo usada por outra relação
                var orderExists = await _relationRepository.AssemblyOrderExistsAsync(
                    viewModel.ProductId, viewModel.AssemblyOrder, viewModel.Id);

                if (orderExists)
                    throw new InvalidOperationException($"A ordem de montagem {viewModel.AssemblyOrder} já está sendo usada por outra relação neste produto");

                // Converter ViewModel para Entity
                var relation = viewModel.ToEntity();

                var success = await _relationRepository.UpdateAsync(relation);

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar relação: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteRelationAsync(int id, string userId)
        {
            try
            {
                var relation = await _relationRepository.GetByIdAsync(id);
                if (relation == null)
                    throw new InvalidOperationException("Relação não encontrada");

                return await _relationRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir relação: {ex.Message}", ex);
            }
        }

        public async Task<bool> RelationExistsAsync(string id)
        {
            if (!int.TryParse(id, out int relationId))
                return false;
                
            return await _relationRepository.ExistsAsync(relationId);
        }

        public async Task<int> GetRelationsCountByProductIdAsync(string productId)
        {
            try
            {
                var relations = await _relationRepository.GetActiveByProductIdAsync(productId);
                return relations.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar relações do produto {ProductId}", productId);
                return 0; // Retorna 0 em caso de erro para não quebrar a aplicação
            }
        }

        #endregion

        #region Business Logic

        public async Task<int> AssociateHierarchyToProductAsync(string hierarchyId, string productId, CreateCompositeProductXHierarchyViewModel config, string userId)
        {
            config.ProductComponentHierarchyId = hierarchyId;
            config.ProductId = productId;

            return await CreateRelationAsync(config, userId);
        }

        public async Task<bool> DisassociateHierarchyFromProductAsync(string hierarchyId, string productId, string userId)
        {
            try
            {
                var relation = await _relationRepository.GetByProductAndHierarchyAsync(productId, hierarchyId);
                if (relation == null)
                {
                    return false;
                }

                return await DeleteRelationAsync(relation.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desassociar hierarquia {HierarchyId} do produto {ProductId}", hierarchyId, productId);
                throw;
            }
        }

        public async Task<bool> ReorderProductHierarchiesAsync(string productId, Dictionary<string, int> newOrders, string userId)
        {
            try
            {
                // Converter as chaves string para int
                var intOrders = new Dictionary<int, int>();
                foreach (var kvp in newOrders)
                {
                    if (int.TryParse(kvp.Key, out int relationId))
                    {
                        intOrders[relationId] = kvp.Value;
                    }
                }
                
                var success = await _relationRepository.ReorderRelationsAsync(productId, intOrders);

                if (success)
                {
                    await NotifyProductHierarchiesChangedAsync(productId);
                    _logger.LogInformation("Hierarquias do produto {ProductId} reordenadas por {UserId}", productId, userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reordenar hierarquias do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> ToggleActiveStatusAsync(string relationId, string userId)
        {
            try
            {
                if (!int.TryParse(relationId, out int relationIdInt))
                    return false;
                    
                var relation = await _relationRepository.GetByIdAsync(relationIdInt);
                if (relation == null)
                {
                    return false;
                }

                // CompositeProductXHierarchy não possui mais funcionalidade de ativo/inativo
                // Mantemos o método por compatibilidade, mas sempre consideramos como "ativo"
                _logger.LogInformation("Status da relação {RelationId} mantido como ativo (sem alteração) por {UserId}", 
                    relationId, userId);

                return true; // Sempre retorna sucesso pois não há operação real
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status da relação {RelationId}", relationId);
                throw;
            }
        }

        public async Task<bool> DuplicateProductHierarchyConfigAsync(string sourceProductId, string targetProductId, string userId)
        {
            try
            {
                var sourceRelations = await _relationRepository.GetByProductIdAsync(sourceProductId, includeInactive: false);
                if (!sourceRelations.Any())
                {
                    return true; // Nada para duplicar
                }

                var targetRelations = sourceRelations.Select(r => new CompositeProductXHierarchy
                {
                    ProductComponentHierarchyId = r.ProductComponentHierarchyId,
                    ProductId = targetProductId,
                    MinQuantity = r.MinQuantity,
                    MaxQuantity = r.MaxQuantity,
                    IsOptional = r.IsOptional,
                    AssemblyOrder = r.AssemblyOrder,
                    Notes = r.Notes
                });

                var success = await _relationRepository.CreateBatchAsync(targetRelations);

                if (success)
                {
                    await NotifyProductHierarchiesChangedAsync(targetProductId);
                    _logger.LogInformation("Configuração de hierarquias duplicada de {SourceProductId} para {TargetProductId} por {UserId}", 
                        sourceProductId, targetProductId, userId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao duplicar configuração de hierarquias de {SourceProductId} para {TargetProductId}", 
                    sourceProductId, targetProductId);
                throw;
            }
        }

        public async Task<(bool IsValid, List<string> ValidationErrors)> ValidateHierarchyConfigurationAsync(string productId)
        {
            try
            {
                var errors = new List<string>();
                var relations = await _relationRepository.GetActiveByProductIdAsync(productId);

                if (!relations.Any())
                {
                    errors.Add("Produto não possui hierarquias configuradas.");
                    return (false, errors);
                }

                // Validar ordens de montagem únicas
                var orders = relations.Select(r => r.AssemblyOrder).ToList();
                var duplicateOrders = orders.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key);
                if (duplicateOrders.Any())
                {
                    errors.Add($"Ordens de montagem duplicadas: {string.Join(", ", duplicateOrders)}");
                }

                // Validar configurações de quantidade
                foreach (var relation in relations)
                {
                    if (relation.MinQuantity <= 0)
                    {
                        errors.Add($"Hierarquia '{relation.ProductComponentHierarchy?.Name}' possui quantidade mínima inválida.");
                    }

                    if (relation.MaxQuantity > 0 && relation.MaxQuantity < relation.MinQuantity)
                    {
                        errors.Add($"Hierarquia '{relation.ProductComponentHierarchy?.Name}' possui quantidade máxima menor que a mínima.");
                    }
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar configuração de hierarquias do produto {ProductId}", productId);
                throw;
            }
        }

        #endregion

        #region Query Operations

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetProductHierarchiesAsync(string productId, bool includeInactive = false)
        {
            try
            {
                var relations = await _relationRepository.GetByProductIdAsync(productId, includeInactive);
                return relations.Select(r => r.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetHierarchyProductsAsync(string hierarchyId, bool includeInactive = false)
        {
            try
            {
                var relations = await _relationRepository.GetByHierarchyIdAsync(hierarchyId, includeInactive);
                return relations.Select(r => r.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos da hierarquia {HierarchyId}", hierarchyId);
                throw;
            }
        }

        public async Task<CompositeProductXHierarchyViewModel?> GetProductHierarchyRelationAsync(string productId, string hierarchyId)
        {
            try
            {
                var relation = await _relationRepository.GetByProductAndHierarchyAsync(productId, hierarchyId);
                return relation?.ToViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relação entre produto {ProductId} e hierarquia {HierarchyId}", productId, hierarchyId);
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetOrderedProductHierarchiesAsync(string productId)
        {
            return await GetProductHierarchiesAsync(productId, includeInactive: false);
        }

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetActiveProductHierarchiesAsync(string productId)
        {
            try
            {
                var relations = await _relationRepository.GetActiveByProductIdAsync(productId);
                return relations.Select(r => r.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias ativas do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetRequiredProductHierarchiesAsync(string productId)
        {
            try
            {
                var relations = await _relationRepository.GetRequiredByProductIdAsync(productId);
                return relations.Select(r => r.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias obrigatórias do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetOptionalProductHierarchiesAsync(string productId)
        {
            try
            {
                var relations = await _relationRepository.GetOptionalByProductIdAsync(productId);
                return relations.Select(r => r.ToViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias opcionais do produto {ProductId}", productId);
                throw;
            }
        }

        #endregion

        #region ViewModels

        public async Task<CreateCompositeProductXHierarchyViewModel> PrepareCreateViewModelAsync(string productId, string? hierarchyId = null)
        {
            try
            {
                var viewModel = new CreateCompositeProductXHierarchyViewModel
                {
                    ProductId = productId,
                    ProductComponentHierarchyId = hierarchyId ?? string.Empty,
                    IsProductIdReadonly = true,
                    AssemblyOrder = await _relationRepository.GetNextAssemblyOrderAsync(productId)
                };

                // Carregar informações do produto
                var product = await _productRepository.GetByIdAsync(productId);
                viewModel.ProductName = product?.Name ?? string.Empty;

                // Carregar hierarquias disponíveis
                viewModel.AvailableHierarchies = await GetAvailableHierarchiesForProductAsync(productId);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao preparar ViewModel de criação para produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<EditCompositeProductXHierarchyViewModel?> PrepareEditViewModelAsync(int relationId)
        {
            try
            {
                var relation = await _relationRepository.GetByIdAsync(relationId);
                if (relation == null) return null;

                var viewModel = relation.ToEditViewModel();

                // Carregar hierarquias disponíveis (incluindo a atual)
                viewModel.AvailableHierarchies = await GetAvailableHierarchiesForProductAsync(relation.ProductId);
                
                // Adicionar a hierarquia atual se não estiver na lista
                if (!viewModel.AvailableHierarchies.Any(h => h.Value == relation.ProductComponentHierarchyId))
                {
                    var hierarchy = await _hierarchyRepository.GetByIdAsync(relation.ProductComponentHierarchyId);
                    if (hierarchy != null)
                    {
                        viewModel.AvailableHierarchies.Insert(0, new SelectListItem
                        {
                            Value = hierarchy.Id,
                            Text = hierarchy.Name,
                            Selected = true
                        });
                    }
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao preparar ViewModel de edição para relação {RelationId}", relationId);
                throw;
            }
        }

        public async Task<CompositeProductXHierarchyIndexViewModel> PrepareIndexViewModelAsync(
            string? searchTerm = null,
            string? productId = null,
            string? hierarchyId = null,
            bool? isActive = null,
            bool? isOptional = null,
            int page = 1,
            int pageSize = 25,
            string sortBy = "AssemblyOrder",
            string sortDirection = "asc")
        {
            try
            {
                var skip = (page - 1) * pageSize;
                var (relations, totalCount) = await _relationRepository.SearchAsync(
                    searchTerm, productId, hierarchyId, isActive, isOptional,
                    skip, pageSize, sortBy, sortDirection);

                var viewModel = new CompositeProductXHierarchyIndexViewModel
                {
                    Relations = relations.Select(r => r.ToViewModel()).ToList(),
                    SearchTerm = searchTerm,
                    ProductId = productId,
                    HierarchyId = hierarchyId,
                    IsActive = isActive,
                    IsOptional = isOptional,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                // Carregar estatísticas
                if (!string.IsNullOrEmpty(productId))
                {
                    viewModel.ContextProductId = productId;
                    var product = await _productRepository.GetByIdAsync(productId);
                    viewModel.ContextProductName = product?.Name ?? string.Empty;
                }

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao preparar ViewModel de índice");
                throw;
            }
        }

        public async Task<List<SelectListItem>> GetAvailableHierarchiesForProductAsync(string productId)
        {
            try
            {
                // Buscar todas as hierarquias ativas
                var allHierarchies = await _hierarchyRepository.GetActiveHierarchiesAsync();
                
                // Buscar hierarquias já associadas ao produto
                var associatedHierarchyIds = (await _relationRepository.GetByProductIdAsync(productId))
                    .Select(r => r.ProductComponentHierarchyId)
                    .ToHashSet();

                return allHierarchies
                    .Where(h => !associatedHierarchyIds.Contains(h.Id))
                    .Select(h => new SelectListItem
                    {
                        Value = h.Id,
                        Text = h.Name + (!string.IsNullOrEmpty(h.Description) ? $" - {h.Description}" : "")
                    })
                    .OrderBy(h => h.Text)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar hierarquias disponíveis para produto {ProductId}", productId);
                return new List<SelectListItem>();
            }
        }

        public async Task<List<SelectListItem>> GetUnassociatedHierarchiesAsync(string productId)
        {
            return await GetAvailableHierarchiesForProductAsync(productId);
        }

        #endregion

        #region Validation

        public async Task<bool> IsHierarchyAssociatedToProductAsync(string productId, string hierarchyId)
        {
            return await _relationRepository.RelationExistsAsync(productId, hierarchyId);
        }

        public async Task<bool> IsAssemblyOrderInUseAsync(string productId, int assemblyOrder, string? excludeRelationId = null)
        {
            int? excludeId = null;
            if (!string.IsNullOrEmpty(excludeRelationId) && int.TryParse(excludeRelationId, out int excludeIdInt))
            {
                excludeId = excludeIdInt;
            }
            
            return await _relationRepository.AssemblyOrderExistsAsync(productId, assemblyOrder, excludeId);
        }

        public async Task<int> GetNextAvailableAssemblyOrderAsync(string productId)
        {
            return await _relationRepository.GetNextAssemblyOrderAsync(productId);
        }

        public async Task<bool> ValidateQuantityConfigurationAsync(int minQuantity, int maxQuantity)
        {
            if (minQuantity <= 0) return false;
            if (maxQuantity > 0 && maxQuantity < minQuantity) return false;
            return true;
        }

        public async Task<(bool CanAssociate, string? Reason)> CanAssociateHierarchyToProductAsync(string hierarchyId, string productId)
        {
            try
            {
                // Verificar se produto existe e é composto
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return (false, "Produto não encontrado.");
                }

                // Verificar se hierarquia existe
                var hierarchy = await _hierarchyRepository.GetByIdAsync(hierarchyId);
                if (hierarchy == null)
                {
                    return (false, "Hierarquia não encontrada.");
                }

                // Verificar se já existe relação
                var relationExists = await _relationRepository.RelationExistsAsync(productId, hierarchyId);
                if (relationExists)
                {
                    return (false, "Já existe uma relação entre este produto e hierarquia.");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar associação de hierarquia {HierarchyId} ao produto {ProductId}", hierarchyId, productId);
                return (false, "Erro interno do servidor.");
            }
        }

        public async Task<(bool CanDelete, string? Reason)> CanDeleteRelationAsync(string relationId)
        {
            try
            {
                if (!int.TryParse(relationId, out int relationIdInt))
                    return (false, "ID da relação deve ser um número válido.");
                    
                var relation = await _relationRepository.GetByIdAsync(relationIdInt);
                if (relation == null)
                {
                    return (false, "Relação não encontrada.");
                }

                // Por enquanto, sempre permite deletar
                // Aqui podem ser adicionadas regras de negócio específicas
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar exclusão da relação {RelationId}", relationId);
                return (false, "Erro interno do servidor.");
            }
        }

        #endregion

        #region Statistics and Analytics

        public async Task<int> CountProductHierarchiesAsync(string productId, bool includeInactive = false)
        {
            return await _relationRepository.CountByProductIdAsync(productId, includeInactive);
        }

        public async Task<int> CountHierarchyUsageAsync(string hierarchyId, bool includeInactive = false)
        {
            return await _relationRepository.CountByHierarchyIdAsync(hierarchyId, includeInactive);
        }

        public async Task<Dictionary<string, int>> GetHierarchyUsageStatisticsAsync(string hierarchyId)
        {
            return await _relationRepository.GetHierarchyUsageStatsAsync(hierarchyId);
        }

        public async Task<Dictionary<string, int>> GetTopProductsUsingHierarchiesAsync(int limit = 10)
        {
            return await _relationRepository.GetTopProductsUsingHierarchiesAsync(limit);
        }

        public async Task<Dictionary<string, int>> GetMostUsedHierarchiesAsync(int limit = 10)
        {
            return await _relationRepository.GetTopUsedHierarchiesAsync(limit);
        }

        public async Task<decimal> CalculateProductHierarchiesCostAsync(string productId)
        {
            try
            {
                // CompositeProductXHierarchy não possui mais campos de custo
                // Retornando 0 por compatibilidade
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular custo das hierarquias do produto {ProductId}", productId);
                throw;
            }
        }

        public async Task<int> CalculateProductHierarchiesProcessingTimeAsync(string productId)
        {
            try
            {
                // CompositeProductXHierarchy não possui mais campos de tempo de processamento
                // Retornando 0 por compatibilidade
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular tempo de processamento das hierarquias do produto {ProductId}", productId);
                throw;
            }
        }

        #endregion

        #region Batch Operations

        public async Task<bool> CreateMultipleRelationsAsync(IEnumerable<CreateCompositeProductXHierarchyViewModel> viewModels, string userId)
        {
            try
            {
                var relations = viewModels.Select(vm => vm.ToEntity());

                var success = await _relationRepository.CreateBatchAsync(relations);

                if (success)
                {
                    var productIds = viewModels.Select(vm => vm.ProductId).Distinct();
                    foreach (var productId in productIds)
                    {
                        await NotifyProductHierarchiesChangedAsync(productId);
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar múltiplas relações CompositeProductXHierarchy");
                throw;
            }
        }

        public async Task<bool> UpdateMultipleRelationsStatusAsync(IEnumerable<string> relationIds, bool isActive, string userId)
        {
            try
            {
                // Converter string IDs para int IDs
                var intRelationIds = relationIds
                    .Where(id => int.TryParse(id, out _))
                    .Select(id => int.Parse(id))
                    .ToList();
                    
                var success = await _relationRepository.UpdateStatusBatchAsync(intRelationIds, isActive);

                if (success)
                {
                    // Notificar mudanças nos produtos afetados
                    var relations = await _relationRepository.GetWithFullDataAsync();
                    var productIds = relations.Where(r => intRelationIds.Contains(r.Id))
                        .Select(r => r.ProductId)
                        .Distinct();
                    
                    foreach (var productId in productIds)
                    {
                        await NotifyProductHierarchiesChangedAsync(productId);
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status de múltiplas relações");
                throw;
            }
        }

        public async Task<bool> DeleteMultipleRelationsAsync(IEnumerable<string> relationIds, string userId)
        {
            try
            {
                // Converter string IDs para int IDs
                var intRelationIds = relationIds
                    .Where(id => int.TryParse(id, out _))
                    .Select(id => int.Parse(id))
                    .ToList();
                    
                // Obter produtos afetados antes da exclusão
                var relations = await _relationRepository.GetWithFullDataAsync();
                var productIds = relations.Where(r => intRelationIds.Contains(r.Id))
                    .Select(r => r.ProductId)
                    .Distinct()
                    .ToList();

                var success = await _relationRepository.DeleteBatchAsync(intRelationIds);

                if (success)
                {
                    foreach (var productId in productIds)
                    {
                        await NotifyProductHierarchiesChangedAsync(productId);
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir múltiplas relações");
                throw;
            }
        }

        public async Task<bool> ImportHierarchyConfigurationAsync(string templateProductId, string targetProductId, string userId)
        {
            return await DuplicateProductHierarchyConfigAsync(templateProductId, targetProductId, userId);
        }

        #endregion

        #region Integration with Other Services

        public async Task<bool> IsCompositeProductAsync(string productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                return product?.ProductType == ProductType.Composite;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se produto {ProductId} é composto", productId);
                return false;
            }
        }

        public async Task<bool> IsHierarchyActiveAsync(string hierarchyId)
        {
            try
            {
                var hierarchy = await _hierarchyRepository.GetByIdAsync(hierarchyId);
                return hierarchy?.StateCode == Models.Enumerators.ObjectState.Active;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se hierarquia {HierarchyId} está ativa", hierarchyId);
                return false;
            }
        }

        public async Task<(string Name, string Type)?> GetProductBasicInfoAsync(string productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null) return null;

                return (product.Name, product.ProductType.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações básicas do produto {ProductId}", productId);
                return null;
            }
        }

        public async Task<(string Name, string Description)?> GetHierarchyBasicInfoAsync(string hierarchyId)
        {
            try
            {
                var hierarchy = await _hierarchyRepository.GetByIdAsync(hierarchyId);
                if (hierarchy == null) return null;

                return (hierarchy.Name, hierarchy.Description ?? string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações básicas da hierarquia {HierarchyId}", hierarchyId);
                return null;
            }
        }

        public async Task NotifyProductHierarchiesChangedAsync(string productId)
        {
            // Implementar notificação para cache, webhooks, etc.
            // Por enquanto, apenas log
            _logger.LogInformation("Hierarquias do produto {ProductId} foram modificadas", productId);
            await Task.CompletedTask;
        }

        #endregion
    }
} 