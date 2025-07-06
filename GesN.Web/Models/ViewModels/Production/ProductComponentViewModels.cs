using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;

namespace GesN.Web.Models.ViewModels.Production
{
    public class ProductComponentViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Produto Composto")]
        public string CompositeProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto Composto")]
        public string? CompositeProductName { get; set; }

        [Display(Name = "Produto Componente")]
        public string ComponentProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Componente")]
        public string? ComponentProductName { get; set; }

        [Display(Name = "SKU do Componente")]
        public string? ComponentProductSKU { get; set; }

        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; }

        [Display(Name = "Unidade")]
        public string? Unit { get; set; }

        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; }

        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; }

        [Display(Name = "Observações")]
        public string? Notes { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Custo Total")]
        public decimal TotalCost { get; set; }

        // Propriedades calculadas
        [Display(Name = "Tipo")]
        public string OptionalDisplay => IsOptional ? "Opcional" : "Obrigatório";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductComponentViewModel
    {
        [Required(ErrorMessage = "O produto composto é obrigatório")]
        [Display(Name = "Produto Composto")]
        public string CompositeProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O componente é obrigatório")]
        [Display(Name = "Componente")]
        public string ComponentProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; } = 1;

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public string Unit { get; set; } = "Unidades";

        [Display(Name = "Componente Opcional")]
        public bool IsOptional { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A ordem deve ser maior ou igual a zero")]
        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        [Display(Name = "Observações")]
        public string? Notes { get; set; }

        [Display(Name = "Componentes Disponíveis")]
        public List<ComponentSelectionViewModel> AvailableComponents { get; set; } = new();
    }

    public class EditProductComponentViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto composto é obrigatório")]
        [Display(Name = "Produto Composto")]
        public string CompositeProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto Composto")]
        public string? CompositeProductName { get; set; }

        [Required(ErrorMessage = "O componente é obrigatório")]
        [Display(Name = "Componente")]
        public string ComponentProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public decimal Quantity { get; set; } = 1;

        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public string Unit { get; set; } = "Unidades";

        [Display(Name = "Componente Opcional")]
        public bool IsOptional { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A ordem deve ser maior ou igual a zero")]
        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        [Display(Name = "Observações")]
        public string? Notes { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }
        
        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }
        
        [Display(Name = "Criado Por")]
        public string? CreatedBy { get; set; }
        
        [Display(Name = "Estado")]
        public string? StateCode { get; set; }
        
        [Display(Name = "Produto Componente")]
        public string? ComponentProduct { get; set; }
        
        public decimal CalculateTotalCost()
        {
            return 0; // Placeholder - será implementado quando necessário
        }

        [Display(Name = "Componentes Disponíveis")]
        public List<ComponentSelectionViewModel> AvailableComponents { get; set; } = new();
    }

    public class ProductComponentDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string CompositeProductId { get; set; } = string.Empty;
        public string? CompositeProductName { get; set; }
        public string ComponentProductId { get; set; } = string.Empty;
        public string? ComponentProductName { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public bool IsOptional { get; set; }
        public int AssemblyOrder { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        public string OptionalDisplay => IsOptional ? "Opcional" : "Obrigatório";
        public string AssemblyOrderDisplay => AssemblyOrder.ToString();
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class ProductComponentIndexViewModel
    {
        public List<ProductComponentViewModel> Components { get; set; } = new();
        public string CompositeProductId { get; set; } = string.Empty;
        public string? CompositeProductName { get; set; }
        public ProductComponentStatisticsViewModel Statistics { get; set; } = new();
    }

    public class ProductComponentStatisticsViewModel
    {
        public int TotalComponents { get; set; }
        public int OptionalComponents { get; set; }
        public int RequiredComponents { get; set; }
        public decimal EstimatedTotalCost { get; set; }
    }

    public class ComponentSelectionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public decimal? Cost { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ProductComponentSearchViewModel
    {
        [Display(Name = "Produto Composto")]
        public string? CompositeProductId { get; set; }

        [Display(Name = "Componente")]
        public string? ComponentProductId { get; set; }

        [Display(Name = "Tipo")]
        public bool? IsOptional { get; set; }

        public List<OptionalSelectionViewModel> GetAvailableOptionalTypes()
        {
            return new List<OptionalSelectionViewModel>
            {
                new() { Value = null, Text = "Todos", IsSelected = true },
                new() { Value = false, Text = "Obrigatório", IsSelected = false },
                new() { Value = true, Text = "Opcional", IsSelected = false }
            };
        }
    }

    public class OptionalSelectionViewModel
    {
        public bool? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 