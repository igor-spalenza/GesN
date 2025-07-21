using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductGroupRepository
    {
        Task<IEnumerable<ProductGroup>> GetAllAsync();
        Task<ProductGroup?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroup>> GetActiveAsync();
        Task<IEnumerable<ProductGroup>> GetByCategoryAsync(string categoryId);
        Task<IEnumerable<ProductGroup>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductGroup>> GetPagedAsync(int page, int pageSize);
        Task<ProductGroup?> GetWithItemsAsync(string id);
        Task<ProductGroup?> GetWithExchangeRulesAsync(string id);
        Task<ProductGroup?> GetCompleteAsync(string id);
        Task<string> CreateAsync(ProductGroup productGroup);
        Task<bool> UpdateAsync(ProductGroup productGroup);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<bool> HasItemsAsync(string id);
        Task<bool> HasExchangeRulesAsync(string id);
        Task<decimal> CalculateGroupPriceAsync(string id, IEnumerable<string> selectedItemIds);
    }
} 