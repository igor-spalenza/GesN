using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductGroupOptionRepository
    {
        Task<IEnumerable<ProductGroupOption>> GetAllAsync();
        Task<ProductGroupOption?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupOption>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupOption>> GetRequiredByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupOption>> GetOptionalByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupOption>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(ProductGroupOption groupOption);
        Task<bool> UpdateAsync(ProductGroupOption groupOption);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupOption>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductGroupOption>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        Task<IEnumerable<ProductGroupOption>> GetOrderedByDisplayAsync(string productGroupId);
        Task<bool> OptionExistsInGroupAsync(string productGroupId, string optionName);
    }
} 