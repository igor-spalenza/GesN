using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductGroupItemRepository
    {
        Task<IEnumerable<ProductGroupItem>> GetAllAsync();
        Task<ProductGroupItem?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupItem>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetByProductIdAsync(string productId);
        Task<IEnumerable<ProductGroupItem>> GetAvailableByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetOptionalByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(ProductGroupItem groupItem);
        Task<bool> UpdateAsync(ProductGroupItem groupItem);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductGroupItem>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        Task<bool> ItemExistsInGroupAsync(string productGroupId, string productId);
        Task<decimal> CalculateGroupTotalPriceAsync(string productGroupId, IEnumerable<string> selectedItemIds);
        
        // Métodos específicos para ProductCategory
        Task<IEnumerable<ProductGroupItem>> GetByProductCategoryIdAsync(string productCategoryId);
        Task<ProductGroupItem?> GetByProductGroupAndCategoryAsync(string productGroupId, string productCategoryId);
        Task<bool> CategoryItemExistsInGroupAsync(string productGroupId, string productCategoryId);
        Task<IEnumerable<ProductGroupItem>> GetCategoryItemsByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetProductItemsByGroupAsync(string productGroupId);
    }
} 