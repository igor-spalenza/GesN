using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    // ViewModels para Listagem e Index
    public class IngredientIndexViewModel
    {
        public IEnumerable<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public IngredientStatisticsViewModel Statistics { get; set; } = new();
        public IngredientSearchViewModel Search { get; set; } = new();
    }

    public class IngredientStatisticsViewModel
    {
        public int TotalIngredients { get; set; }
        public int ActiveIngredients { get; set; }
        public int InactiveIngredients { get; set; }
        public int LowStockIngredients { get; set; }
        public int PerishableIngredients { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class IngredientSearchViewModel
    {
        public string SearchTerm { get; set; } = string.Empty;
        public bool ShowInactive { get; set; } = false;
        public bool ShowLowStock { get; set; } = false;
        public bool ShowPerishable { get; set; } = false;
        public string? SupplierId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ViewModels para CRUD
    public class CreateIngredientViewModel
    {
        [Required(ErrorMessage = "O nome do ingrediente é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; }

        [Required(ErrorMessage = "O custo por unidade é obrigatório")]
        [Display(Name = "Custo por Unidade")]
        [Range(0.01, 999999.99, ErrorMessage = "O custo deve estar entre 0,01 e 999.999,99")]
        public decimal CostPerUnit { get; set; }

        [Display(Name = "Fornecedor")]
        public string? SupplierId { get; set; }

        [Display(Name = "Estoque Mínimo")]
        [Range(0, 999999.99, ErrorMessage = "O estoque mínimo deve estar entre 0 e 999.999,99")]
        public decimal MinStock { get; set; } = 0;

        [Display(Name = "Estoque Atual")]
        [Range(0, 999999.99, ErrorMessage = "O estoque atual deve estar entre 0 e 999.999,99")]
        public decimal CurrentStock { get; set; } = 0;

        [Display(Name = "Dias para Vencimento")]
        [Range(0, 9999, ErrorMessage = "Os dias para vencimento devem estar entre 0 e 9999")]
        public int? ExpirationDays { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;

        // Propriedades auxiliares para Autocomplete
        public string SupplierName { get; set; } = string.Empty;
    }

    public class EditIngredientViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do ingrediente é obrigatório")]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; }

        [Required(ErrorMessage = "O custo por unidade é obrigatório")]
        [Display(Name = "Custo por Unidade")]
        [Range(0.01, 999999.99, ErrorMessage = "O custo deve estar entre 0,01 e 999.999,99")]
        public decimal CostPerUnit { get; set; }

        [Display(Name = "Fornecedor")]
        public string? SupplierId { get; set; }

        [Display(Name = "Estoque Mínimo")]
        [Range(0, 999999.99, ErrorMessage = "O estoque mínimo deve estar entre 0 e 999.999,99")]
        public decimal MinStock { get; set; }

        [Display(Name = "Estoque Atual")]
        [Range(0, 999999.99, ErrorMessage = "O estoque atual deve estar entre 0 e 999.999,99")]
        public decimal CurrentStock { get; set; }

        [Display(Name = "Dias para Vencimento")]
        [Range(0, 9999, ErrorMessage = "Os dias para vencimento devem estar entre 0 e 9999")]
        public int? ExpirationDays { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Propriedades auxiliares
        public string SupplierName { get; set; } = string.Empty;
    }

    public class IngredientDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductionUnit Unit { get; set; }
        public decimal CostPerUnit { get; set; }
        public string? SupplierId { get; set; }
        public decimal MinStock { get; set; }
        public decimal CurrentStock { get; set; }
        public int? ExpirationDays { get; set; }
        public bool IsActive { get; set; }

        // Metadados
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Propriedades relacionais
        public string SupplierName { get; set; } = string.Empty;

        // Propriedades calculadas
        public bool IsLowStock => CurrentStock <= MinStock;
        public bool IsPerishable => ExpirationDays.HasValue && ExpirationDays > 0;
    }

    // ViewModel para Autocomplete
    public class IngredientAutocompleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ProductionUnit Unit { get; set; }
        public decimal CostPerUnit { get; set; }
        public decimal CurrentStock { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    // ViewModel para atualização de estoque
    public class UpdateIngredientStockViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentStock { get; set; }
        
        [Required(ErrorMessage = "O novo estoque é obrigatório")]
        [Display(Name = "Novo Estoque")]
        [Range(0, 999999.99, ErrorMessage = "O estoque deve estar entre 0 e 999.999,99")]
        public decimal NewStock { get; set; }
    }
}

// Extension Methods para conversão
namespace GesN.Web.Models.ViewModels.Production
{
    public static class IngredientMappingExtensions
    {
        // Entity → ViewModel
        public static IngredientDetailsViewModel ToDetailsViewModel(this Ingredient ingredient)
        {
            return new IngredientDetailsViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                Unit = ingredient.Unit,
                CostPerUnit = ingredient.CostPerUnit,
                SupplierId = ingredient.SupplierId,
                MinStock = ingredient.MinStock,
                CurrentStock = ingredient.CurrentStock,
                ExpirationDays = ingredient.ExpirationDays,
                IsActive = ingredient.IsActive,
                CreatedAt = ingredient.CreatedAt,
                CreatedBy = ingredient.CreatedBy,
                LastModifiedAt = ingredient.LastModifiedAt,
                LastModifiedBy = ingredient.LastModifiedBy,
                SupplierName = ingredient.Supplier?.Name ?? string.Empty
            };
        }

        public static EditIngredientViewModel ToEditViewModel(this Ingredient ingredient)
        {
            return new EditIngredientViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Description = ingredient.Description,
                Unit = ingredient.Unit,
                CostPerUnit = ingredient.CostPerUnit,
                SupplierId = ingredient.SupplierId,
                MinStock = ingredient.MinStock,
                CurrentStock = ingredient.CurrentStock,
                ExpirationDays = ingredient.ExpirationDays,
                IsActive = ingredient.IsActive,
                CreatedAt = ingredient.CreatedAt,
                CreatedBy = ingredient.CreatedBy,
                LastModifiedAt = ingredient.LastModifiedAt,
                LastModifiedBy = ingredient.LastModifiedBy,
                SupplierName = ingredient.Supplier?.Name ?? string.Empty
            };
        }

        public static IngredientAutocompleteViewModel ToAutocompleteViewModel(this Ingredient ingredient)
        {
            return new IngredientAutocompleteViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Unit = ingredient.Unit,
                CostPerUnit = ingredient.CostPerUnit,
                CurrentStock = ingredient.CurrentStock,
                SupplierName = ingredient.Supplier?.Name ?? string.Empty
            };
        }

        public static UpdateIngredientStockViewModel ToUpdateStockViewModel(this Ingredient ingredient)
        {
            return new UpdateIngredientStockViewModel
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                CurrentStock = ingredient.CurrentStock,
                NewStock = ingredient.CurrentStock
            };
        }

        // ViewModel → Entity
        public static Ingredient ToEntity(this CreateIngredientViewModel viewModel)
        {
            var ingredient = new Ingredient
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Unit = viewModel.Unit,
                CostPerUnit = viewModel.CostPerUnit,
                SupplierId = viewModel.SupplierId,
                MinStock = viewModel.MinStock,
                CurrentStock = viewModel.CurrentStock,
                ExpirationDays = viewModel.ExpirationDays
            };

            if (viewModel.IsActive)
                ingredient.Activate();
            else
                ingredient.Deactivate();

            return ingredient;
        }

        public static Ingredient ToEntity(this EditIngredientViewModel viewModel)
        {
            var ingredient = new Ingredient
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                Unit = viewModel.Unit,
                CostPerUnit = viewModel.CostPerUnit,
                SupplierId = viewModel.SupplierId,
                MinStock = viewModel.MinStock,
                CurrentStock = viewModel.CurrentStock,
                ExpirationDays = viewModel.ExpirationDays
            };

            if (viewModel.IsActive)
                ingredient.Activate();
            else
                ingredient.Deactivate();

            return ingredient;
        }

        // Collections
        public static IEnumerable<IngredientDetailsViewModel> ToDetailsViewModels(this IEnumerable<Ingredient> ingredients)
        {
            return ingredients.Select(i => i.ToDetailsViewModel());
        }

        public static IEnumerable<IngredientAutocompleteViewModel> ToAutocompleteViewModels(this IEnumerable<Ingredient> ingredients)
        {
            return ingredients.Select(i => i.ToAutocompleteViewModel());
        }
    }
} 