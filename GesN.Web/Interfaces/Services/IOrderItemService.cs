using GesN.Web.Models.Entities.Sales;

namespace GesN.Web.Interfaces.Services
{
    public interface IOrderItemService
    {
        // CRUD Operations
        Task<OrderItem?> GetByIdAsync(string id);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId);
        Task<OrderItem> CreateAsync(OrderItem orderItem);
        Task<bool> UpdateAsync(OrderItem orderItem);
        Task<bool> DeleteAsync(string id);
        
        // Business Operations
        Task<bool> ExistsAsync(string id);
        Task<decimal> CalculateItemTotalAsync(string id);
        Task<decimal> CalculateOrderTotalAsync(string orderId);
        Task<int> CountByOrderAsync(string orderId);
        Task<bool> CanDeleteAsync(string id);
        
        // Validation
        Task<IEnumerable<string>> ValidateOrderItemAsync(OrderItem orderItem);
        Task<bool> ValidateQuantityAsync(string productId, int quantity);
        
        // Bulk Operations
        Task<bool> DeleteByOrderIdAsync(string orderId);
        Task<IEnumerable<OrderItem>> CreateBulkAsync(IEnumerable<OrderItem> orderItems);
        Task<bool> UpdateOrderItemsAsync(string orderId, IEnumerable<OrderItem> orderItems);
    }
} 