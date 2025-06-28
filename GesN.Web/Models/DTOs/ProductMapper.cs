using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.DTOs
{
    /// <summary>
    /// Mapper responsável por converter ProductDto para as classes específicas de Product
    /// </summary>
    public static class ProductMapper
    {
        /// <summary>
        /// Converte um ProductDto para a classe específica baseada no ProductType
        /// </summary>
        /// <param name="productDto">DTO com dados do banco</param>
        /// <returns>Instância da classe específica (SimpleProduct, CompositeProduct, ProductGroup) ou null se o tipo for inválido</returns>
        public static Product? ToEntity(ProductDto? productDto)
        {
            if (productDto == null) 
                return null;

            return productDto.ProductType switch
            {
                ProductType.Simple => MapToSimpleProduct(productDto),
                ProductType.Composite => MapToCompositeProduct(productDto),
                ProductType.Group => MapToProductGroup(productDto),
                _ => null
            };
        }

        /// <summary>
        /// Converte uma coleção de ProductDto para as classes específicas
        /// </summary>
        /// <param name="productDtos">Lista de DTOs</param>
        /// <returns>Lista de produtos convertidos (filtra nulos)</returns>
        public static IEnumerable<Product> ToEntities(IEnumerable<ProductDto> productDtos)
        {
            return productDtos
                .Select(ToEntity)
                .Where(product => product != null)!;
        }

        /// <summary>
        /// Mapeia ProductDto para SimpleProduct
        /// </summary>
        private static SimpleProduct MapToSimpleProduct(ProductDto dto)
        {
            return new SimpleProduct
            {
                Id = dto.Id,
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantityPrice = dto.QuantityPrice,
                UnitPrice = dto.UnitPrice,
                Cost = dto.Cost,
                CategoryId = dto.CategoryId,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Note = dto.Note,
                AssemblyTime = dto.AssemblyTime,
                AssemblyInstructions = dto.AssemblyInstructions,
                StateCode = (ObjectState)dto.StateCode,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy
            };
        }

        /// <summary>
        /// Mapeia ProductDto para CompositeProduct
        /// </summary>
        private static CompositeProduct MapToCompositeProduct(ProductDto dto)
        {
            return new CompositeProduct
            {
                Id = dto.Id,
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantityPrice = dto.QuantityPrice,
                UnitPrice = dto.UnitPrice,
                Cost = dto.Cost,
                CategoryId = dto.CategoryId,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Note = dto.Note,
                AssemblyTime = dto.AssemblyTime,
                AssemblyInstructions = dto.AssemblyInstructions,
                StateCode = (ObjectState)dto.StateCode,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy
            };
        }

        /// <summary>
        /// Mapeia ProductDto para ProductGroup
        /// </summary>
        private static ProductGroup MapToProductGroup(ProductDto dto)
        {
            return new ProductGroup
            {
                Id = dto.Id,
                SKU = dto.SKU,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                QuantityPrice = dto.QuantityPrice,
                UnitPrice = dto.UnitPrice,
                Cost = dto.Cost,
                CategoryId = dto.CategoryId,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                Note = dto.Note,
                AssemblyTime = dto.AssemblyTime,
                AssemblyInstructions = dto.AssemblyInstructions,
                StateCode = (ObjectState)dto.StateCode,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                LastModifiedAt = dto.LastModifiedAt,
                LastModifiedBy = dto.LastModifiedBy
            };
        }
    }
} 