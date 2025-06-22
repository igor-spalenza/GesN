using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(string id);
        Task<Supplier?> GetByNameAsync(string name);
        Task<Supplier?> GetByEmailAsync(string email);
        Task<Supplier?> GetByDocumentAsync(string documentNumber);
        Task<IEnumerable<Supplier>> GetActiveAsync();
        Task<IEnumerable<Supplier>> SearchAsync(string searchTerm);
        Task<IEnumerable<Supplier>> SearchForAutocompleteAsync(string searchTerm);
        Task<string> CreateAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<Supplier>> GetPagedAsync(int page, int pageSize);
    }
} 