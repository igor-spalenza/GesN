using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    // Enum para identificar tipos de produto
    public enum ProductType
    {
        SimpleProduct,
        CompositeProduct,
        ProductGroup
    }

    // ViewModels para Listagem e Index
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public ProductStatisticsViewModel Statistics { get; set; } = new();
        public ProductSearchViewModel Search { get; set; } = new();
    }

    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int SimpleProducts { get; set; }
        public int CompositeProducts { get; set; }
        public int ProductGroups { get; set; }
        public int LowStockProducts { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class ProductSearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool ShowInactive { get; set; } = false;
        public ProductType? ProductType { get; set; }
        public string? CategoryId { get; set; }
        public string? SupplierId { get; set; }
        public bool ShowLowStock { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ViewModels para CRUD - SimpleProduct (por enquanto)
    public class CreateSimpleProductViewModel
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Código")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "O preço unitário é obrigatório")]
        [Display(Name = "Preço Unitário")]
        [Range(0.01, 999999.99, ErrorMessage = "O preço deve estar entre 0,01 e 999.999,99")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "O custo é obrigatório")]
        [Display(Name = "Custo")]
        [Range(0.01, 999999.99, ErrorMessage = "O custo deve estar entre 0,01 e 999.999,99")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public string CategoryId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O fornecedor é obrigatório")]
        [Display(Name = "Fornecedor")]
        public string SupplierId { get; set; } = string.Empty;

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;

        // Propriedades específicas do SimpleProduct
        [Display(Name = "Estoque Mínimo")]
        [Range(0, 999999.99, ErrorMessage = "O estoque mínimo deve estar entre 0 e 999.999,99")]
        public decimal MinStock { get; set; } = 0;

        [Display(Name = "Estoque Atual")]
        [Range(0, 999999.99, ErrorMessage = "O estoque atual deve estar entre 0 e 999.999,99")]
        public decimal CurrentStock { get; set; } = 0;

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; }

        [Display(Name = "Permite Personalização")]
        public bool AllowCustomization { get; set; } = false;

        [Display(Name = "Tempo de Montagem (minutos)")]
        [Range(0, 9999, ErrorMessage = "O tempo de montagem deve estar entre 0 e 9999 minutos")]
        public int? AssemblyTime { get; set; }

        [Display(Name = "Instruções de Montagem")]
        [StringLength(1000, ErrorMessage = "As instruções devem ter no máximo 1000 caracteres")]
        public string? AssemblyInstructions { get; set; }

        // Propriedades auxiliares para Autocomplete
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        // Lista de ingredientes
        public List<ProductIngredientViewModel> Ingredients { get; set; } = new();
    }

    public class EditSimpleProductViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Display(Name = "Código")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "O preço unitário é obrigatório")]
        [Display(Name = "Preço Unitário")]
        [Range(0.01, 999999.99, ErrorMessage = "O preço deve estar entre 0,01 e 999.999,99")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "O custo é obrigatório")]
        [Display(Name = "Custo")]
        [Range(0.01, 999999.99, ErrorMessage = "O custo deve estar entre 0,01 e 999.999,99")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        [Display(Name = "Categoria")]
        public string CategoryId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O fornecedor é obrigatório")]
        [Display(Name = "Fornecedor")]
        public string SupplierId { get; set; } = string.Empty;

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        // Propriedades específicas do SimpleProduct
        [Display(Name = "Estoque Mínimo")]
        [Range(0, 999999.99, ErrorMessage = "O estoque mínimo deve estar entre 0 e 999.999,99")]
        public decimal MinStock { get; set; }

        [Display(Name = "Estoque Atual")]
        [Range(0, 999999.99, ErrorMessage = "O estoque atual deve estar entre 0 e 999.999,99")]
        public decimal CurrentStock { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; }

        [Display(Name = "Permite Personalização")]
        public bool AllowCustomization { get; set; }

        [Display(Name = "Tempo de Montagem (minutos)")]
        [Range(0, 9999, ErrorMessage = "O tempo de montagem deve estar entre 0 e 9999 minutos")]
        public int? AssemblyTime { get; set; }

        [Display(Name = "Instruções de Montagem")]
        [StringLength(1000, ErrorMessage = "As instruções devem ter no máximo 1000 caracteres")]
        public string? AssemblyInstructions { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Propriedades auxiliares
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        // Lista de ingredientes
        public List<ProductIngredientViewModel> Ingredients { get; set; } = new();
    }

    public class ProductDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }
        public string CategoryId { get; set; } = string.Empty;
        public string SupplierId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public ProductType ProductType { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Propriedades relacionais
        public string CategoryName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;

        // Propriedades específicas do SimpleProduct (quando aplicável)
        public decimal? MinStock { get; set; }
        public decimal? CurrentStock { get; set; }
        public ProductionUnit? Unit { get; set; }
        public bool? AllowCustomization { get; set; }
        public int? AssemblyTime { get; set; }
        public string? AssemblyInstructions { get; set; }

        // Lista de ingredientes
        public List<ProductIngredientViewModel> Ingredients { get; set; } = new();

        // Propriedades calculadas
        public bool IsLowStock => MinStock.HasValue && CurrentStock.HasValue && CurrentStock <= MinStock;
        public decimal ProfitMargin => UnitPrice > 0 ? ((UnitPrice - Cost) / UnitPrice) * 100 : 0;
    }

    // ViewModel para ProductIngredient
    public class ProductIngredientViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string IngredientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [Range(0.01, 999999.99, ErrorMessage = "A quantidade deve estar entre 0,01 e 999.999,99")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; }

        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; } = false;

        [Display(Name = "Observações")]
        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        public string? Notes { get; set; }

        // Propriedades do ingrediente (para exibição)
        public string IngredientName { get; set; } = string.Empty;
        public decimal IngredientCostPerUnit { get; set; }
        public decimal IngredientCurrentStock { get; set; }
        public string SupplierName { get; set; } = string.Empty;

        // Propriedades calculadas
        public decimal TotalCost => Quantity * IngredientCostPerUnit;
        public bool HasSufficientStock => IngredientCurrentStock >= Quantity;
    }

    // ViewModel para Autocomplete
    public class ProductAutocompleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public decimal UnitPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public bool IsActive { get; set; }
    }

    // ViewModel para seleção de tipo de produto
    public class ProductTypeSelectionViewModel
    {
        [Required(ErrorMessage = "Selecione o tipo de produto")]
        [Display(Name = "Tipo de Produto")]
        public ProductType ProductType { get; set; }
    }
}

// Extension Methods para conversão
namespace GesN.Web.Models.ViewModels.Production
{
    public static class ProductMappingExtensions
    {
        // SimpleProduct → ViewModel
        public static ProductDetailsViewModel ToDetailsViewModel(this SimpleProduct product)
        {
            return new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Code = product.Code,
                UnitPrice = product.UnitPrice,
                Cost = product.Cost,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                IsActive = product.IsActive,
                ProductType = ProductType.SimpleProduct,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                LastModifiedAt = product.LastModifiedAt,
                LastModifiedBy = product.LastModifiedBy,
                CategoryName = product.Category?.Name ?? string.Empty,
                SupplierName = product.Supplier?.Name ?? string.Empty,
                MinStock = product.MinStock,
                CurrentStock = product.CurrentStock,
                Unit = product.Unit,
                AllowCustomization = product.AllowCustomization,
                AssemblyTime = product.AssemblyTime,
                AssemblyInstructions = product.AssemblyInstructions,
                Ingredients = new List<ProductIngredientViewModel>() // TODO: Carregar ingredientes
            };
        }

        public static EditSimpleProductViewModel ToEditViewModel(this SimpleProduct product)
        {
            return new EditSimpleProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Code = product.Code,
                UnitPrice = product.UnitPrice,
                Cost = product.Cost,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                IsActive = product.IsActive,
                MinStock = product.MinStock,
                CurrentStock = product.CurrentStock,
                Unit = product.Unit,
                AllowCustomization = product.AllowCustomization,
                AssemblyTime = product.AssemblyTime,
                AssemblyInstructions = product.AssemblyInstructions,
                CreatedAt = product.CreatedAt,
                CreatedBy = product.CreatedBy,
                LastModifiedAt = product.LastModifiedAt,
                LastModifiedBy = product.LastModifiedBy,
                CategoryName = product.Category?.Name ?? string.Empty,
                SupplierName = product.Supplier?.Name ?? string.Empty,
                Ingredients = new List<ProductIngredientViewModel>() // TODO: Carregar ingredientes
            };
        }

        public static ProductAutocompleteViewModel ToAutocompleteViewModel(this Product product)
        {
            var productType = product switch
            {
                SimpleProduct => ProductType.SimpleProduct,
                // CompositeProduct => ProductType.CompositeProduct, // Para próxima sprint
                // ProductGroup => ProductType.ProductGroup, // Para próxima sprint
                _ => ProductType.SimpleProduct
            };

            return new ProductAutocompleteViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                UnitPrice = product.UnitPrice,
                CategoryName = product.Category?.Name ?? string.Empty,
                ProductType = productType,
                IsActive = product.IsActive
            };
        }

        // ViewModel → Entity
        public static SimpleProduct ToSimpleProductEntity(this CreateSimpleProductViewModel viewModel)
        {
            var product = new SimpleProduct
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Code = viewModel.Code,
                UnitPrice = viewModel.UnitPrice,
                Cost = viewModel.Cost,
                CategoryId = viewModel.CategoryId,
                SupplierId = viewModel.SupplierId,
                MinStock = viewModel.MinStock,
                CurrentStock = viewModel.CurrentStock,
                Unit = viewModel.Unit,
                AllowCustomization = viewModel.AllowCustomization,
                AssemblyTime = viewModel.AssemblyTime,
                AssemblyInstructions = viewModel.AssemblyInstructions
            };

            if (viewModel.IsActive)
                product.Activate();
            else
                product.Deactivate();

            return product;
        }

        public static SimpleProduct ToSimpleProductEntity(this EditSimpleProductViewModel viewModel)
        {
            var product = new SimpleProduct
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Code = viewModel.Code,
                UnitPrice = viewModel.UnitPrice,
                Cost = viewModel.Cost,
                CategoryId = viewModel.CategoryId,
                SupplierId = viewModel.SupplierId,
                MinStock = viewModel.MinStock,
                CurrentStock = viewModel.CurrentStock,
                Unit = viewModel.Unit,
                AllowCustomization = viewModel.AllowCustomization,
                AssemblyTime = viewModel.AssemblyTime,
                AssemblyInstructions = viewModel.AssemblyInstructions
            };

            if (viewModel.IsActive)
                product.Activate();
            else
                product.Deactivate();

            return product;
        }

        // ProductIngredient conversions
        public static ProductIngredientViewModel ToViewModel(this ProductIngredient productIngredient)
        {
            return new ProductIngredientViewModel
            {
                Id = productIngredient.Id,
                ProductId = productIngredient.ProductId,
                IngredientId = productIngredient.IngredientId,
                Quantity = productIngredient.Quantity,
                Unit = productIngredient.Unit,
                IsOptional = productIngredient.IsOptional,
                Notes = productIngredient.Notes,
                IngredientName = productIngredient.Ingredient?.Name ?? string.Empty,
                IngredientCostPerUnit = productIngredient.Ingredient?.CostPerUnit ?? 0,
                IngredientCurrentStock = productIngredient.Ingredient?.CurrentStock ?? 0,
                SupplierName = productIngredient.Ingredient?.Supplier?.Name ?? string.Empty
            };
        }

        public static ProductIngredient ToEntity(this ProductIngredientViewModel viewModel)
        {
            return new ProductIngredient
            {
                Id = viewModel.Id,
                ProductId = viewModel.ProductId,
                IngredientId = viewModel.IngredientId,
                Quantity = viewModel.Quantity,
                Unit = viewModel.Unit,
                IsOptional = viewModel.IsOptional,
                Notes = viewModel.Notes
            };
        }

        // Collections
        public static IEnumerable<ProductDetailsViewModel> ToDetailsViewModels(this IEnumerable<SimpleProduct> products)
        {
            return products.Select(p => p.ToDetailsViewModel());
        }

        public static IEnumerable<ProductAutocompleteViewModel> ToAutocompleteViewModels(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToAutocompleteViewModel());
        }

        public static IEnumerable<ProductIngredientViewModel> ToViewModels(this IEnumerable<ProductIngredient> productIngredients)
        {
            return productIngredients.Select(pi => pi.ToViewModel());
        }
    }
} 