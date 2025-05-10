using GesN.Web.Models;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Pedido> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<int> AddAsync(Pedido clienteDto);
        Task UpdateAsync(Pedido clienteDto);
        Task DeleteAsync(int id);
    }
}
