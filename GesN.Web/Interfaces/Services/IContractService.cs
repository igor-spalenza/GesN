using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Services
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> GetAllContractsAsync();
        Task<Contract?> GetContractByIdAsync(string id);
        Task<Contract?> GetContractByNumberAsync(string contractNumber);
        Task<IEnumerable<Contract>> GetContractsByCustomerAsync(string customerId);
        Task<IEnumerable<Contract>> GetContractsByStatusAsync(ContractStatus status);
        Task<IEnumerable<Contract>> GetActiveContractsAsync();
        Task<IEnumerable<Contract>> GetExpiringContractsAsync(int days = 30);
        Task<IEnumerable<Contract>> SearchContractsAsync(string searchTerm);
        Task<string> CreateContractAsync(Contract contract, string createdBy);
        Task<bool> UpdateContractAsync(Contract contract, string modifiedBy);
        Task<bool> DeleteContractAsync(string id);
        Task<bool> ContractExistsAsync(string id);
        Task<int> GetContractCountAsync();
        Task<IEnumerable<Contract>> GetPagedContractsAsync(int page, int pageSize);
        Task<bool> ConfirmContractAsync(string contractId, string confirmedBy);
        Task<bool> SignContractAsync(string contractId, string signedBy, DateTime? signDate = null);
        Task<bool> SuspendContractAsync(string contractId, string suspendedBy);
        Task<bool> CancelContractAsync(string contractId, string cancelledBy);
        Task<bool> CompleteContractAsync(string contractId, string completedBy);
        Task<bool> RenewContractAsync(string contractId, DateTime newEndDate, string renewedBy);
        Task<bool> ValidateContractDataAsync(Contract contract);
        Task<decimal> CalculateContractValueAsync(string contractId);
        Task<IEnumerable<Contract>> GetContractsNearExpirationAsync();
    }
} 