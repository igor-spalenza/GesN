using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    /// <summary>
    /// ViewModel base para ProductIngredient
    /// </summary>
    public class ProductIngredientViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ingrediente é obrigatório")]
        [Display(Name = "Ingrediente")]
        public string IngredientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; } = ProductionUnit.Unidades;

        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; } = false;

        [Display(Name = "Observações")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Propriedades para exibição
        public string ProductName { get; set; } = string.Empty;
        public string IngredientName { get; set; } = string.Empty;
        public decimal IngredientCostPerUnit { get; set; }
        public decimal IngredientCurrentStock { get; set; }
        public string UnitDisplay => GetUnitDisplay();

        // Propriedades calculadas
        public decimal TotalCost => Quantity * IngredientCostPerUnit;
        public string TotalCostDisplay => TotalCost.ToString("C2");
        public string QuantityDisplay => $"{Quantity:N2} {UnitDisplay}";
        public bool HasSufficientStock => IngredientCurrentStock >= Quantity;
        public string StockStatus => HasSufficientStock ? "Disponível" : "Insuficiente";

        private string GetUnitDisplay()
        {
            return Unit switch
            {
                ProductionUnit.Unidades => "UN",
                ProductionUnit.Quilogramas => "KG",
                ProductionUnit.Gramas => "G",
                ProductionUnit.Litros => "L",
                ProductionUnit.Mililitros => "ML",
                _ => Unit.ToString()
            };
        }
    }

    /// <summary>
    /// ViewModel para criação de ProductIngredient
    /// </summary>
    public class CreateProductIngredientViewModel : ProductIngredientViewModel
    {
        public List<SelectListItem> AvailableIngredients { get; set; } = new();
        public List<SelectListItem> AvailableUnits { get; set; } = new();

        public CreateProductIngredientViewModel()
        {
            AvailableUnits = GetUnitsSelectList();
        }

        private List<SelectListItem> GetUnitsSelectList()
        {
            return Enum.GetValues<ProductionUnit>()
                .Select(u => new SelectListItem
                {
                    Value = ((int)u).ToString(),
                    Text = GetUnitDisplayName(u)
                }).ToList();
        }

        private string GetUnitDisplayName(ProductionUnit unit)
        {
            return unit switch
            {
                ProductionUnit.Unidades => "Unidades (UN)",
                ProductionUnit.Quilogramas => "Quilogramas (KG)",
                ProductionUnit.Gramas => "Gramas (G)",
                ProductionUnit.Litros => "Litros (L)",
                ProductionUnit.Mililitros => "Mililitros (ML)",
                _ => unit.ToString()
            };
        }
    }

    /// <summary>
    /// ViewModel para edição de ProductIngredient
    /// </summary>
    public class EditProductIngredientViewModel : CreateProductIngredientViewModel
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";
    }

    /// <summary>
    /// ViewModel para detalhes de ProductIngredient
    /// </summary>
    public class ProductIngredientDetailsViewModel : ProductIngredientViewModel
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";

        // Informações detalhadas do ingrediente
        public string IngredientDescription { get; set; } = string.Empty;
        public string IngredientSupplier { get; set; } = string.Empty;
        public DateTime? IngredientExpiryDate { get; set; }
        public string IngredientExpiryDisplay => IngredientExpiryDate?.ToString("dd/MM/yyyy") ?? "Não informado";
        
        // Status e alertas
        public bool IsExpiringSoon => IngredientExpiryDate.HasValue && 
                                     IngredientExpiryDate.Value <= DateTime.Now.AddDays(30);
        public bool IsExpired => IngredientExpiryDate.HasValue && 
                                IngredientExpiryDate.Value <= DateTime.Now;
        public string ExpiryStatus
        {
            get
            {
                if (IsExpired) return "Vencido";
                if (IsExpiringSoon) return "Vencendo em breve";
                return "Dentro da validade";
            }
        }
    }

    /// <summary>
    /// ViewModel para listagem de ingredientes de um produto
    /// </summary>
    public class ProductIngredientsIndexViewModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public List<ProductIngredientViewModel> Ingredients { get; set; } = new();
        public ProductIngredientsStatisticsViewModel Statistics { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para estatísticas de ingredientes
    /// </summary>
    public class ProductIngredientsStatisticsViewModel
    {
        public int TotalIngredients { get; set; }
        public int OptionalIngredients { get; set; }
        public int RequiredIngredients { get; set; }
        public decimal TotalCost { get; set; }
        public int IngredientsWithSufficientStock { get; set; }
        public int IngredientsWithInsufficientStock { get; set; }
        
        public string TotalCostDisplay => TotalCost.ToString("C2");
        public bool HasInsufficientStock => IngredientsWithInsufficientStock > 0;
        public decimal StockAvailabilityPercentage => TotalIngredients > 0 
            ? (decimal)IngredientsWithSufficientStock / TotalIngredients * 100 
            : 0;
    }

    /// <summary>
    /// ViewModel para busca e seleção de ingredientes
    /// </summary>
    public class IngredientSelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CostPerUnit { get; set; }
        public decimal CurrentStock { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public string CostDisplay => CostPerUnit.ToString("C2");
        public string StockDisplay => $"{CurrentStock:N2} {Unit}";
    }
    /// <summary>
    /// Extensões para conversão entre Entity e ViewModel
    /// </summary>
    public static class ProductIngredientMappingExtensions
    {
        public static ProductIngredientViewModel ToViewModel(this ProductIngredient entity)
        {
            return new ProductIngredientViewModel
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                IngredientId = entity.IngredientId,
                Quantity = entity.Quantity,
                Unit = entity.Unit,
                IsOptional = entity.IsOptional,
                Notes = entity.Notes,
                ProductName = entity.Product?.Name ?? string.Empty,
                IngredientName = entity.Ingredient?.Name ?? string.Empty,
                IngredientCostPerUnit = entity.Ingredient?.CostPerUnit ?? 0,
                IngredientCurrentStock = entity.Ingredient?.CurrentStock ?? 0
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

        public static EditProductIngredientViewModel ToEditViewModel(this ProductIngredient entity)
        {
            return new EditProductIngredientViewModel
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                IngredientId = entity.IngredientId,
                Quantity = entity.Quantity,
                Unit = entity.Unit,
                IsOptional = entity.IsOptional,
                Notes = entity.Notes,
                ProductName = entity.Product?.Name ?? string.Empty,
                IngredientName = entity.Ingredient?.Name ?? string.Empty,
                IngredientCostPerUnit = entity.Ingredient?.CostPerUnit ?? 0,
                IngredientCurrentStock = entity.Ingredient?.CurrentStock ?? 0,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.LastModifiedAt
            };
        }

        public static ProductIngredientDetailsViewModel ToDetailsViewModel(this ProductIngredient entity)
        {
            return new ProductIngredientDetailsViewModel
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                IngredientId = entity.IngredientId,
                Quantity = entity.Quantity,
                Unit = entity.Unit,
                IsOptional = entity.IsOptional,
                Notes = entity.Notes,
                ProductName = entity.Product?.Name ?? string.Empty,
                IngredientName = entity.Ingredient?.Name ?? string.Empty,
                IngredientCostPerUnit = entity.Ingredient?.CostPerUnit ?? 0,
                IngredientCurrentStock = entity.Ingredient?.CurrentStock ?? 0,
                CreatedAt = entity.CreatedAt,
                ModifiedAt = entity.LastModifiedAt,
                IngredientDescription = entity.Ingredient?.Description ?? string.Empty,
                IngredientSupplier = entity.Ingredient?.Supplier?.Name ?? string.Empty,
                IngredientExpiryDate = null // Ingredient não tem ExpiryDate na entidade atual
            };
        }

        public static List<ProductIngredientViewModel> ToViewModels(this IEnumerable<ProductIngredient> entities)
        {
            return entities.Select(e => e.ToViewModel()).ToList();
        }
    }
} 