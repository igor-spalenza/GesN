using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductGroupExchangeRuleService
    {
        // CRUD Operations
        Task<ProductGroupExchangeRule?> GetByIdAsync(string id);
        Task<IEnumerable<ProductGroupExchangeRule>> GetAllAsync();
        Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByFromProductIdAsync(string fromProductId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByToProductIdAsync(string toProductId);
        Task<ProductGroupExchangeRule> CreateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> UpdateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> DeleteAsync(string id);
        
        // Business Operations
        Task<bool> ExistsAsync(string id);
        Task<int> CountByProductGroupAsync(string productGroupId);
        Task<bool> ExchangeRuleExistsAsync(string productGroupId, string fromProductId, string toProductId);
        Task<bool> CanDeleteAsync(string id);
        Task<decimal> GetExchangePriceAsync(string exchangeRuleId);
        
        // Search and Filter
        Task<IEnumerable<ProductGroupExchangeRule>> SearchAsync(string searchTerm);
        Task<(IEnumerable<ProductGroupExchangeRule> Rules, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
        Task<(IEnumerable<ProductGroupExchangeRule> Rules, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        
        // Validation
        Task<IEnumerable<string>> ValidateExchangeRuleAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> ValidateProductsCompatibilityAsync(string fromProductId, string toProductId);
        
        // Bulk Operations
        Task<bool> DeleteByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> CreateBulkAsync(IEnumerable<ProductGroupExchangeRule> exchangeRules);
        Task<bool> UpdateGroupExchangeRulesAsync(string productGroupId, IEnumerable<ProductGroupExchangeRule> exchangeRules);
    }
} 