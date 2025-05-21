using GesN.Web.Models;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IClienteRepository
    {
        Task<Cliente> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
        Task<IEnumerable<Cliente>> BuscarPorNomeOuTelefoneAsync(string termo);
    }
}
