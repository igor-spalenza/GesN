using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Services
{
    /// <summary>
    /// Interface para o serviço de Demandas
    /// Define operações de negócio e validações para a entidade Demand
    /// </summary>
    public interface IDemandService
    {
        // Operações CRUD básicas
        Task<IEnumerable<Demand>> GetAllAsync();
        Task<Demand?> GetByIdAsync(string id);
        Task<string> CreateAsync(Demand demand);
        Task<bool> UpdateAsync(Demand demand);
        Task<bool> DeleteAsync(string id);

        // Consultas específicas do domínio
        Task<IEnumerable<Demand>> GetByOrderItemIdAsync(string orderItemId);
        Task<IEnumerable<Demand>> GetByProductionOrderIdAsync(string productionOrderId);
        Task<IEnumerable<Demand>> GetByProductIdAsync(string productId);
        Task<IEnumerable<Demand>> GetByStatusAsync(DemandStatus status);
        Task<IEnumerable<Demand>> GetOverdueDemandsAsync();
        Task<IEnumerable<Demand>> GetUpcomingDemandsAsync(int days = 7);

        // Consultas para dashboard
        Task<IEnumerable<Demand>> GetPendingDemandsAsync();
        Task<IEnumerable<Demand>> GetConfirmedDemandsAsync();
        Task<IEnumerable<Demand>> GetInProductionDemandsAsync();
        Task<IEnumerable<Demand>> GetEndingDemandsAsync();
        Task<IEnumerable<Demand>> GetDeliveredDemandsAsync();

        // Pesquisa e filtros
        Task<IEnumerable<Demand>> SearchAsync(string searchTerm);
        Task<IEnumerable<Demand>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Demand>> GetPagedAsync(int page, int pageSize);

        // Operações de negócio
        Task<bool> ConfirmDemandAsync(string demandId, string userId);
        Task<bool> MarkAsProducedAsync(string demandId, string userId);
        Task<bool> MarkAsEndingAsync(string demandId, string userId);
        Task<bool> MarkAsDeliveredAsync(string demandId, string userId);
        Task<bool> UpdateExpectedDateAsync(string demandId, DateTime newExpectedDate, string userId);
        Task<bool> AssignToProductionOrderAsync(string demandId, string productionOrderId, string userId);

        // Validações de negócio
        Task<bool> CanConfirmAsync(string demandId);
        Task<bool> CanMarkAsProducedAsync(string demandId);
        Task<bool> CanMarkAsEndingAsync(string demandId);
        Task<bool> CanMarkAsDeliveredAsync(string demandId);
        Task<bool> CanDeleteAsync(string demandId);
        Task<IEnumerable<string>> ValidateDemandAsync(Demand demand);

        // Operações em lote
        Task<bool> UpdateStatusBatchAsync(IEnumerable<string> demandIds, DemandStatus newStatus, string userId);
        Task<bool> DeleteBatchAsync(IEnumerable<string> demandIds, string userId);

        // Relatórios e estatísticas
        Task<Dictionary<DemandStatus, int>> GetStatusDistributionAsync();
        Task<int> GetTotalActiveDemandsAsync();
        Task<int> GetOverdueCountAsync();
        Task<IEnumerable<Demand>> GetProductionQueueAsync();

        // Integração com outros módulos
        Task<bool> CreateDemandFromOrderItemAsync(string orderItemId, string userId);
        Task<IEnumerable<Demand>> GetDemandsForProductionPlanningAsync();
        Task<bool> NotifyStatusChangeAsync(string demandId, DemandStatus newStatus);

        // Análises de negócio
        Task<decimal> CalculateAverageLeadTimeAsync();
        Task<IEnumerable<Demand>> GetDelayedDemandsAsync();
        Task<Dictionary<string, int>> GetDemandsByProductAsync();
        Task<IEnumerable<Demand>> GetMostUrgentDemandsAsync(int count = 10);
    }
} 