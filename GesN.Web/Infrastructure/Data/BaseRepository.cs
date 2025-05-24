using Dapper;
using GesN.Web.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace GesN.Web.Infrastructure.Data
{
    public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly string _tableName;

        protected BaseRepository(IUnitOfWork unitOfWork, string tableName)
        {
            _unitOfWork = unitOfWork;
            _tableName = tableName;
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            var query = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<TEntity>(
                query,
                new { Id = id },
                _unitOfWork.Transaction
            );
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query = $"SELECT * FROM {_tableName}";
            return await _unitOfWork.Connection.QueryAsync<TEntity>(
                query,
                transaction: _unitOfWork.Transaction
            );
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Nota: Esta é uma implementação básica. Para consultas mais complexas,
            // você precisará implementar um construtor de consultas SQL mais robusto
            var all = await GetAllAsync();
            return all.Where(predicate.Compile());
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "Id" && !p.GetGetMethod().IsVirtual);
            
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var parameters = string.Join(", ", properties.Select(p => "@" + p.Name));
            
            var query = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";
            
            await _unitOfWork.Connection.ExecuteAsync(
                query,
                entity,
                _unitOfWork.Transaction
            );
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "Id" && !p.GetGetMethod().IsVirtual);
            
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            
            var query = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";
            
            await _unitOfWork.Connection.ExecuteAsync(
                query,
                entity,
                _unitOfWork.Transaction
            );
        }

        public virtual async Task DeleteAsync(object id)
        {
            var query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            await _unitOfWork.Connection.ExecuteAsync(
                query,
                new { Id = id },
                _unitOfWork.Transaction
            );
        }

        public virtual async Task<bool> ExistsAsync(object id)
        {
            var query = $"SELECT COUNT(1) FROM {_tableName} WHERE Id = @Id";
            var count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                query,
                new { Id = id },
                _unitOfWork.Transaction
            );
            return count > 0;
        }

        protected async Task<T> QuerySingleAsync<T>(string sql, object? param = default)
        {
            return await _unitOfWork.Connection.QuerySingleOrDefaultAsync<T>(
                sql,
                param,
                _unitOfWork.Transaction
            );
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = default)
        {
            return await _unitOfWork.Connection.QueryAsync<T>(
                sql,
                param,
                _unitOfWork.Transaction
            );
        }

        protected async Task ExecuteAsync(string sql, object? param = default)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                sql,
                param,
                _unitOfWork.Transaction
            );
        }
    }
} 