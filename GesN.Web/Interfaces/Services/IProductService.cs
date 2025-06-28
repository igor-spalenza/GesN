using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductService
    {
        // CRUD Operations
        Task<Product?> GetByIdAsync(string id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetActiveAsync();
        Task<IEnumerable<Product>> GetByTypeAsync(ProductType productType);
        Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string id);
        Task<bool> ActivateAsync(string id);
        Task<bool> DeactivateAsync(string id);

        // Search and Filter
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, ProductType? productType = null);
        
        // Business Logic
        Task<bool> ExistsAsync(string id);
        Task<bool> IsSKUUniqueAsync(string sku, string? excludeId = null);
        Task<decimal> CalculateProductCostAsync(string productId);
        Task<int> GetEstimatedAssemblyTimeAsync(string productId);
        
        // Product Relationships
        Task<IEnumerable<Product>> GetAvailableComponentsAsync(string? excludeProductId = null);
        Task<IEnumerable<Product>> GetSimpleProductsAsync();
        Task<IEnumerable<Product>> GetCompositeProductsAsync();
        Task<IEnumerable<Product>> GetProductGroupsAsync();
        
        // Validation
        Task<bool> CanDeleteAsync(string id);
        Task<IEnumerable<string>> ValidateProductAsync(Product product);

        // Métodos adicionais necessários
        Task<IEnumerable<Product>> GetCategoriesAsync();
    }
} 