using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    /// <summary>
    /// ViewModel principal para exibição de ProductComponent
    /// </summary>
    public class ProductComponentViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        [Display(Name = "Nome da Hierarquia")]
        public string? ProductComponentHierarchyName { get; set; }

        [Display(Name = "Custo Adicional")]
        public decimal AdditionalCost { get; set; }

        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; }

        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Criado por")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Última modificação")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Modificado por")]
        public string? LastModifiedBy { get; set; }

        // Propriedades calculadas
        [Display(Name = "Status")]
        public string StatusDisplay => StateCode switch
        {
            ObjectState.Active => "✅ Ativo",
            ObjectState.Inactive => "❌ Inativo",
            _ => "❓ Indefinido"
        };

        [Display(Name = "Custo Adicional")]
        public string FormattedAdditionalCost => AdditionalCost.ToString("C2");

        [Display(Name = "Criado em")]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [Display(Name = "Última modificação")]
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Resumo")]
        public string Summary
        {
            get
            {
                var summary = Name;
                if (!string.IsNullOrWhiteSpace(Description))
                    summary += $" - {Description}";
                if (AdditionalCost > 0)
                    summary += $" (Custo adicional: {FormattedAdditionalCost})";
                return summary;
            }
        }
    }

    /// <summary>
    /// ViewModel para criação de ProductComponent
    /// </summary>
    public class CreateProductComponentViewModel
    {
        [Required(ErrorMessage = "O nome do componente é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A hierarquia de componentes é obrigatória")]
        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        [Display(Name = "Nome da Hierarquia")]
        public string? ProductComponentHierarchyName { get; set; }

        [Display(Name = "Custo Adicional")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo adicional deve ser maior ou igual a zero")]
        public decimal AdditionalCost { get; set; } = 0;

        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;

        // Lista para popular dropdown/autocomplete
        [Display(Name = "Hierarquias Disponíveis")]
        public List<HierarchySelectionViewModel> AvailableHierarchies { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para edição de ProductComponent
    /// </summary>
    public class EditProductComponentViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome do componente é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Descrição")]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A hierarquia de componentes é obrigatória")]
        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        [Display(Name = "Nome da Hierarquia")]
        public string? ProductComponentHierarchyName { get; set; }

        [Display(Name = "Custo Adicional")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo adicional deve ser maior ou igual a zero")]
        public decimal AdditionalCost { get; set; } = 0;

        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;

        // Informações de auditoria (somente leitura)
        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Criado por")]
        public string CreatedBy { get; set; } = string.Empty;

        [Display(Name = "Última modificação")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Modificado por")]
        public string? LastModifiedBy { get; set; }

        // Lista para popular dropdown/autocomplete
        [Display(Name = "Hierarquias Disponíveis")]
        public List<HierarchySelectionViewModel> AvailableHierarchies { get; set; } = new();

        // Propriedades calculadas
        [Display(Name = "Criado em")]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [Display(Name = "Última modificação")]
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    /// <summary>
    /// ViewModel para detalhes de ProductComponent
    /// </summary>
    public class ProductComponentDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ProductComponentHierarchyId { get; set; } = string.Empty;
        public string? ProductComponentHierarchyName { get; set; }
        public decimal AdditionalCost { get; set; }
        public ObjectState StateCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }

        // Propriedades calculadas
        public string StatusDisplay => StateCode switch
        {
            ObjectState.Active => "✅ Ativo",
            ObjectState.Inactive => "❌ Inativo",
            _ => "❓ Indefinido"
        };

        public string FormattedAdditionalCost => AdditionalCost.ToString("C2");
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        public string Summary
        {
            get
            {
                var summary = Name;
                if (!string.IsNullOrWhiteSpace(Description))
                    summary += $" - {Description}";
                if (AdditionalCost > 0)
                    summary += $" (Custo adicional: {FormattedAdditionalCost})";
                return summary;
            }
        }
    }

    /// <summary>
    /// ViewModel para listagem de ProductComponent
    /// </summary>
    public class ProductComponentIndexViewModel
    {
        public List<ProductComponentViewModel> Components { get; set; } = new();
        public ProductComponentStatisticsViewModel Statistics { get; set; } = new();
        public ProductComponentSearchViewModel SearchFilters { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para estatísticas de ProductComponent
    /// </summary>
    public class ProductComponentStatisticsViewModel
    {
        public int TotalComponents { get; set; }
        public int ActiveComponents { get; set; }
        public int InactiveComponents { get; set; }
        public decimal TotalAdditionalCosts { get; set; }
        public decimal AverageAdditionalCost { get; set; }

        public string FormattedTotalAdditionalCosts => TotalAdditionalCosts.ToString("C2");
        public string FormattedAverageAdditionalCost => AverageAdditionalCost.ToString("C2");
    }

    /// <summary>
    /// ViewModel para busca/filtro de ProductComponent
    /// </summary>
    public class ProductComponentSearchViewModel
    {
        [Display(Name = "Nome")]
        public string? Name { get; set; }

        [Display(Name = "Hierarquia")]
        public string? ProductComponentHierarchyId { get; set; }

        [Display(Name = "Status")]
        public ObjectState? StateCode { get; set; }

        [Display(Name = "Custo Mínimo")]
        public decimal? MinAdditionalCost { get; set; }

        [Display(Name = "Custo Máximo")]
        public decimal? MaxAdditionalCost { get; set; }

        public List<ComponentStatusSelectionViewModel> GetAvailableStatuses()
        {
            return new List<ComponentStatusSelectionViewModel>
            {
                new() { Value = null, Text = "Todos", IsSelected = true },
                new() { Value = ObjectState.Active, Text = "Ativo", IsSelected = false },
                new() { Value = ObjectState.Inactive, Text = "Inativo", IsSelected = false }
            };
        }
    }





    /// <summary>
    /// ViewModel para autocomplete de ProductComponentHierarchy
    /// </summary>
    public class ProductComponentHierarchyAutocompleteViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Label => !string.IsNullOrWhiteSpace(Description) ? 
            $"{Name} - {Description}" : Name;
        public string Value => Name;
    }

    /// <summary>
    /// Classe helper para conversão entre entidade e ViewModel
    /// </summary>
    public static class ProductComponentMappingExtensions
    {
        public static ProductComponentViewModel ToViewModel(this ProductComponent entity)
        {
            return new ProductComponentViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductComponentHierarchyName = entity.ProductComponentHierarchy?.Name,
                AdditionalCost = entity.AdditionalCost,
                StateCode = entity.StateCode,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                LastModifiedAt = entity.LastModifiedAt,
                LastModifiedBy = entity.LastModifiedBy
            };
        }

        public static ProductComponentDetailsViewModel ToDetailsViewModel(this ProductComponent entity)
        {
            return new ProductComponentDetailsViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductComponentHierarchyName = entity.ProductComponentHierarchy?.Name,
                AdditionalCost = entity.AdditionalCost,
                StateCode = entity.StateCode,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                LastModifiedAt = entity.LastModifiedAt,
                LastModifiedBy = entity.LastModifiedBy
            };
        }

        public static EditProductComponentViewModel ToEditViewModel(this ProductComponent entity)
        {
            return new EditProductComponentViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductComponentHierarchyName = entity.ProductComponentHierarchy?.Name,
                AdditionalCost = entity.AdditionalCost,
                StateCode = entity.StateCode,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                LastModifiedAt = entity.LastModifiedAt,
                LastModifiedBy = entity.LastModifiedBy
            };
        }

        public static ProductComponent ToEntity(this CreateProductComponentViewModel viewModel)
        {
            return new ProductComponent
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                ProductComponentHierarchyId = viewModel.ProductComponentHierarchyId,
                AdditionalCost = viewModel.AdditionalCost,
                StateCode = viewModel.StateCode
            };
        }

        public static ProductComponent UpdateEntity(this EditProductComponentViewModel viewModel, ProductComponent entity)
        {
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.ProductComponentHierarchyId = viewModel.ProductComponentHierarchyId;
            entity.AdditionalCost = viewModel.AdditionalCost;
            entity.StateCode = viewModel.StateCode;
            
            return entity;
        }
    }

    /// <summary>
    /// ViewModel para seleção de status de componente
    /// </summary>
    public class ComponentStatusSelectionViewModel
    {
        public ObjectState? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 