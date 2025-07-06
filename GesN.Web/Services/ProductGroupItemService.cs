using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    public class ProductGroupItemService : IProductGroupItemService
    {
        private readonly IProductGroupItemRepository _productGroupItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly ILogger<ProductGroupItemService> _logger;

        public ProductGroupItemService(
            IProductGroupItemRepository productGroupItemRepository,
            IProductRepository productRepository,
            IProductGroupRepository productGroupRepository,
            ILogger<ProductGroupItemService> logger)
        {
            _productGroupItemRepository = productGroupItemRepository;
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
            _logger = logger;
        }

        // CRUD Operations
        public async Task<ProductGroupItem?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do item do grupo não fornecido");
                    return null;
                }

                return await _productGroupItemRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar item do grupo por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> GetAllAsync()
        {
            try
            {
                return await _productGroupItemRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os itens dos grupos");
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo não fornecido");
                    return Enumerable.Empty<ProductGroupItem>();
                }

                return await _productGroupItemRepository.GetByProductGroupIdAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> GetByProductIdAsync(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    _logger.LogWarning("ID do produto não fornecido");
                    return Enumerable.Empty<ProductGroupItem>();
                }

                return await _productGroupItemRepository.GetByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<ProductGroupItem> CreateAsync(ProductGroupItem groupItem)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateGroupItemAsync(groupItem);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                // Verificar se o item já existe no grupo
                if (await ItemExistsInGroupAsync(groupItem.ProductGroupId, groupItem.ProductId))
                {
                    throw new InvalidOperationException("Este produto já está no grupo");
                }

                // Definir valores padrão
                groupItem.Id = Guid.NewGuid().ToString();
                groupItem.CreatedAt = DateTime.UtcNow;
                groupItem.StateCode = ObjectState.Active;

                var itemId = await _productGroupItemRepository.CreateAsync(groupItem);
                groupItem.Id = itemId;
                
                _logger.LogInformation("Item do grupo criado com sucesso: {Id}", itemId);
                return groupItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item do grupo");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProductGroupItem groupItem)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateGroupItemAsync(groupItem);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                groupItem.LastModifiedAt = DateTime.UtcNow;
                var result = await _productGroupItemRepository.UpdateAsync(groupItem);
                
                if (result)
                {
                    _logger.LogInformation("Item do grupo atualizado com sucesso: {Id}", groupItem.Id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do grupo: {Id}", groupItem.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (!await CanDeleteAsync(id))
                {
                    throw new InvalidOperationException("Este item do grupo não pode ser excluído.");
                }

                var result = await _productGroupItemRepository.DeleteAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Item do grupo excluído com sucesso: {Id}", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir item do grupo: {Id}", id);
                throw;
            }
        }

        // Business Operations
        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                return await _productGroupItemRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do item do grupo: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> GetAvailableByGroupAsync(string productGroupId)
        {
            try
            {
                return await _productGroupItemRepository.GetAvailableByGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens disponíveis do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> GetOptionalByGroupAsync(string productGroupId)
        {
            try
            {
                return await _productGroupItemRepository.GetOptionalByGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens opcionais do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            try
            {
                return await _productGroupItemRepository.CountByProductGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar itens do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> ItemExistsInGroupAsync(string productGroupId, string productId)
        {
            try
            {
                return await _productGroupItemRepository.ItemExistsInGroupAsync(productGroupId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se item existe no grupo: {GroupId}, {ProductId}", productGroupId, productId);
                throw;
            }
        }

        public async Task<decimal> CalculateGroupTotalPriceAsync(string productGroupId, IEnumerable<string> selectedItemIds)
        {
            try
            {
                return await _productGroupItemRepository.CalculateGroupTotalPriceAsync(productGroupId, selectedItemIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular total do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> CanDeleteAsync(string id)
        {
            try
            {
                var item = await _productGroupItemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return false;
                }

                // Verificar se o item pode ser excluído (regras de negócio)
                // TODO: Implementar lógica específica conforme necessário
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se item pode ser excluído: {Id}", id);
                throw;
            }
        }

        // Search and Filter
        public async Task<IEnumerable<ProductGroupItem>> SearchAsync(string searchTerm)
        {
            try
            {
                return await _productGroupItemRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens por termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<(IEnumerable<ProductGroupItem> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            try
            {
                IEnumerable<ProductGroupItem> items;
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    items = await _productGroupItemRepository.SearchAsync(searchTerm);
                }
                else
                {
                    items = await _productGroupItemRepository.GetPagedAsync(page, pageSize);
                }
                
                return (items, items.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens paginados");
                throw;
            }
        }

        public async Task<(IEnumerable<ProductGroupItem> Items, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            try
            {
                var items = await _productGroupItemRepository.GetByProductGroupPagedAsync(productGroupId, page, pageSize);
                return (items, items.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do grupo paginados: {GroupId}", productGroupId);
                throw;
            }
        }

        // Validation
        public async Task<IEnumerable<string>> ValidateGroupItemAsync(ProductGroupItem groupItem)
        {
            var errors = new List<string>();

            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(groupItem.ProductGroupId))
                {
                    errors.Add("ID do grupo é obrigatório");
                }

                if (string.IsNullOrEmpty(groupItem.ProductId))
                {
                    errors.Add("ID do produto é obrigatório");
                }

                if (groupItem.Quantity <= 0)
                {
                    errors.Add("Quantidade deve ser maior que zero");
                }

                if (groupItem.ExtraPrice < 0)
                {
                    errors.Add("Preço extra não pode ser negativo");
                }

                // Validar se o grupo existe
                if (!string.IsNullOrEmpty(groupItem.ProductGroupId))
                {
                    var productGroup = await _productGroupRepository.GetByIdAsync(groupItem.ProductGroupId);
                    if (productGroup == null)
                    {
                        errors.Add("Grupo de produto não encontrado");
                    }
                }

                // Validar se o produto existe
                if (!string.IsNullOrEmpty(groupItem.ProductId))
                {
                    var product = await _productRepository.GetByIdAsync(groupItem.ProductId);
                    if (product == null)
                    {
                        errors.Add("Produto não encontrado");
                    }
                    else if (product.StateCode != ObjectState.Active)
                    {
                        errors.Add("Produto não está ativo");
                    }
                }

                // Validar quantidade
                if (!string.IsNullOrEmpty(groupItem.ProductId) && groupItem.Quantity > 0)
                {
                    var isValidQuantity = await ValidateItemQuantityAsync(groupItem.ProductId, groupItem.Quantity);
                    if (!isValidQuantity)
                    {
                        errors.Add("Quantidade não é válida para este produto");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar item do grupo");
                errors.Add("Erro interno na validação");
            }

            return errors;
        }

        public async Task<bool> ValidateItemQuantityAsync(string productId, int quantity)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return false;
                }

                // TODO: Implementar lógica de validação de quantidade se necessário
                // Por enquanto, sempre retorna true
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar quantidade do item: {ProductId}", productId);
                throw;
            }
        }

        // Bulk Operations
        public async Task<bool> DeleteByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                var items = await _productGroupItemRepository.GetByProductGroupIdAsync(productGroupId);
                foreach (var item in items)
                {
                    await _productGroupItemRepository.DeleteAsync(item.Id);
                }

                _logger.LogInformation("Itens do grupo excluídos com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir itens do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupItem>> CreateBulkAsync(IEnumerable<ProductGroupItem> groupItems)
        {
            var createdItems = new List<ProductGroupItem>();

            try
            {
                foreach (var item in groupItems)
                {
                    var createdItem = await CreateAsync(item);
                    createdItems.Add(createdItem);
                }

                _logger.LogInformation("Criados {Count} itens de grupo em lote", createdItems.Count);
                return createdItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar itens do grupo em lote");
                throw;
            }
        }

        public async Task<bool> UpdateGroupItemsAsync(string productGroupId, IEnumerable<ProductGroupItem> groupItems)
        {
            try
            {
                // Excluir itens existentes
                await DeleteByProductGroupIdAsync(productGroupId);

                // Criar novos itens
                await CreateBulkAsync(groupItems);

                _logger.LogInformation("Itens do grupo atualizados com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar itens do grupo: {GroupId}", productGroupId);
                throw;
            }
        }
    }
} 