using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.ViewModels.Sales;

namespace GesN.Web.Interfaces.Services
{
    /// <summary>
    /// Interface do serviço de pedidos
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<Order>> GetAllOrdersAsync();

        /// <summary>
        /// Obtém um pedido pelo ID
        /// </summary>
        Task<Order?> GetOrderByIdAsync(string id);

        /// <summary>
        /// Obtém um pedido pelo número sequencial
        /// </summary>
        Task<Order?> GetOrderByNumberAsync(string numberSequence);

        /// <summary>
        /// Obtém todos os pedidos de um cliente
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(string customerId);

        /// <summary>
        /// Obtém todos os pedidos com um determinado status
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<Order>> GetActiveOrdersAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de entrega
        /// </summary>
        Task<IEnumerable<Order>> GetPendingDeliveryOrdersAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de impressão
        /// </summary>
        Task<IEnumerable<Order>> GetPendingPrintOrdersAsync();

        /// <summary>
        /// Busca pedidos por termo de pesquisa
        /// </summary>
        Task<IEnumerable<Order>> SearchOrdersAsync(string searchTerm);

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        Task<string> CreateOrderAsync(CreateOrderViewModel order);

        /// <summary>
        /// Atualiza um pedido existente
        /// </summary>
        Task<bool> UpdateOrderAsync(Order order, string modifiedBy);

        /// <summary>
        /// Exclui um pedido (soft delete)
        /// </summary>
        Task<bool> DeleteOrderAsync(string id);

        /// <summary>
        /// Verifica se um pedido existe
        /// </summary>
        Task<bool> OrderExistsAsync(string id);

        /// <summary>
        /// Obtém o total de pedidos ativos
        /// </summary>
        Task<int> GetOrderCountAsync();

        /// <summary>
        /// Obtém pedidos paginados
        /// </summary>
        Task<IEnumerable<Order>> GetPagedOrdersAsync(int page, int pageSize);

        /// <summary>
        /// Obtém o próximo número sequencial disponível
        /// </summary>
        Task<string> GetNextOrderNumberAsync();

        /// <summary>
        /// Atualiza o status de impressão de um pedido
        /// </summary>
        Task<bool> UpdateOrderPrintStatusAsync(string id, PrintStatus status, int? batchNumber = null);

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(string id, OrderStatus status, string modifiedBy);

        /// <summary>
        /// Adiciona um item ao pedido
        /// </summary>
        Task<string> AddOrderItemAsync(string orderId, OrderItem item, string modifiedBy);

        /// <summary>
        /// Atualiza um item do pedido
        /// </summary>
        Task<bool> UpdateOrderItemAsync(string orderId, OrderItem item, string modifiedBy);

        /// <summary>
        /// Remove um item do pedido
        /// </summary>
        Task<bool> RemoveOrderItemAsync(string orderId, string itemId, string modifiedBy);

        /// <summary>
        /// Obtém todos os itens de um pedido
        /// </summary>
        Task<IEnumerable<OrderItem>> GetOrderItemsAsync(string orderId);

        /// <summary>
        /// Obtém todos os itens de um pedido pelo ID do pedido
        /// </summary>
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(string orderId);

        /// <summary>
        /// Obtém um item de pedido pelo ID
        /// </summary>
        Task<OrderItem?> GetOrderItemByIdAsync(string id);

        /// <summary>
        /// Valida os dados de um pedido
        /// </summary>
        Task<bool> ValidateOrderDataAsync(Order order);
        Task<bool> ValidateCreateOrderDataAsync(CreateOrderViewModel order);

        /// <summary>
        /// Valida os dados de um item de pedido
        /// </summary>
        Task<bool> ValidateOrderItemDataAsync(OrderItem item);

        /// <summary>
        /// Calcula os totais de um pedido
        /// </summary>
        Task CalculateOrderTotalsAsync(string orderId);

        /// <summary>
        /// Finaliza um pedido
        /// </summary>
        Task<bool> CompleteOrderAsync(string id, string modifiedBy);

        /// <summary>
        /// Cancela um pedido
        /// </summary>
        Task<bool> CancelOrderAsync(string id, string modifiedBy);

        /// <summary>
        /// Confirma um pedido
        /// </summary>
        Task<bool> ConfirmOrderAsync(string id, string modifiedBy);

        /// <summary>
        /// Marca um pedido como em produção
        /// </summary>
        Task<bool> StartOrderProductionAsync(string id, string modifiedBy);

        /// <summary>
        /// Marca um pedido como pronto para entrega
        /// </summary>
        Task<bool> MarkOrderReadyForDeliveryAsync(string id, string modifiedBy);

        /// <summary>
        /// Marca um pedido como em entrega
        /// </summary>
        Task<bool> StartOrderDeliveryAsync(string id, string modifiedBy);

        /// <summary>
        /// Marca um pedido como entregue
        /// </summary>
        Task<bool> MarkOrderDeliveredAsync(string id, string modifiedBy);
    }
} 