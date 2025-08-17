using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Interfaces.Repositories
{
    /// <summary>
    /// Interface para repositório de CompositeProductXHierarchy
    /// Gerencia as relações entre produtos compostos e hierarquias de componentes
    /// </summary>
    public interface ICompositeProductXHierarchyRepository
    {
        #region CRUD Básico
        
        /// <summary>
        /// Criar nova relação CompositeProductXHierarchy
        /// </summary>
        Task<int> CreateAsync(CompositeProductXHierarchy relation);
        
        /// <summary>
        /// Buscar relação por ID
        /// </summary>
        Task<CompositeProductXHierarchy?> GetByIdAsync(int id);
        
        /// <summary>
        /// Atualizar relação existente
        /// </summary>
        Task<bool> UpdateAsync(CompositeProductXHierarchy relation);
        
        /// <summary>
        /// Excluir relação por ID
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Verificar se relação existe
        /// </summary>
        Task<bool> ExistsAsync(int id);
        
        #endregion

        #region Consultas Específicas
        
        /// <summary>
        /// Buscar todas as relações de um produto composto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetByProductIdAsync(string productId, bool includeInactive = false);
        
        /// <summary>
        /// Buscar todas as relações de uma hierarquia
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetByHierarchyIdAsync(string hierarchyId, bool includeInactive = false);
        
        /// <summary>
        /// Buscar relação específica entre produto e hierarquia
        /// </summary>
        Task<CompositeProductXHierarchy?> GetByProductAndHierarchyAsync(string productId, string hierarchyId);
        
        /// <summary>
        /// Buscar relações ordenadas por AssemblyOrder para um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetOrderedByProductIdAsync(string productId, bool includeInactive = false);
        
        /// <summary>
        /// Buscar relações ativas de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetActiveByProductIdAsync(string productId);
        
        /// <summary>
        /// Buscar relações obrigatórias de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetRequiredByProductIdAsync(string productId);
        
        /// <summary>
        /// Buscar relações opcionais de um produto
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetOptionalByProductIdAsync(string productId);
        
        #endregion

        #region Operações de Validação
        
        /// <summary>
        /// Verificar se já existe relação entre produto e hierarquia
        /// </summary>
        Task<bool> RelationExistsAsync(string productId, string hierarchyId);
        
        /// <summary>
        /// Verificar se ordem de montagem já está sendo usada no produto
        /// </summary>
        Task<bool> AssemblyOrderExistsAsync(string productId, int assemblyOrder, int? excludeRelationId = null);
        
        /// <summary>
        /// Obter próxima ordem de montagem disponível para um produto
        /// </summary>
        Task<int> GetNextAssemblyOrderAsync(string productId);
        
        /// <summary>
        /// Validar se quantidade está dentro dos limites
        /// </summary>
        Task<bool> ValidateQuantityLimitsAsync(int id, int quantity);
        
        #endregion

        #region Operações em Lote
        
        /// <summary>
        /// Criar múltiplas relações
        /// </summary>
        Task<bool> CreateBatchAsync(IEnumerable<CompositeProductXHierarchy> relations);
        
        /// <summary>
        /// Ativar/desativar relações em lote
        /// </summary>
        Task<bool> UpdateStatusBatchAsync(IEnumerable<int> relationIds, bool isActive);
        
        /// <summary>
        /// Excluir múltiplas relações
        /// </summary>
        Task<bool> DeleteBatchAsync(IEnumerable<int> relationIds);
        
        /// <summary>
        /// Reordenar múltiplas relações
        /// </summary>
        Task<bool> ReorderRelationsAsync(string productId, Dictionary<int, int> newOrders);
        
        #endregion

        #region Estatísticas e Relatórios
        
        /// <summary>
        /// Contar relações por produto
        /// </summary>
        Task<int> CountByProductIdAsync(string productId, bool includeInactive = false);
        
        /// <summary>
        /// Contar relações por hierarquia
        /// </summary>
        Task<int> CountByHierarchyIdAsync(string hierarchyId, bool includeInactive = false);
        
        /// <summary>
        /// Obter estatísticas de uso de uma hierarquia
        /// </summary>
        Task<Dictionary<string, int>> GetHierarchyUsageStatsAsync(string hierarchyId);
        
        /// <summary>
        /// Obter produtos que mais usam hierarquias
        /// </summary>
        Task<Dictionary<string, int>> GetTopProductsUsingHierarchiesAsync(int limit = 10);
        
        /// <summary>
        /// Obter hierarquias mais utilizadas
        /// </summary>
        Task<Dictionary<string, int>> GetTopUsedHierarchiesAsync(int limit = 10);
        
        #endregion

        #region Consultas Complexas
        
        /// <summary>
        /// Buscar relações com dados completos (incluindo navegação)
        /// </summary>
        Task<IEnumerable<CompositeProductXHierarchy>> GetWithFullDataAsync(string? productId = null, string? hierarchyId = null);
        
        /// <summary>
        /// Buscar relações por filtros avançados
        /// </summary>
        Task<(IEnumerable<CompositeProductXHierarchy> Relations, int TotalCount)> SearchAsync(
            string? searchTerm = null,
            string? productId = null,
            string? hierarchyId = null,
            bool? isActive = null,
            bool? isOptional = null,
            int skip = 0,
            int take = 25,
            string sortBy = "AssemblyOrder",
            string sortDirection = "asc");
        
        #endregion
    }
} 