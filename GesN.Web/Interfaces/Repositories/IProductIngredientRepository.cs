using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IProductIngredientRepository
    {
        Task<IEnumerable<ProductIngredient>> GetAllAsync();
        Task<ProductIngredient?> GetByIdAsync(string id);
        Task<IEnumerable<ProductIngredient>> GetByProductIdAsync(string productId);
        Task<IEnumerable<ProductIngredient>> GetByIngredientIdAsync(string ingredientId);
        Task<ProductIngredient?> GetByProductAndIngredientAsync(string productId, string ingredientId);
        Task<string> CreateAsync(ProductIngredient productIngredient);
        Task<bool> UpdateAsync(ProductIngredient productIngredient);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByProductIdAsync(string productId);
        Task<bool> DeleteByIngredientIdAsync(string ingredientId);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<ProductIngredient>> GetPagedAsync(int page, int pageSize);
    }
} 