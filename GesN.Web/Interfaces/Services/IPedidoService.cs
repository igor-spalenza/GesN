using GesN.Web.Models;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Pedido> GetByIdAsync(int id);
        Task<IEnumerable<Pedido>> GetAllAsync();
        Task<int> AddAsync(Pedido pedidoDto);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(Pedido pedidoDto);
        Task DeleteAsync(int id);
    }
}
