using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.ViewModels.Production;
using GesN.Web.Models.Enumerators;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GesN.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Demandas de Produção
    /// </summary>
    [Authorize]
    public class DemandController : Controller
    {
        private readonly IDemandService _demandService;
        private readonly IProductService _productService;
        private readonly IOrderItemService _orderItemService;
        private readonly IProductionOrderService _productionOrderService;

        public DemandController(
            IDemandService demandService,
            IProductService productService,
            IOrderItemService orderItemService,
            IProductionOrderService productionOrderService)
        {
            _demandService = demandService;
            _productService = productService;
            _orderItemService = orderItemService;
            _productionOrderService = productionOrderService;
        }

        #region Views Principais

        /// <summary>
        /// Página principal de listagem de demandas
        /// </summary>
        public async Task<IActionResult> Index(
            string? searchTerm = null,
            DemandStatus? statusFilter = null,
            string? productIdFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null,
            bool showOverdueOnly = false,
            int page = 1,
            int pageSize = 20,
            string sortBy = "ExpectedDate",
            string sortDirection = "asc")
        {
            try
            {
                var demands = await GetFilteredDemandsAsync(searchTerm, statusFilter, productIdFilter, 
                    startDateFilter, endDateFilter, showOverdueOnly);

                // Aplicar paginação e ordenação
                var sortedDemands = ApplySorting(demands, sortBy, sortDirection);
                var paginatedDemands = ApplyPagination(sortedDemands, page, pageSize);

                // Converter para ViewModels
                var demandViewModels = new List<DemandViewModel>();
                foreach (var demand in paginatedDemands)
                {
                    var viewModel = demand.ToViewModel();
                    
                    // Carregar dados adicionais
                    await PopulateViewModelAdditionalDataAsync(viewModel);
                    
                    demandViewModels.Add(viewModel);
                }

                // Preparar ViewModel da página
                var indexViewModel = new DemandIndexViewModel
                {
                    Demands = demandViewModels,
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    ProductIdFilter = productIdFilter,
                    StartDateFilter = startDateFilter,
                    EndDateFilter = endDateFilter,
                    ShowOverdueOnly = showOverdueOnly,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalItems = demands.Count(),
                    SortBy = sortBy,
                    SortDirection = sortDirection,
                    ShowFilters = !string.IsNullOrEmpty(searchTerm) || statusFilter.HasValue || 
                                 !string.IsNullOrEmpty(productIdFilter) || showOverdueOnly
                };

                // Carregar estatísticas do dashboard
                await PopulateDashboardStatsAsync(indexViewModel);

                // Carregar listas para filtros
                await PopulateFilterListsAsync(indexViewModel);

                return View(indexViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar demandas: {ex.Message}";
                return View(new DemandIndexViewModel());
            }
        }

        /// <summary>
        /// Exibir detalhes de uma demanda
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID da demanda não informado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var demand = await _demandService.GetByIdAsync(id);
                if (demand == null)
                {
                    TempData["ErrorMessage"] = "Demanda não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = demand.ToDetailsViewModel();
                
                // Carregar dados relacionados
                await PopulateDetailsViewModelAsync(viewModel, demand);

                // Verificar permissões do usuário
                var userId = GetCurrentUserId();
                await PopulateActionPermissionsAsync(viewModel, demand, userId);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar detalhes da demanda: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Formulário de criação (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(string? orderItemId = null, string? productId = null)
        {
            try
            {
                var viewModel = new CreateDemandViewModel();

                // Pre-popular campos se fornecidos
                if (!string.IsNullOrEmpty(orderItemId))
                {
                    viewModel.OrderItemId = orderItemId;
                    
                    // Buscar dados do OrderItem para pre-popular outros campos
                    var orderItem = await _orderItemService.GetByIdAsync(orderItemId);
                    if (orderItem != null)
                    {
                        viewModel.ProductId = orderItem.ProductId ?? "";
                        viewModel.Quantity = orderItem.Quantity.ToString();
                        
                        // Buscar nome do produto
                        var product = await _productService.GetByIdAsync(orderItem.ProductId);
                        var productName = product?.Name ?? "Produto não identificado";
                        viewModel.SelectedOrderItemDisplay = $"Pedido: {orderItem.OrderId} - Item: {productName}";
                    }
                }

                if (!string.IsNullOrEmpty(productId))
                {
                    viewModel.ProductId = productId;
                    var product = await _productService.GetByIdAsync(productId);
                    if (product != null)
                    {
                        viewModel.SelectedProductDisplay = $"{product.Name} ({product.SKU})";
                    }
                }

                // Carregar listas para dropdowns
                await PopulateCreateViewModelListsAsync(viewModel);

                return PartialView("_Create", viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao carregar formulário de criação: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Criar demanda (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDemandViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateViewModelListsAsync(viewModel);
                return PartialView("_Create", viewModel);
            }

            try
            {
                var demand = viewModel.ToEntity();
                demand.CreatedBy = GetCurrentUserId();

                var demandId = await _demandService.CreateAsync(demand);

                TempData["SuccessMessage"] = "Demanda criada com sucesso!";
                return Json(new { success = true, message = "Demanda criada com sucesso!", demandId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao criar demanda: {ex.Message}");
                await PopulateCreateViewModelListsAsync(viewModel);
                return PartialView("_Create", viewModel);
            }
        }

        /// <summary>
        /// Formulário de edição (GET)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da demanda não informado." });
            }

            try
            {
                var demand = await _demandService.GetByIdAsync(id);
                if (demand == null)
                {
                    return Json(new { success = false, message = "Demanda não encontrada." });
                }

                var viewModel = new EditDemandViewModel
                {
                    Id = demand.Id,
                    OrderItemId = demand.OrderItemId,
                    ProductionOrderId = demand.ProductionOrderId,
                    ProductId = demand.ProductId,
                    Quantity = demand.Quantity,
                    Status = demand.Status,
                    ExpectedDate = demand.ExpectedDate,
                    Notes = demand.Notes ?? "",
                    StartedAt = demand.StartedAt,
                    CompletedAt = demand.CompletedAt
                };

                // Verificar permissões de edição
                var userId = GetCurrentUserId();
                await PopulateEditPermissionsAsync(viewModel, demand, userId);

                // Carregar listas para dropdowns
                await PopulateEditViewModelListsAsync(viewModel);

                // Carregar dados auxiliares
                await PopulateEditViewModelDataAsync(viewModel, demand);

                return PartialView("_Edit", viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao carregar demanda: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atualizar demanda (POST)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditDemandViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEditViewModelListsAsync(viewModel);
                return PartialView("_Edit", viewModel);
            }

            try
            {
                var demand = await _demandService.GetByIdAsync(viewModel.Id);
                if (demand == null)
                {
                    ModelState.AddModelError("", "Demanda não encontrada.");
                    return PartialView("_Edit", viewModel);
                }

                // Atualizar entidade
                viewModel.UpdateEntity(demand);
                demand.LastModifiedBy = GetCurrentUserId();

                var success = await _demandService.UpdateAsync(demand);
                if (success)
                {
                    TempData["SuccessMessage"] = "Demanda atualizada com sucesso!";
                    return Json(new { success = true, message = "Demanda atualizada com sucesso!" });
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao atualizar demanda.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao atualizar demanda: {ex.Message}");
            }

            await PopulateEditViewModelListsAsync(viewModel);
            return PartialView("_Edit", viewModel);
        }

        /// <summary>
        /// Excluir demanda
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da demanda não informado." });
            }

            try
            {
                var demand = await _demandService.GetByIdAsync(id);
                if (demand == null)
                {
                    return Json(new { success = false, message = "Demanda não encontrada." });
                }

                if (!await _demandService.CanDeleteAsync(id))
                {
                    return Json(new { success = false, message = "Demanda não pode ser excluída." });
                }

                var userId = GetCurrentUserId();
                var success = await _demandService.DeleteAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Demanda excluída com sucesso!";
                    return Json(new { success = true, message = "Demanda excluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao excluir demanda." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao excluir demanda: {ex.Message}" });
            }
        }

        #endregion

        #region Business Operations

        /// <summary>
        /// Confirmar demanda
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string id)
        {
            return await ExecuteStatusChangeAsync(id, 
                async (demandId, userId) => await _demandService.ConfirmDemandAsync(demandId, userId),
                "Demanda confirmada com sucesso!",
                "Erro ao confirmar demanda");
        }

        /// <summary>
        /// Marcar como produzido
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsProduced(string id)
        {
            return await ExecuteStatusChangeAsync(id,
                async (demandId, userId) => await _demandService.MarkAsProducedAsync(demandId, userId),
                "Demanda marcada como produzida!",
                "Erro ao marcar demanda como produzida");
        }

        /// <summary>
        /// Marcar como finalizando
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsEnding(string id)
        {
            return await ExecuteStatusChangeAsync(id,
                async (demandId, userId) => await _demandService.MarkAsEndingAsync(demandId, userId),
                "Demanda marcada como finalizando!",
                "Erro ao marcar demanda como finalizando");
        }

        /// <summary>
        /// Marcar como entregue
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDelivered(string id)
        {
            return await ExecuteStatusChangeAsync(id,
                async (demandId, userId) => await _demandService.MarkAsDeliveredAsync(demandId, userId),
                "Demanda marcada como entregue!",
                "Erro ao marcar demanda como entregue");
        }

        /// <summary>
        /// Atualizar data prevista
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateExpectedDate(string id, DateTime newExpectedDate)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da demanda não informado." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _demandService.UpdateExpectedDateAsync(id, newExpectedDate, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Data prevista atualizada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atualizar data prevista." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao atualizar data prevista: {ex.Message}" });
            }
        }

        /// <summary>
        /// Atribuir a ordem de produção
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignToProductionOrder(string id, string productionOrderId)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da demanda não informado." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await _demandService.AssignToProductionOrderAsync(id, productionOrderId, userId);

                if (success)
                {
                    return Json(new { success = true, message = "Demanda atribuída à ordem de produção!" });
                }
                else
                {
                    return Json(new { success = false, message = "Erro ao atribuir demanda à ordem de produção." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Erro ao atribuir demanda: {ex.Message}" });
            }
        }

        #endregion

        #region API Endpoints

        /// <summary>
        /// Buscar demandas para autocomplete
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string term, int limit = 10)
        {
            try
            {
                var demands = await _demandService.SearchAsync(term);
                var results = new List<object>();
                
                foreach (var d in demands.Take(limit))
                {
                    // Buscar nome do produto
                    var product = await _productService.GetByIdAsync(d.ProductId);
                    var productName = product?.Name ?? "Produto não identificado";
                    
                    results.Add(new
                    {
                        id = d.Id,
                        text = $"{productName} - {d.Quantity} - {d.GetStatusDisplay()}",
                        status = d.Status.ToString(),
                        expectedDate = d.ExpectedDate?.ToString("dd/MM/yyyy"),
                        isOverdue = d.IsOverdue()
                    });
                }

                return Json(results);
            }
            catch (Exception)
            {
                return Json(new object[] { });
            }
        }

        /// <summary>
        /// Obter estatísticas do dashboard
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    pending = (await _demandService.GetPendingDemandsAsync()).Count(),
                    confirmed = (await _demandService.GetConfirmedDemandsAsync()).Count(),
                    produced = (await _demandService.GetInProductionDemandsAsync()).Count(),
                    ending = (await _demandService.GetEndingDemandsAsync()).Count(),
                    delivered = (await _demandService.GetDeliveredDemandsAsync()).Count(),
                    overdue = await _demandService.GetOverdueCountAsync(),
                    total = await _demandService.GetTotalActiveDemandsAsync()
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<IEnumerable<GesN.Web.Models.Entities.Production.Demand>> GetFilteredDemandsAsync(
            string? searchTerm, DemandStatus? statusFilter, string? productIdFilter,
            DateTime? startDateFilter, DateTime? endDateFilter, bool showOverdueOnly)
        {
            IEnumerable<GesN.Web.Models.Entities.Production.Demand> demands;

            if (showOverdueOnly)
            {
                demands = await _demandService.GetOverdueDemandsAsync();
            }
            else if (statusFilter.HasValue)
            {
                demands = await _demandService.GetByStatusAsync(statusFilter.Value);
            }
            else if (!string.IsNullOrEmpty(searchTerm))
            {
                demands = await _demandService.SearchAsync(searchTerm);
            }
            else if (startDateFilter.HasValue && endDateFilter.HasValue)
            {
                demands = await _demandService.GetByDateRangeAsync(startDateFilter.Value, endDateFilter.Value);
            }
            else if (!string.IsNullOrEmpty(productIdFilter))
            {
                demands = await _demandService.GetByProductIdAsync(productIdFilter);
            }
            else
            {
                demands = await _demandService.GetAllAsync();
            }

            return demands;
        }

        private IEnumerable<GesN.Web.Models.Entities.Production.Demand> ApplySorting(
            IEnumerable<GesN.Web.Models.Entities.Production.Demand> demands, string sortBy, string sortDirection)
        {
            var isAscending = sortDirection.ToLower() == "asc";

            return sortBy.ToLower() switch
            {
                "expecteddate" => isAscending ? 
                    demands.OrderBy(d => d.ExpectedDate) : 
                    demands.OrderByDescending(d => d.ExpectedDate),
                "status" => isAscending ?
                    demands.OrderBy(d => d.Status) :
                    demands.OrderByDescending(d => d.Status),
                "createdat" => isAscending ?
                    demands.OrderBy(d => d.CreatedAt) :
                    demands.OrderByDescending(d => d.CreatedAt),
                _ => isAscending ?
                    demands.OrderBy(d => d.ExpectedDate) :
                    demands.OrderByDescending(d => d.ExpectedDate)
            };
        }

        private IEnumerable<GesN.Web.Models.Entities.Production.Demand> ApplyPagination(
            IEnumerable<GesN.Web.Models.Entities.Production.Demand> demands, int page, int pageSize)
        {
            return demands.Skip((page - 1) * pageSize).Take(pageSize);
        }

        private async Task PopulateViewModelAdditionalDataAsync(DemandViewModel viewModel)
        {
            // Carregar dados do produto
            var product = await _productService.GetByIdAsync(viewModel.ProductId);
            if (product != null)
            {
                viewModel.ProductName = product.Name;
                viewModel.ProductSKU = product.SKU ?? "";
            }

            // Verificar permissões
            viewModel.CanConfirm = await _demandService.CanConfirmAsync(viewModel.Id);
            viewModel.CanMarkAsProduced = await _demandService.CanMarkAsProducedAsync(viewModel.Id);
            viewModel.CanMarkAsEnding = await _demandService.CanMarkAsEndingAsync(viewModel.Id);
            viewModel.CanMarkAsDelivered = await _demandService.CanMarkAsDeliveredAsync(viewModel.Id);
            viewModel.CanDelete = await _demandService.CanDeleteAsync(viewModel.Id);
            viewModel.CanEdit = viewModel.Status != DemandStatus.Delivered;
        }

        private async Task PopulateDashboardStatsAsync(DemandIndexViewModel viewModel)
        {
            viewModel.PendingCount = (await _demandService.GetPendingDemandsAsync()).Count();
            viewModel.ConfirmedCount = (await _demandService.GetConfirmedDemandsAsync()).Count();
            viewModel.ProducedCount = (await _demandService.GetInProductionDemandsAsync()).Count();
            viewModel.EndingCount = (await _demandService.GetEndingDemandsAsync()).Count();
            viewModel.DeliveredCount = (await _demandService.GetDeliveredDemandsAsync()).Count();
            viewModel.OverdueCount = await _demandService.GetOverdueCountAsync();
        }

        private async Task PopulateFilterListsAsync(DemandIndexViewModel viewModel)
        {
            // Status filter
            viewModel.AvailableStatuses = Enum.GetValues<DemandStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = GetStatusDisplayName(s)
                }).ToList();

            // Products filter
            var products = await _productService.GetAllAsync();
            viewModel.AvailableProducts = products
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = $"{p.Name} ({p.SKU})"
                }).ToList();
        }

        private async Task PopulateCreateViewModelListsAsync(CreateDemandViewModel viewModel)
        {
            // Order Items
            var orderItems = await _orderItemService.GetAllAsync();
            var orderItemsList = new List<SelectListItem>();
            
            foreach (var oi in orderItems)
            {
                // Buscar nome do produto
                var product = await _productService.GetByIdAsync(oi.ProductId);
                var productName = product?.Name ?? "Produto não identificado";
                
                orderItemsList.Add(new SelectListItem
                {
                    Value = oi.Id,
                    Text = $"Pedido {oi.OrderId} - {productName} ({oi.Quantity})"
                });
            }
            
            viewModel.AvailableOrderItems = orderItemsList;

            // Products
            var products = await _productService.GetAllAsync();
            viewModel.AvailableProducts = products
                .Select(p => new SelectListItem
                {
                    Value = p.Id,
                    Text = $"{p.Name} ({p.SKU})"
                }).ToList();

            // Production Orders
            var productionOrders = await _productionOrderService.GetAllAsync();
            viewModel.AvailableProductionOrders = productionOrders
                .Select(po => new SelectListItem
                {
                    Value = po.Id,
                    Text = $"OP-{po.Id.Substring(0, 8)} - {po.Status}"
                }).ToList();
        }

        private async Task PopulateEditViewModelListsAsync(EditDemandViewModel viewModel)
        {
            // Mesmas listas do Create, mais Status
            await PopulateCreateViewModelListsAsync(new CreateDemandViewModel
            {
                AvailableOrderItems = viewModel.AvailableOrderItems,
                AvailableProducts = viewModel.AvailableProducts,
                AvailableProductionOrders = viewModel.AvailableProductionOrders
            });

            // Status
            viewModel.AvailableStatuses = Enum.GetValues<DemandStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = GetStatusDisplayName(s)
                }).ToList();
        }

        private async Task PopulateDetailsViewModelAsync(DemandDetailsViewModel viewModel, 
            GesN.Web.Models.Entities.Production.Demand demand)
        {
            // Carregar dados do produto
            var product = await _productService.GetByIdAsync(demand.ProductId);
            if (product != null)
            {
                viewModel.ProductName = product.Name;
                viewModel.ProductSKU = product.SKU ?? "";
            }

            // Carregar dados do OrderItem
            var orderItem = await _orderItemService.GetByIdAsync(demand.OrderItemId);
            if (orderItem != null)
            {
                // Buscar nome do produto
                var orderProduct = await _productService.GetByIdAsync(orderItem.ProductId);
                var orderProductName = orderProduct?.Name ?? "Produto não identificado";
                viewModel.OrderItemDisplay = $"Pedido {orderItem.OrderId} - {orderProductName}";
            }

            // Carregar dados da Production Order
            if (!string.IsNullOrEmpty(demand.ProductionOrderId))
            {
                var productionOrder = await _productionOrderService.GetByIdAsync(demand.ProductionOrderId);
                if (productionOrder != null)
                {
                    viewModel.ProductionOrderDisplay = $"OP-{productionOrder.Id.Substring(0, 8)} - {productionOrder.Status}";
                }
            }
        }

        private async Task PopulateActionPermissionsAsync(DemandDetailsViewModel viewModel, 
            GesN.Web.Models.Entities.Production.Demand demand, string userId)
        {
            viewModel.CanConfirm = await _demandService.CanConfirmAsync(demand.Id);
            viewModel.CanMarkAsProduced = await _demandService.CanMarkAsProducedAsync(demand.Id);
            viewModel.CanMarkAsEnding = await _demandService.CanMarkAsEndingAsync(demand.Id);
            viewModel.CanMarkAsDelivered = await _demandService.CanMarkAsDeliveredAsync(demand.Id);
            viewModel.CanDelete = await _demandService.CanDeleteAsync(demand.Id);
            viewModel.CanEdit = demand.Status != DemandStatus.Delivered;
        }

        private async Task PopulateEditPermissionsAsync(EditDemandViewModel viewModel, 
            GesN.Web.Models.Entities.Production.Demand demand, string userId)
        {
            viewModel.CanChangeStatus = demand.Status != DemandStatus.Delivered;
            viewModel.CanChangeExpectedDate = demand.Status != DemandStatus.Delivered;
            viewModel.IsReadonly = demand.Status == DemandStatus.Delivered;
        }

        private async Task PopulateEditViewModelDataAsync(EditDemandViewModel viewModel, 
            GesN.Web.Models.Entities.Production.Demand demand)
        {
            var product = await _productService.GetByIdAsync(demand.ProductId);
            if (product != null)
            {
                viewModel.ProductName = product.Name;
            }

            var orderItem = await _orderItemService.GetByIdAsync(demand.OrderItemId);
            if (orderItem != null)
            {
                // Buscar nome do produto
                var orderProduct = await _productService.GetByIdAsync(orderItem.ProductId);
                var orderProductName = orderProduct?.Name ?? "Produto não identificado";
                viewModel.OrderItemDisplay = $"Pedido {orderItem.OrderId} - {orderProductName}";
            }

            viewModel.StatusDisplay = demand.GetStatusDisplay();
        }

        private async Task<IActionResult> ExecuteStatusChangeAsync(string id, 
            Func<string, string, Task<bool>> operation, string successMessage, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID da demanda não informado." });
            }

            try
            {
                var userId = GetCurrentUserId();
                var success = await operation(id, userId);

                if (success)
                {
                    return Json(new { success = true, message = successMessage });
                }
                else
                {
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"{errorMessage}: {ex.Message}" });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        }

        private string GetStatusDisplayName(DemandStatus status)
        {
            return status switch
            {
                DemandStatus.Pending => "Pendente",
                DemandStatus.Confirmed => "Confirmado",
                DemandStatus.Produced => "Produzido",
                DemandStatus.Ending => "Finalizando",
                DemandStatus.Delivered => "Entregue",
                _ => status.ToString()
            };
        }

        #endregion
    }
} 