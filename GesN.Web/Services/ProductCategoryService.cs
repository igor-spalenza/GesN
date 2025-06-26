using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly ILogger<ProductCategoryService> _logger;

        public ProductCategoryService(
            IProductCategoryRepository productCategoryRepository,
            ILogger<ProductCategoryService> logger)
        {
            _productCategoryRepository = productCategoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            try
            {
                return await _productCategoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                throw;
            }
        }

        public async Task<ProductCategory?> GetCategoryByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _productCategoryRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria por ID: {Id}", id);
                throw;
            }
        }

        public async Task<ProductCategory?> GetCategoryByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return await _productCategoryRepository.GetByNameAsync(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria por nome: {Name}", name);
                throw;
            }
        }

        public async Task<IEnumerable<ProductCategory>> GetActiveCategoriesAsync()
        {
            try
            {
                return await _productCategoryRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias ativas");
                throw;
            }
        }

        public async Task<IEnumerable<ProductCategory>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveCategoriesAsync();

                return await _productCategoryRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar categorias: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<ProductCategory>> SearchCategoriesForAutocompleteAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<ProductCategory>();

                return await _productCategoryRepository.SearchForAutocompleteAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar categorias para autocomplete: {SearchTerm}", searchTerm);
                return Enumerable.Empty<ProductCategory>();
            }
        }

        public async Task<string> CreateCategoryAsync(ProductCategory category, string createdBy)
        {
            try
            {
                if (!await ValidateCategoryDataAsync(category))
                {
                    throw new ArgumentException("Dados da categoria inválidos");
                }

                // Verificar se já existe categoria com o mesmo nome
                var existingCategory = await _productCategoryRepository.GetByNameAsync(category.Name);
                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"Já existe uma categoria com o nome: {category.Name}");
                }

                category.SetCreatedBy(createdBy);
                var categoryId = await _productCategoryRepository.CreateAsync(category);

                _logger.LogInformation("Categoria criada com sucesso: {CategoryId} por {CreatedBy}", categoryId, createdBy);
                return categoryId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria: {CategoryName}", category.Name);
                throw;
            }
        }

        public async Task<bool> UpdateCategoryAsync(ProductCategory category, string modifiedBy)
        {
            try
            {
                if (!await ValidateCategoryDataAsync(category))
                {
                    throw new ArgumentException("Dados da categoria inválidos");
                }

                // Verificar se a categoria existe
                var existingCategory = await _productCategoryRepository.GetByIdAsync(category.Id);
                if (existingCategory == null)
                {
                    throw new InvalidOperationException("Categoria não encontrada");
                }

                // Verificar se não há outra categoria com o mesmo nome (exceto ela mesma)
                var categoryWithSameName = await _productCategoryRepository.GetByNameAsync(category.Name);
                if (categoryWithSameName != null && categoryWithSameName.Id != category.Id)
                {
                    throw new InvalidOperationException($"Já existe uma categoria com o nome: {category.Name}");
                }

                category.UpdateModification(modifiedBy);
                var success = await _productCategoryRepository.UpdateAsync(category);

                if (success)
                {
                    _logger.LogInformation("Categoria atualizada com sucesso: {CategoryId} por {ModifiedBy}", category.Id, modifiedBy);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar categoria: {CategoryId}", category.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            try
            {
                var category = await _productCategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return false;
                }

                var success = await _productCategoryRepository.DeleteAsync(id);
                
                if (success)
                {
                    _logger.LogInformation("Categoria excluída com sucesso: {CategoryId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria: {CategoryId}", id);
                throw;
            }
        }

        public async Task<bool> CategoryExistsAsync(string id)
        {
            try
            {
                return await _productCategoryRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da categoria: {CategoryId}", id);
                throw;
            }
        }

        public async Task<int> GetCategoryCountAsync()
        {
            try
            {
                return await _productCategoryRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar categorias");
                throw;
            }
        }

        public async Task<int> GetActiveCategoryCountAsync()
        {
            try
            {
                var activeCategories = await GetActiveCategoriesAsync();
                return activeCategories.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar categorias ativas");
                throw;
            }
        }

        public async Task<IEnumerable<ProductCategory>> GetPagedCategoriesAsync(int page, int pageSize)
        {
            try
            {
                return await _productCategoryRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias paginadas: página {Page}, tamanho {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ValidateCategoryDataAsync(ProductCategory category)
        {
            try
            {
                return category != null && category.HasCompleteData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados da categoria");
                return false;
            }
        }
    }
} 