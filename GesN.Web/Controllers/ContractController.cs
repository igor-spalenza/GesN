using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<ContractController> _logger;

        public ContractController(
            IContractService contractService,
            ICustomerService customerService,
            ILogger<ContractController> logger)
        {
            _contractService = contractService;
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os contratos
        /// </summary>
        public async Task<IActionResult> Index(string searchTerm = "", string status = "", int page = 1, int pageSize = 10)
        {
            try
            {
                IEnumerable<Contract> contracts;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    contracts = await _contractService.SearchContractsAsync(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }
                else if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ContractStatus>(status, out var contractStatus))
                {
                    contracts = await _contractService.GetContractsByStatusAsync(contractStatus);
                    ViewBag.Status = status;
                }
                else
                {
                    contracts = await _contractService.GetPagedContractsAsync(page, pageSize);
                }

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalContracts = await _contractService.GetContractCountAsync();
                ViewBag.StatusList = GetStatusSelectList();

                return View(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar contratos");
                TempData["ErrorMessage"] = "Erro ao carregar a lista de contratos.";
                return View(new List<Contract>());
            }
        }

        /// <summary>
        /// Exibe detalhes de um contrato
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Contrato não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contrato não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do contrato.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exibe formulário para criação de contrato
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View(new Contract());
        }

        /// <summary>
        /// Processa criação de contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View(contract);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var contractId = await _contractService.CreateContractAsync(contract, userId);

                TempData["SuccessMessage"] = $"Contrato '{contract.Title}' criado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = contractId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar contrato: {ContractTitle}", contract.Title);
                ModelState.AddModelError("", "Erro interno. Tente novamente.");
            }

            await LoadViewBagData();
            return View(contract);
        }

        /// <summary>
        /// Exibe formulário para edição de contrato
        /// </summary>
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Contrato não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contrato não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                await LoadViewBagData();
                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contrato para edição: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do contrato.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Processa edição de contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Contract contract)
        {
            if (id != contract.Id)
            {
                TempData["ErrorMessage"] = "Dados inconsistentes.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await LoadViewBagData();
                return View(contract);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.UpdateContractAsync(contract, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Contrato '{contract.Title}' atualizado com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = contract.Id });
                }

                ModelState.AddModelError("", "Não foi possível atualizar o contrato.");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar contrato: {ContractId}", contract.Id);
                ModelState.AddModelError("", "Erro interno. Tente novamente.");
            }

            await LoadViewBagData();
            return View(contract);
        }

        /// <summary>
        /// Exibe confirmação para exclusão de contrato
        /// </summary>
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "Contrato não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contrato não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contrato para exclusão: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro ao carregar os dados do contrato.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Processa exclusão de contrato
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contrato não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var success = await _contractService.DeleteContractAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Contrato '{contract.Title}' excluído com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível excluir o contrato.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Confirma um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.ConfirmContractAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato confirmado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível confirmar o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Assina um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(string id, string signedBy)
        {
            try
            {
                var success = await _contractService.SignContractAsync(id, signedBy);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato assinado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível assinar o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao assinar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Suspende um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suspend(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.SuspendContractAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato suspenso com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível suspender o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao suspender contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Cancela um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.CancelContractAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato cancelado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível cancelar o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Finaliza um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.CompleteContractAsync(id, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato finalizado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível finalizar o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Renova um contrato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(string id, DateTime newEndDate)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                var success = await _contractService.RenewContractAsync(id, newEndDate, userId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Contrato renovado com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Não foi possível renovar o contrato.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar contrato: {ContractId}", id);
                TempData["ErrorMessage"] = "Erro interno. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>
        /// Lista contratos próximos do vencimento
        /// </summary>
        public async Task<IActionResult> Expiring()
        {
            try
            {
                var contracts = await _contractService.GetExpiringContractsAsync();
                ViewBag.Title = "Contratos Próximos do Vencimento";
                return View("Index", contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos próximos do vencimento");
                TempData["ErrorMessage"] = "Erro ao carregar contratos próximos do vencimento.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Lista contratos de um cliente específico
        /// </summary>
        public async Task<IActionResult> ByCustomer(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                TempData["ErrorMessage"] = "Cliente não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var contracts = await _contractService.GetContractsByCustomerAsync(customerId);
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                
                ViewBag.Title = $"Contratos de {customer?.GetDisplayName() ?? "Cliente"}";
                ViewBag.CustomerId = customerId;
                
                return View("Index", contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos do cliente: {CustomerId}", customerId);
                TempData["ErrorMessage"] = "Erro ao carregar contratos do cliente.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Busca contratos via AJAX
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }

            try
            {
                var contracts = await _contractService.SearchContractsAsync(term);
                var result = contracts.Select(c => new
                {
                    id = c.Id,
                    contractNumber = c.ContractNumber,
                    title = c.Title,
                    customerName = c.Customer?.GetDisplayName(),
                    totalValue = c.TotalValue,
                    status = c.GetStatusDisplay()
                }).Take(10);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar contratos: {SearchTerm}", term);
                return Json(new List<object>());
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Carrega dados para ViewBag
        /// </summary>
        private async Task LoadViewBagData()
        {
            try
            {
                var customers = await _customerService.GetActiveCustomersAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name");
                
                ViewBag.StatusList = GetStatusSelectList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dados para ViewBag");
                ViewBag.Customers = new SelectList(new List<Customer>(), "Id", "Name");
                ViewBag.StatusList = new SelectList(new List<object>(), "Value", "Text");
            }
        }

        /// <summary>
        /// Obtém lista de status para SelectList
        /// </summary>
        private static SelectList GetStatusSelectList()
        {
            var statusList = Enum.GetValues<ContractStatus>().Select(s => new
            {
                Value = s.ToString(),
                Text = s switch
                {
                    ContractStatus.Draft => "📝 Rascunho",
                    ContractStatus.Generated => "✅ Gerado",
                    ContractStatus.Signed => "📋 Assinado",
                    ContractStatus.SentForSignature => "⏸️ Enviado para Assinatura",
                    ContractStatus.Cancelled => "❌ Cancelado",
                    ContractStatus.Completed => "🏁 Finalizado",
                    ContractStatus.Partiallysigned => "🔄 Parcialmente Assinado",
                    ContractStatus.Expired => "⏰ Expirado",
                    _ => s.ToString()
                }
            });

            return new SelectList(statusList, "Value", "Text");
        }

        #endregion
    }
} 