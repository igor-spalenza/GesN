using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductGroupItemRepository _groupItemRepository;
        private readonly IProductGroupOptionRepository _groupOptionRepository;
        private readonly IProductGroupExchangeRuleRepository _exchangeRuleRepository;
        private readonly ILogger<ProductGroupService> _logger;

        public ProductGroupService(
            IProductRepository productRepository,
            IProductGroupItemRepository groupItemRepository,
            IProductGroupOptionRepository groupOptionRepository,
            IProductGroupExchangeRuleRepository exchangeRuleRepository,
            ILogger<ProductGroupService> logger)
        {
            _productRepository = productRepository;
            _groupItemRepository = groupItemRepository;
            _groupOptionRepository = groupOptionRepository;
            _exchangeRuleRepository = exchangeRuleRepository;
            _logger = logger;
        }

        // CRUD básico
        public async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            var products = await _productRepository.GetByTypeAsync(ProductType.Group);
            return products.Cast<ProductGroup>();
        }

        public async Task<ProductGroup?> GetByIdAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product as ProductGroup;
        }

        public async Task<string> CreateAsync(ProductGroup productGroup)
        {
            // Validações de negócio
            if (!await ValidateProductGroupAsync(productGroup))
                throw new InvalidOperationException("Grupo de produto inválido");

            // Configurar dados específicos do grupo
            productGroup.Id = Guid.NewGuid().ToString();
            productGroup.StateCode = ObjectState.Active;
            productGroup.CreatedAt = DateTime.UtcNow;
            productGroup.LastModifiedAt = DateTime.UtcNow;

            return await _productRepository.CreateAsync(productGroup);
        }

        public async Task<bool> UpdateAsync(ProductGroup productGroup)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productGroup.Id);
            if (existingProduct == null || existingProduct.ProductType != ProductType.Group)
                return false;

            // Validações de negócio
            if (!await ValidateProductGroupAsync(productGroup))
                throw new InvalidOperationException("Grupo de produto inválido");

            // Atualizar dados de auditoria
            productGroup.LastModifiedAt = DateTime.UtcNow;

            return await _productRepository.UpdateAsync(productGroup);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var productGroup = await GetByIdAsync(id);
            if (productGroup == null)
                return false;

            // Verificar se pode ser deletado (não tem itens dependentes)
            var groupItems = await _groupItemRepository.GetByProductGroupIdAsync(id);
            if (groupItems.Any())
                throw new InvalidOperationException("Não é possível deletar um grupo que possui itens");

            return await _productRepository.DeleteAsync(id);
        }

        // Validações
        public async Task<bool> ValidateProductGroupAsync(ProductGroup productGroup)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(productGroup.Name))
                return false;

            if (productGroup.UnitPrice < 0)
                return false;

            // Validações específicas de ProductGroup - campos MinItemsRequired e MaxItemsAllowed foram removidos

            return true;
        }

        public async Task<bool> ValidateGroupConfigurationAsync(ProductGroup productGroup)
        {
            // Verificar se tem itens válidos
            if (!await HasValidItemsAsync(productGroup.Id))
                return false;

            // Verificar se todas as opções obrigatórias estão configuradas
            var requiredOptions = await _groupOptionRepository.GetRequiredByGroupAsync(productGroup.Id);
            if (requiredOptions.Any() && !await HasRequiredOptionsAsync(productGroup.Id))
                return false;

            return true;
        }

        public async Task<bool> HasValidItemsAsync(string productGroupId)
        {
            var items = await _groupItemRepository.GetByProductGroupIdAsync(productGroupId);
            return items.Any();
        }

        public async Task<bool> HasRequiredOptionsAsync(string productGroupId)
        {
            var requiredOptions = await _groupOptionRepository.GetRequiredByGroupAsync(productGroupId);
            return requiredOptions.Any();
        }

        // Cálculos
        public async Task<decimal> CalculateBasePriceAsync(string productGroupId)
        {
            var productGroup = await GetByIdAsync(productGroupId);
            if (productGroup == null)
                return 0;

            return productGroup.UnitPrice;
        }

        public async Task<decimal> CalculateMinimumPriceAsync(string productGroupId)
        {
            var basePrice = await CalculateBasePriceAsync(productGroupId);
            var requiredItems = await _groupItemRepository.GetByProductGroupIdAsync(productGroupId);
            var filteredItems = requiredItems.Where(i => !i.IsOptional);
            
            decimal minExtraPrice = 0;
            foreach (var item in filteredItems)
            {
                minExtraPrice += item.ExtraPrice * item.MinQuantity;
            }

            return basePrice + minExtraPrice;
        }

        public async Task<decimal> CalculateMaximumPriceAsync(string productGroupId)
        {
            var basePrice = await CalculateBasePriceAsync(productGroupId);
            var allItems = await _groupItemRepository.GetByProductGroupIdAsync(productGroupId);
            
            decimal maxExtraPrice = 0;
            foreach (var item in allItems)
            {
                maxExtraPrice += item.ExtraPrice * (item.MaxQuantity ?? item.Quantity);
            }

            return basePrice + maxExtraPrice;
        }

        public async Task<int> GetMinimumItemsRequiredAsync(string productGroupId)
        {
            // Campo removido - retorna 0 por padrão
            await Task.CompletedTask;
            return 0;
        }

        public async Task<int> GetMaximumItemsAllowedAsync(string productGroupId)
        {
            // Campo removido - retorna ilimitado por padrão
            await Task.CompletedTask;
            return int.MaxValue;
        }

        // Consultas específicas
        public async Task<IEnumerable<ProductGroup>> GetActiveGroupsAsync()
        {
            var activeProducts = await _productRepository.GetActiveProductsAsync();
            return activeProducts.Where(p => p.ProductType == ProductType.Group).Cast<ProductGroup>();
        }

        public async Task<IEnumerable<ProductGroup>> GetByCategoryAsync(string categoryId)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId);
            return products.Where(p => p.ProductType == ProductType.Group).Cast<ProductGroup>();
        }

        public async Task<IEnumerable<ProductGroup>> SearchAsync(string searchTerm)
        {
            var products = await _productRepository.SearchAsync(searchTerm);
            return products.Where(p => p.ProductType == ProductType.Group).Cast<ProductGroup>();
        }

        public async Task<IEnumerable<ProductGroup>> GetPagedAsync(int page, int pageSize)
        {
            var products = await _productRepository.GetPagedAsync(page, pageSize);
            return products.Where(p => p.ProductType == ProductType.Group).Cast<ProductGroup>();
        }

        // Configuração de grupo
        public async Task<bool> ConfigureGroupLimitsAsync(string productGroupId, int minItems, int maxItems)
        {
            // Método mantido para compatibilidade, mas os campos MinItemsRequired e MaxItemsAllowed foram removidos
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> UpdateGroupStatusAsync(string productGroupId, bool isActive)
        {
            try
            {
                var productGroup = await GetByIdAsync(productGroupId);
                if (productGroup == null) return false;

                productGroup.StateCode = isActive ? ObjectState.Active : ObjectState.Inactive;
                return await _productRepository.UpdateAsync(productGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do grupo de produto: {ProductGroupId}", productGroupId);
                return false;
            }
        }

        // Gerenciamento de itens do grupo
        public async Task<IEnumerable<ProductGroupItem>> GetGroupItemsAsync(string productGroupId)
        {
            return await _groupItemRepository.GetByProductGroupIdAsync(productGroupId);
        }

        public async Task<ProductGroupItem?> GetGroupItemByIdAsync(string itemId)
        {
            return await _groupItemRepository.GetByIdAsync(itemId);
        }

        public async Task<bool> AddGroupItemAsync(ProductGroupItem item)
        {
            try
            {
                var itemId = await _groupItemRepository.CreateAsync(item);
                return !string.IsNullOrEmpty(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar item ao grupo");
                return false;
            }
        }

        public async Task<bool> UpdateGroupItemAsync(ProductGroupItem item)
        {
            try
            {
                return await _groupItemRepository.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do grupo: {ItemId}", item.Id);
                return false;
            }
        }

        public async Task<bool> RemoveGroupItemAsync(string itemId)
        {
            try
            {
                return await _groupItemRepository.DeleteAsync(itemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item do grupo: {ItemId}", itemId);
                return false;
            }
        }

        // Gerenciamento de opções do grupo
        public async Task<IEnumerable<ProductGroupOption>> GetGroupOptionsAsync(string productGroupId)
        {
            return await _groupOptionRepository.GetByProductGroupIdAsync(productGroupId);
        }

        public async Task<ProductGroupOption?> GetGroupOptionByIdAsync(string optionId)
        {
            return await _groupOptionRepository.GetByIdAsync(optionId);
        }

        public async Task<bool> AddGroupOptionAsync(ProductGroupOption option)
        {
            try
            {
                var optionId = await _groupOptionRepository.CreateAsync(option);
                return !string.IsNullOrEmpty(optionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar opção ao grupo");
                return false;
            }
        }

        public async Task<bool> UpdateGroupOptionAsync(ProductGroupOption option)
        {
            try
            {
                return await _groupOptionRepository.UpdateAsync(option);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar opção do grupo: {OptionId}", option.Id);
                return false;
            }
        }

        public async Task<bool> RemoveGroupOptionAsync(string optionId)
        {
            try
            {
                return await _groupOptionRepository.DeleteAsync(optionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover opção do grupo: {OptionId}", optionId);
                return false;
            }
        }

        // Gerenciamento de regras de troca
        public async Task<IEnumerable<ProductGroupExchangeRule>> GetExchangeRulesAsync(string productGroupId)
        {
            return await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
        }

        public async Task<ProductGroupExchangeRule?> GetExchangeRuleByIdAsync(string ruleId)
        {
            return await _exchangeRuleRepository.GetByIdAsync(ruleId);
        }

        public async Task<bool> AddExchangeRuleAsync(ProductGroupExchangeRule rule)
        {
            try
            {
                var ruleId = await _exchangeRuleRepository.CreateAsync(rule);
                return !string.IsNullOrEmpty(ruleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar regra de troca");
                return false;
            }
        }

        public async Task<bool> UpdateExchangeRuleAsync(ProductGroupExchangeRule rule)
        {
            try
            {
                return await _exchangeRuleRepository.UpdateAsync(rule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar regra de troca: {RuleId}", rule.Id);
                return false;
            }
        }

        public async Task<bool> RemoveExchangeRuleAsync(string ruleId)
        {
            try
            {
                return await _exchangeRuleRepository.DeleteAsync(ruleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover regra de troca: {RuleId}", ruleId);
                return false;
            }
        }

        // Cálculos específicos
        public async Task<decimal> CalculateGroupPriceAsync(string productGroupId)
        {
            var items = await _groupItemRepository.GetByProductGroupIdAsync(productGroupId);
            
            decimal totalPrice = 0;
            foreach (var item in items)
            {
                totalPrice += item.ExtraPrice;
            }
            
            return totalPrice;
        }
    }
} 