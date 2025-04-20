using GesN.Web.Models;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IPedidoRepository
    {
        Task<Pedido> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task AddAsync(Pedido pedido);
        Task UpdateAsync(Pedido pedido);
        Task DeleteAsync(int id);
    }
}
