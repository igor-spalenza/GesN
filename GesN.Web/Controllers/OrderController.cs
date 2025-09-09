using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
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
        private readonly IProductService _productService;
        private readonly ICompositeProductXHierarchyService _compositeProductXHierarchyService;
        private readonly IProductComponentService _productComponentService;
        private readonly IProductGroupService _productGroupService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IOrderItemService _orderItemService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ICustomerService customerService,
            IProductService productService,
            ICompositeProductXHierarchyService compositeProductXHierarchyService,
            IProductComponentService productComponentService,
            IProductGroupService productGroupService,
            IProductCategoryService productCategoryService,
            IOrderItemService orderItemService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _customerService = customerService;
            _productService = productService;
            _compositeProductXHierarchyService = compositeProductXHierarchyService;
            _productComponentService = productComponentService;
            _productGroupService = productGroupService;
            _productCategoryService = productCategoryService;
            _orderItemService = orderItemService;
            _logger = logger;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            try
            {
                var orders = await _orderService.GetActiveOrdersAsync();
                
                // Converte as entidades para ViewModels
                var orderViewModels = orders.Select(o => new OrderEntryViewModel
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
                }).ToList();

                // Cria o ViewModel para a página Index
                var indexViewModel = new OrderEntryIndexViewModel
                {
                    Orders = orderViewModels,
                    Statistics = await _orderService.GetOrderStatisticsAsync(),
                    Search = new OrderEntrySearchViewModel(),
                    TotalOrders = orderViewModels.Count,
                    CurrentPage = 1,
                    PageSize = 50,
                    TotalPages = 1
                };

                return View(indexViewModel);
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
                var orderViewModels = orders.Select(o => new OrderEntryViewModel
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

                var detailsViewModel = new OrderEntryDetailsViewModel
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
                    Items = order.Items?.Select(i => new OrderEntryItemViewModel
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountAmount = i.DiscountAmount,
                        TaxAmount = i.TaxAmount,
                        Notes = i.Notes
                    }).ToList() ?? new List<OrderEntryItemViewModel>()
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
        public async Task<IActionResult> Create(CreateOrderEntryViewModel orderViewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(orderViewModel);
                }

                // Obter ID do usuário logado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";

                // Cria o pedido
                var orderId = await _orderService.CreateOrderAsync(orderViewModel, userId);
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
        public async Task<IActionResult> SalvarNovo([FromForm] CreateOrderEntryViewModel orderViewModel)
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

                // Obter ID do usuário logado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
                
                var orderId = await _orderService.CreateOrderAsync(orderViewModel, userId);
                
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

                var editViewModel = new EditOrderEntryViewModel
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
                    Items = order.Items?.Select(i => new OrderEntryItemViewModel
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.Product?.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountAmount = i.DiscountAmount,
                        TaxAmount = i.TaxAmount,
                        Notes = i.Notes
                    }).ToList() ?? new List<OrderEntryItemViewModel>()
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
        public async Task<IActionResult> Edit(string id, OrderEntry order)
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
                order.LastModifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema";
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Confirmed, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.InProduction, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.ReadyForDelivery, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.InDelivery, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Delivered, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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
                var success = await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Completed, User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Sistema");
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

        /// <summary>
        /// Adiciona um produto ao carrinho do pedido
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AdicionarProdutoAoCarrinho([FromBody] AdicionarProdutoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OrderId) || string.IsNullOrEmpty(request.ProductId))
                {
                    return Json(new { success = false, message = "Dados incompletos" });
                }

                // Verifica se o pedido existe
                var order = await _orderService.GetOrderByIdAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Pedido não encontrado" });
                }

                // Buscar dados do produto para preencher o OrderItem
                var product = await _productService.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Produto não encontrado" });
                }

                // Criar OrderItem
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid().ToString(),
                    OrderId = request.OrderId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity > 0 ? request.Quantity : 1,
                    UnitPrice = product.UnitPrice,
                    DiscountAmount = 0,
                    TaxAmount = 0,
                    Notes = "",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "Sistema"
                };

                await _orderItemService.CreateAsync(orderItem);

                // Recarregar os itens do pedido para retornar
                var updatedOrder = await _orderService.GetOrderByIdAsync(request.OrderId);
                var itemsViewModel = updatedOrder?.Items?.Select(i => new OrderEntryItemViewModel
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    DiscountAmount = i.DiscountAmount,
                    TaxAmount = i.TaxAmount,
                    Notes = i.Notes
                }).ToList() ?? new List<OrderEntryItemViewModel>();

                return Json(new { 
                    success = true, 
                    message = $"Produto '{product.Name}' adicionado ao carrinho!",
                    totalItems = itemsViewModel.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar produto ao carrinho");
                return Json(new { success = false, message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// [MIGRADO] Método movido para OrderItemController.ReloadItems
        /// Mantido temporariamente para compatibilidade
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> RecarregarItens(string orderId)
        {
            // Redirecionar para OrderItemController
            return RedirectToAction("ReloadItems", "OrderItem", new { orderId = orderId });
        }

        // GET: Order/CompositeProductModal
        // GET: Order/ProductCatalog - Endpoint para carregar catálogo seguindo padrão _Grid.cshtml
        [HttpGet]
        public async Task<IActionResult> ProductCatalog(string? category = null, string? search = null)
        {
            try
            {
                // Usar ProductController para obter dados (reutilizando lógica existente)
                var catalogResult = await GetProductCatalogData(category, search);
                
                return PartialView("_ProductCatalog", catalogResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar catálogo de produtos");
                var emptyViewModel = new GesN.Web.Models.ViewModels.Production.ProductCatalogViewModel
                {
                    Products = new List<GesN.Web.Models.Entities.Production.Product>(),
                    TotalProducts = 0
                };
                return PartialView("_ProductCatalog", emptyViewModel);
            }
        }

        private async Task<GesN.Web.Models.ViewModels.Production.ProductCatalogViewModel> GetProductCatalogData(string? category, string? search)
        {
            // Obter produtos ativos usando o padrão do projeto
            var allProducts = await _productService.GetActiveAsync();
            
            // Aplicar filtros nas entidades Product
            var filteredProducts = allProducts.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrWhiteSpace(p.SKU) && p.SKU.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                filteredProducts = filteredProducts.Where(p =>
                    !string.IsNullOrWhiteSpace(p.Category) &&
                    p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // ProductCatalogViewModel espera IEnumerable<Product> (entidades)
            var viewModel = new GesN.Web.Models.ViewModels.Production.ProductCatalogViewModel
            {
                Products = filteredProducts, // Usar entidades Product diretamente
                CurrentCategory = category,
                SearchTerm = search,
                TotalProducts = filteredProducts.Count()
            };

            return viewModel;
        }

        // GET: Order/SimpleProductModal
        [HttpGet]
        public async Task<IActionResult> SimpleProductModal(string productId, int initialQuantity = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                    return BadRequest("ProductId é obrigatório");

                // Buscar dados do produto simples
                var product = await _productService.GetByIdAsync(productId);
                
                if (product == null)
                {
                    return PartialView("_Error", "Produto não encontrado");
                }

                if (product.ProductType != ProductType.Simple)
                {
                    return PartialView("_Error", "Este endpoint é específico para produtos simples");
                }

                if (product.StateCode != ObjectState.Active)
                {
                    return PartialView("_Error", "Produto não está disponível");
                }

                // Criar objeto dinâmico com dados do produto
                var productData = new
                {
                    id = product.Id,
                    name = product.Name,
                    sku = product.SKU,
                    description = product.Description,
                    category = product.Category,
                    unitPrice = product.UnitPrice,
                    assemblyTime = product.AssemblyTime,
                    assemblyInstructions = product.AssemblyInstructions,
                    imageUrl = product.ImageUrl,
                    initialQuantity = initialQuantity
                };

                return PartialView("_SimpleProductModal", productData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modal de produto simples: {ProductId}", productId);
                return PartialView("_Error", "Erro ao carregar configuração do produto");
            }
        }

        // GET: Order/CompositeProductModal
        [HttpGet]
        public async Task<IActionResult> CompositeProductModal(string productId, int initialQuantity = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                    return BadRequest("ProductId é obrigatório");

                // Buscar dados do produto composto
                var product = await _productService.GetByIdAsync(productId);
                
                if (product == null)
                {
                    return PartialView("_Error", "Produto não encontrado");
                }

                if (product.ProductType != ProductType.Composite)
                {
                    return PartialView("_Error", "Este endpoint é específico para produtos compostos");
                }

                if (product.StateCode != ObjectState.Active)
                {
                    return PartialView("_Error", "Produto não está disponível");
                }

                // Carregar hierarquias e componentes reais
                var hierarchyRelations = await _compositeProductXHierarchyService.GetActiveProductHierarchiesAsync(productId);
                var hierarchyList = hierarchyRelations.ToList();

                if (!hierarchyList.Any())
                {
                    return PartialView("_Error", "Produto composto não possui hierarquias configuradas");
                }

                // Carregar componentes para cada hierarquia
                var hierarchiesWithComponents = new List<object>();

                foreach (var hierarchyRelation in hierarchyList.OrderBy(h => h.AssemblyOrder))
                {
                    // Buscar componentes da hierarquia
                    var components = await _productComponentService.GetByHierarchyIdAsync(hierarchyRelation.ProductComponentHierarchyId);
                    var activeComponents = components.Where(c => c.StateCode == ObjectState.Active).ToList();

                    if (!activeComponents.Any())
                    {
                        _logger.LogWarning("Hierarquia {HierarchyName} não possui componentes ativos", hierarchyRelation.HierarchyName);
                        continue;
                    }

                    hierarchiesWithComponents.Add(new
                    {
                        id = hierarchyRelation.ProductComponentHierarchyId,
                        name = hierarchyRelation.HierarchyName,
                        isOptional = hierarchyRelation.IsOptional,
                        minQuantity = hierarchyRelation.MinQuantity,
                        maxQuantity = hierarchyRelation.MaxQuantity,
                        assemblyOrder = hierarchyRelation.AssemblyOrder,
                        notes = hierarchyRelation.Notes,
                        components = activeComponents.Select(c => new
                        {
                            id = c.Id,
                            name = c.Name,
                            description = c.Description,
                            additionalCost = c.AdditionalCost
                        }).ToArray()
                    });
                }

                // Criar objeto dinâmico com dados reais do produto
                var productData = new
                {
                    id = product.Id,
                    name = product.Name,
                    sku = product.SKU,
                    description = product.Description,
                    category = product.Category,
                    unitPrice = product.UnitPrice,
                    assemblyTime = product.AssemblyTime,
                    assemblyInstructions = product.AssemblyInstructions,
                    imageUrl = product.ImageUrl,
                    initialQuantity = initialQuantity,
                    hierarchies = hierarchiesWithComponents.ToArray()
                };

                return PartialView("_CompositeProductModal", productData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modal de produto composto: {ProductId}", productId);
                return PartialView("_Error", "Erro ao carregar configuração do produto");
            }
        }

        // GET: Order/GroupProductModal
        [HttpGet]
        public async Task<IActionResult> GroupProductModal(string productId, int initialQuantity = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                    return BadRequest("ProductId é obrigatório");

                // Buscar dados do produto grupo
                var product = await _productService.GetByIdAsync(productId);
                
                if (product == null)
                {
                    return PartialView("_Error", "Produto não encontrado");
                }

                if (product.ProductType != ProductType.Group)
                {
                    return PartialView("_Error", "Este endpoint é específico para grupos de produtos");
                }

                if (product.StateCode != ObjectState.Active)
                {
                    return PartialView("_Error", "Produto não está disponível");
                }

                // Carregar itens do grupo com dados reais
                var groupItems = await _productGroupService.GetGroupItemsWithProductDataAsync(productId);
                var groupItemsList = groupItems.ToList();

                if (!groupItemsList.Any())
                {
                    return PartialView("_Error", "Grupo de produtos não possui itens configurados");
                }

                // Carregar regras de troca
                var exchangeRules = await _productGroupService.GetExchangeRulesAsync(productId);
                var exchangeRulesList = exchangeRules.ToList();

                // Processar itens do grupo com opções de produtos
                var processedGroupItems = new List<object>();

                foreach (var groupItem in groupItemsList)
                {
                    var productOptions = new List<object>();

                    // Se o item se relaciona diretamente com um produto
                    if (!string.IsNullOrEmpty(groupItem.ProductId) && groupItem.Product != null)
                    {
                        productOptions.Add(new
                        {
                            id = groupItem.Product.Id,
                            name = groupItem.Product.Name,
                            description = groupItem.Product.Description ?? "",
                            price = groupItem.Product.UnitPrice,
                            effectivePrice = groupItem.Product.UnitPrice + groupItem.ExtraPrice,
                            isAvailable = groupItem.Product.StateCode == ObjectState.Active,
                            productType = groupItem.Product.ProductType.ToString(),
                            sku = groupItem.Product.SKU ?? ""
                        });
                    }
                    // Se o item se relaciona indiretamente através de categoria
                    else if (!string.IsNullOrEmpty(groupItem.ProductCategoryId))
                    {
                        // Buscar produtos da categoria
                        var categoryProducts = await _productService.GetActiveAsync();
                        var productsInCategory = categoryProducts
                            .Where(p => p.CategoryId == groupItem.ProductCategoryId && p.StateCode == ObjectState.Active)
                            .ToList();

                        foreach (var categoryProduct in productsInCategory)
                        {
                            productOptions.Add(new
                            {
                                id = categoryProduct.Id,
                                name = categoryProduct.Name,
                                description = categoryProduct.Description ?? "",
                                price = categoryProduct.UnitPrice,
                                effectivePrice = categoryProduct.UnitPrice + groupItem.ExtraPrice,
                                isAvailable = categoryProduct.StateCode == ObjectState.Active,
                                productType = categoryProduct.ProductType.ToString(),
                                sku = categoryProduct.SKU ?? ""
                            });
                        }
                    }

                    processedGroupItems.Add(new
                    {
                        id = groupItem.Id,
                        displayName = _productGroupService.GetGroupItemDisplayName(groupItem),
                        itemType = groupItem.GetItemType(),
                        isOptional = groupItem.IsOptional,
                        minQuantity = groupItem.MinQuantity,
                        maxQuantity = groupItem.MaxQuantity,
                        defaultQuantity = groupItem.DefaultQuantity,
                        extraPrice = groupItem.ExtraPrice,
                        productOptions = productOptions.ToArray()
                    });
                }

                // Processar regras de troca
                var processedExchangeRules = exchangeRulesList.Select(rule => new
                {
                    id = rule.Id,
                    description = $"{rule.SourceGroupItemWeight} → {rule.TargetGroupItemWeight}",
                    ratioDescription = $"Proporção {rule.ExchangeRatio}:1",
                    sourceGroupItemId = rule.SourceGroupItemId,
                    targetGroupItemId = rule.TargetGroupItemId,
                    exchangeRatio = rule.ExchangeRatio,
                    isActive = rule.IsActive
                }).ToArray();

                // Calcular estatísticas do grupo
                var requiredItems = groupItemsList.Count(item => !item.IsOptional);
                var optionalItems = groupItemsList.Count(item => item.IsOptional);

                // Criar objeto dinâmico com dados reais do grupo
                var productData = new
                {
                    id = product.Id,
                    name = product.Name,
                    sku = product.SKU,
                    description = product.Description,
                    category = product.Category,
                    unitPrice = product.UnitPrice,
                    assemblyTime = product.AssemblyTime,
                    assemblyInstructions = product.AssemblyInstructions,
                    imageUrl = product.ImageUrl,
                    initialQuantity = initialQuantity,
                    totalItems = groupItemsList.Count,
                    requiredItems = requiredItems,
                    optionalItems = optionalItems,
                    groupItems = processedGroupItems.ToArray(),
                    exchangeRules = processedExchangeRules
                };

                return PartialView("_GroupProductModal", productData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modal de grupo de produtos: {ProductId}", productId);
                return PartialView("_Error", "Erro ao carregar configuração do grupo");
            }
        }
    }

    /// <summary>
    /// Request para adicionar produto ao carrinho
    /// </summary>
    public class AdicionarProdutoRequest
    {
        public string OrderId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// Request específico para adicionar produto simples ao carrinho
    /// </summary>
    public class AddSimpleProductRequest
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }
    }
} 
