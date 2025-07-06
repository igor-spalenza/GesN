using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    // ProductGroupItem ViewModels
    public class ProductGroupItemViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto")]
        public string? ProductName { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantity { get; set; }

        [Display(Name = "Quantidade Mínima")]
        public int? MinQuantity { get; set; }

        [Display(Name = "Quantidade Máxima")]
        public int? MaxQuantity { get; set; }

        [Display(Name = "Quantidade Padrão")]
        public int? DefaultQuantity { get; set; }

        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; }

        [Display(Name = "Preço Extra")]
        [DataType(DataType.Currency)]
        public decimal? ExtraPrice { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Tipo")]
        public string OptionalDisplay => IsOptional ? "Opcional" : "Obrigatório";

        [Display(Name = "Preço Extra")]
        public string ExtraPriceDisplay => ExtraPrice?.ToString("C") ?? "Sem custo extra";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductGroupItemViewModel
    {
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Mínima")]
        public int? MinQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade máxima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Máxima")]
        public int? MaxQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade padrão deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Padrão")]
        public int? DefaultQuantity { get; set; }

        [Display(Name = "Item Opcional")]
        public bool IsOptional { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço extra deve ser maior ou igual a zero")]
        [Display(Name = "Preço Extra")]
        [DataType(DataType.Currency)]
        public decimal? ExtraPrice { get; set; }

        [Display(Name = "Produtos Disponíveis")]
        public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();
    }

    public class EditProductGroupItemViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Mínima")]
        public int? MinQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade máxima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Máxima")]
        public int? MaxQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade padrão deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Padrão")]
        public int? DefaultQuantity { get; set; }

        [Display(Name = "Item Opcional")]
        public bool IsOptional { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O preço extra deve ser maior ou igual a zero")]
        [Display(Name = "Preço Extra")]
        [DataType(DataType.Currency)]
        public decimal? ExtraPrice { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Produtos Disponíveis")]
        public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();
    }

    // ProductGroupOption ViewModels
    public class ProductGroupOptionViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Display(Name = "Tipo de Opção")]
        public string? OptionType { get; set; }

        [Display(Name = "Obrigatória")]
        public bool IsRequired { get; set; }

        [Display(Name = "Ordem de Exibição")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Tipo")]
        public string RequiredDisplay => IsRequired ? "Obrigatória" : "Opcional";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductGroupOptionViewModel
    {
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "O tipo de opção deve ter no máximo {1} caracteres")]
        [Display(Name = "Tipo de Opção")]
        public string? OptionType { get; set; }

        [Display(Name = "Opção Obrigatória")]
        public bool IsRequired { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A ordem deve ser maior que zero")]
        [Display(Name = "Ordem de Exibição")]
        public int? DisplayOrder { get; set; }
    }

    public class EditProductGroupOptionViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "O tipo de opção deve ter no máximo {1} caracteres")]
        [Display(Name = "Tipo de Opção")]
        public string? OptionType { get; set; }

        [Display(Name = "Opção Obrigatória")]
        public bool IsRequired { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A ordem deve ser maior que zero")]
        [Display(Name = "Ordem de Exibição")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }
    }

    // ProductGroupExchangeRule ViewModels
    public class ProductGroupExchangeRuleViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Display(Name = "Produto Original")]
        public string OriginalProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto Original")]
        public string? OriginalProductName { get; set; }

        [Display(Name = "Produto de Troca")]
        public string ExchangeProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto de Troca")]
        public string? ExchangeProductName { get; set; }

        [Display(Name = "Taxa de Troca")]
        public decimal ExchangeRatio { get; set; }

        [Display(Name = "Custo Adicional")]
        [DataType(DataType.Currency)]
        public decimal? AdditionalCost { get; set; }

        [Display(Name = "Ativa")]
        public bool IsActive { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Status")]
        public string ActiveDisplay => IsActive ? "Ativa" : "Inativa";

        [Display(Name = "Custo Adicional")]
        public string AdditionalCostDisplay => AdditionalCost?.ToString("C") ?? "Sem custo adicional";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Última Modificação")]
        public string FormattedModifiedAt => ModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CreateProductGroupExchangeRuleViewModel
    {
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto original é obrigatório")]
        [Display(Name = "Produto Original")]
        public string OriginalProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto de troca é obrigatório")]
        [Display(Name = "Produto de Troca")]
        public string ExchangeProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A taxa de troca é obrigatória")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A taxa de troca deve ser maior que zero")]
        [Display(Name = "Taxa de Troca")]
        public decimal ExchangeRatio { get; set; } = 1.0m;

        [Range(0, double.MaxValue, ErrorMessage = "O custo adicional deve ser maior ou igual a zero")]
        [Display(Name = "Custo Adicional")]
        [DataType(DataType.Currency)]
        public decimal? AdditionalCost { get; set; }

        [Display(Name = "Regra Ativa")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Produtos Disponíveis")]
        public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();
    }

    public class EditProductGroupExchangeRuleViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        [Display(Name = "Nome do Grupo")]
        public string? ProductGroupName { get; set; }

        [Required(ErrorMessage = "O produto original é obrigatório")]
        [Display(Name = "Produto Original")]
        public string OriginalProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto de troca é obrigatório")]
        [Display(Name = "Produto de Troca")]
        public string ExchangeProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A taxa de troca é obrigatória")]
        [Range(0.01, double.MaxValue, ErrorMessage = "A taxa de troca deve ser maior que zero")]
        [Display(Name = "Taxa de Troca")]
        public decimal ExchangeRatio { get; set; } = 1.0m;

        [Range(0, double.MaxValue, ErrorMessage = "O custo adicional deve ser maior ou igual a zero")]
        [Display(Name = "Custo Adicional")]
        [DataType(DataType.Currency)]
        public decimal? AdditionalCost { get; set; }

        [Display(Name = "Regra Ativa")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Data de Criação")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Produtos Disponíveis")]
        public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();
    }

    // Shared ViewModels
    public class ProductSelectionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Code { get; set; }
        public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ProductGroupManagementViewModel
    {
        public string ProductGroupId { get; set; } = string.Empty;
        public string? ProductGroupName { get; set; }
        public List<ProductGroupItemViewModel> Items { get; set; } = new();
        public List<ProductGroupOptionViewModel> Options { get; set; } = new();
        public List<ProductGroupExchangeRuleViewModel> ExchangeRules { get; set; } = new();
        public ProductGroupStatisticsViewModel Statistics { get; set; } = new();
    }

    public class ProductGroupStatisticsViewModel
    {
        public int TotalItems { get; set; }
        public int OptionalItems { get; set; }
        public int RequiredItems { get; set; }
        public int TotalOptions { get; set; }
        public int RequiredOptions { get; set; }
        public int TotalExchangeRules { get; set; }
        public int ActiveExchangeRules { get; set; }
        public decimal EstimatedBasePrice { get; set; }
    }

    // ProductGroup Main ViewModels
    public class ProductGroupViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Display(Name = "Preço Base")]
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }
        
        [Display(Name = "SKU")]
        public string? SKU { get; set; }
        
        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }
        
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        
        [Display(Name = "Estado")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;

        // Propriedades calculadas
        [Display(Name = "Status")]
        public string StatusDisplay => IsActive ? "Ativo" : "Inativo";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [Display(Name = "Última Modificação")]
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class ProductGroupIndexViewModel
    {
        public List<ProductGroupViewModel> ProductGroups { get; set; } = new();
        public List<ProductGroupViewModel> Groups { get; set; } = new();
        public ProductGroupStatisticsViewModel Statistics { get; set; } = new();
        public int TotalGroups { get; set; }
    }

    public class CreateProductGroupViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O preço base é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço base deve ser maior ou igual a zero")]
        [Display(Name = "Preço Base")]
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "SKU")]
        public string? SKU { get; set; }
        
        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }
        
        [Display(Name = "Preço")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        
        [Display(Name = "Estado")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;
    }

    public class EditProductGroupViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "O preço base é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço base deve ser maior ou igual a zero")]
        [Display(Name = "Preço Base")]
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Ativo")]
        public bool IsActive { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }
    }
} 