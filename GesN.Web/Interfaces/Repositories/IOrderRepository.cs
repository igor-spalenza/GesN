using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Repositories
{
    /// <summary>
    /// Interface do repositório de pedidos
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetAllAsync();

        /// <summary>
        /// Obtém um pedido pelo ID
        /// </summary>
        Task<OrderEntry?> GetByIdAsync(string id);

        /// <summary>
        /// Obtém um pedido pelo número sequencial
        /// </summary>
        Task<OrderEntry?> GetByNumberAsync(string numberSequence);

        /// <summary>
        /// Obtém todos os pedidos de um cliente
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetByCustomerIdAsync(string customerId);

        /// <summary>
        /// Obtém todos os pedidos com um determinado status
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetByStatusAsync(OrderStatus status);

        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetActiveAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de entrega
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPendingDeliveryAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de impressão
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPendingPrintAsync();

        /// <summary>
        /// Busca pedidos por termo de pesquisa
        /// </summary>
        Task<IEnumerable<OrderEntry>> SearchAsync(string searchTerm);

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        Task<string> CreateAsync(OrderEntry order);

        /// <summary>
        /// Atualiza um pedido existente
        /// </summary>
        Task<bool> UpdateAsync(OrderEntry order);

        /// <summary>
        /// Exclui um pedido (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Verifica se um pedido existe
        /// </summary>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Conta o número total de pedidos
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Obtém pedidos paginados
        /// </summary>
        Task<IEnumerable<OrderEntry>> GetPagedAsync(int page, int pageSize);

        /// <summary>
        /// Gera o próximo número sequencial para pedidos
        /// </summary>
        Task<string> GetNextNumberSequenceAsync();

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        Task<bool> UpdateStatusAsync(string id, OrderStatus status, string modifiedBy);

        /// <summary>
        /// Atualiza o status de impressão de um pedido
        /// </summary>
        Task<bool> UpdatePrintStatusAsync(string id, PrintStatus printStatus, string modifiedBy);
    }
} 