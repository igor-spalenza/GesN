using GesN.Web.Models.Entities.Sales;

namespace GesN.Web.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(string id);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<Customer?> GetCustomerByDocumentAsync(string documentNumber);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<string> CreateCustomerAsync(Customer customer, string createdBy);
        Task<bool> UpdateCustomerAsync(Customer customer, string modifiedBy);
        Task<bool> DeleteCustomerAsync(string id);
        Task<bool> CustomerExistsAsync(string id);
        Task<int> GetCustomerCountAsync();
        Task<int> GetActiveCustomerCountAsync();
        Task<IEnumerable<Customer>> GetPagedCustomersAsync(int page, int pageSize);
        Task<bool> ValidateCustomerDataAsync(Customer customer);
    }
} 