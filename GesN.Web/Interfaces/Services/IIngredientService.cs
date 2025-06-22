using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient?> GetIngredientByIdAsync(string id);
        Task<Ingredient?> GetIngredientByNameAsync(string name);
        Task<IEnumerable<Ingredient>> GetActiveIngredientsAsync();
        Task<IEnumerable<Ingredient>> GetIngredientsBySupplierId(string supplierId);
        Task<IEnumerable<Ingredient>> GetLowStockIngredientsAsync();
        Task<IEnumerable<Ingredient>> GetPerishableIngredientsAsync();
        Task<IEnumerable<Ingredient>> SearchIngredientsAsync(string searchTerm);
        Task<IEnumerable<Ingredient>> SearchIngredientsForAutocompleteAsync(string searchTerm);
        Task<string> CreateIngredientAsync(Ingredient ingredient, string createdBy);
        Task<bool> UpdateIngredientAsync(Ingredient ingredient, string modifiedBy);
        Task<bool> UpdateIngredientStockAsync(string id, decimal newStock, string modifiedBy);
        Task<bool> DeleteIngredientAsync(string id);
        Task<bool> IngredientExistsAsync(string id);
        Task<int> GetIngredientCountAsync();
        Task<int> GetActiveIngredientCountAsync();
        Task<IEnumerable<Ingredient>> GetPagedIngredientsAsync(int page, int pageSize);
        Task<bool> ValidateIngredientDataAsync(Ingredient ingredient);
    }
} 