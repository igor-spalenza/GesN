using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductGroupExchangeRuleRepository
    {
        Task<IEnumerable<ProductGroupExchangeRule>> GetAllAsync();
        Task<ProductGroupExchangeRule?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetBySourceGroupItemIdAsync(string sourceGroupItemId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByTargetGroupItemIdAsync(string targetGroupItemId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetActiveByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> UpdateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        Task<IEnumerable<ProductGroupExchangeRule>> GetAvailableExchangesAsync(string productGroupId, string sourceGroupItemId);
        Task<bool> ExchangeRuleExistsAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId);
        Task<bool> ValidateGroupItemsCompatibilityAsync(string sourceGroupItemId, string targetGroupItemId);
    }
} 