using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductComponentRepository
    {
        // Métodos principais com nova estrutura
        Task<IEnumerable<ProductComponent>> GetAllAsync();
        Task<ProductComponent?> GetByIdAsync(string id);
        Task<IEnumerable<ProductComponent>> GetByHierarchyIdAsync(string hierarchyId);
        Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm);
        Task<bool> CreateAsync(ProductComponent component);
        Task<bool> UpdateAsync(ProductComponent component);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();

        // Métodos obsoletos mantidos para compatibilidade (temporariamente)
        [Obsolete("ProductComponent não possui mais CompositeProductId. Use GetByHierarchyIdAsync()")]
        Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais ComponentProductId")]
        Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId);
        
        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId);
        
        [Obsolete("ProductComponent não possui mais CompositeProductId")]
        Task<int> CountByCompositeProductAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais CompositeProductId")]
        Task<IEnumerable<ProductComponent>> GetByCompositeProductPagedAsync(string compositeProductId, int page, int pageSize);
        
        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        Task<bool> ComponentExistsInCompositeAsync(string compositeProductId, string componentProductId);
        
        [Obsolete("ProductComponent não possui mais AssemblyOrder")]
        Task<IEnumerable<ProductComponent>> GetOrderedByAssemblyAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais IsOptional")]
        Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais IsOptional")]
        Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId);
    }
} 