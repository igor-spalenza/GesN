using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<IEnumerable<ProductCategory>> GetAllAsync();
        Task<ProductCategory?> GetByIdAsync(string id);
        Task<ProductCategory?> GetByNameAsync(string name);
        Task<IEnumerable<ProductCategory>> GetActiveAsync();
        Task<IEnumerable<ProductCategory>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductCategory>> SearchForAutocompleteAsync(string searchTerm);
        Task<string> CreateAsync(ProductCategory category);
        Task<bool> UpdateAsync(ProductCategory category);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<ProductCategory>> GetPagedAsync(int page, int pageSize);
    }
} 