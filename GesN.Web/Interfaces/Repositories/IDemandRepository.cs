using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Repositories
{
    /// <summary>
    /// Interface para o repositório de Demandas
    /// Define operações CRUD e consultas específicas para a entidade Demand
    /// </summary>
    public interface IDemandRepository
    {
        // Operações CRUD básicas
        Task<IEnumerable<Demand>> GetAllAsync();
        Task<Demand?> GetByIdAsync(string id);
        Task<string> CreateAsync(Demand demand);
        Task<bool> UpdateAsync(Demand demand);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);

        // Consultas específicas
        Task<IEnumerable<Demand>> GetByOrderItemIdAsync(string orderItemId);
        Task<IEnumerable<Demand>> GetByProductionOrderIdAsync(string productionOrderId);
        Task<IEnumerable<Demand>> GetByProductIdAsync(string productId);
        Task<IEnumerable<Demand>> GetByStatusAsync(DemandStatus status);

        Task<IEnumerable<Demand>> GetOverdueAsync();
        Task<IEnumerable<Demand>> GetDueInDaysAsync(int days);

        // Consultas para dashboard e relatórios
        Task<IEnumerable<Demand>> GetPendingDemandsAsync();
        Task<IEnumerable<Demand>> GetConfirmedDemandsAsync();
        Task<IEnumerable<Demand>> GetInProductionDemandsAsync();
        Task<IEnumerable<Demand>> GetEndingDemandsAsync();
        Task<IEnumerable<Demand>> GetDeliveredDemandsAsync();

        // Consultas por período
        Task<IEnumerable<Demand>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Demand>> GetCreatedInPeriodAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Demand>> GetCompletedInPeriodAsync(DateTime startDate, DateTime endDate);

        // Pesquisa e filtros
        Task<IEnumerable<Demand>> SearchAsync(string searchTerm);
        Task<IEnumerable<Demand>> SearchByProductNameAsync(string productName);
        Task<IEnumerable<Demand>> GetByMultipleStatusAsync(IEnumerable<DemandStatus> statuses);

        // Paginação
        Task<IEnumerable<Demand>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<Demand>> GetPagedByStatusAsync(DemandStatus status, int page, int pageSize);


        // Contadores e estatísticas
        Task<int> CountAsync();
        Task<int> CountByStatusAsync(DemandStatus status);

        Task<int> CountOverdueAsync();
        Task<int> CountDueInDaysAsync(int days);

        // Operações em lote
        Task<bool> UpdateStatusBatchAsync(IEnumerable<string> demandIds, DemandStatus newStatus);

        Task<bool> DeleteBatchAsync(IEnumerable<string> demandIds);

        // Validações e verificações
        Task<bool> HasActiveDemandForOrderItemAsync(string orderItemId);
        Task<bool> CanDeleteAsync(string id);
        Task<IEnumerable<Demand>> GetDependentDemandsAsync(string demandId);

        // Consultas com relacionamentos
        Task<IEnumerable<Demand>> GetWithOrderItemsAsync();
        Task<IEnumerable<Demand>> GetWithProductionOrdersAsync();
        Task<IEnumerable<Demand>> GetWithProductsAsync();
        Task<Demand?> GetWithAllRelationshipsAsync(string id);

        // Operações específicas do domínio
        Task<bool> StartProductionAsync(string id);
        Task<bool> CompleteProductionAsync(string id);
        Task<bool> CancelDemandAsync(string id, string reason);
        Task<bool> UpdateExpectedDateAsync(string id, DateTime newExpectedDate);
        Task<bool> AssignToProductionOrderAsync(string demandId, string productionOrderId);

        // Relatórios e métricas
        Task<IEnumerable<Demand>> GetProductionQueueAsync();
        Task<IEnumerable<Demand>> GetHighPriorityDemandsAsync();
        Task<Dictionary<DemandStatus, int>> GetStatusDistributionAsync();

        Task<IEnumerable<Demand>> GetDemandsWithoutProductionOrderAsync();
    }
} 