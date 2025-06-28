using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    /// <summary>
    /// Implementação do serviço de pedidos
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICustomerRepository customerRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderEntry>> GetAllOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pedidos");
                throw;
            }
        }

        public async Task<OrderEntry?> GetOrderByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do pedido não fornecido");
                    return null;
                }

                var order = await _orderRepository.GetByIdAsync(id);
                
                if (order != null)
                {
                    // Carrega os itens do pedido
                    var items = await _orderItemRepository.GetByOrderIdAsync(id);
                    order.Items = items.ToList();
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por ID: {Id}", id);
                throw;
            }
        }

        public async Task<OrderEntry?> GetOrderByNumberAsync(string numberSequence)
        {
            try
            {
                if (string.IsNullOrEmpty(numberSequence))
                {
                    _logger.LogWarning("Número do pedido não fornecido");
                    return null;
                }

                var order = await _orderRepository.GetByNumberAsync(numberSequence);
                
                if (order != null)
                {
                    // Carrega os itens do pedido
                    var items = await _orderItemRepository.GetByOrderIdAsync(order.Id);
                    order.Items = items.ToList();
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por número: {Number}", numberSequence);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetOrdersByCustomerIdAsync(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogWarning("ID do cliente não fornecido");
                    return Enumerable.Empty<OrderEntry>();
                }

                return await _orderRepository.GetByCustomerIdAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos do cliente: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                return await _orderRepository.GetByStatusAsync(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetActiveOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos ativos");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPendingDeliveryOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetPendingDeliveryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de entrega");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPendingPrintOrdersAsync()
        {
            try
            {
                return await _orderRepository.GetPendingPrintAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de impressão");
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> SearchOrdersAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm))
                {
                    return await GetAllOrdersAsync();
                }

                return await _orderRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por termo: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<string> CreateOrderAsync(CreateOrderViewModel orderViewModel)
        {
            try
            {
                // Validações
                if (!await ValidateCreateOrderDataAsync(orderViewModel))
                    throw new InvalidOperationException("Dados do pedido inválidos");

                // Converte ViewModel para Entity
                var order = new OrderEntry
                {
                    CustomerId = orderViewModel.CustomerId,
                    Type = orderViewModel.Type,
                    OrderDate = DateTime.Now,
                    DeliveryDate = DateTime.Now.AddDays(1),
                    Status = OrderStatus.Draft,
                    PrintStatus = PrintStatus.NotPrinted,
                    Subtotal = 0,
                    DiscountAmount = 0,
                    TaxAmount = 0,
                    TotalAmount = 0
                };

                // Gera número sequencial
                order.NumberSequence = await _orderRepository.GetNextNumberSequenceAsync();

                // Cria o pedido
                var orderId = await _orderRepository.CreateAsync(order);

                return orderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                throw;
            }
        }

        public async Task<bool> UpdateOrderAsync(OrderEntry order, string modifiedBy)
        {
            try
            {
                // Validações
                if (!await ValidateOrderDataAsync(order))
                    throw new InvalidOperationException("Pedido inválido");

                // Define dados de auditoria
                order.LastModifiedBy = modifiedBy;

                // Atualiza o pedido
                var success = await _orderRepository.UpdateAsync(order);

                if (success)
                {
                    _logger.LogInformation("Pedido atualizado com sucesso: {OrderId}", order.Id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do pedido não fornecido para exclusão");
                    return false;
                }

                var success = await _orderRepository.DeleteAsync(id);

                if (success)
                {
                    _logger.LogInformation("Pedido excluído com sucesso: {OrderId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderEntry>> GetPagedOrdersAsync(int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                return await _orderRepository.GetPagedAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos paginados: Page={Page}, PageSize={PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<int> CountOrdersAsync()
        {
            try
            {
                return await _orderRepository.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar pedidos");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, OrderStatus status, string modifiedBy)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do pedido não fornecido para atualização de status");
                    return false;
                }

                var success = await _orderRepository.UpdateStatusAsync(id, status, modifiedBy);

                if (success)
                {
                    _logger.LogInformation("Status do pedido atualizado: {OrderId} -> {Status}", id, status);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<bool> UpdatePrintStatusAsync(string id, PrintStatus printStatus, string modifiedBy)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do pedido não fornecido para atualização de status de impressão");
                    return false;
                }

                var success = await _orderRepository.UpdatePrintStatusAsync(id, printStatus, modifiedBy);

                if (success)
                {
                    _logger.LogInformation("Status de impressão do pedido atualizado: {OrderId} -> {PrintStatus}", id, printStatus);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status de impressão do pedido: {OrderId}", id);
                throw;
            }
        }

        public async Task<OrderStatisticsViewModel> GetOrderStatisticsAsync()
        {
            try
            {
                var allOrders = await _orderRepository.GetAllAsync();
                var stats = new OrderStatisticsViewModel();

                stats.TotalOrders = allOrders.Count();
                stats.DraftOrders = allOrders.Count(o => o.Status == OrderStatus.Draft);
                stats.ConfirmedOrders = allOrders.Count(o => o.Status == OrderStatus.Confirmed);
                stats.InProductionOrders = allOrders.Count(o => o.Status == OrderStatus.InProduction);
                stats.PendingDeliveryOrders = allOrders.Count(o => o.Status == OrderStatus.ReadyForDelivery || o.Status == OrderStatus.InDelivery);
                stats.CompletedOrders = allOrders.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);
                stats.TotalOrdersValue = allOrders.Sum(o => o.TotalAmount);

                var thisMonth = DateTime.Now.Month;
                var thisYear = DateTime.Now.Year;
                stats.NewOrdersThisMonth = allOrders.Count(o => o.CreatedAt.Month == thisMonth && o.CreatedAt.Year == thisYear);

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas dos pedidos");
                throw;
            }
        }

        public OrderViewModel ConvertToViewModel(OrderEntry order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                NumberSequence = order.NumberSequence,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FirstName + " " + order.Customer?.LastName,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate ?? DateTime.Today,
                Type = order.Type,
                Status = order.Status,
                PrintStatus = order.PrintStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount
            };
        }

        public OrderDetailsViewModel ConvertToDetailsViewModel(OrderEntry order)
        {
            var detailsViewModel = new OrderDetailsViewModel
            {
                Id = order.Id,
                NumberSequence = order.NumberSequence,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FirstName + " " + order.Customer?.LastName,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate ?? DateTime.Today,
                Type = order.Type,
                Status = order.Status,
                PrintStatus = order.PrintStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                LastModifiedAt = order.LastModifiedAt
            };

            // Converte itens se existirem
            if (order.Items?.Any() == true)
            {
                detailsViewModel.Items = order.Items.Select(item => new OrderItemViewModel
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Produto não encontrado",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TaxAmount = item.TaxAmount,
                    Notes = item.Notes
                }).ToList();
            }

            return detailsViewModel;
        }

        public EditOrderViewModel ConvertToEditViewModel(OrderEntry order)
        {
            var editViewModel = new EditOrderViewModel
            {
                Id = order.Id,
                NumberSequence = order.NumberSequence,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FirstName + " " + order.Customer?.LastName,
                OrderDate = order.OrderDate,
                DeliveryDate = order.DeliveryDate ?? DateTime.Today,
                Type = order.Type,
                Status = order.Status,
                PrintStatus = order.PrintStatus,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                LastModifiedAt = order.LastModifiedAt
            };

            // Define tipos disponíveis
            editViewModel.AvailableOrderTypes = new List<OrderTypeSelectionViewModel>
            {
                new() { Value = OrderType.Order, Text = "Pedido", IsSelected = order.Type == OrderType.Order },
                new() { Value = OrderType.Event, Text = "Evento", IsSelected = order.Type == OrderType.Event }
            };

            // Converte itens se existirem
            if (order.Items?.Any() == true)
            {
                editViewModel.Items = order.Items.Select(item => new OrderItemViewModel
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Produto não encontrado",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TaxAmount = item.TaxAmount,
                    Notes = item.Notes
                }).ToList();
            }

            return editViewModel;
        }

        public async Task<bool> ValidateOrderDataAsync(OrderEntry order)
        {
            try
            {
                if (order == null)
                {
                    _logger.LogWarning("Pedido nulo fornecido para validação");
                    return false;
                }

                if (string.IsNullOrEmpty(order.CustomerId))
                {
                    _logger.LogWarning("Cliente não informado no pedido");
                    return false;
                }

                // Verifica se o cliente existe
                var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
                if (customer == null)
                {
                    _logger.LogWarning("Cliente não encontrado: {CustomerId}", order.CustomerId);
                    return false;
                }

                if (order.OrderDate == default)
                {
                    _logger.LogWarning("Data do pedido não informada");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados do pedido");
                return false;
            }
        }

        public async Task<bool> ValidateCreateOrderDataAsync(CreateOrderViewModel order)
        {
            try
            {
                if (order == null)
                {
                    _logger.LogWarning("Dados do pedido nulos fornecidos para validação");
                    return false;
                }

                if (string.IsNullOrEmpty(order.CustomerId))
                {
                    _logger.LogWarning("Cliente não informado no pedido");
                    return false;
                }

                // Verifica se o cliente existe
                var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
                if (customer == null)
                {
                    _logger.LogWarning("Cliente não encontrado: {CustomerId}", order.CustomerId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados de criação do pedido");
                return false;
            }
        }
    }
} 