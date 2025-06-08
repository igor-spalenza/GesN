using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE StateCode = @StateCode
                ORDER BY FirstName, LastName";

            return await connection.QueryAsync<Customer>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<Customer?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE Id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Customer>(sql, new { Id = id });
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE Email = @Email AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Customer>(sql, 
                new { Email = email, StateCode = (int)ObjectState.Active });
        }

        public async Task<Customer?> GetByDocumentAsync(string documentNumber)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE DocumentNumber = @DocumentNumber AND StateCode = @StateCode";

            return await connection.QuerySingleOrDefaultAsync<Customer>(sql, 
                new { DocumentNumber = documentNumber, StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Customer>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE StateCode = @StateCode
                ORDER BY FirstName, LastName";

            return await connection.QueryAsync<Customer>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE StateCode = @StateCode
                  AND (FirstName LIKE @SearchTerm
                       OR LastName LIKE @SearchTerm
                       OR Email LIKE @SearchTerm
                       OR DocumentNumber LIKE @SearchTerm
                       OR Phone LIKE @SearchTerm)
                ORDER BY FirstName, LastName";

            var searchPattern = $"%{searchTerm}%";
            return await connection.QueryAsync<Customer>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });
        }

        public async Task<string> CreateAsync(Customer customer)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Customer (Id, FirstName, LastName, Email, DocumentNumber, DocumentType, Phone, GoogleContactId, StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
                VALUES (@Id, @FirstName, @LastName, @Email, @DocumentNumber, @DocumentType, @Phone, @GoogleContactId, @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy)";
            
            await connection.ExecuteAsync(sql, customer);
            return customer.Id;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Customer SET 
                FirstName = @FirstName, LastName = @LastName, Email = @Email, DocumentNumber = @DocumentNumber, 
                DocumentType = @DocumentType, Phone = @Phone, GoogleContactId = @GoogleContactId,
                StateCode = @StateCode, LastModifiedAt = @LastModifiedAt, LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, customer);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE Customer SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Customer WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM Customer WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Customer>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT * FROM Customer 
                WHERE StateCode = @StateCode
                ORDER BY FirstName, LastName
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            return await connection.QueryAsync<Customer>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });
        }
    }
} 