using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductionOrderService
    {
        // CRUD básico
        Task<IEnumerable<ProductionOrder>> GetAllAsync();
        Task<ProductionOrder?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProductionOrder productionOrder);
        Task<bool> UpdateAsync(ProductionOrder productionOrder);
        Task<bool> DeleteAsync(string id);
        
        // Consultas por relacionamentos
        Task<IEnumerable<ProductionOrder>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<ProductionOrder>> GetByProductIdAsync(string productId);
        Task<IEnumerable<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status);
        Task<IEnumerable<ProductionOrder>> GetByAssignedUserAsync(string userId);
        
        // Gerenciamento de status
        Task<bool> StartProductionAsync(string productionOrderId, string assignedTo);
        Task<bool> CompleteProductionAsync(string productionOrderId, decimal? actualTime = null);
        Task<bool> CancelProductionAsync(string productionOrderId, string reason);
        Task<bool> PauseProductionAsync(string productionOrderId, string reason);
        Task<bool> ResumeProductionAsync(string productionOrderId);
        
        // Agendamento
        Task<bool> ScheduleProductionAsync(string productionOrderId, DateTime scheduledStart, DateTime scheduledEnd);
        Task<bool> RescheduleProductionAsync(string productionOrderId, DateTime newStart, DateTime newEnd);
        Task<IEnumerable<ProductionOrder>> GetScheduledOrdersAsync(DateTime date);
        Task<IEnumerable<ProductionOrder>> GetOverdueOrdersAsync();
        
        // Atribuição e prioridade
        Task<bool> AssignToUserAsync(string productionOrderId, string userId);
        Task<bool> UpdatePriorityAsync(string productionOrderId, ProductionOrderPriority priority);
        Task<IEnumerable<ProductionOrder>> GetByPriorityAsync(ProductionOrderPriority priority);
        
        // Relatórios e análises
        Task<decimal> GetAverageCompletionTimeAsync(string? productId = null);
        Task<int> GetCompletedOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetPendingOrdersCountAsync();
        Task<decimal> GetProductionEfficiencyAsync(string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        
        // Validações
        Task<bool> ValidateProductionOrderAsync(ProductionOrder order);
        Task<bool> CanStartProductionAsync(string productionOrderId);
        Task<bool> CanCompleteProductionAsync(string productionOrderId);
        
        // Pesquisa e paginação
        Task<IEnumerable<ProductionOrder>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductionOrder>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 