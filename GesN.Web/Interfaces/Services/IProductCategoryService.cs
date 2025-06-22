using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductCategoryService
    {
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task<ProductCategory?> GetCategoryByIdAsync(string id);
        Task<ProductCategory?> GetCategoryByNameAsync(string name);
        Task<IEnumerable<ProductCategory>> GetActiveCategoriesAsync();
        Task<IEnumerable<ProductCategory>> SearchCategoriesAsync(string searchTerm);
        Task<IEnumerable<ProductCategory>> SearchCategoriesForAutocompleteAsync(string searchTerm);
        Task<string> CreateCategoryAsync(ProductCategory category, string createdBy);
        Task<bool> UpdateCategoryAsync(ProductCategory category, string modifiedBy);
        Task<bool> DeleteCategoryAsync(string id);
        Task<bool> CategoryExistsAsync(string id);
        Task<int> GetCategoryCountAsync();
        Task<int> GetActiveCategoryCountAsync();
        Task<IEnumerable<ProductCategory>> GetPagedCategoriesAsync(int page, int pageSize);
        Task<bool> ValidateCategoryDataAsync(ProductCategory category);
    }
} 