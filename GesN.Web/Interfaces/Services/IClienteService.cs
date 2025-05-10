using GesN.Web.Models;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Interfaces.Services
{
    public interface IClienteService
    {
        Task<Cliente> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task AddAsync(ClienteCreateDto cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}
