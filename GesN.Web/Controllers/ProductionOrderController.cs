using Microsoft.AspNetCore.Mvc;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using Microsoft.AspNetCore.Authorization;

namespace GesN.Web.Controllers
{
    [Authorize]
    public class ProductionOrderController : Controller
    {
        private readonly IProductionOrderService _productionOrderService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public ProductionOrderController(
            IProductionOrderService productionOrderService,
            IProductService productService,
            IOrderService orderService)
        {
            _productionOrderService = productionOrderService;
            _productService = productService;
            _orderService = orderService;
        }

        // GET: ProductionOrder
        public async Task<IActionResult> Index(ProductionOrderStatus? status = null)
        {
            IEnumerable<ProductionOrder> orders;
            
            if (status.HasValue)
            {
                orders = await _productionOrderService.GetByStatusAsync(status.Value);
                ViewBag.CurrentStatus = status.Value;
            }
            else
            {
                orders = await _productionOrderService.GetAllAsync();
            }

            await PopulateViewBagAsync();
            return View(orders);
        }

        // GET: ProductionOrder/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _productionOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // GET: ProductionOrder/Create
        public async Task<IActionResult> Create(string? orderId = null, string? productId = null)
        {
            await PopulateDropdownsAsync();
            
            var productionOrder = new ProductionOrder
            {
                Priority = ProductionOrderPriority.Normal
            };
            
            if (!string.IsNullOrEmpty(orderId))
                productionOrder.OrderId = orderId;
                
            if (!string.IsNullOrEmpty(productId))
                productionOrder.ProductId = productId;
            
            return View(productionOrder);
        }

        // POST: ProductionOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionOrder productionOrder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _productionOrderService.CreateAsync(productionOrder);
                    TempData["SuccessMessage"] = "Ordem de produção criada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(productionOrder);
        }

        // GET: ProductionOrder/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _productionOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            await PopulateDropdownsAsync();
            return View(order);
        }

        // POST: ProductionOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductionOrder productionOrder)
        {
            if (id != productionOrder.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var success = await _productionOrderService.UpdateAsync(productionOrder);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Ordem de produção atualizada com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ordem de produção não encontrada.");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Erro interno do servidor.");
                }
            }

            await PopulateDropdownsAsync();
            return View(productionOrder);
        }

        // GET: ProductionOrder/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _productionOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: ProductionOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var success = await _productionOrderService.DeleteAsync(id);
                if (success)
                {
                    TempData["SuccessMessage"] = "Ordem de produção excluída com sucesso!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ordem de produção não encontrada.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao excluir ordem de produção.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Start Production
        [HttpPost]
        public async Task<IActionResult> StartProduction(string id, string assignedTo)
        {
            try
            {
                var success = await _productionOrderService.StartProductionAsync(id, assignedTo);
                if (success)
                {
                    return Json(new { success = true, message = "Produção iniciada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ordem de produção não encontrada." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: Complete Production
        [HttpPost]
        public async Task<IActionResult> CompleteProduction(string id, decimal? actualTime = null)
        {
            try
            {
                var success = await _productionOrderService.CompleteProductionAsync(id, actualTime);
                if (success)
                {
                    return Json(new { success = true, message = "Produção concluída com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ordem de produção não encontrada." });
                }
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: Cancel Production
        [HttpPost]
        public async Task<IActionResult> CancelProduction(string id, string reason)
        {
            try
            {
                var success = await _productionOrderService.CancelProductionAsync(id, reason);
                if (success)
                {
                    return Json(new { success = true, message = "Produção cancelada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ordem de produção não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: Pause Production
        [HttpPost]
        public async Task<IActionResult> PauseProduction(string id, string reason)
        {
            try
            {
                var success = await _productionOrderService.PauseProductionAsync(id, reason);
                if (success)
                {
                    return Json(new { success = true, message = "Produção pausada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ordem de produção não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // POST: Resume Production
        [HttpPost]
        public async Task<IActionResult> ResumeProduction(string id)
        {
            try
            {
                var success = await _productionOrderService.ResumeProductionAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Produção retomada com sucesso!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ordem de produção não encontrada." });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro interno do servidor." });
            }
        }

        // GET: Schedule Production
        public async Task<IActionResult> Schedule(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var order = await _productionOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        // POST: Schedule Production
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Schedule(string id, DateTime scheduledStart, DateTime scheduledEnd)
        {
            try
            {
                var success = await _productionOrderService.ScheduleProductionAsync(id, scheduledStart, scheduledEnd);
                if (success)
                {
                    TempData["SuccessMessage"] = "Produção agendada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Ordem de produção não encontrada.";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Erro ao agendar produção.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var pendingCount = await _productionOrderService.GetPendingOrdersCountAsync();
            var completedCount = await _productionOrderService.GetCompletedOrdersCountAsync(DateTime.Today.AddDays(-30), DateTime.Today);
            var overdueOrders = await _productionOrderService.GetOverdueOrdersAsync();
            var avgCompletionTime = await _productionOrderService.GetAverageCompletionTimeAsync();

            ViewBag.PendingCount = pendingCount;
            ViewBag.CompletedCount = completedCount;
            ViewBag.OverdueCount = overdueOrders.Count();
            ViewBag.AverageCompletionTime = avgCompletionTime;

            return View(overdueOrders);
        }

        // GET: Overdue Orders
        public async Task<IActionResult> Overdue()
        {
            var overdueOrders = await _productionOrderService.GetOverdueOrdersAsync();
            return View("Index", overdueOrders);
        }

        // AJAX: Search orders
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var orders = await _productionOrderService.SearchAsync(term);
                return PartialView("_OrderList", orders);
            }
            catch (Exception)
            {
                return PartialView("_OrderList", new List<ProductionOrder>());
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            var products = await _productService.GetAllAsync();
            ViewBag.Products = products.Select(p => new { p.Id, p.Name });

            var orders = await _orderService.GetActiveOrdersAsync();
            ViewBag.Orders = orders.Select(o => new { o.Id, Name = $"Pedido #{o.Id}" });
        }

        private async Task PopulateViewBagAsync()
        {
            ViewBag.StatusList = Enum.GetValues<ProductionOrderStatus>()
                .Select(s => new { Value = s, Text = GetStatusDisplayName(s) });

            ViewBag.PriorityList = Enum.GetValues<ProductionOrderPriority>()
                .Select(p => new { Value = p, Text = GetPriorityDisplayName(p) });
        }

        private static string GetStatusDisplayName(ProductionOrderStatus status)
        {
            return status switch
            {
                ProductionOrderStatus.Pending => "Pendente",
                ProductionOrderStatus.Scheduled => "Agendado",
                ProductionOrderStatus.InProgress => "Em Progresso",
                ProductionOrderStatus.Paused => "Pausado",
                ProductionOrderStatus.Completed => "Concluído",
                ProductionOrderStatus.Cancelled => "Cancelado",
                ProductionOrderStatus.Failed => "Falhou",
                _ => status.ToString()
            };
        }

        private static string GetPriorityDisplayName(ProductionOrderPriority priority)
        {
            return priority switch
            {
                ProductionOrderPriority.Low => "Baixa",
                ProductionOrderPriority.Normal => "Normal",
                ProductionOrderPriority.High => "Alta",
                ProductionOrderPriority.Urgent => "Urgente",
                _ => priority.ToString()
            };
        }
    }
} 