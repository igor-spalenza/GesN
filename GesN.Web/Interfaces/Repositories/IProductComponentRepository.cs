using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductComponentRepository
    {
        Task<IEnumerable<ProductComponent>> GetAllAsync();
        Task<ProductComponent?> GetByIdAsync(string id);
        Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId);
        Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(ProductComponent component);
        Task<bool> UpdateAsync(ProductComponent component);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<int> CountByCompositeProductAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductComponent>> GetByCompositeProductPagedAsync(string compositeProductId, int page, int pageSize);
        Task<bool> ComponentExistsInCompositeAsync(string compositeProductId, string componentProductId);
        Task<IEnumerable<ProductComponent>> GetOrderedByAssemblyAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId);
        Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId);
    }
} 