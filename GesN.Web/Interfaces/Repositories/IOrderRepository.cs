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
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Obtém um pedido pelo ID
        /// </summary>
        Task<Order?> GetByIdAsync(string id);

        /// <summary>
        /// Obtém um pedido pelo número sequencial
        /// </summary>
        Task<Order?> GetByNumberAsync(string numberSequence);

        /// <summary>
        /// Obtém todos os pedidos de um cliente
        /// </summary>
        Task<IEnumerable<Order>> GetByCustomerIdAsync(string customerId);

        /// <summary>
        /// Obtém todos os pedidos com um determinado status
        /// </summary>
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

        /// <summary>
        /// Obtém todos os pedidos ativos
        /// </summary>
        Task<IEnumerable<Order>> GetActiveAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de entrega
        /// </summary>
        Task<IEnumerable<Order>> GetPendingDeliveryAsync();

        /// <summary>
        /// Obtém todos os pedidos pendentes de impressão
        /// </summary>
        Task<IEnumerable<Order>> GetPendingPrintAsync();

        /// <summary>
        /// Busca pedidos por termo de pesquisa
        /// </summary>
        Task<IEnumerable<Order>> SearchAsync(string searchTerm);

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        Task<string> CreateAsync(Order order);

        /// <summary>
        /// Atualiza um pedido existente
        /// </summary>
        Task<bool> UpdateAsync(Order order);

        /// <summary>
        /// Exclui um pedido (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Verifica se um pedido existe
        /// </summary>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Obtém o total de pedidos ativos
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Obtém pedidos paginados
        /// </summary>
        Task<IEnumerable<Order>> GetPagedAsync(int page, int pageSize);

        /// <summary>
        /// Obtém o próximo número sequencial disponível
        /// </summary>
        Task<string> GetNextNumberSequenceAsync();

        /// <summary>
        /// Atualiza o status de impressão de um pedido
        /// </summary>
        Task<bool> UpdatePrintStatusAsync(string id, PrintStatus status, int? batchNumber = null);

        /// <summary>
        /// Atualiza o status de um pedido
        /// </summary>
        Task<bool> UpdateStatusAsync(string id, OrderStatus status);
    }
} 