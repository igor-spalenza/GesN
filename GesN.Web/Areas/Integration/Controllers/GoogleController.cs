using GesN.Web.Interfaces.Services;
using GesN.Web.Areas.Integration.Services;
using GesN.Web.Areas.Integration.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.PeopleService.v1;
using Google.Apis.Util.Store;

namespace GesN.Web.Areas.Integration.Controllers
{
    [Area("Integration")]
    [Authorize]
    public class GoogleController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IGooglePeopleService _googlePeopleService;
        private readonly GoogleWorkspaceSettings _settings;
        private readonly ILogger<GoogleController> _logger;
        private readonly IWebHostEnvironment _environment;

        public GoogleController(
            ICustomerService customerService,
            IGooglePeopleService googlePeopleService,
            IOptions<GoogleWorkspaceSettings> settings,
            ILogger<GoogleController> logger,
            IWebHostEnvironment environment)
        {
            _customerService = customerService;
            _googlePeopleService = googlePeopleService;
            _settings = settings.Value;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Página de configuração da integração Google
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var model = new GoogleIntegrationViewModel
            {
                IsEnabled = _settings.IsEnabled,
                Domain = _settings.Domain,
                AutoCreateContacts = _settings.AutoCreateContacts,
                AutoSync = _settings.AutoSync,
                HasServiceAccount = !string.IsNullOrEmpty(_settings.ServiceAccountKeyPath) && 
                                 System.IO.File.Exists(_settings.ServiceAccountKeyPath),
                HasClientCredentials = !string.IsNullOrEmpty(_settings.ClientId) && 
                                     !string.IsNullOrEmpty(_settings.ClientSecret)
            };

            // Verificar se o usuário já está autorizado
            if (model.HasClientCredentials)
            {
                model.IsAuthorized = await IsUserAuthorizedAsync();
                if (!model.IsAuthorized)
                {
                    model.AuthorizationUrl = await GetAuthorizationUrlAsync();
                }
            }

            return View(model);
        }

        /// <summary>
        /// Inicia o processo de autorização OAuth2
        /// </summary>
        public async Task<IActionResult> Authorize()
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.ClientId) || string.IsNullOrEmpty(_settings.ClientSecret))
                {
                    TempData["Error"] = "Credenciais OAuth2 não configuradas.";
                    return RedirectToAction("Index");
                }

                var authUrl = await GetAuthorizationUrlAsync();
                if (!string.IsNullOrEmpty(authUrl))
                {
                    return Redirect(authUrl);
                }
                else
                {
                    TempData["Error"] = "Erro ao gerar URL de autorização.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar autorização OAuth2");
                TempData["Error"] = "Erro interno ao iniciar autorização.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Callback do OAuth2 após autorização
        /// </summary>
        public async Task<IActionResult> OAuth2Callback(string code, string error, string state)
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    TempData["Error"] = $"Erro na autorização: {error}";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(code))
                {
                    TempData["Error"] = "Código de autorização não recebido.";
                    return RedirectToAction("Index");
                }

                // Processar o callback
                var result = await ProcessOAuth2CallbackAsync(code);
                if (result)
                {
                    TempData["Success"] = "Autorização concluída com sucesso!";
                }
                else
                {
                    TempData["Error"] = "Erro ao processar autorização.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback OAuth2");
                TempData["Error"] = "Erro interno no callback de autorização.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Valida credenciais do Google
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ValidateCredentials()
        {
            try
            {
                if (!_googlePeopleService.IsEnabled)
                {
                    return Json(new { success = false, message = "Integração com Google está desabilitada nas configurações." });
                }

                var isValid = await _googlePeopleService.ValidateCredentialsAsync();
                
                if (isValid)
                {
                    return Json(new { success = true, message = "Credenciais validadas com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Credenciais inválidas ou sem permissões necessárias." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar credenciais do Google");
                return Json(new { success = false, message = "Erro interno ao validar credenciais." });
            }
        }

        /// <summary>
        /// Sincroniza um cliente específico com Google Contacts
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SyncCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    return Json(new { success = false, message = "ID do cliente não informado." });
                }

                if (!_googlePeopleService.IsEnabled)
                {
                    return Json(new { success = false, message = "Integração com Google está desabilitada." });
                }

                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Cliente não encontrado." });
                }

                var googleContactId = await _googlePeopleService.SyncContactAsync(customer);
                
                if (!string.IsNullOrEmpty(googleContactId))
                {
                    // Atualizar o cliente com o Google Contact ID
                    customer.GoogleContactId = googleContactId;
                    customer.UpdateModification("Google Manual Sync");
                    await _customerService.UpdateCustomerAsync(customer, "Google Manual Sync");

                    return Json(new { 
                        success = true, 
                        message = $"Cliente '{customer.FullName}' sincronizado com sucesso!",
                        googleContactId = googleContactId
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Falha ao sincronizar cliente com Google Contacts." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao sincronizar cliente {CustomerId} com Google", customerId);
                return Json(new { success = false, message = "Erro interno ao sincronizar cliente." });
            }
        }

        /// <summary>
        /// Sincroniza todos os clientes ativos com Google Contacts
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SyncAllCustomers()
        {
            try
            {
                if (!_googlePeopleService.IsEnabled)
                {
                    return Json(new { success = false, message = "Integração com Google está desabilitada." });
                }

                var customers = await _customerService.GetActiveCustomersAsync();
                var results = new List<object>();
                int successCount = 0;
                int errorCount = 0;

                foreach (var customer in customers)
                {
                    try
                    {
                        var googleContactId = await _googlePeopleService.SyncContactAsync(customer);
                        
                        if (!string.IsNullOrEmpty(googleContactId))
                        {
                            customer.GoogleContactId = googleContactId;
                            customer.UpdateModification("Google Bulk Sync");
                            await _customerService.UpdateCustomerAsync(customer, "Google Bulk Sync");
                            
                            successCount++;
                            results.Add(new { 
                                customerId = customer.Id, 
                                customerName = customer.FullName,
                                success = true, 
                                googleContactId = googleContactId 
                            });
                        }
                        else
                        {
                            errorCount++;
                            results.Add(new { 
                                customerId = customer.Id, 
                                customerName = customer.FullName,
                                success = false, 
                                error = "Falha na sincronização" 
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        results.Add(new { 
                            customerId = customer.Id, 
                            customerName = customer.FullName,
                            success = false, 
                            error = ex.Message 
                        });
                        _logger.LogError(ex, "Erro ao sincronizar cliente {CustomerId} durante sincronização em massa", customer.Id);
                    }
                }

                return Json(new { 
                    success = true, 
                    message = $"Sincronização concluída: {successCount} sucessos, {errorCount} falhas",
                    successCount = successCount,
                    errorCount = errorCount,
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante sincronização em massa com Google");
                return Json(new { success = false, message = "Erro interno durante sincronização em massa." });
            }
        }

        /// <summary>
        /// Obtém estatísticas da integração
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var customers = await _customerService.GetActiveCustomersAsync();
                var totalCustomers = customers.Count();
                var syncedCustomers = customers.Count(c => !string.IsNullOrEmpty(c.GoogleContactId));
                var unsyncedCustomers = totalCustomers - syncedCustomers;

                return Json(new { 
                    totalCustomers = totalCustomers,
                    syncedCustomers = syncedCustomers,
                    unsyncedCustomers = unsyncedCustomers,
                    syncPercentage = totalCustomers > 0 ? (syncedCustomers * 100 / totalCustomers) : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas da integração Google");
                return Json(new { 
                    totalCustomers = 0,
                    syncedCustomers = 0,
                    unsyncedCustomers = 0,
                    syncPercentage = 0
                });
            }
        }

        /// <summary>
        /// Verifica se o usuário já está autorizado
        /// </summary>
        private async Task<bool> IsUserAuthorizedAsync()
        {
            try
            {
                var flow = CreateAuthFlow();
                var dataStore = flow.DataStore;
                var token = await dataStore.GetAsync<Google.Apis.Auth.OAuth2.Responses.TokenResponse>("user");
                return token != null && !string.IsNullOrEmpty(token.AccessToken);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gera a URL de autorização OAuth2
        /// </summary>
        private async Task<string?> GetAuthorizationUrlAsync()
        {
            try
            {
                var flow = CreateAuthFlow();
                var authCode = new AuthorizationCodeWebApp(flow, Url.Action("OAuth2Callback", "Google", null, Request.Scheme), "users/me");
                var result = await authCode.AuthorizeAsync("user", CancellationToken.None);
                
                return result.RedirectUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar URL de autorização");
                return null;
            }
        }

        /// <summary>
        /// Processa o callback do OAuth2
        /// </summary>
        private async Task<bool> ProcessOAuth2CallbackAsync(string code)
        {
            try
            {
                var flow = CreateAuthFlow();
                var authCode = new AuthorizationCodeWebApp(flow, Url.Action("OAuth2Callback", "Google", null, Request.Scheme), "users/me");
                
                var token = await flow.ExchangeCodeForTokenAsync("user", code, Url.Action("OAuth2Callback", "Google", null, Request.Scheme), CancellationToken.None);
                
                return token != null && !string.IsNullOrEmpty(token.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar callback OAuth2");
                return false;
            }
        }

        /// <summary>
        /// Cria o fluxo de autorização
        /// </summary>
        private GoogleAuthorizationCodeFlow CreateAuthFlow()
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _settings.ClientId,
                ClientSecret = _settings.ClientSecret
            };

            return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { PeopleServiceService.Scope.ContactsReadonly, PeopleServiceService.Scope.Contacts },
                DataStore = new FileDataStore(GetTokenStorePath(), true)
            });
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
    }

    /// <summary>
    /// ViewModel para página de integração Google
    /// </summary>
    public class GoogleIntegrationViewModel
    {
        public bool IsEnabled { get; set; }
        public string Domain { get; set; } = string.Empty;
        public bool AutoCreateContacts { get; set; }
        public bool AutoSync { get; set; }
        public bool HasServiceAccount { get; set; }
        public bool HasClientCredentials { get; set; }
        public bool IsAuthorized { get; set; }
        public string? AuthorizationUrl { get; set; }
        public bool IsConfigurationValid => IsEnabled && (HasServiceAccount || (HasClientCredentials && IsAuthorized));
    }
} 