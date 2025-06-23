using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductComponentService
    {
        Task<IEnumerable<ProductComponent>> GetAllAsync();
        Task<ProductComponent?> GetByIdAsync(string id);
        Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId);
        Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId);
        Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId);
        Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId);
        
        Task<string> CreateAsync(ProductComponent component);
        Task<bool> UpdateAsync(ProductComponent component);
        Task<bool> DeleteAsync(string id);
        
        Task<bool> ValidateComponentAsync(ProductComponent component);
        Task<bool> CanCreateComponentAsync(string compositeProductId, string componentProductId);
        Task<decimal> CalculateComponentCostAsync(string compositeProductId);
        Task<decimal> CalculateAssemblyTimeAsync(string compositeProductId);
        
        Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductComponent>> GetPagedAsync(int page, int pageSize);
    }
} 