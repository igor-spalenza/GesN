using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IIngredientRepository
    {
        Task<IEnumerable<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(string id);
        Task<Ingredient?> GetByNameAsync(string name);
        Task<IEnumerable<Ingredient>> GetActiveAsync();
        Task<IEnumerable<Ingredient>> GetBySupplierId(string supplierId);
        Task<IEnumerable<Ingredient>> GetLowStockAsync();
        Task<IEnumerable<Ingredient>> GetPerishableAsync();
        Task<IEnumerable<Ingredient>> SearchAsync(string searchTerm);
        Task<IEnumerable<Ingredient>> SearchForAutocompleteAsync(string searchTerm);
        Task<string> CreateAsync(Ingredient ingredient);
        Task<bool> UpdateAsync(Ingredient ingredient);
        Task<bool> UpdateStockAsync(string id, decimal newStock);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();
        Task<IEnumerable<Ingredient>> GetPagedAsync(int page, int pageSize);
    }
} 