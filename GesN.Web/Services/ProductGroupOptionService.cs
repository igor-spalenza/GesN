using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    public class ProductGroupOptionService : IProductGroupOptionService
    {
        private readonly IProductGroupOptionRepository _productGroupOptionRepository;
        private readonly IProductGroupRepository _productGroupRepository;
        private readonly ILogger<ProductGroupOptionService> _logger;

        public ProductGroupOptionService(
            IProductGroupOptionRepository productGroupOptionRepository,
            IProductGroupRepository productGroupRepository,
            ILogger<ProductGroupOptionService> logger)
        {
            _productGroupOptionRepository = productGroupOptionRepository;
            _productGroupRepository = productGroupRepository;
            _logger = logger;
        }

        // CRUD Operations
        public async Task<ProductGroupOption?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID da opção do grupo não fornecido");
                    return null;
                }

                return await _productGroupOptionRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar opção do grupo por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupOption>> GetAllAsync()
        {
            try
            {
                return await _productGroupOptionRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as opções dos grupos");
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupOption>> GetByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                if (string.IsNullOrEmpty(productGroupId))
                {
                    _logger.LogWarning("ID do grupo não fornecido");
                    return Enumerable.Empty<ProductGroupOption>();
                }

                return await _productGroupOptionRepository.GetByProductGroupIdAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar opções do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<ProductGroupOption> CreateAsync(ProductGroupOption groupOption)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateGroupOptionAsync(groupOption);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                // Verificar se o nome da opção já existe no grupo
                if (await ValidateOptionNameAsync(groupOption.ProductGroupId, groupOption.Name))
                {
                    throw new InvalidOperationException("Já existe uma opção com este nome no grupo");
                }

                // Definir valores padrão
                groupOption.Id = Guid.NewGuid().ToString();
                groupOption.CreatedAt = DateTime.UtcNow;
                groupOption.StateCode = ObjectState.Active;

                var optionId = await _productGroupOptionRepository.CreateAsync(groupOption);
                groupOption.Id = optionId;
                
                _logger.LogInformation("Opção do grupo criada com sucesso: {Id}", optionId);
                return groupOption;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar opção do grupo");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProductGroupOption groupOption)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateGroupOptionAsync(groupOption);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                // Verificar se o nome da opção já existe no grupo (excluindo o próprio item)
                if (await ValidateOptionNameAsync(groupOption.ProductGroupId, groupOption.Name, groupOption.Id))
                {
                    throw new InvalidOperationException("Já existe uma opção com este nome no grupo");
                }

                groupOption.LastModifiedAt = DateTime.UtcNow;
                var result = await _productGroupOptionRepository.UpdateAsync(groupOption);
                
                if (result)
                {
                    _logger.LogInformation("Opção do grupo atualizada com sucesso: {Id}", groupOption.Id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar opção do grupo: {Id}", groupOption.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (!await CanDeleteAsync(id))
                {
                    throw new InvalidOperationException("Esta opção do grupo não pode ser excluída.");
                }

                var result = await _productGroupOptionRepository.DeleteAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Opção do grupo excluída com sucesso: {Id}", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir opção do grupo: {Id}", id);
                throw;
            }
        }

        // Business Operations
        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                return await _productGroupOptionRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da opção do grupo: {Id}", id);
                throw;
            }
        }

        public async Task<int> CountByProductGroupAsync(string productGroupId)
        {
            try
            {
                return await _productGroupOptionRepository.CountByProductGroupAsync(productGroupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar opções do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<bool> CanDeleteAsync(string id)
        {
            try
            {
                var option = await _productGroupOptionRepository.GetByIdAsync(id);
                if (option == null)
                {
                    return false;
                }

                // Verificar se a opção pode ser excluída (regras de negócio)
                // TODO: Implementar lógica específica conforme necessário
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se opção pode ser excluída: {Id}", id);
                throw;
            }
        }

        // Search and Filter
        public async Task<IEnumerable<ProductGroupOption>> SearchAsync(string searchTerm)
        {
            try
            {
                return await _productGroupOptionRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar opções por termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<(IEnumerable<ProductGroupOption> Options, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
        {
            try
            {
                IEnumerable<ProductGroupOption> options;
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    options = await _productGroupOptionRepository.SearchAsync(searchTerm);
                }
                else
                {
                    options = await _productGroupOptionRepository.GetPagedAsync(page, pageSize);
                }
                
                return (options, options.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar opções paginadas");
                throw;
            }
        }

        public async Task<(IEnumerable<ProductGroupOption> Options, int TotalCount)> GetByProductGroupPagedAsync(string productGroupId, int page, int pageSize)
        {
            try
            {
                var options = await _productGroupOptionRepository.GetByProductGroupPagedAsync(productGroupId, page, pageSize);
                return (options, options.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar opções do grupo paginadas: {GroupId}", productGroupId);
                throw;
            }
        }

        // Validation
        public async Task<IEnumerable<string>> ValidateGroupOptionAsync(ProductGroupOption groupOption)
        {
            var errors = new List<string>();

            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(groupOption.ProductGroupId))
                {
                    errors.Add("ID do grupo é obrigatório");
                }

                if (string.IsNullOrEmpty(groupOption.Name))
                {
                    errors.Add("Nome da opção é obrigatório");
                }

                if (groupOption.Name?.Length > 100)
                {
                    errors.Add("Nome da opção não pode ter mais de 100 caracteres");
                }

                // Validar se o grupo existe
                if (!string.IsNullOrEmpty(groupOption.ProductGroupId))
                {
                    var productGroup = await _productGroupRepository.GetByIdAsync(groupOption.ProductGroupId);
                    if (productGroup == null)
                    {
                        errors.Add("Grupo de produto não encontrado");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar opção do grupo");
                errors.Add("Erro interno na validação");
            }

            return errors;
        }

        public async Task<bool> ValidateOptionNameAsync(string productGroupId, string optionName, string? excludeId = null)
        {
            try
            {
                var options = await _productGroupOptionRepository.GetByProductGroupIdAsync(productGroupId);
                var existingOption = options.FirstOrDefault(o => o.Name == optionName);
                
                if (existingOption == null) return false;
                if (!string.IsNullOrEmpty(excludeId) && existingOption.Id == excludeId) return false;
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar nome da opção: {OptionName}", optionName);
                throw;
            }
        }

        // Bulk Operations
        public async Task<bool> DeleteByProductGroupIdAsync(string productGroupId)
        {
            try
            {
                var options = await _productGroupOptionRepository.GetByProductGroupIdAsync(productGroupId);
                foreach (var option in options)
                {
                    await _productGroupOptionRepository.DeleteAsync(option.Id);
                }

                _logger.LogInformation("Opções do grupo excluídas com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir opções do grupo: {GroupId}", productGroupId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductGroupOption>> CreateBulkAsync(IEnumerable<ProductGroupOption> groupOptions)
        {
            var createdOptions = new List<ProductGroupOption>();

            try
            {
                foreach (var option in groupOptions)
                {
                    var createdOption = await CreateAsync(option);
                    createdOptions.Add(createdOption);
                }

                _logger.LogInformation("Criadas {Count} opções de grupo em lote", createdOptions.Count);
                return createdOptions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar opções do grupo em lote");
                throw;
            }
        }

        public async Task<bool> UpdateGroupOptionsAsync(string productGroupId, IEnumerable<ProductGroupOption> groupOptions)
        {
            try
            {
                // Excluir opções existentes
                await DeleteByProductGroupIdAsync(productGroupId);

                // Criar novas opções
                await CreateBulkAsync(groupOptions);

                _logger.LogInformation("Opções do grupo atualizadas com sucesso: {GroupId}", productGroupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar opções do grupo: {GroupId}", productGroupId);
                throw;
            }
        }
    }
} 