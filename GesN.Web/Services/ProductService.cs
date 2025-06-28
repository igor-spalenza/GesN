using GesN.Web.Interfaces.Services;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductComponentRepository _productComponentRepository;
        private readonly IProductIngredientRepository _productIngredientRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductComponentRepository productComponentRepository,
            IProductIngredientRepository productIngredientRepository,
            IOrderItemRepository orderItemRepository)
        {
            _productRepository = productRepository;
            _productComponentRepository = productComponentRepository;
            _productIngredientRepository = productIngredientRepository;
            _orderItemRepository = orderItemRepository;
        }

        // CRUD Operations
        public async Task<Product?> GetByIdAsync(string id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveAsync()
        {
            return await _productRepository.GetActiveProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType)
        {
            return await _productRepository.GetByTypeAsync(productType);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId)
        {
            return await _productRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            // Validações de negócio
            var validationErrors = await ValidateProductAsync(product);
            if (validationErrors.Any())
            {
                throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
            }

            // Verificar se o SKU é único
            if (!string.IsNullOrEmpty(product.SKU) && !await IsSKUUniqueAsync(product.SKU))
            {
                throw new InvalidOperationException("Já existe um produto com este SKU.");
            }

            // Definir valores padrão
            product.Id = Guid.NewGuid().ToString();
            product.CreatedAt = DateTime.UtcNow;
            product.StateCode = ObjectState.Active;

            var productId = await _productRepository.CreateAsync(product);
            product.Id = productId;
            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            // Validações de negócio
            var validationErrors = await ValidateProductAsync(product);
            if (validationErrors.Any())
            {
                throw new InvalidOperationException($"Erros de validação: {string.Join(", ", validationErrors)}");
            }

            // Verificar se o SKU é único (excluindo o próprio produto)
            if (!string.IsNullOrEmpty(product.SKU) && !await IsSKUUniqueAsync(product.SKU, product.Id))
            {
                throw new InvalidOperationException("Já existe um produto com este SKU.");
            }

            product.LastModifiedAt = DateTime.UtcNow;
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!await CanDeleteAsync(id))
            {
                throw new InvalidOperationException("Este produto não pode ser excluído pois está sendo usado em outros registros.");
            }

            return await _productRepository.DeleteAsync(id);
        }

        public async Task<bool> ActivateAsync(string id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;
            
            product.StateCode = ObjectState.Active;
            product.LastModifiedAt = DateTime.UtcNow;
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeactivateAsync(string id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        // Search and Filter
        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            return await _productRepository.SearchAsync(searchTerm);
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, ProductType? productType = null)
        {
            IEnumerable<Product> products;
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = await _productRepository.SearchAsync(searchTerm);
            }
            else if (productType.HasValue)
            {
                products = await _productRepository.GetByTypeAsync(productType.Value);
            }
            else
            {
                products = await _productRepository.GetPagedAsync(page, pageSize);
            }
            
            return (products, products.Count());
        }

        // Business Logic
        public async Task<bool> ExistsAsync(string id)
        {
            return await _productRepository.ExistsAsync(id);
        }

        public async Task<bool> IsSKUUniqueAsync(string sku, string? excludeId = null)
        {
            var products = await _productRepository.SearchAsync(sku);
            var existingProduct = products.FirstOrDefault(p => p.SKU == sku);
            
            if (existingProduct == null) return true;
            if (!string.IsNullOrEmpty(excludeId) && existingProduct.Id == excludeId) return true;
            
            return false;
        }

        public async Task<decimal> CalculateProductCostAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return 0;

            decimal totalCost = product.Cost;

            // Para produtos compostos, somar o custo dos componentes
            if (product.ProductType == ProductType.Composite)
            {
                var components = await _productComponentRepository.GetByCompositeProductIdAsync(productId);
                foreach (var component in components)
                {
                    var componentProduct = await _productRepository.GetByIdAsync(component.ComponentProductId);
                    if (componentProduct != null)
                    {
                        totalCost += componentProduct.Cost * component.Quantity;
                    }
                }
            }

            // Para produtos com ingredientes, somar o custo dos ingredientes
            var ingredients = await _productIngredientRepository.GetByProductIdAsync(productId);
            foreach (var ingredient in ingredients)
            {
                // Assumindo que há um custo por unidade do ingrediente
                // Você pode ajustar esta lógica conforme sua necessidade
                totalCost += ingredient.Quantity * 0.1m; // Valor exemplo
            }

            return totalCost;
        }

        public async Task<int> GetEstimatedAssemblyTimeAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return 0;

            int totalTime = product.AssemblyTime;

            // Para produtos compostos, somar o tempo de montagem dos componentes
            if (product.ProductType == ProductType.Composite)
            {
                var components = await _productComponentRepository.GetByCompositeProductIdAsync(productId);
                foreach (var component in components)
                {
                    var componentProduct = await _productRepository.GetByIdAsync(component.ComponentProductId);
                    if (componentProduct != null)
                    {
                        totalTime += componentProduct.AssemblyTime * (int)component.Quantity;
                    }
                }
            }

            return totalTime;
        }

        // Stock Management - Removido pois não há mais controle de estoque direto

        // Product Relationships
        public async Task<IEnumerable<Product>> GetAvailableComponentsAsync(string? excludeProductId = null)
        {
            var products = await _productRepository.GetActiveProductsAsync();
            if (!string.IsNullOrEmpty(excludeProductId))
            {
                products = products.Where(p => p.Id != excludeProductId);
            }
            return products;
        }

        public async Task<IEnumerable<Product>> GetSimpleProductsAsync()
        {
            return await _productRepository.GetByTypeAsync(ProductType.Simple);
        }

        public async Task<IEnumerable<Product>> GetCompositeProductsAsync()
        {
            return await _productRepository.GetByTypeAsync(ProductType.Composite);
        }

        public async Task<IEnumerable<Product>> GetProductGroupsAsync()
        {
            return await _productRepository.GetByTypeAsync(ProductType.Group);
        }

        // Validation
        public async Task<bool> CanDeleteAsync(string id)
        {
            // Verificar se o produto está sendo usado como componente
            var usedAsComponent = await _productComponentRepository.GetByComponentProductIdAsync(id);
            if (usedAsComponent.Any())
                return false;

            // Verificar se o produto está em pedidos
            var usedInOrders = await _orderItemRepository.GetByProductIdAsync(id);
            if (usedInOrders.Any())
                return false;

            return true;
        }

        public async Task<IEnumerable<string>> ValidateProductAsync(Product product)
        {
            var errors = new List<string>();

            // Validações básicas
            if (string.IsNullOrWhiteSpace(product.Name))
                errors.Add("Nome é obrigatório.");

            // SKU é opcional na nova estrutura

            if (product.UnitPrice < 0)
                errors.Add("Preço não pode ser negativo.");

            if (product.Cost < 0)
                errors.Add("Custo não pode ser negativo.");

            // Validações específicas por tipo
            switch (product.ProductType)
            {
                case ProductType.Group:
                    // Validações básicas de grupo - campos específicos foram removidos
                    break;

                case ProductType.Composite:
                    if (product.AssemblyTime < 0)
                        errors.Add("Tempo de montagem não pode ser negativo.");
                    break;
            }

            await Task.CompletedTask;
            return errors;
        }

        // Métodos adicionais necessários
        public async Task<IEnumerable<Product>> GetCategoriesAsync()
        {
            // TODO: Implementar quando necessário - retorna lista vazia por enquanto
            await Task.CompletedTask;
            return Enumerable.Empty<Product>();
        }
    }
} 