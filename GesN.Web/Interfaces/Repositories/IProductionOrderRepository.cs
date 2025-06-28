using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductionOrderRepository
    {
        Task<IEnumerable<ProductionOrder>> GetAllAsync();
        Task<ProductionOrder?> GetByIdAsync(string id);
        Task<IEnumerable<ProductionOrder>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<ProductionOrder>> GetByOrderItemIdAsync(string orderItemId);
        Task<IEnumerable<ProductionOrder>> GetByProductIdAsync(string productId);
        Task<IEnumerable<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status);
        Task<IEnumerable<ProductionOrder>> GetByPriorityAsync(ProductionOrderPriority priority);
        Task<IEnumerable<ProductionOrder>> GetByAssignedToAsync(string assignedTo);
        Task<IEnumerable<ProductionOrder>> GetPendingOrdersAsync();
        Task<IEnumerable<ProductionOrder>> GetInProgressOrdersAsync();
        Task<IEnumerable<ProductionOrder>> GetCompletedOrdersAsync();
        Task<IEnumerable<ProductionOrder>> GetScheduledOrdersAsync();
        Task<IEnumerable<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProductionOrder>> SearchAsync(string searchTerm);
        Task<string> CreateAsync(ProductionOrder productionOrder);
        Task<bool> UpdateAsync(ProductionOrder productionOrder);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<int> CountByStatusAsync(ProductionOrderStatus status);
        Task<IEnumerable<ProductionOrder>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductionOrder>> GetByStatusPagedAsync(ProductionOrderStatus status, int page, int pageSize);
        Task<bool> UpdateStatusAsync(string id, ProductionOrderStatus status);
        Task<bool> AssignToUserAsync(string id, string assignedTo);
        Task<decimal> GetAverageCompletionTimeAsync(string? productId = null);
        Task<IEnumerable<ProductionOrder>> GetOverdueOrdersAsync();
    }
} 