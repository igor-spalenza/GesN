using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILogger<IngredientService> _logger;

        public IngredientService(
            IIngredientRepository ingredientRepository,
            ILogger<IngredientService> logger)
        {
            _ingredientRepository = ingredientRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            try
            {
                return await _ingredientRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os ingredientes");
                throw;
            }
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _ingredientRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingrediente por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Ingredient?> GetIngredientByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return await _ingredientRepository.GetByNameAsync(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingrediente por nome: {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> GetActiveIngredientsAsync()
        {
            try
            {
                return await _ingredientRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> GetIngredientsBySupplierId(string supplierId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supplierId))
                    return Enumerable.Empty<Ingredient>();

                return await _ingredientRepository.GetBySupplierId(supplierId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes por fornecedor: {SupplierId}", supplierId);
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> GetLowStockIngredientsAsync()
        {
            try
            {
                return await _ingredientRepository.GetLowStockAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes com estoque baixo");
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> GetPerishableIngredientsAsync()
        {
            try
            {
                return await _ingredientRepository.GetPerishableAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes perecíveis");
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> SearchIngredientsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveIngredientsAsync();

                return await _ingredientRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar ingredientes: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> SearchIngredientsForAutocompleteAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<Ingredient>();

                return await _ingredientRepository.SearchForAutocompleteAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar ingredientes para autocomplete: {SearchTerm}", searchTerm);
                return Enumerable.Empty<Ingredient>();
            }
        }

        public async Task<string> CreateIngredientAsync(Ingredient ingredient, string createdBy)
        {
            try
            {
                if (!await ValidateIngredientDataAsync(ingredient))
                {
                    throw new ArgumentException("Dados do ingrediente inválidos");
                }

                // Verificar se já existe ingrediente com o mesmo nome
                var existingIngredient = await _ingredientRepository.GetByNameAsync(ingredient.Name);
                if (existingIngredient != null)
                {
                    throw new InvalidOperationException($"Já existe um ingrediente com o nome: {ingredient.Name}");
                }

                ingredient.SetCreatedBy(createdBy);
                var ingredientId = await _ingredientRepository.CreateAsync(ingredient);

                _logger.LogInformation("Ingrediente criado com sucesso: {IngredientId} por {CreatedBy}", ingredientId, createdBy);
                return ingredientId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ingrediente: {IngredientName}", ingredient.Name);
                throw;
            }
        }

        public async Task<bool> UpdateIngredientAsync(Ingredient ingredient, string modifiedBy)
        {
            try
            {
                if (!await ValidateIngredientDataAsync(ingredient))
                {
                    throw new ArgumentException("Dados do ingrediente inválidos");
                }

                // Verificar se o ingrediente existe
                var existingIngredient = await _ingredientRepository.GetByIdAsync(ingredient.Id);
                if (existingIngredient == null)
                {
                    throw new InvalidOperationException("Ingrediente não encontrado");
                }

                // Verificar se não há outro ingrediente com o mesmo nome (exceto ele mesmo)
                var ingredientWithSameName = await _ingredientRepository.GetByNameAsync(ingredient.Name);
                if (ingredientWithSameName != null && ingredientWithSameName.Id != ingredient.Id)
                {
                    throw new InvalidOperationException($"Já existe um ingrediente com o nome: {ingredient.Name}");
                }

                ingredient.UpdateModification(modifiedBy);
                var success = await _ingredientRepository.UpdateAsync(ingredient);

                if (success)
                {
                    _logger.LogInformation("Ingrediente atualizado com sucesso: {IngredientId} por {ModifiedBy}", ingredient.Id, modifiedBy);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ingrediente: {IngredientId}", ingredient.Id);
                throw;
            }
        }

        public async Task<bool> UpdateIngredientStockAsync(string id, decimal newStock, string modifiedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException("ID do ingrediente é obrigatório");
                }

                if (newStock < 0)
                {
                    throw new ArgumentException("O estoque não pode ser negativo");
                }

                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                if (ingredient == null)
                {
                    throw new InvalidOperationException("Ingrediente não encontrado");
                }

                var success = await _ingredientRepository.UpdateStockAsync(id, newStock);

                if (success)
                {
                    _logger.LogInformation("Estoque do ingrediente atualizado: {IngredientId} de {OldStock} para {NewStock} por {ModifiedBy}", 
                        id, ingredient.CurrentStock, newStock, modifiedBy);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar estoque do ingrediente: {IngredientId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteIngredientAsync(string id)
        {
            try
            {
                var ingredient = await _ingredientRepository.GetByIdAsync(id);
                if (ingredient == null)
                {
                    return false;
                }

                var success = await _ingredientRepository.DeleteAsync(id);
                
                if (success)
                {
                    _logger.LogInformation("Ingrediente excluído com sucesso: {IngredientId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir ingrediente: {IngredientId}", id);
                throw;
            }
        }

        public async Task<bool> IngredientExistsAsync(string id)
        {
            try
            {
                return await _ingredientRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do ingrediente: {IngredientId}", id);
                throw;
            }
        }

        public async Task<int> GetIngredientCountAsync()
        {
            try
            {
                return await _ingredientRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar ingredientes");
                throw;
            }
        }

        public async Task<int> GetActiveIngredientCountAsync()
        {
            try
            {
                var activeIngredients = await GetActiveIngredientsAsync();
                return activeIngredients.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar ingredientes ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Ingredient>> GetPagedIngredientsAsync(int page, int pageSize)
        {
            try
            {
                return await _ingredientRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ingredientes paginados: página {Page}, tamanho {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ValidateIngredientDataAsync(Ingredient ingredient)
        {
            try
            {
                return ingredient != null && ingredient.HasCompleteData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados do ingrediente");
                return false;
            }
        }
    }
} 