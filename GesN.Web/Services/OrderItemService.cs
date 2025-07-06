using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using Microsoft.Extensions.Logging;

namespace GesN.Web.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderItemService> _logger;

        public OrderItemService(
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            ILogger<OrderItemService> logger)
        {
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        // CRUD Operations
        public async Task<OrderItem?> GetByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("ID do item do pedido não fornecido");
                    return null;
                }

                return await _orderItemRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar item do pedido por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            try
            {
                // Como não existe GetAllAsync no repositório, vamos implementar uma busca básica
                // Você pode ajustar conforme necessário
                _logger.LogWarning("GetAllAsync não implementado no repositório. Retornando lista vazia.");
                return Enumerable.Empty<OrderItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os itens de pedidos");
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    _logger.LogWarning("ID do pedido não fornecido");
                    return Enumerable.Empty<OrderItem>();
                }

                return await _orderItemRepository.GetByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    _logger.LogWarning("ID do produto não fornecido");
                    return Enumerable.Empty<OrderItem>();
                }

                return await _orderItemRepository.GetByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens do produto: {ProductId}", productId);
                throw;
            }
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateOrderItemAsync(orderItem);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                // Definir valores padrão
                orderItem.Id = Guid.NewGuid().ToString();
                orderItem.CreatedAt = DateTime.UtcNow;
                orderItem.StateCode = ObjectState.Active;

                var itemId = await _orderItemRepository.CreateAsync(orderItem);
                orderItem.Id = itemId;
                
                _logger.LogInformation("Item do pedido criado com sucesso: {Id}", itemId);
                return orderItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar item do pedido");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OrderItem orderItem)
        {
            try
            {
                // Validações de negócio
                var validationErrors = await ValidateOrderItemAsync(orderItem);
                if (validationErrors.Any())
                {
                    throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
                }

                orderItem.LastModifiedAt = DateTime.UtcNow;
                var result = await _orderItemRepository.UpdateAsync(orderItem);
                
                if (result)
                {
                    _logger.LogInformation("Item do pedido atualizado com sucesso: {Id}", orderItem.Id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar item do pedido: {Id}", orderItem.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                if (!await CanDeleteAsync(id))
                {
                    throw new InvalidOperationException("Este item do pedido não pode ser excluído.");
                }

                var result = await _orderItemRepository.DeleteAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Item do pedido excluído com sucesso: {Id}", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir item do pedido: {Id}", id);
                throw;
            }
        }

        // Business Operations
        public async Task<bool> ExistsAsync(string id)
        {
            try
            {
                return await _orderItemRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do item do pedido: {Id}", id);
                throw;
            }
        }

        public async Task<decimal> CalculateItemTotalAsync(string id)
        {
            try
            {
                var item = await _orderItemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return 0;
                }

                return item.TotalPrice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular total do item: {Id}", id);
                throw;
            }
        }

        public async Task<decimal> CalculateOrderTotalAsync(string orderId)
        {
            try
            {
                var items = await _orderItemRepository.GetByOrderIdAsync(orderId);
                return items.Sum(item => item.TotalPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular total do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<int> CountByOrderAsync(string orderId)
        {
            try
            {
                return await _orderItemRepository.CountByOrderIdAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> CanDeleteAsync(string id)
        {
            try
            {
                var item = await _orderItemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    return false;
                }

                // Verificar se o item pode ser excluído (regras de negócio)
                // Por exemplo, não permitir exclusão se o pedido já foi faturado
                // TODO: Implementar lógica específica conforme necessário
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se item pode ser excluído: {Id}", id);
                throw;
            }
        }

        // Validation
        public async Task<IEnumerable<string>> ValidateOrderItemAsync(OrderItem orderItem)
        {
            var errors = new List<string>();

            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(orderItem.OrderId))
                {
                    errors.Add("ID do pedido é obrigatório");
                }

                if (string.IsNullOrEmpty(orderItem.ProductId))
                {
                    errors.Add("ID do produto é obrigatório");
                }

                if (orderItem.Quantity <= 0)
                {
                    errors.Add("Quantidade deve ser maior que zero");
                }

                if (orderItem.UnitPrice < 0)
                {
                    errors.Add("Preço unitário não pode ser negativo");
                }

                if (orderItem.DiscountAmount < 0)
                {
                    errors.Add("Desconto não pode ser negativo");
                }

                if (orderItem.TaxAmount < 0)
                {
                    errors.Add("Impostos não podem ser negativos");
                }

                if (orderItem.DiscountAmount > orderItem.Subtotal)
                {
                    errors.Add("Desconto não pode ser maior que o subtotal");
                }

                // Validar se o produto existe
                if (!string.IsNullOrEmpty(orderItem.ProductId))
                {
                    var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                    if (product == null)
                    {
                        errors.Add("Produto não encontrado");
                    }
                    else if (product.StateCode != ObjectState.Active)
                    {
                        errors.Add("Produto não está ativo");
                    }
                }

                // Validar quantidade disponível
                if (!string.IsNullOrEmpty(orderItem.ProductId) && orderItem.Quantity > 0)
                {
                    var isValidQuantity = await ValidateQuantityAsync(orderItem.ProductId, orderItem.Quantity);
                    if (!isValidQuantity)
                    {
                        errors.Add("Quantidade solicitada não disponível");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar item do pedido");
                errors.Add("Erro interno na validação");
            }

            return errors;
        }

        public async Task<bool> ValidateQuantityAsync(string productId, int quantity)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return false;
                }

                // TODO: Implementar lógica de validação de estoque se necessário
                // Por enquanto, sempre retorna true
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar quantidade do produto: {ProductId}", productId);
                throw;
            }
        }

        // Bulk Operations
        public async Task<bool> DeleteByOrderIdAsync(string orderId)
        {
            try
            {
                var items = await _orderItemRepository.GetByOrderIdAsync(orderId);
                foreach (var item in items)
                {
                    await _orderItemRepository.DeleteAsync(item.Id);
                }

                _logger.LogInformation("Itens do pedido excluídos com sucesso: {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir itens do pedido: {OrderId}", orderId);
                throw;
            }
        }

        public async Task<IEnumerable<OrderItem>> CreateBulkAsync(IEnumerable<OrderItem> orderItems)
        {
            var createdItems = new List<OrderItem>();

            try
            {
                foreach (var item in orderItems)
                {
                    var createdItem = await CreateAsync(item);
                    createdItems.Add(createdItem);
                }

                _logger.LogInformation("Criados {Count} itens de pedido em lote", createdItems.Count);
                return createdItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar itens do pedido em lote");
                throw;
            }
        }

        public async Task<bool> UpdateOrderItemsAsync(string orderId, IEnumerable<OrderItem> orderItems)
        {
            try
            {
                // Excluir itens existentes
                await DeleteByOrderIdAsync(orderId);

                // Criar novos itens
                await CreateBulkAsync(orderItems);

                _logger.LogInformation("Itens do pedido atualizados com sucesso: {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar itens do pedido: {OrderId}", orderId);
                throw;
            }
        }
    }
} 