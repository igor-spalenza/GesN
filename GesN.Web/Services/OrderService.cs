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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
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

        public async Task<Order?> GetOrderByIdAsync(string id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                if (order != null)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(id)).ToList();
                }
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Order?> GetOrderByNumberAsync(string numberSequence)
        {
            try
            {
                var order = await _orderRepository.GetByNumberAsync(numberSequence);
                if (order != null)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por número: {Number}", numberSequence);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            try
            {
                var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos do cliente: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                var orders = await _orderRepository.GetByStatusAsync(status);
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos por status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetActiveAsync();
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos ativos");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetPendingDeliveryOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetPendingDeliveryAsync();
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de entrega");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetPendingPrintOrdersAsync()
        {
            try
            {
                var orders = await _orderRepository.GetPendingPrintAsync();
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos pendentes de impressão");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm)
        {
            try
            {
                var orders = await _orderRepository.SearchAsync(searchTerm);
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
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
                var order = new Order
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

        public async Task<bool> UpdateOrderAsync(Order order, string modifiedBy)
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

                // Atualiza os itens
                if (success && order.Items != null)
                {
                    // Remove itens excluídos
                    var existingItems = await _orderItemRepository.GetByOrderIdAsync(order.Id);
                    var itemsToDelete = existingItems.Where(ei => !order.Items.Any(i => i.Id == ei.Id));
                    foreach (var item in itemsToDelete)
                    {
                        await _orderItemRepository.DeleteAsync(item.Id);
                    }

                    // Atualiza ou cria itens
                    foreach (var item in order.Items)
                    {
                        item.OrderId = order.Id;
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            await _orderItemRepository.CreateAsync(item);
                        }
                        else
                        {
                            await _orderItemRepository.UpdateAsync(item);
                        }
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido: {Id}", order.Id);
                throw;
            }
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se pode ser excluído
                if (order.Status != OrderStatus.Draft)
                    throw new InvalidOperationException("Apenas pedidos em rascunho podem ser excluídos");

                // Exclui os itens
                await _orderItemRepository.DeleteByOrderIdAsync(id);

                // Exclui o pedido
                return await _orderRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> OrderExistsAsync(string id)
        {
            try
            {
                return await _orderRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<int> GetOrderCountAsync()
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

        public async Task<IEnumerable<Order>> GetPagedOrdersAsync(int page, int pageSize)
        {
            try
            {
                var orders = await _orderRepository.GetPagedAsync(page, pageSize);
                foreach (var order in orders)
                {
                    order.Items = (await _orderItemRepository.GetByOrderIdAsync(order.Id)).ToList();
                }
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos paginados: Page={Page}, PageSize={PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<string> GetNextOrderNumberAsync()
        {
            try
            {
                return await _orderRepository.GetNextNumberSequenceAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter próximo número de pedido");
                throw;
            }
        }

        public async Task<bool> UpdateOrderPrintStatusAsync(string id, PrintStatus status, int? batchNumber = null)
        {
            try
            {
                // Verifica se o pedido existe
                if (!await _orderRepository.ExistsAsync(id))
                    return false;

                return await _orderRepository.UpdatePrintStatusAsync(id, status, batchNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status de impressão do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, OrderStatus status, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se a transição de status é válida
                if (!IsValidStatusTransition(order.Status, status))
                    throw new InvalidOperationException($"Transição de status inválida: {order.Status} -> {status}");

                return await _orderRepository.UpdateStatusAsync(id, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(string id)
        {
            try
            {
                return await _orderItemRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar item de pedido por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(string orderId)
        {
            try
            {
                return await _orderItemRepository.GetByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsAsync(string orderId)
        {
            try
            {
                return await _orderItemRepository.GetByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<string> AddOrderItemAsync(string orderId, OrderItem item, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    throw new InvalidOperationException("Pedido não encontrado");

                // Verifica se o pedido pode ser alterado
                if (order.Status != OrderStatus.Draft)
                    throw new InvalidOperationException("Apenas pedidos em rascunho podem ser alterados");

                // Valida o item
                if (!await ValidateOrderItemDataAsync(item))
                    throw new InvalidOperationException("Item de pedido inválido");

                // Adiciona o item
                item.OrderId = orderId;
                item.CreatedBy = modifiedBy;
                item.LastModifiedBy = modifiedBy;
                var itemId = await _orderItemRepository.CreateAsync(item);

                // Recalcula totais do pedido
                await CalculateOrderTotalsAsync(orderId);

                return itemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar item ao pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> UpdateOrderItemAsync(string orderId, OrderItem item, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser alterado
                if (order.Status != OrderStatus.Draft)
                    throw new InvalidOperationException("Apenas pedidos em rascunho podem ser alterados");

                // Verifica se o item existe
                var existingItem = await _orderItemRepository.GetByIdAsync(item.Id);
                if (existingItem == null)
                    return false;

                // Verifica se o item pertence ao pedido
                if (existingItem.OrderId != orderId)
                    throw new InvalidOperationException("Item não pertence ao pedido");

                // Valida o item
                if (!await ValidateOrderItemDataAsync(item))
                    throw new InvalidOperationException("Item de pedido inválido");

                // Atualiza o item
                item.OrderId = orderId;
                item.LastModifiedBy = modifiedBy;
                var success = await _orderItemRepository.UpdateAsync(item);

                // Recalcula totais do pedido
                if (success)
                {
                    await CalculateOrderTotalsAsync(orderId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> RemoveOrderItemAsync(string orderId, string itemId, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser alterado
                if (order.Status != OrderStatus.Draft)
                    throw new InvalidOperationException("Apenas pedidos em rascunho podem ser alterados");

                // Verifica se o item existe
                var item = await _orderItemRepository.GetByIdAsync(itemId);
                if (item == null)
                    return false;

                // Verifica se o item pertence ao pedido
                if (item.OrderId != orderId)
                    throw new InvalidOperationException("Item não pertence ao pedido");

                // Remove o item
                var success = await _orderItemRepository.DeleteAsync(itemId);

                // Recalcula totais do pedido
                if (success)
                {
                    await CalculateOrderTotalsAsync(orderId);
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover item do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> CompleteOrderAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser completado
                if (order.Status != OrderStatus.Delivered)
                    throw new InvalidOperationException("Apenas pedidos entregues podem ser completados");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.Completed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao completar pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser cancelado
                if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
                    throw new InvalidOperationException("Pedido não pode ser cancelado");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.Cancelled);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ConfirmOrderAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser confirmado
                if (order.Status != OrderStatus.Draft)
                    throw new InvalidOperationException("Apenas pedidos em rascunho podem ser confirmados");

                // Verifica se tem itens
                var items = await _orderItemRepository.GetByOrderIdAsync(id);
                if (!items.Any())
                    throw new InvalidOperationException("Pedido não possui itens");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.Confirmed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> MarkOrderReadyForDeliveryAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser marcado para entrega
                if (order.Status != OrderStatus.InProduction)
                    throw new InvalidOperationException("Apenas pedidos em produção podem ser marcados para entrega");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.ReadyForDelivery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar pedido para entrega: {Id}", id);
                throw;
            }
        }

        public async Task<bool> StartOrderDeliveryAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode iniciar entrega
                if (order.Status != OrderStatus.ReadyForDelivery)
                    throw new InvalidOperationException("Apenas pedidos prontos para entrega podem iniciar entrega");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.InDelivery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar entrega do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> MarkOrderDeliveredAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode ser entregue
                if (order.Status != OrderStatus.InDelivery)
                    throw new InvalidOperationException("Apenas pedidos em entrega podem ser marcados como entregues");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.Delivered);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao entregar pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> StartOrderProductionAsync(string id, string modifiedBy)
        {
            try
            {
                // Verifica se o pedido existe
                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                    return false;

                // Verifica se o pedido pode iniciar produção
                if (order.Status != OrderStatus.Confirmed)
                    throw new InvalidOperationException("Apenas pedidos confirmados podem iniciar produção");

                // Atualiza o status
                return await _orderRepository.UpdateStatusAsync(id, OrderStatus.InProduction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar produção do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ValidateOrderDataAsync(Order order)
        {
            // Validações básicas
            if (order == null)
                return false;

            if (string.IsNullOrEmpty(order.CustomerId))
                return false;

            if (order.OrderDate == default)
                return false;

            if (order.DeliveryDate == default)
                return false;

            if (order.DeliveryDate < order.OrderDate)
                return false;

            // Verifica se o cliente existe
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            if (customer == null)
                return false;

            // Valida os itens
            if (order.Items != null)
            {
                foreach (var item in order.Items)
                {
                    if (!await ValidateOrderItemDataAsync(item))
                        return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidateOrderItemDataAsync(OrderItem item)
        {
            if (item == null)
                return false;

            if (string.IsNullOrEmpty(item.ProductId))
                return false;

            if (item.Quantity <= 0)
                return false;

            if (item.UnitPrice < 0)
                return false;

            if (item.DiscountAmount < 0)
                return false;

            if (item.TaxAmount < 0)
                return false;

            return true;
        }

        public async Task<bool> ValidateCreateOrderDataAsync(CreateOrderViewModel orderViewModel)
        {
            if (orderViewModel == null)
                return false;

            if (string.IsNullOrEmpty(orderViewModel.CustomerId))
                return false;

            // Verifica se o cliente existe
            var customer = await _customerRepository.GetByIdAsync(orderViewModel.CustomerId);
            if (customer == null)
                return false;

            return true;
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                // Rascunho -> Confirmado
                (OrderStatus.Draft, OrderStatus.Confirmed) => true,

                // Confirmado -> Em Produção
                (OrderStatus.Confirmed, OrderStatus.InProduction) => true,

                // Em Produção -> Pronto para Entrega
                (OrderStatus.InProduction, OrderStatus.ReadyForDelivery) => true,

                // Pronto para Entrega -> Em Entrega
                (OrderStatus.ReadyForDelivery, OrderStatus.InDelivery) => true,

                // Em Entrega -> Entregue
                (OrderStatus.InDelivery, OrderStatus.Delivered) => true,

                // Entregue -> Concluído
                (OrderStatus.Delivered, OrderStatus.Completed) => true,

                // Qualquer status -> Cancelado (exceto Concluído e Cancelado)
                (var status, OrderStatus.Cancelled) => 
                    status != OrderStatus.Completed && status != OrderStatus.Cancelled,

                // Mantém o mesmo status
                (var current, var next) when current == next => true,

                // Demais transições são inválidas
                _ => false
            };
        }

        public async Task CalculateOrderTotalsAsync(string orderId)
        {
            try
            {
                // Busca o pedido
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return;

                // Busca os itens
                var items = await _orderItemRepository.GetByOrderIdAsync(orderId);

                // Calcula os totais
                order.Subtotal = items.Sum(i => i.Subtotal);
                order.DiscountAmount = items.Sum(i => i.DiscountAmount);
                order.TaxAmount = items.Sum(i => i.TaxAmount);
                order.TotalAmount = items.Sum(i => i.TotalPrice);

                // Atualiza o pedido
                await _orderRepository.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recalcular totais do pedido: {OrderId}", orderId);
                throw;
            }
        }
    }
} 