using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    public class ProductGroupExchangeRuleService : IProductGroupExchangeRuleService
    {
        private readonly IProductGroupExchangeRuleRepository _exchangeRuleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly ILogger<ProductGroupExchangeRuleService> _logger;

        public ProductGroupExchangeRuleService(
            IProductGroupExchangeRuleRepository exchangeRuleRepository,
            IProductRepository productRepository,
            IProductGroupRepository productGroupRepository,
            ILogger<ProductGroupExchangeRuleService> logger)
        {
            _exchangeRuleRepository = exchangeRuleRepository;
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
            _logger = logger;
        }

        // CRUD Operations
        public async Task<ProductGroupExchangeRule?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID da regra de troca não fornecido");
                    return null;
                }

                return await _exchangeRuleRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regra de troca por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetAllAsync()
        {
            try
            {
                return await _exchangeRuleRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as regras de troca");
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        // Old methods removed - replaced with SourceGroupItem and TargetGroupItem versions

        public async Task<string> CreateAsync(ProductGroupExchangeRule exchangeRule)
        {
            try
            {
                // Validações de negócio
                var validationResult = await ValidateExchangeRuleAsync(exchangeRule);
                if (!validationResult)
                {
                    throw new InvalidOperationException("Regra de troca inválida");
                }

                // Verificar se a regra de troca já existe
                if (await ExchangeRuleExistsAsync(exchangeRule.ProductGroupId, exchangeRule.SourceGroupItemId, exchangeRule.TargetGroupItemId))
                {
                    throw new InvalidOperationException("Já existe uma regra de troca entre estes itens de grupo");
                }

                // Definir valores padrão
                exchangeRule.Id = Guid.NewGuid().ToString();
                exchangeRule.CreatedAt = DateTime.UtcNow;
                exchangeRule.StateCode = ObjectState.Active;

                var ruleId = await _exchangeRuleRepository.CreateAsync(exchangeRule);
                
                _logger.LogInformation("Regra de troca criada com sucesso: {Id}", ruleId);
                return ruleId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar regra de troca");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProductGroupExchangeRule exchangeRule)
        {
            try
            {
                // Validações de negócio
                var validationResult = await ValidateExchangeRuleAsync(exchangeRule);
                if (!validationResult)
                {
                    throw new InvalidOperationException("Regra de troca inválida");
                }

                exchangeRule.LastModifiedAt = DateTime.UtcNow;
                var result = await _exchangeRuleRepository.UpdateAsync(exchangeRule);
                
                if (result)
                {
                    _logger.LogInformation("Regra de troca atualizada com sucesso: {Id}", exchangeRule.Id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar regra de troca: {Id}", exchangeRule.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (!await CanDeleteAsync(id))
                {
                    throw new InvalidOperationException("Esta regra de troca não pode ser excluída.");
                }

                var result = await _exchangeRuleRepository.DeleteAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Regra de troca excluída com sucesso: {Id}", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir regra de troca: {Id}", id);
                throw;
            }
        }

        // Business Operations
        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                return await _exchangeRuleRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da regra de troca: {Id}", id);
                throw;
            }
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            try
            {
                return await _exchangeRuleRepository.CountByProductGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar regras de troca do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> ExchangeRuleExistsAsync(string productGroupId, string sourceGroupItemId, string targetGroupItemId)
        {
            try
            {
                return await _exchangeRuleRepository.ExchangeRuleExistsAsync(productGroupId, sourceGroupItemId, targetGroupItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se regra de troca existe: {GroupId}, {SourceGroupItemId}, {TargetGroupItemId}", 
                    productGroupId, sourceGroupItemId, targetGroupItemId);
                throw;
            }
        }

        public async Task<bool> CanDeleteAsync(string id)
        {
            try
            {
                var rule = await _exchangeRuleRepository.GetByIdAsync(id);
                if (rule == null)
                {
                    return false;
                }

                // Verificar se a regra pode ser excluída (regras de negócio)
                // TODO: Implementar lógica específica conforme necessário
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se regra pode ser excluída: {Id}", id);
                throw;
            }
        }

        public async Task<decimal> CalculateEffectiveRatioAsync(string exchangeRuleId)
        {
            try
            {
                var rule = await _exchangeRuleRepository.GetByIdAsync(exchangeRuleId);
                return rule?.CalculateEffectiveRatio() ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular proporção efetiva da troca: {Id}", exchangeRuleId);
                throw;
            }
        }

        // Search and Filter
        public async Task<IEnumerable<ProductGroupExchangeRule>> SearchAsync(string searchTerm)
        {
            try
            {
                return await _exchangeRuleRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras por termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<(IEnumerable<ProductGroupExchangeRule> Rules, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            try
            {
                IEnumerable<ProductGroupExchangeRule> rules;
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    rules = await _exchangeRuleRepository.SearchAsync(searchTerm);
                }
                else
                {
                    rules = await _exchangeRuleRepository.GetPagedAsync(page, pageSize);
                }
                
                return (rules, rules.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras paginadas");
                throw;
            }
        }

        // Method removed to avoid duplicate - using the simple version that matches interface

        // Validation
        public async Task<bool> ValidateProductsCompatibilityAsync(string fromProductId, string toProductId)
        {
            try
            {
                var fromProduct = await _productRepository.GetByIdAsync(fromProductId);
                var toProduct = await _productRepository.GetByIdAsync(toProductId);
                
                if (fromProduct == null || toProduct == null)
                {
                    return false;
                }

                // Para esta implementação, consideramos que qualquer produto ativo é compatível
                // No futuro, podemos implementar lógica mais complexa baseada em categorias, 
                // tipos de produtos, etc.
                return fromProduct.StateCode == ObjectState.Active && 
                       toProduct.StateCode == ObjectState.Active;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar compatibilidade dos produtos: {FromProductId} -> {ToProductId}", fromProductId, toProductId);
                return false;
            }
        }

        // Bulk Operations
        public async Task<bool> DeleteByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                var rules = await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
                foreach (var rule in rules)
                {
                    await _exchangeRuleRepository.DeleteAsync(rule.Id);
                }

                _logger.LogInformation("Regras de troca do grupo excluídas com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir regras de troca do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> CreateBulkAsync(IEnumerable<ProductGroupExchangeRule> exchangeRules)
        {
            var createdRules = new List<ProductGroupExchangeRule>();

            try
            {
                foreach (var rule in exchangeRules)
                {
                    var createdRuleId = await CreateAsync(rule);
                    
                    // Get the created rule by ID to return the full entity
                    var createdRule = await GetByIdAsync(createdRuleId);
                    if (createdRule != null)
                    {
                        createdRules.Add(createdRule);
                    }
                }

                _logger.LogInformation("Criadas {Count} regras de troca em lote", createdRules.Count);
                return createdRules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar regras de troca em lote");
                throw;
            }
        }

        public async Task<bool> UpdateGroupExchangeRulesAsync(string productGroupId, IEnumerable<ProductGroupExchangeRule> exchangeRules)
        {
            try
            {
                // Excluir regras existentes
                await DeleteExchangeRulesByGroupAsync(productGroupId);

                // Criar novas regras
                await CreateBulkAsync(exchangeRules);

                _logger.LogInformation("Regras de troca do grupo atualizadas com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar regras de troca do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        // New methods to match the updated interface
        public async Task<IEnumerable<ProductGroupExchangeRule>> GetBySourceGroupItemIdAsync(string sourceGroupItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceGroupItemId))
                {
                    _logger.LogWarning("ID do item original não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetBySourceGroupItemIdAsync(sourceGroupItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca por item original: {SourceGroupItemId}", sourceGroupItemId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByTargetGroupItemIdAsync(string targetGroupItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(targetGroupItemId))
                {
                    _logger.LogWarning("ID do item de troca não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetByTargetGroupItemIdAsync(targetGroupItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca por item de troca: {TargetGroupItemId}", targetGroupItemId);
                throw;
            }
        }

        public async Task<bool> ValidateGroupItemsCompatibilityAsync(string sourceGroupItemId, string targetGroupItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceGroupItemId) || string.IsNullOrEmpty(targetGroupItemId))
                {
                    _logger.LogWarning("IDs dos itens não fornecidos para validação de compatibilidade");
                    return false;
                }

                return await _exchangeRuleRepository.ValidateGroupItemsCompatibilityAsync(sourceGroupItemId, targetGroupItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar compatibilidade dos itens: {SourceId} -> {TargetId}", sourceGroupItemId, targetGroupItemId);
                throw;
            }
        }

        public async Task<decimal> CalculateEffectiveRatioAsync(string exchangeRuleId, int sourceWeight, int targetWeight)
        {
            try
            {
                if (string.IsNullOrEmpty(exchangeRuleId))
                {
                    _logger.LogWarning("ID da regra de troca não fornecido");
                    return 1.0m;
                }

                var rule = await _exchangeRuleRepository.GetByIdAsync(exchangeRuleId);
                if (rule == null)
                {
                    _logger.LogWarning("Regra de troca não encontrada: {ExchangeRuleId}", exchangeRuleId);
                    return 1.0m;
                }

                return rule.CalculateEffectiveRatio();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular proporção efetiva da regra: {ExchangeRuleId}", exchangeRuleId);
                throw;
            }
        }

        public async Task<bool> ValidateExchangeRuleAsync(ProductGroupExchangeRule exchangeRule)
        {
            try
            {
                if (exchangeRule == null)
                {
                    _logger.LogWarning("Regra de troca não fornecida para validação");
                    return false;
                }

                // Validar se o grupo existe
                var group = await _productGroupRepository.GetByIdAsync(exchangeRule.ProductGroupId);
                if (group == null)
                {
                    _logger.LogWarning("Grupo de produto não encontrado: {ProductGroupId}", exchangeRule.ProductGroupId);
                    return false;
                }

                // Validar se não existe regra duplicada
                var existingRule = await _exchangeRuleRepository.ExchangeRuleExistsAsync(
                    exchangeRule.ProductGroupId,
                    exchangeRule.SourceGroupItemId,
                    exchangeRule.TargetGroupItemId);

                if (existingRule)
                {
                    _logger.LogWarning("Regra de troca já existe para o grupo: {ProductGroupId}", exchangeRule.ProductGroupId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar regra de troca");
                throw;
            }
        }

        public async Task<bool> ValidateExchangeRatioAsync(decimal exchangeRatio)
        {
            try
            {
                return exchangeRatio > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar proporção de troca: {ExchangeRatio}", exchangeRatio);
                throw;
            }
        }

        public async Task<bool> ValidateGroupItemWeightsAsync(int sourceWeight, int targetWeight)
        {
            try
            {
                return sourceWeight > 0 && targetWeight > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar pesos dos itens: {SourceWeight}, {TargetWeight}", sourceWeight, targetWeight);
                throw;
            }
        }

        public async Task<decimal> GetAverageExchangeRatioAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return 1.0m;
                }

                var rules = await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
                if (!rules.Any())
                {
                    return 1.0m;
                }

                return rules.Average(r => r.ExchangeRatio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular proporção média de troca: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        public async Task<int> CountExchangeRulesAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return 0;
                }

                var rules = await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
                return rules.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar regras de troca: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> HasActiveExchangeRulesAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return false;
                }

                var rules = await _exchangeRuleRepository.GetActiveByGroupAsync(productGroupId);
                return rules.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar regras de troca ativas: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> UpdateExchangeRuleStatusAsync(string exchangeRuleId, bool isActive)
        {
            try
            {
                if (string.IsNullOrEmpty(exchangeRuleId))
                {
                    _logger.LogWarning("ID da regra de troca não fornecido");
                    return false;
                }

                var rule = await _exchangeRuleRepository.GetByIdAsync(exchangeRuleId);
                if (rule == null)
                {
                    _logger.LogWarning("Regra de troca não encontrada: {ExchangeRuleId}", exchangeRuleId);
                    return false;
                }

                rule.IsActive = isActive;
                return await _exchangeRuleRepository.UpdateAsync(rule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status da regra de troca: {ExchangeRuleId}", exchangeRuleId);
                throw;
            }
        }

        public async Task<bool> UpdateMultipleExchangeRulesAsync(IEnumerable<ProductGroupExchangeRule> exchangeRules)
        {
            try
            {
                if (exchangeRules == null || !exchangeRules.Any())
                {
                    _logger.LogWarning("Nenhuma regra de troca fornecida para atualização");
                    return false;
                }

                var updateTasks = exchangeRules.Select(rule => _exchangeRuleRepository.UpdateAsync(rule));
                var results = await Task.WhenAll(updateTasks);

                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar múltiplas regras de troca");
                throw;
            }
        }

        public async Task<bool> DeleteExchangeRulesByGroupAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return false;
                }

                var rules = await _exchangeRuleRepository.GetByProductGroupIdAsync(productGroupId);
                if (!rules.Any())
                {
                    return true;
                }

                var deleteTasks = rules.Select(rule => _exchangeRuleRepository.DeleteAsync(rule.Id));
                var results = await Task.WhenAll(deleteTasks);

                return results.All(r => r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir regras de troca do grupo: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        // Keep existing methods but mark them as deprecated - removed duplicate

        // Add missing methods from interface
        public async Task<IEnumerable<ProductGroupExchangeRule>> GetActiveByGroupAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetActiveByGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca ativas do grupo: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                if (page < 1)
                {
                    _logger.LogWarning("Página deve ser maior que zero");
                    page = 1;
                }

                if (pageSize < 1)
                {
                    _logger.LogWarning("Tamanho da página deve ser maior que zero");
                    pageSize = 10;
                }

                return await _exchangeRuleRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca paginadas: página {Page}, tamanho {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                if (page < 1)
                {
                    _logger.LogWarning("Página deve ser maior que zero");
                    page = 1;
                }

                if (pageSize < 1)
                {
                    _logger.LogWarning("Tamanho da página deve ser maior que zero");
                    pageSize = 10;
                }

                return await _exchangeRuleRepository.GetByProductGroupPagedAsync(productGroupId, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca paginadas do grupo: {ProductGroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetAvailableExchangesAsync(string productGroupId, string sourceGroupItemId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo de produto não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                if (string.IsNullOrEmpty(sourceGroupItemId))
                {
                    _logger.LogWarning("ID do item original não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetAvailableExchangesAsync(productGroupId, sourceGroupItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar trocas disponíveis: {ProductGroupId} -> {SourceGroupItemId}", productGroupId, sourceGroupItemId);
                throw;
            }
        }
    }
} 