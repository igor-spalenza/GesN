using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Areas.Integration.Models.Settings;
using Microsoft.Extensions.Options;

namespace GesN.Web.Areas.Integration.Services
{
    /// <summary>
    /// Serviço para integração com Google People API
    /// </summary>
    public class GooglePeopleService : IGooglePeopleService, IDisposable
    {
        private readonly GoogleWorkspaceSettings _settings;
        private readonly ILogger<GooglePeopleService> _logger;
        private readonly IWebHostEnvironment _environment;
        private PeopleServiceService? _peopleService;
        private bool _disposed = false;

        public GooglePeopleService(
            IOptions<GoogleWorkspaceSettings> settings, 
            ILogger<GooglePeopleService> logger,
            IWebHostEnvironment environment)
        {
            _settings = settings.Value;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Indica se a integração está habilitada
        /// </summary>
        public bool IsEnabled => _settings.IsEnabled;

        /// <summary>
        /// Inicializa o serviço Google People se ainda não foi inicializado
        /// </summary>
        private async Task<PeopleServiceService?> GetPeopleServiceAsync()
        {
            if (_peopleService != null)
                return _peopleService;

            if (!_settings.IsEnabled)
            {
                _logger.LogWarning("Google Workspace integration is disabled");
                return null;
            }

            try
            {
                GoogleCredential credential;

                // Verificar se tem configuração de Service Account primeiro
                if (!string.IsNullOrEmpty(_settings.ServiceAccountKeyPath) && File.Exists(_settings.ServiceAccountKeyPath))
                {
                    // Usar Service Account
                    using var stream = new FileStream(_settings.ServiceAccountKeyPath, FileMode.Open, FileAccess.Read);
                    credential = GoogleCredential.FromStream(stream)
                        .CreateScoped(PeopleServiceService.Scope.ContactsReadonly, PeopleServiceService.Scope.Contacts);

                    if (!string.IsNullOrEmpty(_settings.ImpersonateUserEmail))
                    {
                        credential = credential.CreateWithUser(_settings.ImpersonateUserEmail);
                    }
                }
                // OAuth 2.0 para aplicações web
                else if (!string.IsNullOrEmpty(_settings.ClientId) && !string.IsNullOrEmpty(_settings.ClientSecret))
                {
                    var clientSecrets = new ClientSecrets
                    {
                        ClientId = _settings.ClientId,
                        ClientSecret = _settings.ClientSecret
                    };

                    // Configurar o fluxo OAuth 2.0
                    var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = clientSecrets,
                        Scopes = new[] { PeopleServiceService.Scope.ContactsReadonly, PeopleServiceService.Scope.Contacts },
                        DataStore = new FileDataStore(GetTokenStorePath(), true)
                    });

                    // Para aplicação web, vamos usar um usuário padrão
                    var authCode = new AuthorizationCodeWebApp(flow, "http://localhost:7250", "users/me");
                    
                    // Tentar obter credencial existente
                    var result = await authCode.AuthorizeAsync("user", CancellationToken.None);
                    if (result.Credential != null)
                    {
                        credential = GoogleCredential.FromAccessToken(result.Credential.Token.AccessToken);
                    }
                    else
                    {
                        _logger.LogWarning("OAuth2 authorization required. Please visit the authorization URL first.");
                        return null;
                    }
                }
                else
                {
                    _logger.LogError("No valid Google credentials configured");
                    return null;
                }

                _peopleService = new PeopleServiceService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "GesN - Customer Management",
                });

                _logger.LogInformation("Google People Service initialized successfully");
                return _peopleService;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Google People Service");
                return null;
            }
        }

        /// <summary>
        /// Obtém o caminho para armazenar os tokens
        /// </summary>
        private string GetTokenStorePath()
        {
            try
            {
                if (_environment?.ContentRootPath != null)
                {
                    return Path.Combine(_environment.ContentRootPath, "TokenStore");
                }
                else
                {
                    // Fallback para diretório da aplicação
                    return Path.Combine(Directory.GetCurrentDirectory(), "TokenStore");
                }
            }
            catch
            {
                // Último recurso - usar diretório temporário
                return Path.Combine(Path.GetTempPath(), "GesN_TokenStore");
            }
        }

        /// <summary>
        /// Cria um contato no Google Contacts
        /// </summary>
        public async Task<string?> CreateContactAsync(Customer customer)
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return null;

                var person = CreatePersonFromCustomer(customer);
                
                var request = service.People.CreateContact(person);
                var createdPerson = await request.ExecuteAsync();

                _logger.LogInformation("Created contact in Google for customer {CustomerId}: {GoogleContactId}", 
                    customer.Id, createdPerson.ResourceName);

                return ExtractContactId(createdPerson.ResourceName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create contact in Google for customer {CustomerId}", customer.Id);
                return null;
            }
        }

        /// <summary>
        /// Atualiza um contato no Google Contacts
        /// </summary>
        public async Task<bool> UpdateContactAsync(Customer customer, string googleContactId)
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return false;

                var resourceName = $"people/{googleContactId}";
                
                // Primeiro, buscar o contato atual para obter o etag
                var getRequest = service.People.Get(resourceName);
                getRequest.PersonFields = "names,emailAddresses,phoneNumbers,metadata";
                var existingPerson = await getRequest.ExecuteAsync();

                if (existingPerson == null)
                {
                    _logger.LogWarning("Contact not found in Google: {GoogleContactId}", googleContactId);
                    return false;
                }

                // Criar pessoa atualizada mantendo o etag
                var updatedPerson = CreatePersonFromCustomer(customer);
                updatedPerson.ETag = existingPerson.ETag;

                var updateRequest = service.People.UpdateContact(updatedPerson, resourceName);
                updateRequest.UpdatePersonFields = "names,emailAddresses,phoneNumbers";
                
                await updateRequest.ExecuteAsync();

                _logger.LogInformation("Updated contact in Google for customer {CustomerId}: {GoogleContactId}", 
                    customer.Id, googleContactId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update contact in Google for customer {CustomerId}: {GoogleContactId}", 
                    customer.Id, googleContactId);
                return false;
            }
        }

        /// <summary>
        /// Remove um contato do Google Contacts
        /// </summary>
        public async Task<bool> DeleteContactAsync(string googleContactId)
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return false;

                var resourceName = $"people/{googleContactId}";
                var request = service.People.DeleteContact(resourceName);
                await request.ExecuteAsync();

                _logger.LogInformation("Deleted contact from Google: {GoogleContactId}", googleContactId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete contact from Google: {GoogleContactId}", googleContactId);
                return false;
            }
        }

        /// <summary>
        /// Busca um contato no Google Contacts por email
        /// </summary>
        public async Task<string?> FindContactByEmailAsync(string email)
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return null;

                var request = service.People.SearchContacts();
                request.Query = email;
                request.ReadMask = "names,emailAddresses";
                
                var response = await request.ExecuteAsync();

                var contact = response.Results?.FirstOrDefault(r => 
                    r.Person?.EmailAddresses?.Any(e => 
                        string.Equals(e.Value, email, StringComparison.OrdinalIgnoreCase)) == true);

                if (contact?.Person?.ResourceName != null)
                {
                    return ExtractContactId(contact.Person.ResourceName);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to find contact by email in Google: {Email}", email);
                return null;
            }
        }

        /// <summary>
        /// Busca um contato no Google Contacts por ID
        /// </summary>
        public async Task<GoogleContactData?> GetContactAsync(string googleContactId)
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return null;

                var resourceName = $"people/{googleContactId}";
                var request = service.People.Get(resourceName);
                request.PersonFields = "names,emailAddresses,phoneNumbers,metadata";
                
                var person = await request.ExecuteAsync();
                if (person == null) return null;

                // Fix for DateTime conversion issue
                DateTime? lastModified = null;
                var source = person.Metadata?.Sources?.FirstOrDefault();
                if (source?.UpdateTimeDateTimeOffset != null)
                {
                    lastModified = source.UpdateTimeDateTimeOffset.Value.DateTime;
                }

                return new GoogleContactData
                {
                    Id = googleContactId,
                    Name = person.Names?.FirstOrDefault()?.DisplayName ?? "",
                    Email = person.EmailAddresses?.FirstOrDefault()?.Value ?? "",
                    Phone = person.PhoneNumbers?.FirstOrDefault()?.Value ?? "",
                    LastModified = lastModified
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contact from Google: {GoogleContactId}", googleContactId);
                return null;
            }
        }

        /// <summary>
        /// Sincroniza um cliente com Google Contacts
        /// </summary>
        public async Task<string?> SyncContactAsync(Customer customer)
        {
            if (!_settings.AutoSync) 
            {
                _logger.LogDebug("Auto sync disabled for customer {CustomerId}", customer.Id);
                return null;
            }

            try
            {
                        _logger.LogInformation("Iniciando sincronização para cliente {CustomerId} - {CustomerName} ({CustomerEmail})",
            customer.Id, customer.FullName, customer.Email ?? "sem email");

                // Primeiro, tentar encontrar contato existente por email
                string? existingContactId = null;
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    _logger.LogDebug("Procurando contato existente por email: {Email}", customer.Email);
                    existingContactId = await FindContactByEmailAsync(customer.Email);
                    
                    if (existingContactId != null)
                    {
                        _logger.LogInformation("Contato existente encontrado para {Email}: {GoogleContactId}", 
                            customer.Email, existingContactId);
                    }
                    else
                    {
                        _logger.LogDebug("Nenhum contato existente encontrado para {Email}", customer.Email);
                    }
                }

                if (existingContactId != null)
                {
                    // Atualizar contato existente
                    _logger.LogInformation("Atualizando contato existente {GoogleContactId} para cliente {CustomerId}", 
                        existingContactId, customer.Id);
                    var updated = await UpdateContactAsync(customer, existingContactId);
                    return updated ? existingContactId : null;
                }
                else
                {
                    // Criar novo contato
                    _logger.LogInformation("Criando novo contato no Google para cliente {CustomerId}", customer.Id);
                    var newContactId = await CreateContactAsync(customer);
                    
                    if (!string.IsNullOrEmpty(newContactId))
                    {
                        _logger.LogInformation("Novo contato criado com sucesso: {GoogleContactId} para cliente {CustomerId}", 
                            newContactId, customer.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao criar novo contato para cliente {CustomerId}", customer.Id);
                    }
                    
                    return newContactId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync contact for customer {CustomerId}", customer.Id);
                return null;
            }
        }

        /// <summary>
        /// Valida se as credenciais estão funcionando
        /// </summary>
        public async Task<bool> ValidateCredentialsAsync()
        {
            try
            {
                var service = await GetPeopleServiceAsync();
                if (service == null) return false;

                // Fazer uma requisição simples para testar as credenciais
                var request = service.People.Get("people/me");
                request.PersonFields = "names";
                await request.ExecuteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate Google credentials");
                return false;
            }
        }

        /// <summary>
        /// Cria um objeto Person do Google a partir de um Customer
        /// </summary>
        private Person CreatePersonFromCustomer(Customer customer)
        {
            var person = new Person();

            // Nome
            if (!string.IsNullOrEmpty(customer.FirstName))
            {
                person.Names = new List<Name>
                {
                    new Name
                    {
                        GivenName = customer.FirstName,
                        FamilyName = customer.LastName,
                        DisplayName = customer.FullName
                    }
                };
            }

            // Email
            if (!string.IsNullOrEmpty(customer.Email))
            {
                person.EmailAddresses = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Value = customer.Email,
                        Type = "work"
                    }
                };
            }

            // Telefone
            if (!string.IsNullOrEmpty(customer.Phone))
            {
                person.PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Value = customer.Phone,
                        Type = "work"
                    }
                };
            }

            return person;
        }

        /// <summary>
        /// Extrai o ID do contato do resourceName
        /// </summary>
        private string ExtractContactId(string resourceName)
        {
            return resourceName.Replace("people/", "");
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _peopleService?.Dispose();
                _disposed = true;
            }
        }
    }
} 