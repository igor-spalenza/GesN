using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Areas.Integration.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Entities.ValueObjects;
using System.Collections.Concurrent;

namespace GesN.Web.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IGooglePeopleService _googlePeopleService;
        private readonly ILogger<CustomerService> _logger;
        
        // Controle de sincronizações em andamento para evitar duplicatas
        private static readonly ConcurrentDictionary<string, bool> _syncingCustomers = new();

        public CustomerService(
            ICustomerRepository customerRepository, 
            IGooglePeopleService googlePeopleService,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _googlePeopleService = googlePeopleService;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _customerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os clientes");
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;

                return await _customerRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return null;

                return await _customerRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente por email: {Email}", email);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByDocumentAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    return null;

                return await _customerRepository.GetByDocumentAsync(documentNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente por documento: {DocumentNumber}", documentNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            try
            {
                return await _customerRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetActiveCustomersAsync();

                return await _customerRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar clientes: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersForAutocompleteAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<Customer>();

                return await _customerRepository.SearchForAutocompleteAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar clientes para autocomplete: {SearchTerm}", searchTerm);
                return Enumerable.Empty<Customer>();
            }
        }

        public async Task<string> CreateCustomerAsync(Customer customer, string createdBy)
        {
            try
            {
                // Validar dados do cliente
                if (!await ValidateCustomerDataAsync(customer))
                {
                    throw new ArgumentException("Dados do cliente inválidos");
                }

                // Verificar se já existe cliente com o mesmo email
                if (!string.IsNullOrWhiteSpace(customer.Email))
                {
                    var existingCustomer = await _customerRepository.GetByEmailAsync(customer.Email);
                    if (existingCustomer != null)
                    {
                        throw new InvalidOperationException($"Já existe um cliente com o email: {customer.Email}");
                    }
                }

                // Verificar se já existe cliente com o mesmo documento
                if (!string.IsNullOrWhiteSpace(customer.DocumentNumber))
                {
                    var existingCustomer = await _customerRepository.GetByDocumentAsync(customer.DocumentNumber);
                    if (existingCustomer != null)
                    {
                        throw new InvalidOperationException($"Já existe um cliente com o documento: {customer.DocumentNumber}");
                    }
                }

                // Definir quem criou
                customer.SetCreatedBy(createdBy);

                // PRIMEIRO: Criar cliente no banco de dados
                var customerId = await _customerRepository.CreateAsync(customer);

                _logger.LogInformation("Cliente criado com sucesso: {CustomerId} por {CreatedBy}", customerId, createdBy);

                // DEPOIS: Tentar sincronizar com Google Contacts (sem falhar se der erro)
                TrySyncWithGoogleAsync(customer);

                return customerId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente: {CustomerName}", customer.FullName);
                throw;
            }
        }

        /// <summary>
        /// Tenta sincronizar com Google de forma assíncrona e robusta
        /// </summary>
        private void TrySyncWithGoogleAsync(Customer customer)
        {
            // Não fazer sincronização se já tem Google Contact ID
            if (!string.IsNullOrEmpty(customer.GoogleContactId))
            {
                _logger.LogInformation("Cliente {CustomerId} já possui Google Contact ID: {GoogleContactId}", 
                    customer.Id, customer.GoogleContactId);
                return;
            }

            // Não fazer sincronização se não tem email (necessário para verificar duplicatas)
            if (string.IsNullOrEmpty(customer.Email))
            {
                _logger.LogInformation("Cliente {CustomerId} não possui email, pulando sincronização com Google", customer.Id);
                return;
            }

            // Verificar se já há uma sincronização em andamento para este cliente
            if (!_syncingCustomers.TryAdd(customer.Id, true))
            {
                _logger.LogInformation("Sincronização já em andamento para cliente {CustomerId}, pulando", customer.Id);
                return;
            }

            // Executar sincronização em background, sem bloquear o processo principal
            _ = Task.Run(async () =>
            {
                try
                {
                    if (_googlePeopleService?.IsEnabled == true)
                    {
                        // Adicionar delay para evitar race conditions
                        await Task.Delay(1000);

                        // Verificar novamente se o cliente ainda não tem Google Contact ID
                        // (pode ter sido atualizado por outra operação)
                        var currentCustomer = await _customerRepository.GetByIdAsync(customer.Id);
                        if (currentCustomer != null && !string.IsNullOrEmpty(currentCustomer.GoogleContactId))
                        {
                            _logger.LogInformation("Cliente {CustomerId} já foi sincronizado por outra operação: {GoogleContactId}", 
                                customer.Id, currentCustomer.GoogleContactId);
                            return;
                        }

                        var googleContactId = await _googlePeopleService.SyncContactAsync(customer);
                        if (!string.IsNullOrEmpty(googleContactId))
                        {
                            // Atualizar o cliente com o Google Contact ID
                            customer.GoogleContactId = googleContactId;
                            customer.UpdateModification("Google Auto Sync");
                            await _customerRepository.UpdateAsync(customer);

                            _logger.LogInformation("Cliente sincronizado com Google Contacts: {CustomerId} -> {GoogleContactId}", 
                                customer.Id, googleContactId);
                        }
                        else
                        {
                            _logger.LogWarning("Não foi possível sincronizar cliente {CustomerId} com Google Contacts", customer.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha na sincronização automática com Google Contacts para cliente {CustomerId}. " +
                        "Cliente foi criado com sucesso, mas não foi sincronizado com Google.", customer.Id);
                }
                finally
                {
                    // Remover o cliente da lista de sincronizações em andamento
                    _syncingCustomers.TryRemove(customer.Id, out _);
                }
            });
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, string modifiedBy)
        {
            try
            {
                // Verificar se o cliente existe
                var existingCustomer = await _customerRepository.GetByIdAsync(customer.Id);
                if (existingCustomer == null)
                {
                    throw new ArgumentException($"Cliente não encontrado: {customer.Id}");
                }

                // Validar dados do cliente
                if (!await ValidateCustomerDataAsync(customer))
                {
                    throw new ArgumentException("Dados do cliente inválidos");
                }

                // Verificar duplicação de email (excluindo o próprio cliente)
                if (!string.IsNullOrWhiteSpace(customer.Email))
                {
                    var customerWithEmail = await _customerRepository.GetByEmailAsync(customer.Email);
                    if (customerWithEmail != null && customerWithEmail.Id != customer.Id)
                    {
                        throw new InvalidOperationException($"Já existe outro cliente com o email: {customer.Email}");
                    }
                }

                // Verificar duplicação de documento (excluindo o próprio cliente)
                if (!string.IsNullOrWhiteSpace(customer.DocumentNumber))
                {
                    var customerWithDocument = await _customerRepository.GetByDocumentAsync(customer.DocumentNumber);
                    if (customerWithDocument != null && customerWithDocument.Id != customer.Id)
                    {
                        throw new InvalidOperationException($"Já existe outro cliente com o documento: {customer.DocumentNumber}");
                    }
                }

                // Atualizar modificação
                customer.UpdateModification(modifiedBy);

                // ========== INTEGRAÇÃO GOOGLE PEOPLE API ==========
                // Tentar sincronizar atualizações com Google Contacts
                if (_googlePeopleService.IsEnabled)
                {
                    try
                    {
                        // Se já tem Google Contact ID, tenta atualizar
                        if (!string.IsNullOrEmpty(existingCustomer.GoogleContactId))
                        {
                            var updated = await _googlePeopleService.UpdateContactAsync(customer, existingCustomer.GoogleContactId);
                            if (updated)
                            {
                                customer.GoogleContactId = existingCustomer.GoogleContactId; // Manter o mesmo ID
                                _logger.LogInformation("Cliente atualizado no Google Contacts: {CustomerId} -> {GoogleContactId}", 
                                    customer.Id, existingCustomer.GoogleContactId);
                            }
                            else
                            {
                                _logger.LogWarning("Falha ao atualizar contato no Google Contacts: {CustomerId} -> {GoogleContactId}", 
                                    customer.Id, existingCustomer.GoogleContactId);
                            }
                        }
                        // Se não tem Google Contact ID, tenta criar/sincronizar
                        else
                        {
                            var googleContactId = await _googlePeopleService.SyncContactAsync(customer);
                            if (!string.IsNullOrEmpty(googleContactId))
                            {
                                customer.GoogleContactId = googleContactId;
                                _logger.LogInformation("Cliente sincronizado com Google Contacts durante atualização: {CustomerId} -> {GoogleContactId}", 
                                    customer.Id, googleContactId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Falha ao sincronizar atualização com Google Contacts para cliente {CustomerId}. Continuando sem sincronização.", customer.Id);
                        // Manter o GoogleContactId anterior se a sincronização falhar
                        customer.GoogleContactId = existingCustomer.GoogleContactId;
                    }
                }
                else
                {
                    // Manter o GoogleContactId existente se integração está desabilitada
                    customer.GoogleContactId = existingCustomer.GoogleContactId;
                }

                // Atualizar cliente
                var result = await _customerRepository.UpdateAsync(customer);

                if (result)
                {
                    _logger.LogInformation("Cliente atualizado com sucesso: {CustomerId} por {ModifiedBy}. Google Contact: {GoogleContactId}", 
                        customer.Id, modifiedBy, customer.GoogleContactId ?? "Não sincronizado");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {CustomerId}", customer.Id);
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                // Verificar se o cliente existe
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    throw new ArgumentException($"Cliente não encontrado: {id}");
                }

                // ========== INTEGRAÇÃO GOOGLE PEOPLE API ==========
                // Tentar remover contato do Google Contacts
                if (_googlePeopleService.IsEnabled && !string.IsNullOrEmpty(customer.GoogleContactId))
                {
                    try
                    {
                        var deleted = await _googlePeopleService.DeleteContactAsync(customer.GoogleContactId);
                        if (deleted)
                        {
                            _logger.LogInformation("Contato removido do Google Contacts: {CustomerId} -> {GoogleContactId}", 
                                customer.Id, customer.GoogleContactId);
                        }
                        else
                        {
                            _logger.LogWarning("Falha ao remover contato do Google Contacts: {CustomerId} -> {GoogleContactId}", 
                                customer.Id, customer.GoogleContactId);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Erro ao remover contato do Google Contacts: {CustomerId} -> {GoogleContactId}. Continuando com remoção local.", 
                            customer.Id, customer.GoogleContactId);
                    }
                }

                // Soft delete (desativar)
                var result = await _customerRepository.DeleteAsync(id);

                if (result)
                {
                    _logger.LogInformation("Cliente deletado (desativado) com sucesso: {CustomerId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar cliente: {CustomerId}", id);
                throw;
            }
        }

        public async Task<bool> CustomerExistsAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return false;

                return await _customerRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do cliente: {CustomerId}", id);
                throw;
            }
        }

        public async Task<int> GetCustomerCountAsync()
        {
            try
            {
                return await _customerRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar clientes");
                throw;
            }
        }

        public async Task<int> GetActiveCustomerCountAsync()
        {
            try
            {
                var activeCustomers = await _customerRepository.GetActiveAsync();
                return activeCustomers.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar clientes ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetPagedCustomersAsync(int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100; // Limite máximo

                return await _customerRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes paginados: Page={Page}, PageSize={PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<bool> ValidateCustomerDataAsync(Customer customer)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(customer.FirstName))
                return false;

            // Validar email se fornecido
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(customer.Email);
                    if (addr.Address != customer.Email)
                        return false;
                }
                catch
                {
                    return false;
                }
            }

            await Task.CompletedTask; // Para manter assinatura async
            return true;
        }

        /// <summary>
        /// Sincroniza um cliente existente com Google Contacts manualmente
        /// </summary>
        public async Task<bool> SyncCustomerWithGoogleAsync(string customerId)
        {
            try
            {
                if (!_googlePeopleService.IsEnabled)
                {
                    _logger.LogWarning("Google People API integration is disabled");
                    return false;
                }

                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer not found: {CustomerId}", customerId);
                    return false;
                }

                var googleContactId = await _googlePeopleService.SyncContactAsync(customer);
                if (!string.IsNullOrEmpty(googleContactId))
                {
                    customer.GoogleContactId = googleContactId;
                    customer.UpdateModification("Google Sync");
                    await _customerRepository.UpdateAsync(customer);

                    _logger.LogInformation("Customer manually synced with Google Contacts: {CustomerId} -> {GoogleContactId}", 
                        customerId, googleContactId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error manually syncing customer with Google: {CustomerId}", customerId);
                return false;
            }
        }

        /// <summary>
        /// Valida se as credenciais do Google estão funcionando
        /// </summary>
        public async Task<bool> ValidateGoogleCredentialsAsync()
        {
            try
            {
                if (!_googlePeopleService.IsEnabled)
                    return false;

                return await _googlePeopleService.ValidateCredentialsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating Google credentials");
                return false;
            }
        }
    }
} 