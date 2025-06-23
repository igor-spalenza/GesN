using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Services
{
    public class ProductionOrderService : IProductionOrderService
    {
        private readonly IProductionOrderRepository _productionOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductionOrderService(
            IProductionOrderRepository productionOrderRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _productionOrderRepository = productionOrderRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        // CRUD básico
        public async Task<IEnumerable<ProductionOrder>> GetAllAsync()
        {
            return await _productionOrderRepository.GetAllAsync();
        }

        public async Task<ProductionOrder?> GetByIdAsync(string id)
        {
            return await _productionOrderRepository.GetByIdAsync(id);
        }

        public async Task<string> CreateAsync(ProductionOrder productionOrder)
        {
            // Validações de negócio
            if (!await ValidateProductionOrderAsync(productionOrder))
                throw new InvalidOperationException("Ordem de produção inválida");

            // Configurar dados iniciais
            productionOrder.Id = Guid.NewGuid().ToString();
            productionOrder.Status = ProductionOrderStatus.Pending;
            productionOrder.StateCode = ObjectState.Active;
            productionOrder.CreatedAt = DateTime.UtcNow;
            productionOrder.LastModifiedAt = DateTime.UtcNow;

            // Estimar tempo de produção se não fornecido
            if (productionOrder.EstimatedTime == null)
            {
                var estimatedTime = await EstimateProductionTimeAsync(productionOrder.ProductId, productionOrder.Quantity);
                productionOrder.EstimatedTime = (int?)estimatedTime;
            }

            return await _productionOrderRepository.CreateAsync(productionOrder);
        }

        public async Task<bool> UpdateAsync(ProductionOrder productionOrder)
        {
            var existingOrder = await _productionOrderRepository.GetByIdAsync(productionOrder.Id);
            if (existingOrder == null)
                return false;

            // Validações de negócio
            if (!await ValidateProductionOrderAsync(productionOrder))
                throw new InvalidOperationException("Ordem de produção inválida");

            // Atualizar dados de auditoria
            productionOrder.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(productionOrder);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var order = await _productionOrderRepository.GetByIdAsync(id);
            if (order == null)
                return false;

            // Verificar se pode ser deletada
            if (order.Status == ProductionOrderStatus.InProgress)
                throw new InvalidOperationException("Não é possível deletar uma ordem em produção");

            return await _productionOrderRepository.DeleteAsync(id);
        }

        // Consultas por relacionamentos
        public async Task<IEnumerable<ProductionOrder>> GetByOrderIdAsync(string orderId)
        {
            return await _productionOrderRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<IEnumerable<ProductionOrder>> GetByProductIdAsync(string productId)
        {
            return await _productionOrderRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ProductionOrder>> GetByStatusAsync(ProductionOrderStatus status)
        {
            return await _productionOrderRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<ProductionOrder>> GetByAssignedUserAsync(string userId)
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders.Where(o => o.AssignedTo == userId);
        }

        // Gerenciamento de status
        public async Task<bool> StartProductionAsync(string productionOrderId, string assignedTo)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            if (!await CanStartProductionAsync(productionOrderId))
                throw new InvalidOperationException("Não é possível iniciar esta ordem de produção");

            order.Status = ProductionOrderStatus.InProgress;
            order.ActualStartDate = DateTime.UtcNow;
            order.AssignedTo = assignedTo;
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> CompleteProductionAsync(string productionOrderId, decimal? actualTime = null)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            if (!await CanCompleteProductionAsync(productionOrderId))
                throw new InvalidOperationException("Não é possível completar esta ordem de produção");

            order.Status = ProductionOrderStatus.Completed;
            order.ActualEndDate = DateTime.UtcNow;
            
            if (actualTime.HasValue)
            {
                order.ActualTime = (int?)actualTime.Value;
            }
            else if (order.ActualStartDate.HasValue)
            {
                var timeSpan = DateTime.UtcNow - order.ActualStartDate.Value;
                order.ActualTime = (int)timeSpan.TotalHours;
            }

            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> CancelProductionAsync(string productionOrderId, string reason)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            order.Status = ProductionOrderStatus.Cancelled;
            order.Notes = $"{order.Notes}\nCancelado: {reason}";
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> PauseProductionAsync(string productionOrderId, string reason)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            if (order.Status != ProductionOrderStatus.InProgress)
                return false;

            order.Status = ProductionOrderStatus.Paused;
            order.Notes = $"{order.Notes}\nPausado: {reason}";
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> ResumeProductionAsync(string productionOrderId)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            if (order.Status != ProductionOrderStatus.Paused)
                return false;

            order.Status = ProductionOrderStatus.InProgress;
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        // Agendamento
        public async Task<bool> ScheduleProductionAsync(string productionOrderId, DateTime scheduledStart, DateTime scheduledEnd)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            if (scheduledEnd <= scheduledStart)
                return false;

            order.ScheduledStartDate = scheduledStart;
            order.ScheduledEndDate = scheduledEnd;
            order.Status = ProductionOrderStatus.Scheduled;
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> RescheduleProductionAsync(string productionOrderId, DateTime newStart, DateTime newEnd)
        {
            return await ScheduleProductionAsync(productionOrderId, newStart, newEnd);
        }

        public async Task<IEnumerable<ProductionOrder>> GetScheduledOrdersAsync(DateTime date)
        {
            var allOrders = await _productionOrderRepository.GetByStatusAsync(ProductionOrderStatus.Scheduled);
            return allOrders.Where(o => 
                o.ScheduledStartDate?.Date == date.Date || 
                o.ScheduledEndDate?.Date == date.Date);
        }

        public async Task<IEnumerable<ProductionOrder>> GetOverdueOrdersAsync()
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders.Where(o => 
                o.ScheduledEndDate.HasValue && 
                o.ScheduledEndDate.Value < DateTime.UtcNow &&
                o.Status != ProductionOrderStatus.Completed &&
                o.Status != ProductionOrderStatus.Cancelled);
        }

        // Atribuição e prioridade
        public async Task<bool> AssignToUserAsync(string productionOrderId, string userId)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            order.AssignedTo = userId;
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<bool> UpdatePriorityAsync(string productionOrderId, ProductionOrderPriority priority)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            order.Priority = priority;
            order.LastModifiedAt = DateTime.UtcNow;

            return await _productionOrderRepository.UpdateAsync(order);
        }

        public async Task<IEnumerable<ProductionOrder>> GetByPriorityAsync(ProductionOrderPriority priority)
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders.Where(o => o.Priority == priority);
        }

        // Relatórios e análises
        public async Task<decimal> GetAverageCompletionTimeAsync(string? productId = null)
        {
            var completedOrders = await _productionOrderRepository.GetByStatusAsync(ProductionOrderStatus.Completed);
            
            if (!string.IsNullOrEmpty(productId))
            {
                completedOrders = completedOrders.Where(o => o.ProductId == productId);
            }

            var ordersWithTime = completedOrders.Where(o => o.ActualTime.HasValue);
            
            return ordersWithTime.Any() ? (decimal)ordersWithTime.Average(o => o.ActualTime!.Value) : 0;
        }

        public async Task<int> GetCompletedOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var completedOrders = await _productionOrderRepository.GetByStatusAsync(ProductionOrderStatus.Completed);
            
            if (startDate.HasValue)
            {
                completedOrders = completedOrders.Where(o => o.ActualEndDate >= startDate);
            }
            
            if (endDate.HasValue)
            {
                completedOrders = completedOrders.Where(o => o.ActualEndDate <= endDate);
            }

            return completedOrders.Count();
        }

        public async Task<int> GetPendingOrdersCountAsync()
        {
            var pendingOrders = await _productionOrderRepository.GetByStatusAsync(ProductionOrderStatus.Pending);
            return pendingOrders.Count();
        }

        public async Task<decimal> GetProductionEfficiencyAsync(string? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var completedOrders = await _productionOrderRepository.GetByStatusAsync(ProductionOrderStatus.Completed);
            
            if (!string.IsNullOrEmpty(userId))
            {
                completedOrders = completedOrders.Where(o => o.AssignedTo == userId);
            }
            
            if (startDate.HasValue)
            {
                completedOrders = completedOrders.Where(o => o.ActualEndDate >= startDate);
            }
            
            if (endDate.HasValue)
            {
                completedOrders = completedOrders.Where(o => o.ActualEndDate <= endDate);
            }

            var ordersWithTimes = completedOrders.Where(o => 
                o.EstimatedTime.HasValue && o.ActualTime.HasValue);

            if (!ordersWithTimes.Any())
                return 0;

            var totalEstimated = ordersWithTimes.Sum(o => o.EstimatedTime!.Value);
            var totalActual = ordersWithTimes.Sum(o => o.ActualTime!.Value);

            return totalActual > 0 ? (decimal)((totalEstimated / totalActual) * 100) : 0;
        }

        // Validações
        public async Task<bool> ValidateProductionOrderAsync(ProductionOrder order)
        {
            // Verificar se o produto existe
            var product = await _productRepository.GetByIdAsync(order.ProductId);
            if (product == null)
                return false;

            // Verificar quantidade
            if (order.Quantity <= 0)
                return false;

            // Verificar se o pedido existe (se fornecido)
            if (!string.IsNullOrEmpty(order.OrderId))
            {
                var orderEntry = await _orderRepository.GetByIdAsync(order.OrderId);
                if (orderEntry == null)
                    return false;
            }

            return true;
        }

        public async Task<bool> CanStartProductionAsync(string productionOrderId)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            return order.Status == ProductionOrderStatus.Pending || 
                   order.Status == ProductionOrderStatus.Scheduled;
        }

        public async Task<bool> CanCompleteProductionAsync(string productionOrderId)
        {
            var order = await _productionOrderRepository.GetByIdAsync(productionOrderId);
            if (order == null)
                return false;

            return order.Status == ProductionOrderStatus.InProgress;
        }

        // Pesquisa e paginação
        public async Task<IEnumerable<ProductionOrder>> SearchAsync(string searchTerm)
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders.Where(o => 
                (!string.IsNullOrEmpty(o.AssignedTo) && o.AssignedTo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(o.Notes) && o.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<ProductionOrder>> GetPagedAsync(int page, int pageSize)
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public async Task<IEnumerable<ProductionOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var allOrders = await _productionOrderRepository.GetAllAsync();
            return allOrders.Where(o => 
                o.CreatedAt >= startDate && o.CreatedAt <= endDate);
        }

        private async Task<decimal> EstimateProductionTimeAsync(string productId, decimal quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product is CompositeProduct composite && composite.AssemblyTime.HasValue)
            {
                return composite.AssemblyTime.Value * quantity;
            }

            return 1; // Tempo padrão de 1 hora
        }
    }
} 