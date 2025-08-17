using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductIngredientService
    {
        Task<IEnumerable<ProductIngredient>> GetAllProductIngredientsAsync();
        Task<ProductIngredient?> GetProductIngredientByIdAsync(string id);
        Task<IEnumerable<ProductIngredient>> GetProductIngredientsByProductIdAsync(string productId);
        Task<IEnumerable<ProductIngredient>> GetProductIngredientsByIngredientIdAsync(string ingredientId);
        Task<ProductIngredient?> GetProductIngredientByProductAndIngredientAsync(string productId, string ingredientId);
        Task<string> CreateProductIngredientAsync(ProductIngredient productIngredient, string createdBy);
        Task<bool> UpdateProductIngredientAsync(ProductIngredient productIngredient, string modifiedBy);
        Task<bool> DeleteProductIngredientAsync(string id);
        Task<bool> DeleteProductIngredientsByProductIdAsync(string productId);
        Task<bool> DeleteProductIngredientsByIngredientIdAsync(string ingredientId);
        Task<bool> ProductIngredientExistsAsync(string id);
        Task<int> GetProductIngredientCountAsync();
        Task<IEnumerable<ProductIngredient>> GetPagedProductIngredientsAsync(int page, int pageSize);
        Task<bool> ValidateProductIngredientDataAsync(ProductIngredient productIngredient);
        Task<decimal> CalculateProductIngredientCostAsync(string productId);
        Task<bool> HasSufficientStockForProductAsync(string productId, int quantity = 1);
    }
} 