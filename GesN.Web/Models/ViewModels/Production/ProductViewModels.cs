using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    public class ProductViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Display(Name = "Tipo")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        [Display(Name = "Nome da Categoria")]
        public string? CategoryName { get; set; }

        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal? Cost { get; set; }

        [Display(Name = "Unidade")]
        public string? Unit { get; set; }

        [Display(Name = "Estoque Atual")]
        public int? CurrentStock { get; set; }

        [Display(Name = "Estoque Mínimo")]
        public int? MinStock { get; set; }

        [Display(Name = "Tempo de Montagem (min)")]
        public int? AssemblyTime { get; set; }

        [Display(Name = "Min. Itens Obrigatórios")]
        public int? MinItemsRequired { get; set; }

        [Display(Name = "Max. Itens Permitidos")]
        public int? MaxItemsAllowed { get; set; }

        [Display(Name = "Status")]
        public int StateCode { get; set; } = 1;

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Tipo")]
        public string ProductTypeDisplay => ProductType switch
        {
            ProductType.Simple => "Produto Simples",
            ProductType.Composite => "Produto Composto",
            ProductType.Group => "Grupo de Produtos",
            _ => ProductType.ToString()
        };

        [Display(Name = "Status")]
        public string StateCodeDisplay => StateCode == 1 ? "Ativo" : "Inativo";

        [Display(Name = "Estoque")]
        public string StockDisplay => CurrentStock?.ToString() ?? "N/A";

        [Display(Name = "Preço")]
        public string PriceDisplay => Price?.ToString("C") ?? "N/A";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo {1} caracteres")]
        [Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O tipo de produto é obrigatório")]
        [Display(Name = "Tipo de Produto")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal? Cost { get; set; }

        [StringLength(20, ErrorMessage = "A unidade deve ter no máximo {1} caracteres")]
        [Display(Name = "Unidade")]
        public string? Unit { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
        [Display(Name = "Estoque Atual")]
        public int? CurrentStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
        [Display(Name = "Estoque Mínimo")]
        public int? MinStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        [Display(Name = "Tempo de Montagem (min)")]
        public int? AssemblyTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O mínimo de itens deve ser maior ou igual a zero")]
        [Display(Name = "Min. Itens Obrigatórios")]
        public int? MinItemsRequired { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O máximo de itens deve ser maior ou igual a zero")]
        [Display(Name = "Max. Itens Permitidos")]
        public int? MaxItemsAllowed { get; set; }

        [Display(Name = "Tipos de Produto Disponíveis")]
        public List<ProductTypeSelectionViewModel> AvailableProductTypes { get; set; } = new()
        {
            new() { Value = Models.Enumerators.ProductType.Simple, Text = "Produto Simples", IsSelected = true },
            new() { Value = Models.Enumerators.ProductType.Composite, Text = "Produto Composto", IsSelected = false },
            new() { Value = Models.Enumerators.ProductType.Group, Text = "Grupo de Produtos", IsSelected = false }
        };

        [Display(Name = "Categorias Disponíveis")]
        public List<CategorySelectionViewModel> AvailableCategories { get; set; } = new();
    }

    public class EditProductViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O código é obrigatório")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo {1} caracteres")]
        [Display(Name = "Código")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O tipo de produto é obrigatório")]
        [Display(Name = "Tipo de Produto")]
        public ProductType ProductType { get; set; }

        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal? Cost { get; set; }

        [StringLength(20, ErrorMessage = "A unidade deve ter no máximo {1} caracteres")]
        [Display(Name = "Unidade")]
        public string? Unit { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
        [Display(Name = "Estoque Atual")]
        public int? CurrentStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
        [Display(Name = "Estoque Mínimo")]
        public int? MinStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        [Display(Name = "Tempo de Montagem (min)")]
        public int? AssemblyTime { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O mínimo de itens deve ser maior ou igual a zero")]
        [Display(Name = "Min. Itens Obrigatórios")]
        public int? MinItemsRequired { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O máximo de itens deve ser maior ou igual a zero")]
        [Display(Name = "Max. Itens Permitidos")]
        public int? MaxItemsAllowed { get; set; }

        [Display(Name = "Status")]
        public int StateCode { get; set; } = 1;

        [Display(Name = "Ativo")]
        public bool IsActive => StateCode == 1;

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        public string ProductTypeDisplay => ProductType switch
        {
            ProductType.Simple => "Produto Simples",
            ProductType.Composite => "Produto Composto",
            ProductType.Group => "Grupo de Produtos",
            _ => ProductType.ToString()
        };

        public string ProfitMarginDisplay
        {
            get
            {
                if (Price.HasValue && Cost.HasValue && Cost.Value > 0)
                {
                    var margin = ((Price.Value - Cost.Value) / Cost.Value) * 100;
                    return $"{margin:F1}%";
                }
                return "N/A";
            }
        }

        [Display(Name = "Tipos de Produto Disponíveis")]
        public List<ProductTypeSelectionViewModel> AvailableProductTypes { get; set; } = new()
        {
            new() { Value = Models.Enumerators.ProductType.Simple, Text = "Produto Simples", IsSelected = false },
            new() { Value = Models.Enumerators.ProductType.Composite, Text = "Produto Composto", IsSelected = false },
            new() { Value = Models.Enumerators.ProductType.Group, Text = "Grupo de Produtos", IsSelected = false }
        };

        [Display(Name = "Categorias Disponíveis")]
        public List<CategorySelectionViewModel> AvailableCategories { get; set; } = new();
    }

    public class ProductDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductType ProductType { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal? Price { get; set; }
        public decimal? Cost { get; set; }
        public string? Unit { get; set; }
        public int? CurrentStock { get; set; }
        public int? MinStock { get; set; }
        public int? AssemblyTime { get; set; }
        public int? MinItemsRequired { get; set; }
        public int? MaxItemsAllowed { get; set; }
        public int StateCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        public string ProductTypeDisplay => ProductType switch
        {
            ProductType.Simple => "Produto Simples",
            ProductType.Composite => "Produto Composto",
            ProductType.Group => "Grupo de Produtos",
            _ => ProductType.ToString()
        };

        public string StateCodeDisplay => StateCode == 1 ? "Ativo" : "Inativo";
        public string StockDisplay => CurrentStock?.ToString() ?? "N/A";
        public string PriceDisplay => Price?.ToString("C") ?? "N/A";
        public string CostDisplay => Cost?.ToString("C") ?? "N/A";
        public string AssemblyTimeDisplay => AssemblyTime?.ToString() + " min" ?? "N/A";
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class ProductIndexViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public ProductStatisticsViewModel Statistics { get; set; } = new();
        public ProductSearchViewModel Search { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
    }

    public class ProductStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int SimpleProducts { get; set; }
        public int CompositeProducts { get; set; }
        public int GroupProducts { get; set; }
        public int ProductGroups { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int NewProductsThisMonth { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.Now;
    }

    public class ProductSearchViewModel
    {
        [Display(Name = "Termo de Busca")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Tipo")]
        public ProductType? ProductType { get; set; }

        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        [Display(Name = "Status")]
        public int? StateCode { get; set; }

        public List<ProductTypeSelectionViewModel> GetAvailableProductTypes()
        {
            return new List<ProductTypeSelectionViewModel>
            {
                new() { Value = null, Text = "Todos os Tipos", IsSelected = true },
                new() { Value = Models.Enumerators.ProductType.Simple, Text = "Produto Simples", IsSelected = false },
                new() { Value = Models.Enumerators.ProductType.Composite, Text = "Produto Composto", IsSelected = false },
                new() { Value = Models.Enumerators.ProductType.Group, Text = "Grupo de Produtos", IsSelected = false }
            };
        }

        public List<StateSelectionViewModel> GetAvailableStates()
        {
            return new List<StateSelectionViewModel>
            {
                new() { Value = null, Text = "Todos os Status", IsSelected = true },
                new() { Value = 1, Text = "Ativo", IsSelected = false },
                new() { Value = 0, Text = "Inativo", IsSelected = false }
            };
        }
    }

    public class ProductTypeSelectionViewModel
    {
        public ProductType? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }

        public static List<ProductTypeSelectionViewModel> GetProductTypes()
        {
            return new List<ProductTypeSelectionViewModel>
            {
                new() { Value = Models.Enumerators.ProductType.Simple, Text = "Produto Simples", IsSelected = false },
                new() { Value = Models.Enumerators.ProductType.Composite, Text = "Produto Composto", IsSelected = false },
                new() { Value = Models.Enumerators.ProductType.Group, Text = "Grupo de Produtos", IsSelected = false }
            };
        }
    }

    public class CategorySelectionViewModel
    {
        public string? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class StateSelectionViewModel
    {
        public int? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 