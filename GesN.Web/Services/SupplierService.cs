using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(
            ISupplierRepository supplierRepository,
            ILogger<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            try
            {
                return await _supplierRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os fornecedores");
                throw;
            }
        }

        public async Task<Supplier?> GetSupplierByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _supplierRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedor por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Supplier?> GetSupplierByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return await _supplierRepository.GetByNameAsync(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedor por nome: {Name}", name);
                throw;
            }
        }

        public async Task<Supplier?> GetSupplierByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return null;

                return await _supplierRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedor por email: {Email}", email);
                throw;
            }
        }

        public async Task<Supplier?> GetSupplierByDocumentAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    return null;

                return await _supplierRepository.GetByDocumentAsync(documentNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedor por documento: {DocumentNumber}", documentNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
        {
            try
            {
                return await _supplierRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedores ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveSuppliersAsync();

                return await _supplierRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar fornecedores: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersForAutocompleteAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<Supplier>();

                return await _supplierRepository.SearchForAutocompleteAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar fornecedores para autocomplete: {SearchTerm}", searchTerm);
                return Enumerable.Empty<Supplier>();
            }
        }

        public async Task<string> CreateSupplierAsync(Supplier supplier, string createdBy)
        {
            try
            {
                if (!await ValidateSupplierDataAsync(supplier))
                {
                    throw new ArgumentException("Dados do fornecedor inválidos");
                }

                // Verificar se já existe fornecedor com o mesmo email
                if (!string.IsNullOrWhiteSpace(supplier.Email))
                {
                    var existingSupplier = await _supplierRepository.GetByEmailAsync(supplier.Email);
                    if (existingSupplier != null)
                    {
                        throw new InvalidOperationException($"Já existe um fornecedor com o email: {supplier.Email}");
                    }
                }

                // Verificar se já existe fornecedor com o mesmo documento
                if (!string.IsNullOrWhiteSpace(supplier.DocumentNumber))
                {
                    var existingSupplier = await _supplierRepository.GetByDocumentAsync(supplier.DocumentNumber);
                    if (existingSupplier != null)
                    {
                        throw new InvalidOperationException($"Já existe um fornecedor com o documento: {supplier.DocumentNumber}");
                    }
                }

                supplier.SetCreatedBy(createdBy);
                var supplierId = await _supplierRepository.CreateAsync(supplier);

                _logger.LogInformation("Fornecedor criado com sucesso: {SupplierId} por {CreatedBy}", supplierId, createdBy);
                return supplierId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar fornecedor: {SupplierName}", supplier.Name);
                throw;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier, string modifiedBy)
        {
            try
            {
                if (!await ValidateSupplierDataAsync(supplier))
                {
                    throw new ArgumentException("Dados do fornecedor inválidos");
                }

                // Verificar se o fornecedor existe
                var existingSupplier = await _supplierRepository.GetByIdAsync(supplier.Id);
                if (existingSupplier == null)
                {
                    throw new InvalidOperationException("Fornecedor não encontrado");
                }

                // Verificar se não há outro fornecedor com o mesmo email (exceto ele mesmo)
                if (!string.IsNullOrWhiteSpace(supplier.Email))
                {
                    var supplierWithSameEmail = await _supplierRepository.GetByEmailAsync(supplier.Email);
                    if (supplierWithSameEmail != null && supplierWithSameEmail.Id != supplier.Id)
                    {
                        throw new InvalidOperationException($"Já existe um fornecedor com o email: {supplier.Email}");
                    }
                }

                // Verificar se não há outro fornecedor com o mesmo documento (exceto ele mesmo)
                if (!string.IsNullOrWhiteSpace(supplier.DocumentNumber))
                {
                    var supplierWithSameDocument = await _supplierRepository.GetByDocumentAsync(supplier.DocumentNumber);
                    if (supplierWithSameDocument != null && supplierWithSameDocument.Id != supplier.Id)
                    {
                        throw new InvalidOperationException($"Já existe um fornecedor com o documento: {supplier.DocumentNumber}");
                    }
                }

                supplier.UpdateModification(modifiedBy);
                var success = await _supplierRepository.UpdateAsync(supplier);

                if (success)
                {
                    _logger.LogInformation("Fornecedor atualizado com sucesso: {SupplierId} por {ModifiedBy}", supplier.Id, modifiedBy);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar fornecedor: {SupplierId}", supplier.Id);
                throw;
            }
        }

        public async Task<bool> DeleteSupplierAsync(string id)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdAsync(id);
                if (supplier == null)
                {
                    return false;
                }

                var success = await _supplierRepository.DeleteAsync(id);
                
                if (success)
                {
                    _logger.LogInformation("Fornecedor excluído com sucesso: {SupplierId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir fornecedor: {SupplierId}", id);
                throw;
            }
        }

        public async Task<bool> SupplierExistsAsync(string id)
        {
            try
            {
                return await _supplierRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do fornecedor: {SupplierId}", id);
                throw;
            }
        }

        public async Task<int> GetSupplierCountAsync()
        {
            try
            {
                return await _supplierRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar fornecedores");
                throw;
            }
        }

        public async Task<int> GetActiveSupplierCountAsync()
        {
            try
            {
                var activeSuppliers = await GetActiveSuppliersAsync();
                return activeSuppliers.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar fornecedores ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Supplier>> GetPagedSuppliersAsync(int page, int pageSize)
        {
            try
            {
                return await _supplierRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedores paginados: página {Page}, tamanho {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ValidateSupplierDataAsync(Supplier supplier)
        {
            try
            {
                return supplier != null && supplier.HasCompleteData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados do fornecedor");
                return false;
            }
        }
    }
} 