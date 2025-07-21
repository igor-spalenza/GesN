using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductGroupItemService
    {
        // CRUD Operations
        Task<ProductGroupItem?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupItem>> GetAllAsync();
        Task<IEnumerable<ProductGroupItem>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetByProductIdAsync(string productId);
        Task<ProductGroupItem> CreateAsync(ProductGroupItem groupItem);
        Task<bool> UpdateAsync(ProductGroupItem groupItem);
        Task<bool> DeleteAsync(string id);
        
        // Business Operations
        Task<bool> ExistsAsync(string id);
        Task<IEnumerable<ProductGroupItem>> GetAvailableByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetOptionalByGroupAsync(string productGroupId);
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<bool> ItemExistsInGroupAsync(string productGroupId, string productId);
        Task<bool> CategoryItemExistsInGroupAsync(string productGroupId, string productCategoryId);
        Task<decimal> CalculateGroupTotalPriceAsync(string productGroupId, IEnumerable<string> selectedItemIds);
        Task<bool> CanDeleteAsync(string id);
        
        // Métodos específicos para ProductCategory
        Task<IEnumerable<ProductGroupItem>> GetByProductCategoryIdAsync(string productCategoryId);
        Task<IEnumerable<ProductGroupItem>> GetCategoryItemsByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetProductItemsByGroupAsync(string productGroupId);
        
        // Search and Filter
        Task<IEnumerable<ProductGroupItem>> SearchAsync(string searchTerm);
        Task<(IEnumerable<ProductGroupItem> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<(IEnumerable<ProductGroupItem> Items, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        
        // Validation
        Task<IEnumerable<string>> ValidateGroupItemAsync(ProductGroupItem groupItem);
        Task<bool> ValidateItemQuantityAsync(string productId, int quantity);
        
        // Bulk Operations
        Task<bool> DeleteByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> CreateBulkAsync(IEnumerable<ProductGroupItem> groupItems);
        Task<bool> UpdateGroupItemsAsync(string productGroupId, IEnumerable<ProductGroupItem> groupItems);
    }
} 