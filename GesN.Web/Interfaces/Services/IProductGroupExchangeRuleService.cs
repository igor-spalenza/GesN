using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductGroupExchangeRuleService
    {
        // CRUD básico
        Task<IEnumerable<ProductGroupExchangeRule>> GetAllAsync();
        Task<ProductGroupExchangeRule?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> UpdateAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> DeleteAsync(string id);
        
        // Consultas específicas
        Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupIdAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetBySourceGroupItemIdAsync(string sourceGroupItemId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByTargetGroupItemIdAsync(string targetGroupItemId);
        Task<IEnumerable<ProductGroupExchangeRule>> GetActiveByGroupAsync(string productGroupId);
        Task<IEnumerable<ProductGroupExchangeRule>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductGroupExchangeRule>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize);
        
        // Operações específicas
        Task<IEnumerable<ProductGroupExchangeRule>> GetAvailableExchangesAsync(string productGroupId, string sourceGroupItemId);
        Task<bool> ExchangeRuleExistsAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId);
        Task<bool> ValidateGroupItemsCompatibilityAsync(string sourceGroupItemId, string targetGroupItemId);
        Task<decimal> CalculateEffectiveRatioAsync(string exchangeRuleId, int sourceWeight, int targetWeight);
        
        // Validações
        Task<bool> ValidateExchangeRuleAsync(ProductGroupExchangeRule exchangeRule);
        Task<bool> ValidateExchangeRatioAsync(decimal exchangeRatio);
        Task<bool> ValidateGroupItemWeightsAsync(int sourceWeight, int targetWeight);
        
        // Cálculos e análises
        Task<decimal> GetAverageExchangeRatioAsync(string productGroupId);
        Task<int> CountExchangeRulesAsync(string productGroupId);
        Task<bool> HasActiveExchangeRulesAsync(string productGroupId);
        
        // Operações de lote
        Task<bool> UpdateExchangeRuleStatusAsync(string exchangeRuleId, bool isActive);
        Task<bool> UpdateMultipleExchangeRulesAsync(IEnumerable<ProductGroupExchangeRule> exchangeRules);
        Task<bool> DeleteExchangeRulesByGroupAsync(string productGroupId);
    }
} 