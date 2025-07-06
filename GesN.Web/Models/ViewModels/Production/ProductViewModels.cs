using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    public class ProductViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "SKU")]
        public string? SKU { get; set; } = string.Empty;

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
        public decimal Price { get; set; }

        [Display(Name = "Preço por Quantidade")]
        public int QuantityPrice { get; set; } = 0;

        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [Display(Name = "Imagem")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Observações")]
        public string? Note { get; set; }

        [Display(Name = "Tempo de Montagem (min)")]
        public int AssemblyTime { get; set; } = 0;

        [Display(Name = "Instruções de Montagem")]
        public string? AssemblyInstructions { get; set; }

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

        [Display(Name = "Preço")]
        public string PriceDisplay => Price.ToString("C");

        [Display(Name = "Preço Unitário")]
        public string UnitPriceDisplay => UnitPrice.ToString("C");

        [Display(Name = "Tempo de Montagem")]
        public string AssemblyTimeDisplay
        {
            get
            {
                if (AssemblyTime <= 0)
                    return "Sem montagem";
                
                var hours = AssemblyTime / 60;
                var minutes = AssemblyTime % 60;
                
                if (hours > 0)
                    return $"{hours}h {minutes}min";
                
                return $"{minutes}min";
            }
        }

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductViewModel
    {
        [StringLength(50, ErrorMessage = "O SKU deve ter no máximo {1} caracteres")]
        [Display(Name = "SKU")]
        public string? SKU { get; set; } = string.Empty;

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
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O preço por quantidade deve ser maior ou igual a zero")]
        [Display(Name = "Preço por Quantidade")]
        public int QuantityPrice { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "O preço unitário deve ser maior ou igual a zero")]
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [Display(Name = "URL da Imagem")]
        public string? ImageUrl { get; set; }

        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        [Display(Name = "Observações")]
        public string? Note { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        [Display(Name = "Tempo de Montagem (min)")]
        public int AssemblyTime { get; set; } = 0;

        [StringLength(2000, ErrorMessage = "As instruções de montagem devem ter no máximo {1} caracteres")]
        [Display(Name = "Instruções de Montagem")]
        public string? AssemblyInstructions { get; set; }

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

        [StringLength(50, ErrorMessage = "O SKU deve ter no máximo {1} caracteres")]
        [Display(Name = "SKU")]
        public string? SKU { get; set; } = string.Empty;

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

        [Display(Name = "Nome da Categoria")]
        public string? Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O preço por quantidade deve ser maior ou igual a zero")]
        [Display(Name = "Preço por Quantidade")]
        public int QuantityPrice { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "O preço unitário deve ser maior ou igual a zero")]
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        [Display(Name = "Custo")]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [Display(Name = "URL da Imagem")]
        public string? ImageUrl { get; set; }

        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        [Display(Name = "Observações")]
        public string? Note { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        [Display(Name = "Tempo de Montagem (min)")]
        public int AssemblyTime { get; set; } = 0;

        [StringLength(2000, ErrorMessage = "As instruções de montagem devem ter no máximo {1} caracteres")]
        [Display(Name = "Instruções de Montagem")]
        public string? AssemblyInstructions { get; set; }

        [Display(Name = "Status")]
        public int StateCode { get; set; } = 1;

        [Display(Name = "Ativo")]
        public bool IsActive 
        { 
            get => StateCode == 1; 
            set => StateCode = value ? 1 : 0; 
        }

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
                if (Cost <= 0 || Price <= 0) return "N/A";
                var margin = ((Price - Cost) / Cost) * 100;
                return $"{margin:F1}%";
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
        public string? SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProductType ProductType { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public int QuantityPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Cost { get; set; }
        public string? ImageUrl { get; set; }
        public string? Note { get; set; }
        public int AssemblyTime { get; set; }
        public string? AssemblyInstructions { get; set; }
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
        public string PriceDisplay => Price.ToString("C");
        public string UnitPriceDisplay => UnitPrice.ToString("C");
        public string CostDisplay => Cost.ToString("C");
        public string AssemblyTimeDisplay
        {
            get
            {
                if (AssemblyTime <= 0)
                    return "Sem montagem";
                
                var hours = AssemblyTime / 60;
                var minutes = AssemblyTime % 60;
                
                if (hours > 0)
                    return $"{hours}h {minutes}min";
                
                return $"{minutes}min";
            }
        }
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
                new() { Value = null, Text = "Todos os tipos", IsSelected = !ProductType.HasValue },
                new() { Value = Models.Enumerators.ProductType.Simple, Text = "Produto Simples", IsSelected = ProductType == Models.Enumerators.ProductType.Simple },
                new() { Value = Models.Enumerators.ProductType.Composite, Text = "Produto Composto", IsSelected = ProductType == Models.Enumerators.ProductType.Composite },
                new() { Value = Models.Enumerators.ProductType.Group, Text = "Grupo de Produtos", IsSelected = ProductType == Models.Enumerators.ProductType.Group }
            };
        }

        public List<StateSelectionViewModel> GetAvailableStates()
        {
            return new List<StateSelectionViewModel>
            {
                new() { Value = null, Text = "Todos os status", IsSelected = !StateCode.HasValue },
                new() { Value = 1, Text = "Ativo", IsSelected = StateCode == 1 },
                new() { Value = 0, Text = "Inativo", IsSelected = StateCode == 0 }
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

    /// <summary>
    /// ViewModel expandido para edição de produtos que inclui dados de ProductGroup
    /// para renderização direta das partial views sem necessidade de AJAX
    /// </summary>
    public class EditProductWithGroupsViewModel : EditProductViewModel
    {
        [Display(Name = "Itens do Grupo")]
        public List<ProductGroupItemViewModel> GroupItems { get; set; } = new();

        [Display(Name = "Opções do Grupo")]
        public List<ProductGroupOptionViewModel> GroupOptions { get; set; } = new();

        [Display(Name = "Regras de Troca")]
        public List<ProductGroupExchangeRuleViewModel> ExchangeRules { get; set; } = new();

        [Display(Name = "Componentes")]
        public ProductComponentIndexViewModel? ComponentIndexViewModel { get; set; }

        /// <summary>
        /// Indica se o produto é do tipo Group e deve exibir as abas relacionadas
        /// </summary>
        public bool IsGroup => ProductType == ProductType.Group;

        /// <summary>
        /// Indica se o produto é do tipo Composite e deve exibir a aba de componentes
        /// </summary>
        public bool IsComposite => ProductType == ProductType.Composite;
    }
} 