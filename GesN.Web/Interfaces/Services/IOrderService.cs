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
        Task<IEnumerable<OrderEntry>> GetAllOrdersAsync();

        /// <summary>
        /// Obtém um pedido pelo ID
        /// </summary>
        Task<OrderEntry?> GetOrderByIdAsync(string id);

        /// <summary>
        /// Obtém um pedido pelo número sequencial
        /// </summary>
        Task<OrderEntry?> GetOrderByNumberAsync(string numberSequence);

        /// <summary>
        /// Obtém todos os pedidos de um cliente
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetOrdersByCustomerIdAsync(string customerId);

        /// <summary>
        /// Obtém todos os pedidos com um determinado status
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetActiveOrdersAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de entrega
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPendingDeliveryOrdersAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de impressão
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPendingPrintOrdersAsync();

        /// <summary>
        /// Busca pedidos por termo de pesquisa
        /// </summary>
        Task<IEnumerable<OrderEntry>> SearchOrdersAsync(string searchTerm);

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        Task<string> CreateOrderAsync(CreateOrderViewModel order);

        /// <summary>
        /// Atualiza um pedido existente
        /// </summary>
        Task<bool> UpdateOrderAsync(OrderEntry order, string modifiedBy);

        /// <summary>
        /// Exclui um pedido
        /// </summary>
        Task<bool> DeleteOrderAsync(string id);

        /// <summary>
        /// Obtém pedidos paginados
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPagedOrdersAsync(int page, int pageSize);

        /// <summary>
        /// Conta o número total de pedidos
        /// </summary>
        Task<int> CountOrdersAsync();

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        Task<bool> UpdateOrderStatusAsync(string id, OrderStatus status, string modifiedBy);

        /// <summary>
        /// Atualiza o status de impressão de um pedido
        /// </summary>
        Task<bool> UpdatePrintStatusAsync(string id, PrintStatus printStatus, string modifiedBy);

        /// <summary>
        /// Obtém estatísticas dos pedidos
        /// </summary>
        Task<OrderStatisticsViewModel> GetOrderStatisticsAsync();

        /// <summary>
        /// Converte um pedido para ViewModel
        /// </summary>
        OrderViewModel ConvertToViewModel(OrderEntry order);

        /// <summary>
        /// Converte um pedido para DetailsViewModel
        /// </summary>
        OrderDetailsViewModel ConvertToDetailsViewModel(OrderEntry order);

        /// <summary>
        /// Converte um pedido para EditViewModel
        /// </summary>
        EditOrderViewModel ConvertToEditViewModel(OrderEntry order);

        /// <summary>
        /// Valida os dados de um pedido
        /// </summary>
        Task<bool> ValidateOrderDataAsync(OrderEntry order);

        /// <summary>
        /// Valida os dados de um pedido
        /// </summary>
        Task<bool> ValidateCreateOrderDataAsync(CreateOrderViewModel order);
    }
} 