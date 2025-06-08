using GesN.Web.Models.Entities.Sales;

namespace GesN.Web.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByDocumentAsync(string documentNumber);
        Task<IEnumerable<Customer>> GetActiveAsync();
        Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<Customer>> GetPagedAsync(int page, int pageSize);
    }
} 