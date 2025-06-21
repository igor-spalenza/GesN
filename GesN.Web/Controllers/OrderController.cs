using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GesN.Web.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de pedidos
    /// </summary>
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _customerService = customerService;
            _logger = logger;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetActiveOrdersAsync();
                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos");
                TempData["ErrorMessage"] = "Erro ao listar pedidos. Por favor, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Order/Grid
        public async Task<IActionResult> Grid()
        {
            try
            {
                var orders = await _orderService.GetActiveOrdersAsync();
                var orderViewModels = orders.Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    NumberSequence = o.NumberSequence,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer?.FullName,
                    OrderDate = o.OrderDate,
                    DeliveryDate = o.DeliveryDate ?? DateTime.Today.AddDays(1),
                    Type = o.Type,
                    Status = o.Status,
                    PrintStatus = o.PrintStatus,
                    Subtotal = o.Subtotal,
                    DiscountAmount = o.DiscountAmount,
                    TaxAmount = o.TaxAmount,
                    TotalAmount = o.TotalAmount,
                    Notes = o.Notes,
                    CreatedAt = o.CreatedAt,
                    LastModifiedAt = o.LastModifiedAt
                });

                return PartialView("_Grid", orderViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar grid de pedidos");
                return PartialView("_Error", "Erro ao carregar lista de pedidos");
            }
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes do pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao buscar detalhes do pedido. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/DetailsPartial/5
        public async Task<IActionResult> DetailsPartial(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                var detailsViewModel = new OrderDetailsViewModel
                {
                    Id = order.Id,
                    NumberSequence = order.NumberSequence,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.FullName,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate ?? DateTime.Today.AddDays(1),
                    Type = order.Type,
                    Status = order.Status,
                    PrintStatus = order.PrintStatus,
                    Subtotal = order.Subtotal,
                    DiscountAmount = order.DiscountAmount,
                    TaxAmount = order.TaxAmount,
                    TotalAmount = order.TotalAmount,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    LastModifiedAt = order.LastModifiedAt,
                    Items = order.Items?.Select(i => new OrderItemViewModel
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountAmount = i.DiscountAmount,
                        TaxAmount = i.TaxAmount,
                        Notes = i.Notes
                    }).ToList() ?? new List<OrderItemViewModel>()
                };

                return PartialView("_Details", detailsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar detalhes parciais do pedido: {Id}", id);
                return PartialView("_Error", "Erro ao carregar detalhes do pedido");
            }
        }

        public IActionResult CreatePartial()
        {
            return PartialView("_Create");
        }

        // POST: Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel orderViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(orderViewModel);
                }

                // Cria o pedido
                var orderId = await _orderService.CreateOrderAsync(orderViewModel);
                TempData["SuccessMessage"] = "Pedido criado com sucesso!";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                TempData["ErrorMessage"] = "Erro ao criar pedido. Por favor, tente novamente.";
                return View(orderViewModel);
            }
        }

        // POST: Order/SalvarNovo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovo([FromForm] CreateOrderViewModel orderViewModel)
        {
            try
            {
                // Remove campos automáticos do ModelState
                ModelState.Remove("Id");
                ModelState.Remove("NumberSequence");
                ModelState.Remove("CreatedAt");
                ModelState.Remove("LastModifiedAt");
                ModelState.Remove("CreatedBy");
                ModelState.Remove("LastModifiedBy");

                if (!ModelState.IsValid)
                {
                    // Retorna os erros de validação do modelo
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { 
                        success = false, 
                        message = "Erro de validação", 
                        errors = errors 
                    });
                }
                
                var orderId = await _orderService.CreateOrderAsync(orderViewModel);
                
                // Busca o pedido criado para obter o NumberSequence
                var createdOrder = await _orderService.GetOrderByIdAsync(orderId);
                
                return Json(new
                {
                    success = true,
                    message = "Pedido criado com sucesso!",
                    id = orderId,
                    numberSequence = createdOrder?.NumberSequence
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                return Json(new { 
                    success = false, 
                    message = "Erro ao criar pedido: " + ex.Message 
                });
            }
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Verifica se o pedido pode ser editado
                if (order.Status != OrderStatus.Draft)
                {
                    TempData["ErrorMessage"] = "Apenas pedidos em rascunho podem ser editados.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Carrega lista de clientes
                var customers = await _customerService.GetActiveCustomersAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "FullName");

                // Carrega lista de tipos de pedido
                ViewBag.OrderTypes = new SelectList(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name");

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao carregar formulário. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/EditPartial/5
        public async Task<IActionResult> EditPartial(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                var editViewModel = new EditOrderViewModel
                {
                    Id = order.Id,
                    NumberSequence = order.NumberSequence,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.FullName,
                    OrderDate = order.OrderDate,
                    DeliveryDate = order.DeliveryDate ?? DateTime.Today.AddDays(1),
                    Type = order.Type,
                    Status = order.Status,
                    PrintStatus = order.PrintStatus,
                    Subtotal = order.Subtotal,
                    DiscountAmount = order.DiscountAmount,
                    TaxAmount = order.TaxAmount,
                    TotalAmount = order.TotalAmount,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    LastModifiedAt = order.LastModifiedAt,
                    Items = order.Items?.Select(i => new OrderItemViewModel
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountAmount = i.DiscountAmount,
                        TaxAmount = i.TaxAmount,
                        Notes = i.Notes
                    }).ToList() ?? new List<OrderItemViewModel>()
                };

                return PartialView("_Edit", editViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição parcial do pedido: {Id}", id);
                return PartialView("_Error", "Erro ao carregar formulário de edição");
            }
        }

        // POST: Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Order order)
        {
            try
            {
                if (id != order.Id)
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    // Recarrega dados necessários para o formulário
                    var customers = await _customerService.GetActiveCustomersAsync();
                    ViewBag.Customers = new SelectList(customers, "Id", "FullName");
                    ViewBag.OrderTypes = new SelectList(Enum.GetValues(typeof(OrderType))
                        .Cast<OrderType>()
                        .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name");

                    return View(order);
                }

                // Verifica se o pedido existe
                var existingOrder = await _orderService.GetOrderByIdAsync(id);
                if (existingOrder == null)
                    return NotFound();

                // Verifica se o pedido pode ser editado
                if (existingOrder.Status != OrderStatus.Draft)
                {
                    TempData["ErrorMessage"] = "Apenas pedidos em rascunho podem ser editados.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Define dados de auditoria
                order.LastModifiedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                order.CreatedBy = existingOrder.CreatedBy;
                order.CreatedAt = existingOrder.CreatedAt;
                order.Status = existingOrder.Status;
                order.PrintStatus = existingOrder.PrintStatus;
                order.NumberSequence = existingOrder.NumberSequence;

                // Atualiza o pedido
                var success = await _orderService.UpdateOrderAsync(order, order.LastModifiedBy);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao atualizar pedido. Por favor, tente novamente.";
                    return View(order);
                }

                TempData["SuccessMessage"] = "Pedido atualizado com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao atualizar pedido. Por favor, tente novamente.";

                // Recarrega dados necessários para o formulário
                var customers = await _customerService.GetActiveCustomersAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "FullName");
                ViewBag.OrderTypes = new SelectList(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(t => new { Id = (int)t, Name = t.ToString() }), "Id", "Name");

                return View(order);
            }
        }

        // POST: Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Json(new { success = false, message = "ID do pedido não informado" });

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return Json(new { success = false, message = "Pedido não encontrado" });

                // Verifica se o pedido pode ser excluído
                if (order.Status != OrderStatus.Draft)
                {
                    return Json(new { success = false, message = "Apenas pedidos em rascunho podem ser excluídos" });
                }

                // Exclui o pedido
                var success = await _orderService.DeleteOrderAsync(id);
                if (!success)
                {
                    return Json(new { success = false, message = "Erro ao excluir pedido. Por favor, tente novamente." });
                }

                return Json(new { success = true, message = "Pedido excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Id}", id);
                return Json(new { success = false, message = "Erro ao excluir pedido: " + ex.Message });
            }
        }

        // POST: Order/Confirm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Confirma o pedido
                var success = await _orderService.ConfirmOrderAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao confirmar pedido. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Pedido confirmado com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao confirmar pedido. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Cancela o pedido
                var success = await _orderService.CancelOrderAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao cancelar pedido. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Pedido cancelado com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao cancelar pedido. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/StartProduction/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProduction(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Inicia produção
                var success = await _orderService.StartOrderProductionAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao iniciar produção do pedido. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Produção do pedido iniciada com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar produção do pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao iniciar produção do pedido. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/MarkForDelivery/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkForDelivery(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Marca para entrega
                var success = await _orderService.MarkOrderReadyForDeliveryAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao marcar pedido para entrega. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Pedido marcado para entrega com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar pedido para entrega: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao marcar pedido para entrega. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/StartDelivery/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartDelivery(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Inicia entrega
                var success = await _orderService.StartOrderDeliveryAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao iniciar entrega do pedido. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Entrega do pedido iniciada com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar entrega do pedido: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao iniciar entrega do pedido. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/Deliver/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deliver(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Marca como entregue
                var success = await _orderService.MarkOrderDeliveredAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao marcar pedido como entregue. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Pedido marcado como entregue com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar pedido como entregue: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao marcar pedido como entregue. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Order/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Marca como concluído
                var success = await _orderService.CompleteOrderAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!success)
                {
                    TempData["ErrorMessage"] = "Erro ao marcar pedido como concluído. Por favor, tente novamente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                TempData["SuccessMessage"] = "Pedido marcado como concluído com sucesso!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar pedido como concluído: {Id}", id);
                TempData["ErrorMessage"] = "Erro ao marcar pedido como concluído. Por favor, tente novamente.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Order/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                    return RedirectToAction(nameof(Index));

                var orders = await _orderService.SearchOrdersAsync(searchTerm);
                ViewBag.SearchTerm = searchTerm;
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por termo: {SearchTerm}", searchTerm);
                TempData["ErrorMessage"] = "Erro ao buscar pedidos. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/ByStatus
        public async Task<IActionResult> ByStatus(OrderStatus status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                ViewBag.Status = status;
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por status: {Status}", status);
                TempData["ErrorMessage"] = "Erro ao buscar pedidos. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/PendingDelivery
        public async Task<IActionResult> PendingDelivery()
        {
            try
            {
                var orders = await _orderService.GetPendingDeliveryOrdersAsync();
                ViewBag.Status = "Pendentes de Entrega";
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de entrega");
                TempData["ErrorMessage"] = "Erro ao buscar pedidos. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Order/PendingPrint
        public async Task<IActionResult> PendingPrint()
        {
            try
            {
                var orders = await _orderService.GetPendingPrintOrdersAsync();
                ViewBag.Status = "Pendentes de Impressão";
                return View("Index", orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de impressão");
                TempData["ErrorMessage"] = "Erro ao buscar pedidos. Por favor, tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
