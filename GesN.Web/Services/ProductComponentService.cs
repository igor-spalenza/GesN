using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Services
{
    public class ProductComponentService : IProductComponentService
    {
        private readonly IProductComponentRepository _componentRepository;
        private readonly IProductComponentHierarchyRepository _hierarchyRepository;

        public ProductComponentService(
            IProductComponentRepository componentRepository,
            IProductComponentHierarchyRepository hierarchyRepository)
        {
            _componentRepository = componentRepository;
            _hierarchyRepository = hierarchyRepository;
        }

        public async Task<IEnumerable<ProductComponent>> GetAllAsync()
        {
            return await _componentRepository.GetAllAsync();
        }

        public async Task<ProductComponent?> GetByIdAsync(string id)
        {
            return await _componentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ProductComponent>> GetByHierarchyIdAsync(string hierarchyId)
        {
            return await _componentRepository.GetByHierarchyIdAsync(hierarchyId);
        }

        public async Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm)
        {
            return await _componentRepository.SearchAsync(searchTerm);
        }

        public async Task<bool> CreateAsync(ProductComponent component)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(component.Name))
            {
                throw new InvalidOperationException("O nome do componente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(component.ProductComponentHierarchyId))
            {
                throw new InvalidOperationException("A hierarquia de componentes é obrigatória.");
            }

            if (component.AdditionalCost < 0)
            {
                throw new InvalidOperationException("O custo adicional não pode ser negativo.");
            }

            // Verificar se a hierarquia existe
            var hierarchy = await _hierarchyRepository.GetByIdAsync(component.ProductComponentHierarchyId);
            if (hierarchy == null)
            {
                throw new InvalidOperationException("Hierarquia de componentes não encontrada.");
            }

            // Definir dados de auditoria
            component.Id = Guid.NewGuid().ToString();
            component.CreatedAt = DateTime.UtcNow;
            component.CreatedBy = "system"; // TODO: Obter usuário atual
            component.LastModifiedAt = DateTime.UtcNow;
            component.LastModifiedBy = "system";
            component.StateCode = ObjectState.Active;

            return await _componentRepository.CreateAsync(component);
        }

        public async Task<bool> UpdateAsync(ProductComponent component)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(component.Name))
            {
                throw new InvalidOperationException("O nome do componente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(component.ProductComponentHierarchyId))
            {
                throw new InvalidOperationException("A hierarquia de componentes é obrigatória.");
            }

            if (component.AdditionalCost < 0)
            {
                throw new InvalidOperationException("O custo adicional não pode ser negativo.");
            }

            // Verificar se o componente existe
            var existingComponent = await _componentRepository.GetByIdAsync(component.Id);
            if (existingComponent == null)
            {
                return false;
            }

            // Verificar se a hierarquia existe
            var hierarchy = await _hierarchyRepository.GetByIdAsync(component.ProductComponentHierarchyId);
            if (hierarchy == null)
            {
                throw new InvalidOperationException("Hierarquia de componentes não encontrada.");
            }

            // Atualizar dados de auditoria
            component.LastModifiedAt = DateTime.UtcNow;
            component.LastModifiedBy = "system"; // TODO: Obter usuário atual

            return await _componentRepository.UpdateAsync(component);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _componentRepository.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _componentRepository.ExistsAsync(id);
        }

        public async Task<int> CountAsync()
        {
            return await _componentRepository.CountAsync();
        }

        // Métodos obsoletos mantidos para compatibilidade temporária (retornam valores padrão ou listas vazias)
        [Obsolete("ProductComponent não possui mais CompositeProductId. Use GetByHierarchyIdAsync()")]
        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais ComponentProductId")]
        public async Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais IsOptional")]
        public async Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais IsOptional")]
        public async Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId)
        {
            return new List<ProductComponent>();
        }

        [Obsolete("ProductComponent não possui mais relacionamento direto com produtos")]
        public async Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId)
        {
            return null;
        }

        [Obsolete("ProductComponent não possui mais relacionamentos com produtos compostos")]
        public async Task<bool> CanCreateComponentAsync(string compositeProductId, string componentProductId)
        {
            return false;
        }

        [Obsolete("Cálculo de custo agora é baseado no AdditionalCost dos componentes")]
        public async Task<decimal> CalculateComponentCostAsync(string compositeProductId)
        {
            return 0;
        }

        [Obsolete("Cálculo de tempo de montagem não é mais suportado")]
        public async Task<decimal> CalculateAssemblyTimeAsync(string compositeProductId)
        {
            return 0;
        }
    }
} 