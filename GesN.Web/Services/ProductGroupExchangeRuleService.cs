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

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByFromProductIdAsync(string fromProductId)
        {
            try
            {
                if (string.IsNullOrEmpty(fromProductId))
                {
                    _logger.LogWarning("ID do produto original não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetByOriginalProductIdAsync(fromProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca do produto original: {OriginalProductId}", fromProductId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupExchangeRule>> GetByToProductIdAsync(string toProductId)
        {
            try
            {
                if (string.IsNullOrEmpty(toProductId))
                {
                    _logger.LogWarning("ID do produto de troca não fornecido");
                    return Enumerable.Empty<ProductGroupExchangeRule>();
                }

                return await _exchangeRuleRepository.GetByExchangeProductIdAsync(toProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras de troca do produto de troca: {ExchangeProductId}", toProductId);
                throw;
            }
        }

        public async Task<ProductGroupExchangeRule> CreateAsync(ProductGroupExchangeRule exchangeRule)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateExchangeRuleAsync(exchangeRule);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                // Verificar se a regra de troca já existe
                if (await ExchangeRuleExistsAsync(exchangeRule.ProductGroupId, exchangeRule.OriginalProductId, exchangeRule.ExchangeProductId))
                {
                    throw new InvalidOperationException("Já existe uma regra de troca entre estes produtos no grupo");
                }

                // Definir valores padrão
                exchangeRule.Id = Guid.NewGuid().ToString();
                exchangeRule.CreatedAt = DateTime.UtcNow;
                exchangeRule.StateCode = ObjectState.Active;

                var ruleId = await _exchangeRuleRepository.CreateAsync(exchangeRule);
                exchangeRule.Id = ruleId;
                
                _logger.LogInformation("Regra de troca criada com sucesso: {Id}", ruleId);
                return exchangeRule;
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
                var validationErrors = await ValidateExchangeRuleAsync(exchangeRule);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
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

        public async Task<bool> ExchangeRuleExistsAsync(string productGroupId, string fromProductId, string toProductId)
        {
            try
            {
                return await _exchangeRuleRepository.ExchangeRuleExistsAsync(productGroupId, fromProductId, toProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se regra de troca existe: {GroupId}, {FromProductId}, {ToProductId}", 
                    productGroupId, fromProductId, toProductId);
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

        public async Task<decimal> GetExchangePriceAsync(string exchangeRuleId)
        {
            try
            {
                var rule = await _exchangeRuleRepository.GetByIdAsync(exchangeRuleId);
                return rule?.AdditionalCost ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter preço da troca: {Id}", exchangeRuleId);
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

        public async Task<(IEnumerable<ProductGroupExchangeRule> Rules, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            try
            {
                var rules = await _exchangeRuleRepository.GetByProductGroupPagedAsync(productGroupId, page, pageSize);
                return (rules, rules.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar regras do grupo paginadas: {GroupId}", productGroupId);
                throw;
            }
        }

        // Validation
        public async Task<IEnumerable<string>> ValidateExchangeRuleAsync(ProductGroupExchangeRule exchangeRule)
        {
            var errors = new List<string>();

            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(exchangeRule.ProductGroupId))
                {
                    errors.Add("ID do grupo é obrigatório");
                }

                if (string.IsNullOrEmpty(exchangeRule.OriginalProductId))
                {
                    errors.Add("Produto original é obrigatório");
                }

                if (string.IsNullOrEmpty(exchangeRule.ExchangeProductId))
                {
                    errors.Add("Produto de troca é obrigatório");
                }

                if (exchangeRule.OriginalProductId == exchangeRule.ExchangeProductId)
                {
                    errors.Add("Produto original e de troca devem ser diferentes");
                }

                if (exchangeRule.AdditionalCost < 0)
                {
                    errors.Add("Custo adicional não pode ser negativo");
                }

                // Validar se o grupo existe
                if (!string.IsNullOrEmpty(exchangeRule.ProductGroupId))
                {
                    var productGroup = await _productGroupRepository.GetByIdAsync(exchangeRule.ProductGroupId);
                    if (productGroup == null)
                    {
                        errors.Add("Grupo de produto não encontrado");
                    }
                }

                // Validar se os produtos existem
                if (!string.IsNullOrEmpty(exchangeRule.OriginalProductId))
                {
                    var originalProduct = await _productRepository.GetByIdAsync(exchangeRule.OriginalProductId);
                    if (originalProduct == null)
                    {
                        errors.Add("Produto original não encontrado");
                    }
                    else if (originalProduct.StateCode != ObjectState.Active)
                    {
                        errors.Add("Produto original não está ativo");
                    }
                }

                if (!string.IsNullOrEmpty(exchangeRule.ExchangeProductId))
                {
                    var exchangeProduct = await _productRepository.GetByIdAsync(exchangeRule.ExchangeProductId);
                    if (exchangeProduct == null)
                    {
                        errors.Add("Produto de troca não encontrado");
                    }
                    else if (exchangeProduct.StateCode != ObjectState.Active)
                    {
                        errors.Add("Produto de troca não está ativo");
                    }
                }

                // Validar compatibilidade dos produtos
                if (!string.IsNullOrEmpty(exchangeRule.OriginalProductId) && !string.IsNullOrEmpty(exchangeRule.ExchangeProductId))
                {
                    var areCompatible = await ValidateProductsCompatibilityAsync(exchangeRule.OriginalProductId, exchangeRule.ExchangeProductId);
                    if (!areCompatible)
                    {
                        errors.Add("Os produtos não são compatíveis para troca");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar regra de troca");
                errors.Add("Erro interno na validação");
            }

            return errors;
        }

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

                // TODO: Implementar lógica específica de compatibilidade se necessário
                // Por enquanto, sempre retorna true (todos os produtos são compatíveis)
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar compatibilidade dos produtos: {FromProductId}, {ToProductId}", 
                    fromProductId, toProductId);
                throw;
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
                    var createdRule = await CreateAsync(rule);
                    createdRules.Add(createdRule);
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
                await DeleteByProductGroupIdAsync(productGroupId);

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
    }
} 