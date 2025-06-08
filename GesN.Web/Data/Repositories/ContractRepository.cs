using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using Microsoft.Data.Sqlite;

namespace GesN.Web.Data.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ContractRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                ORDER BY c.CreatedAt DESC";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<Contract?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.Id = @Id";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { Id = id },
                splitOn: "Id");

            return contractDict.Values.FirstOrDefault();
        }

        public async Task<Contract?> GetByContractNumberAsync(string contractNumber)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.ContractNumber = @ContractNumber";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { ContractNumber = contractNumber },
                splitOn: "Id");

            return contractDict.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<Contract>> GetByCustomerIdAsync(string customerId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.CustomerId = @CustomerId
                ORDER BY c.CreatedAt DESC";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { CustomerId = customerId },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.Status = @Status
                ORDER BY c.CreatedAt DESC";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { Status = (int)status },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<IEnumerable<Contract>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.StateCode = @StateCode
                ORDER BY c.CreatedAt DESC";

            var contractDict = new Dictionary<string, Contract>();

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { StateCode = (int)ObjectState.Active },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<IEnumerable<Contract>> GetExpiringAsync(int days = 30)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.StateCode = @StateCode 
                  AND c.EndDate <= @ExpirationDate
                  AND c.Status IN (@Active, @Signed, @Renewed)
                ORDER BY c.EndDate";

            var contractDict = new Dictionary<string, Contract>();
            var expirationDate = DateTime.UtcNow.AddDays(days);

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { 
                    StateCode = (int)ObjectState.Active,
                    ExpirationDate = expirationDate,
                    Active = (int)ContractStatus.Active,
                    Signed = (int)ContractStatus.Signed,
                    Renewed = (int)ContractStatus.Renewed
                },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<IEnumerable<Contract>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.StateCode = @StateCode
                  AND (c.ContractNumber LIKE @SearchTerm
                       OR c.Title LIKE @SearchTerm
                       OR c.Description LIKE @SearchTerm
                       OR cu.Name LIKE @SearchTerm)
                ORDER BY c.CreatedAt DESC";

            var contractDict = new Dictionary<string, Contract>();
            var searchPattern = $"%{searchTerm}%";

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern },
                splitOn: "Id");

            return contractDict.Values;
        }

        public async Task<string> CreateAsync(Contract contract)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Contract (Id, ContractNumber, Title, Description, CustomerId, 
                                    ContractValue, Status, StartDate, EndDate, 
                                    StateCode, CreatedAt, UpdatedAt)
                VALUES (@Id, @ContractNumber, @Title, @Description, @CustomerId, 
                        @ContractValue, @Status, @StartDate, @EndDate,
                        @StateCode, @CreatedAt, @UpdatedAt)";
            
            await connection.ExecuteAsync(sql, contract);
            return contract.Id;
        }

        public async Task<bool> UpdateAsync(Contract contract)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Contract 
                SET ContractNumber = @ContractNumber, Title = @Title, Description = @Description,
                    ContractValue = @ContractValue, Status = @Status, StartDate = @StartDate, 
                    EndDate = @EndDate, UpdatedAt = @UpdatedAt
                WHERE Id = @Id";
            
            var rowsAffected = await connection.ExecuteAsync(sql, contract);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "UPDATE Contract SET StateCode = @StateCode WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { StateCode = (int)ObjectState.Inactive, Id = id });
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(1) FROM Contract WHERE Id = @Id";
            var count = await connection.QuerySingleAsync<int>(sql, new { Id = id });
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string sql = "SELECT COUNT(*) FROM Contract WHERE StateCode = @StateCode";
            return await connection.QuerySingleAsync<int>(sql, new { StateCode = (int)ObjectState.Active });
        }

        public async Task<IEnumerable<Contract>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT c.*, cu.*
                FROM Contract c
                LEFT JOIN Customer cu ON c.CustomerId = cu.Id
                WHERE c.StateCode = @StateCode
                ORDER BY c.CreatedAt DESC
                LIMIT @PageSize OFFSET @Offset";

            var contractDict = new Dictionary<string, Contract>();
            var offset = (page - 1) * pageSize;

            await connection.QueryAsync<Contract, Customer?, Contract>(
                sql,
                (contract, customer) =>
                {
                    if (!contractDict.TryGetValue(contract.Id, out var existingContract))
                    {
                        existingContract = contract;
                        contractDict.Add(contract.Id, existingContract);
                    }

                    if (customer != null && existingContract.Customer == null)
                        existingContract.Customer = customer;

                    return existingContract;
                },
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset },
                splitOn: "Id");

            return contractDict.Values;
        }
    }
} 