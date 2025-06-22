using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Services
{
    public class ProductIngredientService : IProductIngredientService
    {
        private readonly IProductIngredientRepository _productIngredientRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILogger<ProductIngredientService> _logger;

        public ProductIngredientService(
            IProductIngredientRepository productIngredientRepository,
            IIngredientRepository ingredientRepository,
            ILogger<ProductIngredientService> logger)
        {
            _productIngredientRepository = productIngredientRepository;
            _ingredientRepository = ingredientRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductIngredient>> GetAllProductIngredientsAsync()
        {
            try
            {
                return await _productIngredientRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os relacionamentos produto-ingrediente");
                throw;
            }
        }

        public async Task<ProductIngredient?> GetProductIngredientByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _productIngredientRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relacionamento produto-ingrediente por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductIngredient>> GetProductIngredientsByProductIdAsync(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return Enumerable.Empty<ProductIngredient>();

                return await _productIngredientRepository.GetByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductIngredient>> GetProductIngredientsByIngredientIdAsync(string ingredientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ingredientId))
                    return Enumerable.Empty<ProductIngredient>();

                return await _productIngredientRepository.GetByIngredientIdAsync(ingredientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos que usam o ingrediente: {IngredientId}", ingredientId);
                throw;
            }
        }

        public async Task<ProductIngredient?> GetProductIngredientByProductAndIngredientAsync(string productId, string ingredientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(ingredientId))
                    return null;

                return await _productIngredientRepository.GetByProductAndIngredientAsync(productId, ingredientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relacionamento específico produto-ingrediente: {ProductId}, {IngredientId}", productId, ingredientId);
                throw;
            }
        }

        public async Task<string> CreateProductIngredientAsync(ProductIngredient productIngredient, string createdBy)
        {
            try
            {
                if (!await ValidateProductIngredientDataAsync(productIngredient))
                {
                    throw new ArgumentException("Dados do relacionamento produto-ingrediente inválidos");
                }

                // Verificar se já existe relacionamento entre este produto e ingrediente
                var existingRelation = await _productIngredientRepository.GetByProductAndIngredientAsync(
                    productIngredient.ProductId, productIngredient.IngredientId);
                if (existingRelation != null)
                {
                    throw new InvalidOperationException("Já existe um relacionamento entre este produto e ingrediente");
                }

                productIngredient.SetCreatedBy(createdBy);
                var relationId = await _productIngredientRepository.CreateAsync(productIngredient);

                _logger.LogInformation("Relacionamento produto-ingrediente criado: {RelationId} por {CreatedBy}", relationId, createdBy);
                return relationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar relacionamento produto-ingrediente");
                throw;
            }
        }

        public async Task<bool> UpdateProductIngredientAsync(ProductIngredient productIngredient, string modifiedBy)
        {
            try
            {
                if (!await ValidateProductIngredientDataAsync(productIngredient))
                {
                    throw new ArgumentException("Dados do relacionamento produto-ingrediente inválidos");
                }

                // Verificar se o relacionamento existe
                var existingRelation = await _productIngredientRepository.GetByIdAsync(productIngredient.Id);
                if (existingRelation == null)
                {
                    throw new InvalidOperationException("Relacionamento produto-ingrediente não encontrado");
                }

                productIngredient.UpdateModification(modifiedBy);
                var success = await _productIngredientRepository.UpdateAsync(productIngredient);

                if (success)
                {
                    _logger.LogInformation("Relacionamento produto-ingrediente atualizado: {RelationId} por {ModifiedBy}", productIngredient.Id, modifiedBy);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar relacionamento produto-ingrediente: {RelationId}", productIngredient.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProductIngredientAsync(string id)
        {
            try
            {
                var relation = await _productIngredientRepository.GetByIdAsync(id);
                if (relation == null)
                {
                    return false;
                }

                var success = await _productIngredientRepository.DeleteAsync(id);
                
                if (success)
                {
                    _logger.LogInformation("Relacionamento produto-ingrediente excluído: {RelationId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir relacionamento produto-ingrediente: {RelationId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteProductIngredientsByProductIdAsync(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return false;

                var success = await _productIngredientRepository.DeleteByProductIdAsync(productId);
                
                if (success)
                {
                    _logger.LogInformation("Relacionamentos produto-ingrediente excluídos para produto: {ProductId}", productId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir relacionamentos do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> DeleteProductIngredientsByIngredientIdAsync(string ingredientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ingredientId))
                    return false;

                var success = await _productIngredientRepository.DeleteByIngredientIdAsync(ingredientId);
                
                if (success)
                {
                    _logger.LogInformation("Relacionamentos produto-ingrediente excluídos para ingrediente: {IngredientId}", ingredientId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir relacionamentos do ingrediente: {IngredientId}", ingredientId);
                throw;
            }
        }

        public async Task<bool> ProductIngredientExistsAsync(string id)
        {
            try
            {
                return await _productIngredientRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do relacionamento produto-ingrediente: {RelationId}", id);
                throw;
            }
        }

        public async Task<int> GetProductIngredientCountAsync()
        {
            try
            {
                return await _productIngredientRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar relacionamentos produto-ingrediente");
                throw;
            }
        }

        public async Task<IEnumerable<ProductIngredient>> GetPagedProductIngredientsAsync(int page, int pageSize)
        {
            try
            {
                return await _productIngredientRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar relacionamentos produto-ingrediente paginados: página {Page}, tamanho {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ValidateProductIngredientDataAsync(ProductIngredient productIngredient)
        {
            try
            {
                return productIngredient != null && productIngredient.HasCompleteData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados do relacionamento produto-ingrediente");
                return false;
            }
        }

        public async Task<decimal> CalculateProductIngredientCostAsync(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return 0;

                var productIngredients = await _productIngredientRepository.GetByProductIdAsync(productId);
                decimal totalCost = 0;

                foreach (var productIngredient in productIngredients)
                {
                    if (productIngredient.Ingredient != null)
                    {
                        totalCost += productIngredient.Quantity * productIngredient.Ingredient.CostPerUnit;
                    }
                    else
                    {
                        // Buscar o ingrediente se não estiver carregado
                        var ingredient = await _ingredientRepository.GetByIdAsync(productIngredient.IngredientId);
                        if (ingredient != null)
                        {
                            totalCost += productIngredient.Quantity * ingredient.CostPerUnit;
                        }
                    }
                }

                return totalCost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular custo dos ingredientes do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> HasSufficientStockForProductAsync(string productId, int quantity = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return false;

                var productIngredients = await _productIngredientRepository.GetByProductIdAsync(productId);

                foreach (var productIngredient in productIngredients)
                {
                    if (productIngredient.IsOptional)
                        continue;

                    var ingredient = productIngredient.Ingredient;
                    if (ingredient == null)
                    {
                        // Buscar o ingrediente se não estiver carregado
                        ingredient = await _ingredientRepository.GetByIdAsync(productIngredient.IngredientId);
                    }

                    if (ingredient == null)
                        return false;

                    var requiredQuantity = productIngredient.Quantity * quantity;
                    if (ingredient.CurrentStock < requiredQuantity)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar estoque suficiente para produto: {ProductId}", productId);
                throw;
            }
        }
    }
} 