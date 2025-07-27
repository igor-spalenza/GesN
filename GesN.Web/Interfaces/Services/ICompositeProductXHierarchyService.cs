using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.ViewModels.Production;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GesN.Web.Interfaces.Services
{
    /// <summary>
    /// Interface para serviço de CompositeProductXHierarchy
    /// Gerencia a lógica de negócio das relações entre produtos compostos e hierarquias de componentes
    /// </summary>
    public interface ICompositeProductXHierarchyService
    {
        #region CRUD Operations

        /// <summary>
        /// Criar nova relação CompositeProductXHierarchy
        /// </summary>
        Task<int> CreateRelationAsync(CreateCompositeProductXHierarchyViewModel viewModel, string userId);

        /// <summary>
        /// Buscar relação por ID
        /// </summary>
        Task<CompositeProductXHierarchyDetailsViewModel?> GetRelationByIdAsync(int id);

        /// <summary>
        /// Atualizar relação existente
        /// </summary>
        Task<bool> UpdateRelationAsync(EditCompositeProductXHierarchyViewModel viewModel, string userId);

        /// <summary>
        /// Excluir relação por ID
        /// </summary>
        Task<bool> DeleteRelationAsync(int id, string userId);

        /// <summary>
        /// Verificar se relação existe
        /// </summary>
        Task<bool> RelationExistsAsync(string id);

        #endregion

        #region Business Logic

        /// <summary>
        /// Associar hierarquia existente a um produto composto
        /// </summary>
        Task<int> AssociateHierarchyToProductAsync(string hierarchyId, string productId, CreateCompositeProductXHierarchyViewModel config, string userId);

        /// <summary>
        /// Desassociar hierarquia de um produto composto
        /// </summary>
        Task<bool> DisassociateHierarchyFromProductAsync(string hierarchyId, string productId, string userId);

        /// <summary>
        /// Reordenar hierarquias de um produto
        /// </summary>
        Task<bool> ReorderProductHierarchiesAsync(string productId, Dictionary<string, int> newOrders, string userId);

        /// <summary>
        /// Alternar status ativo/inativo de uma relação
        /// </summary>
        Task<bool> ToggleActiveStatusAsync(string relationId, string userId);

        /// <summary>
        /// Duplicar configuração de hierarquia de um produto para outro
        /// </summary>
        Task<bool> DuplicateProductHierarchyConfigAsync(string sourceProductId, string targetProductId, string userId);

        /// <summary>
        /// Validar configuração de hierarquia para um produto
        /// </summary>
        Task<(bool IsValid, List<string> ValidationErrors)> ValidateHierarchyConfigurationAsync(string productId);

        #endregion

        #region Query Operations

        /// <summary>
        /// Obter todas as relações de um produto composto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetProductHierarchiesAsync(string productId, bool includeInactive = false);

        /// <summary>
        /// Obter todas as relações de uma hierarquia
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetHierarchyProductsAsync(string hierarchyId, bool includeInactive = false);

        /// <summary>
        /// Buscar relação específica entre produto e hierarquia
        /// </summary>
        Task<CompositeProductXHierarchyViewModel?> GetProductHierarchyRelationAsync(string productId, string hierarchyId);

        /// <summary>
        /// Obter hierarquias ordenadas por AssemblyOrder
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetOrderedProductHierarchiesAsync(string productId);

        /// <summary>
        /// Obter hierarquias ativas de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetActiveProductHierarchiesAsync(string productId);

        /// <summary>
        /// Obter hierarquias obrigatórias de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetRequiredProductHierarchiesAsync(string productId);

        /// <summary>
        /// Obter hierarquias opcionais de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchyViewModel>> GetOptionalProductHierarchiesAsync(string productId);

        #endregion

        #region ViewModels

        /// <summary>
        /// Preparar ViewModel para criação de nova relação
        /// </summary>
        Task<CreateCompositeProductXHierarchyViewModel> PrepareCreateViewModelAsync(string productId, string? hierarchyId = null);

        /// <summary>
        /// Preparar ViewModel para edição de relação existente
        /// </summary>
        Task<EditCompositeProductXHierarchyViewModel?> PrepareEditViewModelAsync(int relationId);

        /// <summary>
        /// Preparar ViewModel para listagem com filtros
        /// </summary>
        Task<CompositeProductXHierarchyIndexViewModel> PrepareIndexViewModelAsync(
            string? searchTerm = null,
            string? productId = null,
            string? hierarchyId = null,
            bool? isActive = null,
            bool? isOptional = null,
            int page = 1,
            int pageSize = 25,
            string sortBy = "AssemblyOrder",
            string sortDirection = "asc");

        /// <summary>
        /// Preparar listas para dropdowns
        /// </summary>
        Task<List<SelectListItem>> GetAvailableHierarchiesForProductAsync(string productId);

        /// <summary>
        /// Preparar lista de hierarquias disponíveis (não associadas ao produto)
        /// </summary>
        Task<List<SelectListItem>> GetUnassociatedHierarchiesAsync(string productId);

        #endregion

        #region Validation

        /// <summary>
        /// Verificar se já existe relação entre produto e hierarquia
        /// </summary>
        Task<bool> IsHierarchyAssociatedToProductAsync(string productId, string hierarchyId);

        /// <summary>
        /// Verificar se ordem de montagem já está sendo usada
        /// </summary>
        Task<bool> IsAssemblyOrderInUseAsync(string productId, int assemblyOrder, string? excludeRelationId = null);

        /// <summary>
        /// Obter próxima ordem de montagem disponível
        /// </summary>
        Task<int> GetNextAvailableAssemblyOrderAsync(string productId);

        /// <summary>
        /// Validar configuração de quantidade
        /// </summary>
        Task<bool> ValidateQuantityConfigurationAsync(int minQuantity, int maxQuantity);

        /// <summary>
        /// Validar se hierarquia pode ser associada ao produto
        /// </summary>
        Task<(bool CanAssociate, string? Reason)> CanAssociateHierarchyToProductAsync(string hierarchyId, string productId);

        /// <summary>
        /// Validar se relação pode ser excluída
        /// </summary>
        Task<(bool CanDelete, string? Reason)> CanDeleteRelationAsync(string relationId);

        #endregion

        #region Statistics and Analytics

        /// <summary>
        /// Contar relações por produto
        /// </summary>
        Task<int> CountProductHierarchiesAsync(string productId, bool includeInactive = false);

        /// <summary>
        /// Contar relações por hierarquia
        /// </summary>
        Task<int> CountHierarchyUsageAsync(string hierarchyId, bool includeInactive = false);

        /// <summary>
        /// Obter estatísticas de uso de uma hierarquia
        /// </summary>
        Task<Dictionary<string, int>> GetHierarchyUsageStatisticsAsync(string hierarchyId);

        /// <summary>
        /// Obter produtos que mais usam hierarquias
        /// </summary>
        Task<Dictionary<string, int>> GetTopProductsUsingHierarchiesAsync(int limit = 10);

        /// <summary>
        /// Obter hierarquias mais utilizadas
        /// </summary>
        Task<Dictionary<string, int>> GetMostUsedHierarchiesAsync(int limit = 10);

        /// <summary>
        /// Calcular custo total estimado das hierarquias de um produto
        /// </summary>
        Task<decimal> CalculateProductHierarchiesCostAsync(string productId);

        /// <summary>
        /// Calcular tempo total estimado de processamento das hierarquias de um produto
        /// </summary>
        Task<int> CalculateProductHierarchiesProcessingTimeAsync(string productId);

        #endregion

        #region Batch Operations

        /// <summary>
        /// Criar múltiplas relações
        /// </summary>
        Task<bool> CreateMultipleRelationsAsync(IEnumerable<CreateCompositeProductXHierarchyViewModel> viewModels, string userId);



        /// <summary>
        /// Excluir múltiplas relações
        /// </summary>
        Task<bool> DeleteMultipleRelationsAsync(IEnumerable<string> relationIds, string userId);

        /// <summary>
        /// Importar configuração de hierarquias de um produto template
        /// </summary>
        Task<bool> ImportHierarchyConfigurationAsync(string templateProductId, string targetProductId, string userId);

        #endregion

        #region Integration with Other Services

        /// <summary>
        /// Verificar se produto é do tipo Composite
        /// </summary>
        Task<bool> IsCompositeProductAsync(string productId);

        /// <summary>
        /// Verificar se hierarquia existe e está ativa
        /// </summary>
        Task<bool> IsHierarchyActiveAsync(string hierarchyId);

        /// <summary>
        /// Obter informações básicas do produto
        /// </summary>
        Task<(string Name, string Type)?> GetProductBasicInfoAsync(string productId);

        /// <summary>
        /// Obter informações básicas da hierarquia
        /// </summary>
        Task<(string Name, string Description)?> GetHierarchyBasicInfoAsync(string hierarchyId);

        /// <summary>
        /// Notificar mudanças nas hierarquias de um produto (para cache, etc.)
        /// </summary>
        Task NotifyProductHierarchiesChangedAsync(string productId);

        #endregion
    }
} 