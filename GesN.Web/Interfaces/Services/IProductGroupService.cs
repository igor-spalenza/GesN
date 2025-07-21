using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductGroupService
    {
        // CRUD básico
        Task<IEnumerable<ProductGroup>> GetAllAsync();
        Task<ProductGroup?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProductGroup productGroup);
        Task<bool> UpdateAsync(ProductGroup productGroup);
        Task<bool> DeleteAsync(string id);
        
        // Validações
        Task<bool> ValidateProductGroupAsync(ProductGroup productGroup);
        Task<bool> ValidateGroupConfigurationAsync(ProductGroup productGroup);
        Task<bool> HasValidItemsAsync(string productGroupId);
        
        // Cálculos
        Task<decimal> CalculateBasePriceAsync(string productGroupId);
        Task<decimal> CalculateMinimumPriceAsync(string productGroupId);
        Task<decimal> CalculateMaximumPriceAsync(string productGroupId);
        Task<int> GetMinimumItemsRequiredAsync(string productGroupId);
        Task<int> GetMaximumItemsAllowedAsync(string productGroupId);
        
        // Consultas específicas
        Task<IEnumerable<ProductGroup>> GetActiveGroupsAsync();
        Task<IEnumerable<ProductGroup>> GetByCategoryAsync(string categoryId);
        Task<IEnumerable<ProductGroup>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductGroup>> GetPagedAsync(int page, int pageSize);
        
        // Configuração de grupo
        Task<bool> ConfigureGroupLimitsAsync(string productGroupId, int minItems, int maxItems);
        Task<bool> UpdateGroupStatusAsync(string productGroupId, bool isActive);

        // Gerenciamento de itens do grupo
        Task<IEnumerable<ProductGroupItem>> GetGroupItemsAsync(string productGroupId);
        Task<IEnumerable<ProductGroupItem>> GetGroupItemsWithProductDataAsync(string productGroupId);
        Task<ProductGroupItem?> GetGroupItemByIdAsync(string itemId);
        Task<bool> AddGroupItemAsync(ProductGroupItem item);
        Task<bool> UpdateGroupItemAsync(ProductGroupItem item);
        Task<bool> RemoveGroupItemAsync(string itemId);

        // Gerenciamento de regras de troca
        Task<IEnumerable<ProductGroupExchangeRule>> GetExchangeRulesAsync(string productGroupId);
        Task<ProductGroupExchangeRule?> GetExchangeRuleByIdAsync(string ruleId);
        Task<bool> AddExchangeRuleAsync(ProductGroupExchangeRule rule);
        Task<bool> UpdateExchangeRuleAsync(ProductGroupExchangeRule rule);
        Task<bool> RemoveExchangeRuleAsync(string ruleId);

        // Cálculos específicos
        Task<decimal> CalculateGroupPriceAsync(string productGroupId);
    }
} 