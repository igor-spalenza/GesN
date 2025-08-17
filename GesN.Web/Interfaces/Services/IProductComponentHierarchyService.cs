using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de Hierarquias de Componentes de Produto
    /// Define operações de negócio e validações para a entidade ProductComponentHierarchy
    /// </summary>
    public interface IProductComponentHierarchyService
    {
        // Operações CRUD básicas
        Task<IEnumerable<ProductComponentHierarchy>> GetAllAsync();
        Task<ProductComponentHierarchy?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProductComponentHierarchy hierarchy);
        Task<bool> UpdateAsync(ProductComponentHierarchy hierarchy);
        Task<bool> DeleteAsync(string id);

        // Consultas específicas do domínio
        Task<IEnumerable<ProductComponentHierarchy>> GetActiveHierarchiesAsync();
        Task<IEnumerable<ProductComponentHierarchy>> GetByNameAsync(string name);
        Task<ProductComponentHierarchy?> GetByExactNameAsync(string name);
        Task<IEnumerable<ProductComponentHierarchy>> GetByDescriptionAsync(string description);

        // Pesquisa e filtros
        Task<IEnumerable<ProductComponentHierarchy>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductComponentHierarchy>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductComponentHierarchy>> GetPagedActiveAsync(int page, int pageSize);

        // Operações de negócio
        Task<bool> ActivateHierarchyAsync(string hierarchyId, string userId);
        Task<bool> DeactivateHierarchyAsync(string hierarchyId, string userId);
        Task<bool> UpdateNotesAsync(string hierarchyId, string notes, string userId);

        // Validações de negócio
        Task<bool> CanDeleteAsync(string hierarchyId);
        Task<bool> CanActivateAsync(string hierarchyId);
        Task<bool> CanDeactivateAsync(string hierarchyId);
        Task<bool> IsNameUniqueAsync(string name, string? excludeId = null);
        Task<IEnumerable<string>> ValidateHierarchyAsync(ProductComponentHierarchy hierarchy);

        // Operações relacionadas aos componentes
        Task<bool> HasRequiredComponentsAsync(string hierarchyId);
        Task<bool> AreAllComponentsAvailableAsync(string hierarchyId);
        Task<int> GetComponentCountAsync(string hierarchyId);
        Task<IEnumerable<ProductComponent>> GetComponentsAsync(string hierarchyId);
        Task<bool> ValidateComponentStructureAsync(string hierarchyId);

        // Relacionamentos com produtos compostos
        Task<IEnumerable<CompositeProductXHierarchy>> GetCompositeProductRelationsAsync(string hierarchyId);
        Task<bool> IsUsedByCompositeProductsAsync(string hierarchyId);
        Task<int> GetUsageCountAsync(string hierarchyId);

        // Análises e relatórios
        Task<IEnumerable<ProductComponentHierarchy>> GetMostUsedHierarchiesAsync(int count = 10);
        Task<IEnumerable<ProductComponentHierarchy>> GetUnusedHierarchiesAsync();
        Task<Dictionary<string, int>> GetHierarchyUsageStatsAsync();

        // Operações em lote
        Task<bool> ActivateBatchAsync(IEnumerable<string> hierarchyIds, string userId);
        Task<bool> DeactivateBatchAsync(IEnumerable<string> hierarchyIds, string userId);
        Task<bool> DeleteBatchAsync(IEnumerable<string> hierarchyIds, string userId);

        // Duplicação e templates
        Task<string> DuplicateHierarchyAsync(string sourceHierarchyId, string newName, string userId);
        Task<bool> CreateFromTemplateAsync(string templateId, string newName, string userId);

        // Métodos para integração com produtos compostos
        Task<IEnumerable<ProductComponentHierarchy>> GetByCompositeProductIdAsync(string compositeProductId);
        Task<IEnumerable<ProductComponentHierarchy>> GetAvailableForProductAsync(string productId);
        Task<bool> AssignToCompositeProductAsync(string hierarchyId, string productId, string userId);
        Task<bool> UnassignFromCompositeProductAsync(string hierarchyId, string productId, string userId);
        Task<CompositeProductXHierarchy?> GetProductRelationAsync(string hierarchyId, string productId);
    }
} 