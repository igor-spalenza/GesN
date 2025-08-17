using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Entities.ValueObjects;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Página principal com lista de clientes usando ViewModel
        /// </summary>
        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var customers = !string.IsNullOrWhiteSpace(searchTerm) 
                    ? await _customerService.SearchCustomersAsync(searchTerm)
                    : await _customerService.GetPagedCustomersAsync(page, pageSize);

                var totalCustomers = await _customerService.GetCustomerCountAsync();
                var viewModel = customers.ToIndexViewModel(page, pageSize, totalCustomers);
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    viewModel.Search.SearchTerm = searchTerm;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar página de clientes");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de clientes.";
                
                return View(new CustomerIndexViewModel());
            }
        }

        /// <summary>
        /// Exibe detalhes de um cliente usando ViewModel
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Cliente não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Cliente não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = customer.ToDetailsViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente: {CustomerId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exibe formulário para criação de cliente
        /// </summary>
        public IActionResult Create()
        {
            var viewModel = new CreateCustomerViewModel();
            return View(viewModel);
        }

        /// <summary>
        /// Processa criação de cliente usando ViewModel
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var customer = viewModel.ToEntity();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var customerId = await _customerService.CreateCustomerAsync(customer, userId);

                TempData["SuccessMessage"] = $"Cliente '{customer.FullName}' criado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = customerId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente: {CustomerName}", $"{viewModel.FirstName} {viewModel.LastName}".Trim());
                ModelState.AddModelError("", "Erro interno. Tente novamente.");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Exibe formulário para edição de cliente usando ViewModel
        /// </summary>
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Cliente não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Cliente não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = customer.ToEditViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para edição: {CustomerId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Processa edição de cliente usando ViewModel
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditCustomerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                TempData["ErrorMessage"] = "Dados inconsistentes.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Cliente não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                viewModel.UpdateEntity(customer);
                
                var success = await _customerService.UpdateCustomerAsync(customer, userId);
                if (success)
                {
                    TempData["SuccessMessage"] = $"Cliente '{customer.FullName}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = customer.Id });
                }
                else
                {
                    ModelState.AddModelError("", "Não foi possível atualizar o cliente.");
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {CustomerId}", id);
                ModelState.AddModelError("", "Erro interno. Tente novamente.");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Exibe página de confirmação para exclusão
        /// </summary>
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Cliente não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Cliente não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = customer.ToDetailsViewModel();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para exclusão: {CustomerId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Processa exclusão de cliente
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var success = await _customerService.DeleteCustomerAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Cliente excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível excluir o cliente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente: {CustomerId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Retorna lista de clientes como partial view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListaClientes()
        {
            try
            {
                var customers = await _customerService.GetActiveCustomersAsync();
                var viewModels = customers.ToViewModelList();
                return PartialView("_ListaClientes", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de clientes via AJAX");
                return PartialView("_ListaClientes", new List<CustomerViewModel>());
            }
        }

        /// <summary>
        /// Retorna formulário de criação como partial view
        /// </summary>
        [HttpGet]
        public IActionResult FormularioCriacao()
        {
            var viewModel = new CreateCustomerViewModel();
            return PartialView("_Create", viewModel);
        }

        /// <summary>
        /// Retorna formulário de edição como partial view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FormularioEdicao(string id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                var viewModel = customer.ToEditViewModel();
                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição: {CustomerId}", id);
                return BadRequest("Erro ao carregar formulário");
            }
        }

        /// <summary>
        /// Retorna detalhes do cliente como partial view
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DetalhesCliente(string id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                var viewModel = customer.ToDetailsViewModel();
                return PartialView("_Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do cliente: {CustomerId}", id);
                return BadRequest("Erro ao carregar detalhes");
            }
        }

        /// <summary>
        /// Salva novo cliente via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovoCliente(CreateCustomerViewModel viewModel)
        {
            if (viewModel == null)
            {
                return Json(new { success = false, message = "Dados não informados." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, message = "Dados inválidos: " + string.Join(", ", errors) });
            }

            try
            {
                var customer = viewModel.ToEntity();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var customerId = await _customerService.CreateCustomerAsync(customer, userId);

                return Json(new { 
                    success = true, 
                    message = $"Cliente '{customer.FullName}' criado com sucesso!",
                    id = customerId
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente via AJAX: {CustomerName}", $"{viewModel.FirstName} {viewModel.LastName}".Trim());
                return Json(new { success = false, message = "Erro interno. Tente novamente." });
            }
        }

        /// <summary>
        /// Salva edição do cliente via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEdicaoCliente(string id, EditCustomerViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dados inválidos." });
            }

            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Cliente não encontrado." });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                viewModel.UpdateEntity(customer);
                
                var success = await _customerService.UpdateCustomerAsync(customer, userId);
                if (success)
                {
                    return Json(new { success = true, message = $"Cliente '{customer.FullName}' atualizado com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Não foi possível atualizar o cliente." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente via AJAX: {CustomerId}", id);
                return Json(new { success = false, message = "Erro interno. Tente novamente." });
            }
        }

        /// <summary>
        /// Exclui cliente via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirCliente(string id)
        {
            try
            {
                var success = await _customerService.DeleteCustomerAsync(id);

                if (success)
                {
                    return Json(new { success = true, message = "Cliente excluído com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Não foi possível excluir o cliente." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente via AJAX: {CustomerId}", id);
                return Json(new { success = false, message = "Erro interno. Tente novamente." });
            }
        }

        /// <summary>
        /// Busca clientes via AJAX
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string termo)
        {
            try
            {
                var customers = await _customerService.SearchCustomersAsync(termo);
                var viewModels = customers.ToViewModelList();
                return PartialView("_ListaClientes", viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes: {Termo}", termo);
                return BadRequest("Erro ao buscar clientes");
            }
        }

        /// <summary>
        /// Retorna estatísticas dos clientes via AJAX
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EstatisticasClientes()
        {
            try
            {
                var totalCustomers = await _customerService.GetCustomerCountAsync();
                var activeCustomers = await _customerService.GetActiveCustomerCountAsync();

                return Json(new { 
                    total = totalCustomers,
                    ativos = activeCustomers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar estatísticas de clientes");
                return Json(new { total = 0, ativos = 0 });
            }
        }

        /// <summary>
        /// Endpoint para autocomplete de Customer
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscaCustomerAutocomplete(string termo)
        {
            try
            {
                // ✅ VALIDAÇÃO: Minimum length check
                if (string.IsNullOrWhiteSpace(termo) || termo.Length < 2)
                    return Json(new List<object>());

                // ✅ SERVICE LAYER: Usar serviço especializado
                var customers = await _customerService.SearchCustomersForAutocompleteAsync(termo);

                var result = customers
                    .Where(c => c.StateCode == ObjectState.Active) // ✅ FILTRO: Apenas ativos
                    .Take(10) // ✅ PERFORMANCE: Limitar resultados
                    .Select(c => new {
                        id = c.Id,
                        name = c.FullName,
                        description = c.Phone ?? "",
                        label = $"{c.FullName}" + (!string.IsNullOrWhiteSpace(c.Phone) ? $" - {c.Phone}" : ""),
                        value = c.FullName,
                        phone = c.Phone ?? "",
                        email = c.Email ?? ""
                    })
                    .ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                // ✅ ERROR HANDLING: Log e retorno seguro
                _logger.LogError(ex, "Erro ao buscar customers para autocomplete com termo: {Termo}", termo);
                return Json(new List<object>());
            }
        }

    }
} 