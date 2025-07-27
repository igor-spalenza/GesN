using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GesN.Web.Models.ViewModels.Production
{
    /// <summary>
    /// ViewModel para exibição de hierarquias na listagem/grid
    /// </summary>
    public class ProductComponentHierarchyViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        
        // Propriedades computadas para compatibilidade com views
        public bool IsActive => StateCode == ObjectState.Active;
        public string IsActiveDisplay => IsActive ? "Ativa" : "Inativa";
        public string StatusCssClass => IsActive ? "badge bg-success" : "badge bg-secondary";
        
        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }
        public string CreatedAtFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Observações")]
        public string Notes { get; set; } = string.Empty;
        
        // Propriedades calculadas
        public int ComponentCount { get; set; }
        public int UsageCount { get; set; }
        public bool HasRequiredComponents { get; set; }
        public bool AreAllComponentsAvailable { get; set; }
        
        // Propriedades para actions
        public bool CanActivate { get; set; }
        public bool CanDeactivate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDuplicate { get; set; }
        
        // Propriedades específicas para relações em produtos compostos
        [Display(Name = "Qtd. Mínima")]
        public int MinQuantity { get; set; }
        
        [Display(Name = "Qtd. Máxima")]
        public int MaxQuantity { get; set; }
        
        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; }
        
        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; }
        
        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }
        
        // ID da relação CompositeProductXHierarchy quando aplicável
        public int? RelationId { get; set; }
        
        // Nome do produto (usado para exibição)
        public string ProductName { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para criação de hierarquias
    /// </summary>
    public class CreateProductComponentHierarchyViewModel
    {
        [Required(ErrorMessage = "Nome da hierarquia é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Descrição")]
        [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        
        // Propriedade computada para compatibilidade
        public bool IsActive => StateCode == ObjectState.Active;
        
        [Display(Name = "Observações")]
        [MaxLength(2000, ErrorMessage = "Observações devem ter no máximo 2000 caracteres")]
        public string Notes { get; set; } = string.Empty;
        
        // Opções de criação
        [Display(Name = "Criar a partir de template")]
        public string? TemplateId { get; set; }
        
        [Display(Name = "Copiar de hierarquia existente")]
        public string? SourceHierarchyId { get; set; }
        
        // Listas para dropdowns
        public List<SelectListItem> AvailableTemplates { get; set; } = new();
        public List<SelectListItem> AvailableHierarchies { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para edição de hierarquias
    /// </summary>
    public class EditProductComponentHierarchyViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nome da hierarquia é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Descrição")]
        [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        
        // Propriedade computada para compatibilidade
        public bool IsActive => StateCode == ObjectState.Active;
        
        [Display(Name = "Observações")]
        [MaxLength(2000, ErrorMessage = "Observações devem ter no máximo 2000 caracteres")]
        public string Notes { get; set; } = string.Empty;
        
        // Dados de controle
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        
        // Validações de negócio
        public bool CanChangeActiveStatus { get; set; }
        public bool IsReadonly { get; set; }
        
        // Informações adicionais
        public int ComponentCount { get; set; }
        public int UsageCount { get; set; }
        public string OriginalName { get; set; } = string.Empty;
    }

    /// <summary>
    /// ViewModel para exibição detalhada de hierarquias
    /// </summary>
    public class ProductComponentHierarchyDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Descrição")]
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        
        // Propriedades computadas para compatibilidade
        public bool IsActive => StateCode == ObjectState.Active;
        public string IsActiveDisplay => IsActive ? "Ativa" : "Inativa";
        public string StatusDescription { get; set; } = string.Empty;
        
        [Display(Name = "Observações")]
        public string Notes { get; set; } = string.Empty;
        
        // Dados de auditoria
        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }
        public string CreatedAtFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Criado por")]
        public string CreatedBy { get; set; } = string.Empty;
        
        [Display(Name = "Última modificação")]
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedAtFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Modificado por")]
        public string LastModifiedBy { get; set; } = string.Empty;
        
        // Informações de componentes
        [Display(Name = "Total de Componentes")]
        public int ComponentCount { get; set; }
        
        [Display(Name = "Componentes Obrigatórios")]
        public bool HasRequiredComponents { get; set; }
        public string HasRequiredComponentsDisplay { get; set; } = string.Empty;
        
        [Display(Name = "Todos Disponíveis")]
        public bool AreAllComponentsAvailable { get; set; }
        public string AvailabilityDisplay { get; set; } = string.Empty;
        
        // Informações de uso
        [Display(Name = "Usado em Produtos")]
        public int UsageCount { get; set; }
        public string UsageDescription { get; set; } = string.Empty;
        
        // Ações disponíveis
        public bool CanActivate { get; set; }
        public bool CanDeactivate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanDuplicate { get; set; }
        
        // Listas relacionadas
        public List<ProductComponent> Components { get; set; } = new();
        public List<CompositeProductXHierarchy> CompositeProducts { get; set; } = new();
        
        // Informações de relação com produto (quando aplicável)
        public ProductHierarchyRelationInfo? ProductRelationInfo { get; set; }
        
        // Dados adicionais
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para a página de índice/listagem de hierarquias
    /// </summary>
    public class ProductComponentHierarchyIndexViewModel
    {
        public List<ProductComponentHierarchyViewModel> Hierarchies { get; set; } = new();
        
        // Filtros
        public string? SearchTerm { get; set; }
        public ObjectState? StateCodeFilter { get; set; }
        
        // Propriedade computada para compatibilidade
        public bool? IsActiveFilter => StateCodeFilter?.Equals(ObjectState.Active);
        public int? MinUsageCount { get; set; }
        public bool ShowUnusedOnly { get; set; }
        
        // Paginação
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        
        // Estatísticas do dashboard
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int UsedCount { get; set; }
        public int UnusedCount { get; set; }
        public int TotalComponents { get; set; }
        
        // Listas para filtros
        public List<SelectListItem> AvailableStatusFilters { get; set; } = new();
        
        // Configurações de exibição
        public string SortBy { get; set; } = "Name";
        public string SortDirection { get; set; } = "asc";
        public bool ShowFilters { get; set; } = false;
        public string ViewMode { get; set; } = "grid"; // grid, list, card
    }

    /// <summary>
    /// ViewModel para operações em lote
    /// </summary>
    public class BulkHierarchyOperationViewModel
    {
        public List<string> SelectedIds { get; set; } = new();
        public string Operation { get; set; } = string.Empty;
        public ObjectState? NewStateCode { get; set; }
        
        // Propriedade computada para compatibilidade
        public bool? NewActiveStatus => NewStateCode?.Equals(ObjectState.Active);
        public string? NewNotes { get; set; }
        
        public List<SelectListItem> AvailableOperations { get; set; } = new();
        
        // Informações sobre os itens selecionados
        public int SelectedCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int UsedCount { get; set; }
    }

    /// <summary>
    /// ViewModel para duplicação de hierarquia
    /// </summary>
    public class DuplicateHierarchyViewModel
    {
        public string SourceHierarchyId { get; set; } = string.Empty;
        public string SourceHierarchyName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nome da nova hierarquia é obrigatório")]
        [Display(Name = "Nome da Nova Hierarquia")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string NewName { get; set; } = string.Empty;
        
        [Display(Name = "Nova Descrição")]
        [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string NewDescription { get; set; } = string.Empty;
        
        [Display(Name = "Copiar Componentes")]
        public bool CopyComponents { get; set; } = true;
        
        [Display(Name = "Status da Nova Hierarquia")]
        public ObjectState NewStateCode { get; set; } = ObjectState.Active;
        
        // Propriedade computada para compatibilidade
        public bool KeepActive => NewStateCode == ObjectState.Active;
        
        // Informações sobre a hierarquia origem
        public string SourceDescription { get; set; } = string.Empty;
        public int ComponentCount { get; set; }
        public ObjectState SourceStateCode { get; set; } = ObjectState.Active;
        
        // Propriedade computada para compatibilidade
        public bool SourceIsActive => SourceStateCode == ObjectState.Active;
    }

    /// <summary>
    /// ViewModel para seleção de hierarquia em modals/dropdowns
    /// </summary>
    public class HierarchySelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        public int ComponentCount { get; set; }
        public int UsageCount { get; set; }
        
        // Propriedades computadas para compatibilidade
        public bool IsActive => StateCode == ObjectState.Active;
        public string DisplayText => $"{Name} ({ComponentCount} componentes)";
        public string StatusBadge => IsActive ? "Ativa" : "Inativa";
        public string StatusCssClass => IsActive ? "badge bg-success" : "badge bg-secondary";
    }

    /// <summary>
    /// ViewModel para associar hierarquia a produto
    /// </summary>
    public class AssignHierarchyToProductViewModel
    {
        [Required(ErrorMessage = "ID do produto é obrigatório")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hierarquia deve ser selecionada")]
        [Display(Name = "Hierarquia")]
        public string HierarchyId { get; set; } = string.Empty;

        [Display(Name = "Observações")]
        [MaxLength(500, ErrorMessage = "Observações devem ter no máximo 500 caracteres")]
        public string Notes { get; set; } = string.Empty;

        public List<SelectListItem> AvailableHierarchies { get; set; } = new();
        
        // Informações do produto
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
    }

    /// <summary>
    /// Informações sobre a relação entre produto e hierarquia
    /// </summary>
    public class ProductHierarchyRelationInfo
    {
        public DateTime AssignedAt { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public ObjectState StateCode { get; set; } = ObjectState.Active;
        public string Notes { get; set; } = string.Empty;
        
        // Propriedades computadas para compatibilidade
        public bool IsActive => StateCode == ObjectState.Active;
        public string AssignedAtFormatted => AssignedAt.ToString("dd/MM/yyyy HH:mm");
        public string StatusDisplay => IsActive ? "Ativa" : "Inativa";
    }

    /// <summary>
    /// ViewModel para gerenciamento de componentes de hierarquia
    /// </summary>
    public class HierarchyComponentManagementViewModel
    {
        public string HierarchyId { get; set; } = string.Empty;
        public string HierarchyName { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        
        [Display(Name = "Componentes")]
        public List<ProductComponentViewModel> Components { get; set; } = new();
        
        [Display(Name = "Total de Componentes")]
        public int TotalComponents => Components.Count;
        
        [Display(Name = "Componentes Ativos")]
        public int ActiveComponents => Components.Count(c => c.StateCode == ObjectState.Active);
        
        [Display(Name = "Componentes Inativos")]
        public int InactiveComponents => Components.Count(c => c.StateCode == ObjectState.Inactive);
        
        // Informações adicionais
        public bool CanManageComponents { get; set; } = true;
        public bool IsReadonly { get; set; } = false;
    }

    /// <summary>
    /// Classe helper para conversão entre entidade e ViewModel
    /// </summary>
    public static class ProductComponentHierarchyMappingExtensions
    {
        public static ProductComponentHierarchyViewModel ToViewModel(this ProductComponentHierarchy entity)
        {
            return new ProductComponentHierarchyViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description ?? "",
                StateCode = entity.StateCode,
                CreatedAt = entity.CreatedAt,
                CreatedAtFormatted = entity.CreatedAt.ToString("dd/MM/yyyy"),
                Notes = entity.Notes ?? "",
                HasRequiredComponents = entity.HasRequiredComponents(),
                AreAllComponentsAvailable = entity.AreAllComponentsAvailable()
            };
        }

        public static ProductComponentHierarchyDetailsViewModel ToDetailsViewModel(this ProductComponentHierarchy entity)
        {
            return new ProductComponentHierarchyDetailsViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description ?? "",
                StateCode = entity.StateCode,
                StatusDescription = GetStatusDescription(entity),
                Notes = entity.Notes ?? "",
                CreatedAt = entity.CreatedAt,
                CreatedAtFormatted = entity.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                CreatedBy = entity.CreatedBy ?? "",
                LastModifiedAt = entity.LastModifiedAt,
                LastModifiedAtFormatted = entity.LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                LastModifiedBy = entity.LastModifiedBy ?? "",
                HasRequiredComponents = entity.HasRequiredComponents(),
                HasRequiredComponentsDisplay = entity.HasRequiredComponents() ? "Sim" : "Não",
                AreAllComponentsAvailable = entity.AreAllComponentsAvailable(),
                AvailabilityDisplay = entity.GetAvailabilityInfo()
            };
        }

        public static ProductComponentHierarchy ToEntity(this CreateProductComponentHierarchyViewModel viewModel)
        {
            return new ProductComponentHierarchy
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                StateCode = viewModel.StateCode,
                Notes = viewModel.Notes
            };
        }

        public static void UpdateEntity(this EditProductComponentHierarchyViewModel viewModel, ProductComponentHierarchy entity)
        {
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.StateCode = viewModel.StateCode;
            entity.Notes = viewModel.Notes;
        }

        public static HierarchySelectionViewModel ToSelectionViewModel(this ProductComponentHierarchy entity)
        {
            return new HierarchySelectionViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description ?? "",
                StateCode = entity.StateCode
            };
        }

        private static string GetStatusDescription(ProductComponentHierarchy entity)
        {
            if (!entity.IsActive())
                return "Hierarquia desativada - não pode ser utilizada em produtos";

            if (!entity.HasRequiredComponents())
                return "Hierarquia sem componentes obrigatórios configurados";

            if (!entity.AreAllComponentsAvailable())
                return "Alguns componentes desta hierarquia estão indisponíveis";

            return "Hierarquia ativa e pronta para uso";
        }
    }
} 