using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(string id);
        Task<Supplier?> GetSupplierByNameAsync(string name);
        Task<Supplier?> GetSupplierByEmailAsync(string email);
        Task<Supplier?> GetSupplierByDocumentAsync(string documentNumber);
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
        Task<IEnumerable<Supplier>> SearchSuppliersAsync(string searchTerm);
        Task<IEnumerable<Supplier>> SearchSuppliersForAutocompleteAsync(string searchTerm);
        Task<string> CreateSupplierAsync(Supplier supplier, string createdBy);
        Task<bool> UpdateSupplierAsync(Supplier supplier, string modifiedBy);
        Task<bool> DeleteSupplierAsync(string id);
        Task<bool> SupplierExistsAsync(string id);
        Task<int> GetSupplierCountAsync();
        Task<int> GetActiveSupplierCountAsync();
        Task<IEnumerable<Supplier>> GetPagedSuppliersAsync(int page, int pageSize);
        Task<bool> ValidateSupplierDataAsync(Supplier supplier);
    }
} 