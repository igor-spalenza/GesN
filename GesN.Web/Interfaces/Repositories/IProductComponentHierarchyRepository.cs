using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    /// <summary>
    /// Interface para o repositório de Hierarquias de Componentes de Produto
    /// Define operações CRUD e consultas específicas para a entidade ProductComponentHierarchy
    /// </summary>
    public interface IProductComponentHierarchyRepository
    {
        // Operações CRUD básicas
        Task<IEnumerable<ProductComponentHierarchy>> GetAllAsync();
        Task<ProductComponentHierarchy?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProductComponentHierarchy hierarchy);
        Task<bool> UpdateAsync(ProductComponentHierarchy hierarchy);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);

        // Consultas específicas
        Task<IEnumerable<ProductComponentHierarchy>> GetByNameAsync(string name);
        Task<IEnumerable<ProductComponentHierarchy>> GetActiveHierarchiesAsync();

        // Consultas por relacionamentos
        Task<IEnumerable<ProductComponentHierarchy>> GetWithComponentsAsync();
        Task<IEnumerable<ProductComponentHierarchy>> GetWithCompositeProductsAsync();
        Task<ProductComponentHierarchy?> GetWithAllRelationshipsAsync(string id);

        // Consultas específicas do domínio
        Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesForProductAsync(string productId);
        Task<IEnumerable<ProductComponentHierarchy>> GetAvailableHierarchiesAsync();
        Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesWithMinimumComponentsAsync(int minComponents);

        // Pesquisa e filtros
        Task<IEnumerable<ProductComponentHierarchy>> SearchAsync(string searchTerm);
        Task<IEnumerable<ProductComponentHierarchy>> SearchByNameOrDescriptionAsync(string searchTerm);

        // Paginação
        Task<IEnumerable<ProductComponentHierarchy>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<ProductComponentHierarchy>> GetPagedByStatusAsync(bool isActive, int page, int pageSize);

        // Contadores e estatísticas
        Task<int> CountAsync();
        Task<int> CountActiveAsync();
        Task<int> CountInactiveAsync();
        Task<int> CountWithComponentsAsync();

        // Validações e verificações
        Task<bool> IsNameUniqueAsync(string name, string? excludeId = null);
        Task<bool> CanDeleteAsync(string id);
        Task<bool> HasActiveComponentsAsync(string hierarchyId);
        Task<bool> IsBeingUsedByProductsAsync(string hierarchyId);

        // Operações específicas do domínio
        Task<bool> ActivateAsync(string id);
        Task<bool> DeactivateAsync(string id);



        // Relatórios e métricas
        Task<IEnumerable<ProductComponentHierarchy>> GetMostUsedHierarchiesAsync(int top = 10);
        Task<IEnumerable<ProductComponentHierarchy>> GetUnusedHierarchiesAsync();
        Task<Dictionary<string, int>> GetUsageStatisticsAsync();

        // Operações em lote
        Task<bool> UpdateStatusBatchAsync(IEnumerable<string> hierarchyIds, bool isActive);
        Task<bool> DeleteBatchAsync(IEnumerable<string> hierarchyIds);

        // Métodos de otimização
        Task<IEnumerable<ProductComponentHierarchy>> GetHierarchiesWithEstimatedCostAsync();
        Task<IEnumerable<ProductComponentHierarchy>> GetCompleteHierarchiesAsync(); // Com componentes obrigatórios
        Task<IEnumerable<ProductComponentHierarchy>> GetIncompleteHierarchiesAsync(); // Sem componentes ou só opcionais
    }
} 