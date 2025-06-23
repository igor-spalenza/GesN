using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Services
{
    public class ProductComponentService : IProductComponentService
    {
        private readonly IProductComponentRepository _componentRepository;
        private readonly IProductRepository _productRepository;

        public ProductComponentService(
            IProductComponentRepository componentRepository,
            IProductRepository productRepository)
        {
            _componentRepository = componentRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductComponent>> GetAllAsync()
        {
            return await _componentRepository.GetAllAsync();
        }

        public async Task<ProductComponent?> GetByIdAsync(string id)
        {
            return await _componentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ProductComponent>> GetByCompositeProductIdAsync(string compositeProductId)
        {
            return await _componentRepository.GetByCompositeProductIdAsync(compositeProductId);
        }

        public async Task<IEnumerable<ProductComponent>> GetByComponentProductIdAsync(string componentProductId)
        {
            return await _componentRepository.GetByComponentProductIdAsync(componentProductId);
        }

        public async Task<IEnumerable<ProductComponent>> GetOptionalComponentsAsync(string compositeProductId)
        {
            return await _componentRepository.GetOptionalComponentsAsync(compositeProductId);
        }

        public async Task<IEnumerable<ProductComponent>> GetRequiredComponentsAsync(string compositeProductId)
        {
            return await _componentRepository.GetRequiredComponentsAsync(compositeProductId);
        }

        public async Task<ProductComponent?> GetByCompositeAndComponentProductAsync(string compositeProductId, string componentProductId)
        {
            return await _componentRepository.GetByCompositeAndComponentProductAsync(compositeProductId, componentProductId);
        }

        public async Task<string> CreateAsync(ProductComponent component)
        {
            // Validações de negócio
            if (!await ValidateComponentAsync(component))
                throw new InvalidOperationException("Componente inválido");

            if (!await CanCreateComponentAsync(component.CompositeProductId, component.ComponentProductId))
                throw new InvalidOperationException("Não é possível criar este componente");

            // Configurar dados de auditoria
            component.Id = Guid.NewGuid().ToString();
            component.StateCode = ObjectState.Active;
            component.CreatedAt = DateTime.UtcNow;
            component.LastModifiedAt = DateTime.UtcNow;

            return await _componentRepository.CreateAsync(component);
        }

        public async Task<bool> UpdateAsync(ProductComponent component)
        {
            var existingComponent = await _componentRepository.GetByIdAsync(component.Id);
            if (existingComponent == null)
                return false;

            // Validações de negócio
            if (!await ValidateComponentAsync(component))
                throw new InvalidOperationException("Componente inválido");

            // Atualizar dados de auditoria
            component.LastModifiedAt = DateTime.UtcNow;

            return await _componentRepository.UpdateAsync(component);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var component = await _componentRepository.GetByIdAsync(id);
            if (component == null)
                return false;

            return await _componentRepository.DeleteAsync(id);
        }

        public async Task<bool> ValidateComponentAsync(ProductComponent component)
        {
            // Validar se os produtos existem
            var compositeProduct = await _productRepository.GetByIdAsync(component.CompositeProductId);
            var componentProduct = await _productRepository.GetByIdAsync(component.ComponentProductId);

            if (compositeProduct == null || componentProduct == null)
                return false;

            // Validar se o produto composto é do tipo Composite
            if (compositeProduct.ProductType != ProductType.Composite)
                return false;

            // Validar se não é um componente circular (produto não pode ser componente de si mesmo)
            if (component.CompositeProductId == component.ComponentProductId)
                return false;

            // Validar quantidade
            if (component.Quantity <= 0)
                return false;

            // Validar ordem de montagem
            if (component.AssemblyOrder < 0)
                return false;

            return true;
        }

        public async Task<bool> CanCreateComponentAsync(string compositeProductId, string componentProductId)
        {
            // Verificar se já existe este componente
            var existingComponent = await _componentRepository.GetByCompositeAndComponentProductAsync(compositeProductId, componentProductId);
            if (existingComponent != null)
                return false;

            // Verificar dependências circulares
            if (await HasCircularDependencyAsync(compositeProductId, componentProductId))
                return false;

            return true;
        }

        public async Task<decimal> CalculateComponentCostAsync(string compositeProductId)
        {
            var components = await _componentRepository.GetByCompositeProductIdAsync(compositeProductId);
            decimal totalCost = 0;

            foreach (var component in components)
            {
                var componentProduct = await _productRepository.GetByIdAsync(component.ComponentProductId);
                if (componentProduct != null)
                {
                    totalCost += componentProduct.UnitPrice * component.Quantity;
                }
            }

            return totalCost;
        }

        public async Task<decimal> CalculateAssemblyTimeAsync(string compositeProductId)
        {
            var components = await _componentRepository.GetByCompositeProductIdAsync(compositeProductId);
            decimal totalTime = 0;

            foreach (var component in components)
            {
                var componentProduct = await _productRepository.GetByIdAsync(component.ComponentProductId);
                if (componentProduct != null && componentProduct is CompositeProduct composite)
                {
                    // Tempo de montagem baseado no tempo de produção do componente
                    totalTime += (composite.AssemblyTime ?? 0) * component.Quantity;
                }
            }

            return totalTime;
        }

        public async Task<IEnumerable<ProductComponent>> SearchAsync(string searchTerm)
        {
            return await _componentRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<ProductComponent>> GetPagedAsync(int page, int pageSize)
        {
            return await _componentRepository.GetPagedAsync(page, pageSize);
        }

        private async Task<bool> HasCircularDependencyAsync(string compositeProductId, string componentProductId)
        {
            // Verificar se o componentProduct tem o compositeProduct como seu componente
            var componentComponents = await _componentRepository.GetByCompositeProductIdAsync(componentProductId);
            
            foreach (var comp in componentComponents)
            {
                if (comp.ComponentProductId == compositeProductId)
                    return true;

                // Verificação recursiva para dependências mais profundas
                if (await HasCircularDependencyAsync(compositeProductId, comp.ComponentProductId))
                    return true;
            }

            return false;
        }
    }
} 