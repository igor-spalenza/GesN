using GesN.Web.Models;

namespace GesN.Web.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Pedido> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task AddAsync(Pedido clienteDto);
        Task UpdateAsync(Pedido clienteDto);
        Task DeleteAsync(int id);
    }
}
