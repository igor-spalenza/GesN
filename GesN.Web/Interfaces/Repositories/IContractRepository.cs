using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract?> GetByIdAsync(string id);
        Task<Contract?> GetByContractNumberAsync(string contractNumber);
        Task<IEnumerable<Contract>> GetByCustomerIdAsync(string customerId);
        Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status);
        Task<IEnumerable<Contract>> GetActiveAsync();
        Task<IEnumerable<Contract>> GetExpiringAsync(int days = 30);
        Task<IEnumerable<Contract>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(Contract contract);
        Task<bool> UpdateAsync(Contract contract);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<Contract>> GetPagedAsync(int page, int pageSize);
    }
} 