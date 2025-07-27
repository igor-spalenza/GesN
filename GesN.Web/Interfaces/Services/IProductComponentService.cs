using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Services
{
    public interface IProductComponentService
    {
        // Métodos principais com nova estrutura
        Task<IEnumerable<ProductComponent>> GetAllAsync();
        Task<ProductComponent?> GetByIdAsync(string id);
        Task<IEnumerable<ProductComponent>> GetByHierarchyIdAsync(string hierarchyId);
        Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm);
        Task<bool> CreateAsync(ProductComponent component);
        Task<bool> UpdateAsync(ProductComponent component);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<int> CountAsync();

        // Métodos obsoletos mantidos para compatibilidade (temporariamente)
        [Obsolete("ProductComponent não possui mais CompositeProductId. Use GetByHierarchyIdAsync()")]
        Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais ComponentProductId")]
        Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId);
        
        [Obsolete("ProductComponent não possui mais IsOptional")]
        Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais IsOptional")]
        Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId);
        
        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId);
        
        [Obsolete("ProductComponent não possui mais relacionamentos com produtos compostos")]
        Task<bool> CanCreateComponentAsync(string compositeProductId, string componentProductId);
        
        [Obsolete("Cálculo de custo agora é baseado no AdditionalCost dos componentes")]
        Task<decimal> CalculateComponentCostAsync(string compositeProductId);
        
        [Obsolete("Cálculo de tempo de montagem não é mais suportado")]
        Task<decimal> CalculateAssemblyTimeAsync(string compositeProductId);
    }
} 