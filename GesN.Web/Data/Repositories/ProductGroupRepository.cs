using Dapper;
using GesN.Web.Infrastructure.Data;
using GesN.Web.Interfaces.Repositories;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using GesN.Web.Models.DTOs;

namespace GesN.Web.Data.Repositories
{
    public class ProductGroupRepository : IProductGroupRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProductGroupRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ProductGroup>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = 'Group' AND StateCode = @StateCode 
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active });

            return productDtos.Select(dto => (ProductGroup)ProductMapper.ToEntity(dto)!);
        }

        public async Task<ProductGroup?> GetByIdAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE Id = @Id AND ProductType = 'Group'";

            var productDto = await connection.QuerySingleOrDefaultAsync<ProductDto>(sql, new { Id = id });
            
            return productDto != null ? (ProductGroup)ProductMapper.ToEntity(productDto)! : null;
        }

        public async Task<IEnumerable<ProductGroup>> GetActiveAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = 'Group' AND StateCode = @StateCode
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active });

            return productDtos.Select(dto => (ProductGroup)ProductMapper.ToEntity(dto)!);
        }

        public async Task<IEnumerable<ProductGroup>> GetByCategoryAsync(string categoryId)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = 'Group' AND CategoryId = @CategoryId AND StateCode = @StateCode
                ORDER BY Name";

            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { CategoryId = categoryId, StateCode = (int)ObjectState.Active });

            return productDtos.Select(dto => (ProductGroup)ProductMapper.ToEntity(dto)!);
        }

        public async Task<IEnumerable<ProductGroup>> SearchAsync(string searchTerm)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = 'Group' AND StateCode = @StateCode
                  AND (Name LIKE @SearchTerm OR Description LIKE @SearchTerm OR SKU LIKE @SearchTerm)
                ORDER BY Name";

            var searchPattern = $"%{searchTerm}%";
            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active, SearchTerm = searchPattern });

            return productDtos.Select(dto => (ProductGroup)ProductMapper.ToEntity(dto)!);
        }

        public async Task<IEnumerable<ProductGroup>> GetPagedAsync(int page, int pageSize)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT *, ProductType as ProductTypeString FROM Product 
                WHERE ProductType = 'Group' AND StateCode = @StateCode
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset";

            var offset = (page - 1) * pageSize;
            var productDtos = await connection.QueryAsync<ProductDto>(sql, 
                new { StateCode = (int)ObjectState.Active, PageSize = pageSize, Offset = offset });

            return productDtos.Select(dto => (ProductGroup)ProductMapper.ToEntity(dto)!);
        }

        public async Task<ProductGroup?> GetWithItemsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT p.*, p.ProductType as ProductTypeString,
                       gi.*, 
                       cp.*, cp.ProductType as ProductTypeString
                FROM Product p
                LEFT JOIN ProductGroupItem gi ON p.Id = gi.ProductGroupId
                LEFT JOIN Product cp ON gi.ProductId = cp.Id
                WHERE p.Id = @Id AND p.ProductType = 'Group'";

            var productGroupDict = new Dictionary<string, ProductGroup>();

            await connection.QueryAsync<ProductDto, ProductGroupItem, ProductDto, ProductGroup>(
                sql,
                (productDto, groupItem, componentDto) =>
                {
                    if (!productGroupDict.TryGetValue(productDto.Id, out var productGroup))
                    {
                        productGroup = (ProductGroup)ProductMapper.ToEntity(productDto)!;
                        productGroupDict[productDto.Id] = productGroup;
                    }

                    if (groupItem != null)
                    {
                        if (componentDto != null)
                        {
                            groupItem.Product = ProductMapper.ToEntity(componentDto);
                        }
                        productGroup.GroupItems.Add(groupItem);
                    }

                    return productGroup;
                },
                new { Id = id },
                splitOn: "Id,Id"
            );

            return productGroupDict.Values.FirstOrDefault();
        }



        public async Task<ProductGroup?> GetWithExchangeRulesAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT p.*, p.ProductType as ProductTypeString,
                       er.*, 
                       fp.*, fp.ProductType as ProductTypeString,
                       tp.*, tp.ProductType as ProductTypeString
                FROM Product p
                LEFT JOIN ProductGroupExchangeRule er ON p.Id = er.ProductGroupId
                LEFT JOIN Product fp ON er.OriginalProductId = fp.Id
                LEFT JOIN Product tp ON er.ExchangeProductId = tp.Id
                WHERE p.Id = @Id AND p.ProductType = 'Group'";

            var productGroupDict = new Dictionary<string, ProductGroup>();

            await connection.QueryAsync<ProductDto, ProductGroupExchangeRule, ProductDto, ProductDto, ProductGroup>(
                sql,
                (productDto, exchangeRule, fromProductDto, toProductDto) =>
                {
                    if (!productGroupDict.TryGetValue(productDto.Id, out var productGroup))
                    {
                        productGroup = (ProductGroup)ProductMapper.ToEntity(productDto)!;
                        productGroupDict[productDto.Id] = productGroup;
                    }

                    if (exchangeRule != null)
                    {
                        // Exchange rules now use SourceGroupItem and TargetGroupItem instead of direct products
                        productGroup.ExchangeRules.Add(exchangeRule);
                    }

                    return productGroup;
                },
                new { Id = id },
                splitOn: "Id,Id,Id"
            );

            return productGroupDict.Values.FirstOrDefault();
        }

        public async Task<ProductGroup?> GetCompleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT p.*, p.ProductType as ProductTypeString,
                       gi.*, 
                       cp.*, cp.ProductType as ProductTypeString,
                       er.*, 
                       fp.*, fp.ProductType as ProductTypeString,
                       tp.*, tp.ProductType as ProductTypeString
                FROM Product p
                LEFT JOIN ProductGroupItem gi ON p.Id = gi.ProductGroupId
                LEFT JOIN Product cp ON gi.ProductId = cp.Id
                LEFT JOIN ProductGroupExchangeRule er ON p.Id = er.ProductGroupId
                LEFT JOIN Product fp ON er.OriginalProductId = fp.Id
                LEFT JOIN Product tp ON er.ExchangeProductId = tp.Id
                WHERE p.Id = @Id AND p.ProductType = 'Group'";

            var productGroupDict = new Dictionary<string, ProductGroup>();

            await connection.QueryAsync<ProductDto, ProductGroupItem, ProductDto, ProductGroupExchangeRule, ProductDto, ProductDto, ProductGroup>(
                sql,
                (productDto, groupItem, componentDto, exchangeRule, fromProductDto, toProductDto) =>
                {
                    if (!productGroupDict.TryGetValue(productDto.Id, out var productGroup))
                    {
                        productGroup = (ProductGroup)ProductMapper.ToEntity(productDto)!;
                        productGroupDict[productDto.Id] = productGroup;
                    }

                    if (groupItem != null && !productGroup.GroupItems.Any(gi => gi.Id == groupItem.Id))
                    {
                        if (componentDto != null)
                        {
                            groupItem.Product = ProductMapper.ToEntity(componentDto);
                        }
                        productGroup.GroupItems.Add(groupItem);
                    }

                    if (exchangeRule != null && !productGroup.ExchangeRules.Any(er => er.Id == exchangeRule.Id))
                    {
                        // Exchange rules now use SourceGroupItem and TargetGroupItem instead of direct products
                        productGroup.ExchangeRules.Add(exchangeRule);
                    }

                    return productGroup;
                },
                new { Id = id },
                splitOn: "Id,Id,Id,Id,Id"
            );

            return productGroupDict.Values.FirstOrDefault();
        }

        public async Task<string> CreateAsync(ProductGroup productGroup)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                INSERT INTO Product (
                    Id, Name, Description, Price, QuantityPrice, UnitPrice, CategoryId, Category, SKU,
                    ImageUrl, Note, Cost, AssemblyTime, AssemblyInstructions, ProductType, 
                    StateCode, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
                )
                VALUES (
                    @Id, @Name, @Description, @Price, @QuantityPrice, @UnitPrice, @CategoryId, @Category, @SKU,
                    @ImageUrl, @Note, @Cost, @AssemblyTime, @AssemblyInstructions, @ProductType, 
                    @StateCode, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
                )";
            
            var parameters = new
            {
                Id = productGroup.Id,
                Name = productGroup.Name,
                Description = productGroup.Description,
                Price = productGroup.Price,
                QuantityPrice = productGroup.QuantityPrice,
                UnitPrice = productGroup.UnitPrice,
                CategoryId = productGroup.CategoryId,
                Category = productGroup.Category,
                SKU = productGroup.SKU,
                ImageUrl = productGroup.ImageUrl,
                Note = productGroup.Note,
                Cost = productGroup.Cost,
                AssemblyTime = productGroup.AssemblyTime,
                AssemblyInstructions = productGroup.AssemblyInstructions,
                ProductType = productGroup.ProductType.ToString(),
                StateCode = (int)productGroup.StateCode,
                CreatedAt = productGroup.CreatedAt,
                CreatedBy = productGroup.CreatedBy,
                LastModifiedAt = productGroup.LastModifiedAt,
                LastModifiedBy = productGroup.LastModifiedBy
            };
            
            await connection.ExecuteAsync(sql, parameters);
            return productGroup.Id;
        }

        public async Task<bool> UpdateAsync(ProductGroup productGroup)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Product SET 
                    Name = @Name,
                    Description = @Description,
                    Price = @Price,
                    QuantityPrice = @QuantityPrice,
                    UnitPrice = @UnitPrice,
                    CategoryId = @CategoryId,
                    Category = @Category,
                    SKU = @SKU,
                    ImageUrl = @ImageUrl,
                    Note = @Note,
                    Cost = @Cost,
                    AssemblyTime = @AssemblyTime,
                    AssemblyInstructions = @AssemblyInstructions,
                    StateCode = @StateCode,
                    LastModifiedAt = @LastModifiedAt,
                    LastModifiedBy = @LastModifiedBy
                WHERE Id = @Id AND ProductType = 'Group'";
            
            var parameters = new
            {
                Id = productGroup.Id,
                Name = productGroup.Name,
                Description = productGroup.Description,
                Price = productGroup.Price,
                QuantityPrice = productGroup.QuantityPrice,
                UnitPrice = productGroup.UnitPrice,
                CategoryId = productGroup.CategoryId,
                Category = productGroup.Category,
                SKU = productGroup.SKU,
                ImageUrl = productGroup.ImageUrl,
                Note = productGroup.Note,
                Cost = productGroup.Cost,
                AssemblyTime = productGroup.AssemblyTime,
                AssemblyInstructions = productGroup.AssemblyInstructions,
                StateCode = (int)productGroup.StateCode,
                LastModifiedAt = productGroup.LastModifiedAt,
                LastModifiedBy = productGroup.LastModifiedBy
            };
            
            var result = await connection.ExecuteAsync(sql, parameters);
            return result > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                UPDATE Product SET StateCode = @StateCode, LastModifiedAt = @LastModifiedAt 
                WHERE Id = @Id AND ProductType = 'Group'";
            
            var result = await connection.ExecuteAsync(sql, new 
            { 
                Id = id, 
                StateCode = (int)ObjectState.Inactive, 
                LastModifiedAt = DateTime.UtcNow 
            });
            
            return result > 0;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM Product 
                WHERE Id = @Id AND ProductType = 'Group' AND StateCode = @StateCode";
            
            var count = await connection.QuerySingleAsync<int>(sql, new 
            { 
                Id = id, 
                StateCode = (int)ObjectState.Active 
            });
            
            return count > 0;
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM Product 
                WHERE ProductType = 'Group' AND StateCode = @StateCode";
            
            return await connection.QuerySingleAsync<int>(sql, new 
            { 
                StateCode = (int)ObjectState.Active 
            });
        }

        public async Task<bool> HasItemsAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM ProductGroupItem 
                WHERE ProductGroupId = @Id AND StateCode = @StateCode";
            
            var count = await connection.QuerySingleAsync<int>(sql, new 
            { 
                Id = id, 
                StateCode = (int)ObjectState.Active 
            });
            
            return count > 0;
        }



        public async Task<bool> HasExchangeRulesAsync(string id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT COUNT(1) FROM ProductGroupExchangeRule 
                WHERE ProductGroupId = @Id AND StateCode = @StateCode";
            
            var count = await connection.QuerySingleAsync<int>(sql, new 
            { 
                Id = id, 
                StateCode = (int)ObjectState.Active 
            });
            
            return count > 0;
        }

        public async Task<decimal> CalculateGroupPriceAsync(string id, IEnumerable<string> selectedItemIds)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            
            const string sql = @"
                SELECT gi.*, p.Price
                FROM ProductGroupItem gi
                INNER JOIN Product p ON gi.ProductId = p.Id
                WHERE gi.ProductGroupId = @GroupId AND gi.Id IN @ItemIds AND gi.StateCode = @StateCode";
            
            var items = await connection.QueryAsync(sql, new 
            { 
                GroupId = id, 
                ItemIds = selectedItemIds,
                StateCode = (int)ObjectState.Active 
            });
            
            return items.Sum(item => 
                (decimal)item.Price * (int)item.Quantity + 
                (decimal)item.AdditionalPrice);
        }
    }
} 