using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;

namespace GesN.Web.Data.Repositories
{
    /// <summary>
    /// Repositório para gerenciar as relações CompositeProductXHierarchy
    /// </summary>
    public class CompositeProductXHierarchyRepository : ICompositeProductXHierarchyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<CompositeProductXHierarchyRepository> _logger;

        public CompositeProductXHierarchyRepository(
            IDbConnectionFactory connectionFactory,
            ILogger<CompositeProductXHierarchyRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        #region CRUD Básico

        public async Task<int> CreateAsync(CompositeProductXHierarchy relation)
        {
            try
            {
                const string sql = @"
                    INSERT INTO CompositeProductXHierarchy (
                        ProductComponentHierarchyId, ProductId, MinQuantity, MaxQuantity,
                        IsOptional, AssemblyOrder, Notes
                    ) VALUES (
                        @ProductComponentHierarchyId, @ProductId, @MinQuantity, @MaxQuantity,
                        @IsOptional, @AssemblyOrder, @Notes
                    ) RETURNING Id;";

                using var connection = _connectionFactory.CreateConnection();
                
                var parameters = new
                {
                    relation.ProductComponentHierarchyId,
                    relation.ProductId,
                    relation.MinQuantity,
                    relation.MaxQuantity,
                    IsOptional = relation.IsOptional ? 1 : 0,
                    relation.AssemblyOrder,
                    relation.Notes
                };

                var relationId = await connection.QuerySingleAsync<int>(sql, parameters);
                return relationId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao criar relação CompositeProductXHierarchy: {ex.Message}", ex);
            }
        }

        public async Task<CompositeProductXHierarchy?> GetByIdAsync(int id)
        {
            try
            {
                const string sql = @"
                    SELECT cph.Id, cph.ProductComponentHierarchyId, cph.ProductId,
                           cph.MinQuantity, cph.MaxQuantity, cph.IsOptional,
                           cph.AssemblyOrder, cph.Notes,
                           h.Id as HierarchyId, h.Name as HierarchyName,
                           p.Id as ProductId, p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON h.Id = cph.ProductComponentHierarchyId
                    LEFT JOIN Product p ON p.Id = cph.ProductId
                    WHERE cph.Id = @Id;";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync<dynamic>(sql, new { Id = id });

                var row = result.FirstOrDefault();
                return row != null ? MapToEntity(row) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar relação por ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(CompositeProductXHierarchy relation)
        {
            try
            {
                const string sql = @"
                    UPDATE CompositeProductXHierarchy SET
                        ProductComponentHierarchyId = @ProductComponentHierarchyId,
                        ProductId = @ProductId,
                        MinQuantity = @MinQuantity,
                        MaxQuantity = @MaxQuantity,
                        IsOptional = @IsOptional,
                        AssemblyOrder = @AssemblyOrder,
                        Notes = @Notes
                    WHERE Id = @Id";

                using var connection = _connectionFactory.CreateConnection();

                var parameters = new
                {
                    relation.Id,
                    relation.ProductComponentHierarchyId,
                    relation.ProductId,
                    relation.MinQuantity,
                    relation.MaxQuantity,
                    IsOptional = relation.IsOptional ? 1 : 0,
                    relation.AssemblyOrder,
                    relation.Notes
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar relação ID {relation.Id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                const string sql = "DELETE FROM CompositeProductXHierarchy WHERE Id = @Id";

                using var connection = _connectionFactory.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir relação ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                const string sql = "SELECT COUNT(1) FROM CompositeProductXHierarchy WHERE Id = @Id";

                using var connection = _connectionFactory.CreateConnection();
                var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
                
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência da relação ID {id}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Consultas Específicas

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetByProductIdAsync(string productId, bool includeInactive = false)
        {
            try
            {
                // Note: CompositeProductXHierarchy não possui mais IsActive por não herdar de Entity
                // então includeInactive não tem efeito real, mas mantemos para compatibilidade
                const string sql = @"
                    SELECT cph.Id, cph.ProductComponentHierarchyId, cph.ProductId,
                           cph.MinQuantity, cph.MaxQuantity, cph.IsOptional,
                           cph.AssemblyOrder, cph.Notes,
                           h.Id as HierarchyId, h.Name as HierarchyName,
                           p.Id as ProductId, p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON h.Id = cph.ProductComponentHierarchyId
                    LEFT JOIN Product p ON p.Id = cph.ProductId
                    WHERE cph.ProductId = @ProductId
                    ORDER BY cph.AssemblyOrder, h.Name;";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { ProductId = productId });
                
                return results.Select(MapToEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar relações por produto {productId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetByHierarchyIdAsync(string hierarchyId, bool includeInactive = false)
        {
            try
            {
                // Note: CompositeProductXHierarchy não possui mais IsActive por não herdar de Entity
                // então includeInactive não tem efeito real, mas mantemos para compatibilidade
                const string sql = @"
                    SELECT cph.Id, cph.ProductComponentHierarchyId, cph.ProductId,
                           cph.MinQuantity, cph.MaxQuantity, cph.IsOptional,
                           cph.AssemblyOrder, cph.Notes,
                           h.Id as HierarchyId, h.Name as HierarchyName,
                           p.Id as ProductId, p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON h.Id = cph.ProductComponentHierarchyId
                    LEFT JOIN Product p ON p.Id = cph.ProductId
                    WHERE cph.ProductComponentHierarchyId = @HierarchyId
                    ORDER BY p.Name, cph.AssemblyOrder;";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { HierarchyId = hierarchyId });
                
                return results.Select(MapToEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar relações por hierarquia {hierarchyId}: {ex.Message}", ex);
            }
        }

        public async Task<CompositeProductXHierarchy?> GetByProductAndHierarchyAsync(string productId, string hierarchyId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        cph.*,
                        h.Name as HierarchyName,
                        h.Description as HierarchyDescription,
                        p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON cph.ProductComponentHierarchyId = h.Id
                    LEFT JOIN Product p ON cph.ProductId = p.Id
                    WHERE cph.ProductId = @ProductId 
                    AND cph.ProductComponentHierarchyId = @HierarchyId";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, 
                    new { ProductId = productId, HierarchyId = hierarchyId });

                return result != null ? MapToEntity(result) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CompositeProductXHierarchy por ProductId e HierarchyId");
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetOrderedByProductIdAsync(string productId, bool includeInactive = false)
        {
            return await GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetActiveByProductIdAsync(string productId)
        {
            return await GetByProductIdAsync(productId);
        }

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetRequiredByProductIdAsync(string productId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        cph.*,
                        h.Name as HierarchyName,
                        h.Description as HierarchyDescription,
                        p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON cph.ProductComponentHierarchyId = h.Id
                    LEFT JOIN Product p ON cph.ProductId = p.Id
                    WHERE cph.ProductId = @ProductId 
                    AND cph.IsOptional = 0
                    ORDER BY cph.AssemblyOrder ASC";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { ProductId = productId });

                return results.Select(MapToEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CompositeProductXHierarchy obrigatórias por ProductId: {ProductId}", productId);
                throw;
            }
        }

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetOptionalByProductIdAsync(string productId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        cph.*,
                        h.Name as HierarchyName,
                        h.Description as HierarchyDescription,
                        p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON cph.ProductComponentHierarchyId = h.Id
                    LEFT JOIN Product p ON cph.ProductId = p.Id
                    WHERE cph.ProductId = @ProductId 
                    AND cph.IsOptional = 1
                    ORDER BY cph.AssemblyOrder ASC";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { ProductId = productId });

                return results.Select(MapToEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CompositeProductXHierarchy opcionais por ProductId: {ProductId}", productId);
                throw;
            }
        }

        #endregion

        #region Operações de Validação

        public async Task<bool> RelationExistsAsync(string productId, string hierarchyId)
        {
            try
            {
                const string sql = @"SELECT COUNT(1) FROM CompositeProductXHierarchy 
                           WHERE ProductId = @ProductId AND ProductComponentHierarchyId = @HierarchyId";

                using var connection = _connectionFactory.CreateConnection();
                var parameters = new
                {
                    ProductId = productId,
                    HierarchyId = hierarchyId
                };

                var count = await connection.QuerySingleAsync<int>(sql, parameters);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de relação: {ex.Message}", ex);
            }
        }

        public async Task<bool> AssemblyOrderExistsAsync(string productId, int assemblyOrder, int? excludeRelationId = null)
        {
            try
            {
                var sql = "SELECT COUNT(1) FROM CompositeProductXHierarchy WHERE ProductId = @ProductId AND AssemblyOrder = @AssemblyOrder";
                
                if (excludeRelationId.HasValue)
                {
                    sql += " AND Id != @ExcludeRelationId";
                }

                using var connection = _connectionFactory.CreateConnection();
                var parameters = new
                {
                    ProductId = productId,
                    AssemblyOrder = assemblyOrder,
                    ExcludeRelationId = excludeRelationId
                };

                var count = await connection.QuerySingleAsync<int>(sql, parameters);
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao verificar existência de ordem de montagem: {ex.Message}", ex);
            }
        }

        public async Task<int> GetNextAssemblyOrderAsync(string productId)
        {
            try
            {
                const string sql = @"
                    SELECT COALESCE(MAX(AssemblyOrder), 0) + 1 
                    FROM CompositeProductXHierarchy 
                    WHERE ProductId = @ProductId";

                using var connection = _connectionFactory.CreateConnection();
                var nextOrder = await connection.QuerySingleAsync<int>(sql, new { ProductId = productId });
                
                return nextOrder;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter próxima ordem de montagem: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateQuantityLimitsAsync(int id, int quantity)
        {
            try
            {
                const string sql = @"
                    SELECT MinQuantity, MaxQuantity 
                    FROM CompositeProductXHierarchy 
                    WHERE Id = @Id";

                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Id = id });
                
                if (result == null) return false;

                int minQuantity = result.MinQuantity;
                int maxQuantity = result.MaxQuantity;

                return quantity >= minQuantity && (maxQuantity == 0 || quantity <= maxQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar limites de quantidade: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operações em Lote

        public async Task<bool> CreateBatchAsync(IEnumerable<CompositeProductXHierarchy> relations)
        {
            try
            {
                const string sql = @"
                    INSERT INTO CompositeProductXHierarchy (
                        ProductComponentHierarchyId, ProductId, MinQuantity, MaxQuantity,
                        IsOptional, AssemblyOrder, Notes
                    ) VALUES (
                        @ProductComponentHierarchyId, @ProductId, @MinQuantity, @MaxQuantity,
                        @IsOptional, @AssemblyOrder, @Notes
                    )";

                using var connection = _connectionFactory.CreateConnection();
                
                var parameters = relations.Select(r => new
                {
                    r.ProductComponentHierarchyId,
                    r.ProductId,
                    r.MinQuantity,
                    r.MaxQuantity,
                    IsOptional = r.IsOptional ? 1 : 0,
                    r.AssemblyOrder,
                    r.Notes
                });

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar CompositeProductXHierarchy em lote");
                throw;
            }
        }

        public async Task<bool> UpdateStatusBatchAsync(IEnumerable<int> relationIds, bool isActive)
        {
            // Método mantido por compatibilidade, mas sem funcionalidade real
            // pois não há mais o campo IsActive
            return true;
        }

        public async Task<bool> DeleteBatchAsync(IEnumerable<int> relationIds)
        {
            try
            {
                const string sql = "DELETE FROM CompositeProductXHierarchy WHERE Id IN @RelationIds";

                using var connection = _connectionFactory.CreateConnection();
                var rowsAffected = await connection.ExecuteAsync(sql, new { RelationIds = relationIds });
                
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir relações em lote: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReorderRelationsAsync(string productId, Dictionary<int, int> newOrders)
        {
            try
            {
                const string sql = @"
                    UPDATE CompositeProductXHierarchy 
                    SET AssemblyOrder = @NewOrder
                    WHERE Id = @RelationId AND ProductId = @ProductId";

                using var connection = _connectionFactory.CreateConnection();
                
                var parameters = newOrders.Select(kvp => new
                {
                    RelationId = kvp.Key,
                    NewOrder = kvp.Value,
                    ProductId = productId
                });

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao reordenar relações: {ex.Message}", ex);
            }
        }

        #endregion

        #region Estatísticas e Relatórios

        public async Task<int> CountByProductIdAsync(string productId, bool includeInactive = false)
        {
            try
            {
                // Note: CompositeProductXHierarchy não possui mais IsActive por não herdar de Entity
                // então includeInactive não tem efeito real, mas mantemos para compatibilidade
                const string sql = "SELECT COUNT(1) FROM CompositeProductXHierarchy WHERE ProductId = @ProductId";

                using var connection = _connectionFactory.CreateConnection();
                var count = await connection.QuerySingleAsync<int>(sql, new { ProductId = productId });
                
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao contar relações por produto: {ex.Message}", ex);
            }
        }

        public async Task<int> CountByHierarchyIdAsync(string hierarchyId, bool includeInactive = false)
        {
            try
            {
                // Note: CompositeProductXHierarchy não possui mais IsActive por não herdar de Entity
                // então includeInactive não tem efeito real, mas mantemos para compatibilidade
                const string sql = "SELECT COUNT(1) FROM CompositeProductXHierarchy WHERE ProductComponentHierarchyId = @HierarchyId";

                using var connection = _connectionFactory.CreateConnection();
                var count = await connection.QuerySingleAsync<int>(sql, new { HierarchyId = hierarchyId });
                
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao contar relações por hierarquia: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, int>> GetHierarchyUsageStatsAsync(string hierarchyId)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        'Total' as Category,
                        COUNT(1) as Count
                    FROM CompositeProductXHierarchy 
                    WHERE ProductComponentHierarchyId = @HierarchyId
                    
                    UNION ALL
                    
                    SELECT 
                        'Active' as Category,
                        COUNT(1) as Count
                    FROM CompositeProductXHierarchy 
                    WHERE ProductComponentHierarchyId = @HierarchyId
                    
                    UNION ALL
                    
                    SELECT 
                        'Optional' as Category,
                        COUNT(1) as Count
                    FROM CompositeProductXHierarchy 
                    WHERE ProductComponentHierarchyId = @HierarchyId AND IsOptional = 1
                    
                    UNION ALL
                    
                    SELECT 
                        'Required' as Category,
                        COUNT(1) as Count
                    FROM CompositeProductXHierarchy 
                    WHERE ProductComponentHierarchyId = @HierarchyId AND IsOptional = 0";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { HierarchyId = hierarchyId });

                return results.ToDictionary(r => (string)r.Category, r => (int)r.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas de uso da hierarquia: {HierarchyId}", hierarchyId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetTopProductsUsingHierarchiesAsync(int limit = 10)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        p.Name as ProductName,
                        COUNT(cph.Id) as HierarchyCount
                    FROM CompositeProductXHierarchy cph
                    JOIN Product p ON cph.ProductId = p.Id
                    GROUP BY cph.ProductId, p.Name
                    ORDER BY COUNT(cph.Id) DESC
                    LIMIT @Limit";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { Limit = limit });

                return results.ToDictionary(r => (string)r.ProductName, r => (int)r.HierarchyCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter produtos que mais usam hierarquias");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetTopUsedHierarchiesAsync(int limit = 10)
        {
            try
            {
                const string sql = @"
                    SELECT 
                        h.Name as HierarchyName,
                        COUNT(cph.Id) as UsageCount
                    FROM CompositeProductXHierarchy cph
                    JOIN ProductComponentHierarchy h ON cph.ProductComponentHierarchyId = h.Id
                    GROUP BY cph.ProductComponentHierarchyId, h.Name
                    ORDER BY COUNT(cph.Id) DESC
                    LIMIT @Limit";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, new { Limit = limit });

                return results.ToDictionary(r => (string)r.HierarchyName, r => (int)r.UsageCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter hierarquias mais utilizadas");
                throw;
            }
        }

        #endregion

        #region Consultas Complexas

        public async Task<IEnumerable<CompositeProductXHierarchy>> GetWithFullDataAsync(string? productId = null, string? hierarchyId = null)
        {
            try
            {
                var whereConditions = new List<string>();
                var parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(productId))
                {
                    whereConditions.Add("cph.ProductId = @ProductId");
                    parameters.Add("ProductId", productId);
                }

                if (!string.IsNullOrEmpty(hierarchyId))
                {
                    whereConditions.Add("cph.ProductComponentHierarchyId = @HierarchyId");
                    parameters.Add("HierarchyId", hierarchyId);
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var sql = $@"
                    SELECT 
                        cph.*,
                        h.Name as HierarchyName,
                        h.Description as HierarchyDescription,
                        p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON cph.ProductComponentHierarchyId = h.Id
                    LEFT JOIN Product p ON cph.ProductId = p.Id
                    {whereClause}
                    ORDER BY cph.AssemblyOrder ASC";

                using var connection = _connectionFactory.CreateConnection();
                var results = await connection.QueryAsync<dynamic>(sql, parameters);

                return results.Select(MapToEntity).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CompositeProductXHierarchy com dados completos");
                throw;
            }
        }

        public async Task<(IEnumerable<CompositeProductXHierarchy> Relations, int TotalCount)> SearchAsync(
            string? searchTerm = null,
            string? productId = null,
            string? hierarchyId = null,
            bool? isActive = null,
            bool? isOptional = null,
            int skip = 0,
            int take = 25,
            string sortBy = "AssemblyOrder",
            string sortDirection = "asc")
        {
            try
            {
                var whereConditions = new List<string>();
                var parameters = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    whereConditions.Add("(h.Name LIKE @SearchTerm OR p.Name LIKE @SearchTerm OR cph.Notes LIKE @SearchTerm)");
                    parameters.Add("SearchTerm", $"%{searchTerm}%");
                }

                if (!string.IsNullOrWhiteSpace(productId))
                {
                    whereConditions.Add("cph.ProductId = @ProductId");
                    parameters.Add("ProductId", productId);
                }

                if (!string.IsNullOrWhiteSpace(hierarchyId))
                {
                    whereConditions.Add("cph.ProductComponentHierarchyId = @HierarchyId");
                    parameters.Add("HierarchyId", hierarchyId);
                }

                if (isActive.HasValue)
                {
                    // Note: CompositeProductXHierarchy não possui mais IsActive, mas mantemos por compatibilidade
                    // Como todas são consideradas ativas, filtramos apenas se for false
                    if (!isActive.Value)
                    {
                        whereConditions.Add("1 = 0"); // Filtro que nunca será verdadeiro
                    }
                }

                if (isOptional.HasValue)
                {
                    whereConditions.Add("cph.IsOptional = @IsOptional");
                    parameters.Add("IsOptional", isOptional.Value ? 1 : 0);
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";
                
                var validSortColumns = new[] { "AssemblyOrder", "MinQuantity", "MaxQuantity" };
                var sortColumn = validSortColumns.Contains(sortBy) ? sortBy : "AssemblyOrder";
                var sortDirectionStr = sortDirection?.ToUpper() == "DESC" ? "DESC" : "ASC";

                // Query para dados
                var sql = $@"
                    SELECT cph.Id, cph.ProductComponentHierarchyId, cph.ProductId,
                           cph.MinQuantity, cph.MaxQuantity, cph.IsOptional,
                           cph.AssemblyOrder, cph.Notes,
                           h.Id as HierarchyId, h.Name as HierarchyName,
                           p.Id as ProductId, p.Name as ProductName
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON h.Id = cph.ProductComponentHierarchyId
                    LEFT JOIN Product p ON p.Id = cph.ProductId
                    {whereClause}
                    ORDER BY cph.{sortColumn} {sortDirectionStr}, h.Name
                    LIMIT @Take OFFSET @Skip";

                // Query para contagem
                var countSql = $@"
                    SELECT COUNT(*)
                    FROM CompositeProductXHierarchy cph
                    LEFT JOIN ProductComponentHierarchy h ON h.Id = cph.ProductComponentHierarchyId
                    LEFT JOIN Product p ON p.Id = cph.ProductId
                    {whereClause}";

                parameters.Add("Take", take);
                parameters.Add("Skip", skip);

                using var connection = _connectionFactory.CreateConnection();
                
                var relations = await connection.QueryAsync<dynamic>(sql, parameters);
                var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

                return (relations.Select(MapToEntity), totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar relações: {ex.Message}", ex);
            }
        }

        #endregion

        #region Helper Methods

        private static CompositeProductXHierarchy MapToEntity(dynamic row)
        {
            return new CompositeProductXHierarchy
            {
                Id = row.Id,
                ProductComponentHierarchyId = row.ProductComponentHierarchyId,
                ProductId = row.ProductId,
                MinQuantity = row.MinQuantity,
                MaxQuantity = row.MaxQuantity,
                IsOptional = row.IsOptional == 1,
                AssemblyOrder = row.AssemblyOrder,
                Notes = row.Notes,
                // Propriedades de navegação simuladas
                ProductComponentHierarchy = new ProductComponentHierarchy
                {
                    Id = row.HierarchyId ?? row.ProductComponentHierarchyId,
                    Name = row.HierarchyName ?? string.Empty
                },
                Product = new CompositeProduct
                {
                    Id = row.ProductId,
                    Name = row.ProductName ?? string.Empty
                }
            };
        }

        #endregion
    }
} 