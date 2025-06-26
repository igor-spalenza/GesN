using GesN.Web.Models.Entities.Sales;

namespace GesN.Web.Interfaces.Repositories
{
    /// <summary>
    /// Interface do repositório de itens de pedido
    /// </summary>
    public interface IOrderItemRepository
    {
        /// <summary>
        /// Obtém todos os itens de um pedido
        /// </summary>
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId);

        /// <summary>
        /// Obtém um item de pedido pelo ID
        /// </summary>
        Task<OrderItem?> GetByIdAsync(string id);

        /// <summary>
        /// Obtém todos os itens de pedido de um produto
        /// </summary>
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId);

        /// <summary>
        /// Cria um novo item de pedido
        /// </summary>
        Task<string> CreateAsync(OrderItem item);

        /// <summary>
        /// Atualiza um item de pedido existente
        /// </summary>
        Task<bool> UpdateAsync(OrderItem item);

        /// <summary>
        /// Exclui um item de pedido
        /// </summary>
        Task<bool> DeleteAsync(string id);

        /// <summary>
        /// Exclui todos os itens de um pedido
        /// </summary>
        Task<bool> DeleteByOrderIdAsync(string orderId);

        /// <summary>
        /// Verifica se um item de pedido existe
        /// </summary>
        Task<bool> ExistsAsync(string id);

        /// <summary>
        /// Verifica se existem itens para um pedido
        /// </summary>
        Task<bool> ExistsByOrderIdAsync(string orderId);

        /// <summary>
        /// Obtém o total de itens de um pedido
        /// </summary>
        Task<int> CountByOrderIdAsync(string orderId);

        /// <summary>
        /// Obtém o total de itens de um produto
        /// </summary>
        Task<int> CountByProductIdAsync(string productId);
    }
} 