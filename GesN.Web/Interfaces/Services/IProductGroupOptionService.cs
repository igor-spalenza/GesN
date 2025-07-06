using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductGroupOptionService
    {
        // CRUD Operations
        Task<ProductGroupOption?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupOption>> GetAllAsync();
        Task<IEnumerable<ProductGroupOption>> GetByProductGroupIdAsync(string productGroupId);
        Task<ProductGroupOption> CreateAsync(ProductGroupOption groupOption);
        Task<bool> UpdateAsync(ProductGroupOption groupOption);
        Task<bool> DeleteAsync(string id);
        
        // Business Operations
        Task<bool> ExistsAsync(string id);
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<bool> CanDeleteAsync(string id);
        
        // Search and Filter
        Task<IEnumerable<ProductGroupOption>> SearchAsync(string searchTerm);
        Task<(IEnumerable<ProductGroupOption> Options, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<(IEnumerable<ProductGroupOption> Options, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        
        // Validation
        Task<IEnumerable<string>> ValidateGroupOptionAsync(ProductGroupOption groupOption);
        Task<bool> ValidateOptionNameAsync(string productGroupId, string optionName, string? excludeId = null);
        
        // Bulk Operations
        Task<bool> DeleteByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupOption>> CreateBulkAsync(IEnumerable<ProductGroupOption> groupOptions);
        Task<bool> UpdateGroupOptionsAsync(string productGroupId, IEnumerable<ProductGroupOption> groupOptions);
    }
} 