using GesN.Web.Models.Entities.Production;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.ViewModels.Production
{
    /// <summary>
    /// ViewModel para exibição da relação CompositeProductXHierarchy na listagem/grid
    /// </summary>
    public class CompositeProductXHierarchyViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Hierarquia")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;
        
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Display(Name = "Nome da Hierarquia")]
        public string HierarchyName { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Qtd. Mínima")]
        public int MinQuantity { get; set; }

        [Display(Name = "Qtd. Máxima")]
        public int MaxQuantity { get; set; }

        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; }

        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; }

        [Display(Name = "Observações")]
        public string? Notes { get; set; }

        // Propriedades de exibição formatada
        public string MaxQuantityDisplay => MaxQuantity == 0 ? "Ilimitado" : MaxQuantity.ToString();
        public string OptionalDisplay => IsOptional ? "Sim" : "Não";
        public string QuantityRangeDisplay => MaxQuantity == 0 ? $"{MinQuantity}+" : $"{MinQuantity}-{MaxQuantity}";
        
        // Propriedades para compatibilidade com views existentes
        public string HierarchyDescription { get; set; } = string.Empty;
        public int HierarchyComponentCount { get; set; } = 0;
        public bool IsActive { get; set; } = true; // Simplificado - sempre ativo
        public string WeightDisplay { get; set; } = "-";
        public int AdditionalProcessingTime { get; set; } = 0;
    }

    /// <summary>
    /// ViewModel para criação de nova relação CompositeProductXHierarchy
    /// </summary>
    public class CreateCompositeProductXHierarchyViewModel
    {
        [Required(ErrorMessage = "Selecione uma hierarquia")]
        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um produto")]
        [Display(Name = "Produto Composto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a quantidade mínima")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade mínima deve ser maior que zero")]
        [Display(Name = "Quantidade Mínima")]
        public int MinQuantity { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade máxima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Máxima (0 = Ilimitado)")]
        public int MaxQuantity { get; set; } = 0;

        [Display(Name = "Hierarquia Opcional")]
        public bool IsOptional { get; set; } = false;

        [Required(ErrorMessage = "Informe a ordem de montagem")]
        [Range(1, int.MaxValue, ErrorMessage = "Ordem de montagem deve ser maior que zero")]
        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; } = 1;

        [Display(Name = "Observações")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Listas para dropdowns
        public List<SelectListItem> AvailableHierarchies { get; set; } = new();
        public List<SelectListItem> AvailableProducts { get; set; } = new();

        // Propriedades auxiliares para UI
        public bool IsProductIdReadonly { get; set; } = false;
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Converte ViewModel em Entity
        /// </summary>
        public CompositeProductXHierarchy ToEntity()
        {
            return new CompositeProductXHierarchy
            {
                ProductComponentHierarchyId = ProductComponentHierarchyId,
                ProductId = ProductId,
                MinQuantity = MinQuantity,
                MaxQuantity = MaxQuantity,
                IsOptional = IsOptional,
                AssemblyOrder = AssemblyOrder,
                Notes = Notes
            };
        }
    }

    /// <summary>
    /// ViewModel para edição de relação CompositeProductXHierarchy
    /// </summary>
    public class EditCompositeProductXHierarchyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecione uma hierarquia")]
        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione um produto")]
        [Display(Name = "Produto Composto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a quantidade mínima")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade mínima deve ser maior que zero")]
        [Display(Name = "Quantidade Mínima")]
        public int MinQuantity { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade máxima deve ser maior ou igual a zero")]
        [Display(Name = "Quantidade Máxima (0 = Ilimitado)")]
        public int MaxQuantity { get; set; } = 0;

        [Display(Name = "Hierarquia Opcional")]
        public bool IsOptional { get; set; } = false;

        [Required(ErrorMessage = "Informe a ordem de montagem")]
        [Range(1, int.MaxValue, ErrorMessage = "Ordem de montagem deve ser maior que zero")]
        [Display(Name = "Ordem de Montagem")]
        public int AssemblyOrder { get; set; } = 1;

        [Display(Name = "Observações")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Listas para dropdowns
        public List<SelectListItem> AvailableHierarchies { get; set; } = new();
        public List<SelectListItem> AvailableProducts { get; set; } = new();

        /// <summary>
        /// Converte ViewModel em Entity
        /// </summary>
        public CompositeProductXHierarchy ToEntity()
        {
            return new CompositeProductXHierarchy
            {
                Id = Id,
                ProductComponentHierarchyId = ProductComponentHierarchyId,
                ProductId = ProductId,
                MinQuantity = MinQuantity,
                MaxQuantity = MaxQuantity,
                IsOptional = IsOptional,
                AssemblyOrder = AssemblyOrder,
                Notes = Notes
            };
        }
    }

    /// <summary>
    /// ViewModel para exibição detalhada de uma relação CompositeProductXHierarchy
    /// </summary>
    public class CompositeProductXHierarchyDetailsViewModel
    {
        public int Id { get; set; }
        public string ProductComponentHierarchyId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string HierarchyName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool IsOptional { get; set; }
        public int AssemblyOrder { get; set; }
        public string? Notes { get; set; }

        // Propriedades de exibição formatada
        public string MaxQuantityDisplay => MaxQuantity == 0 ? "Ilimitado" : MaxQuantity.ToString();
        public string OptionalDisplay => IsOptional ? "Sim" : "Não";
        public string QuantityRangeDisplay => MaxQuantity == 0 ? $"{MinQuantity}+" : $"{MinQuantity}-{MaxQuantity}";
        
        // Propriedades simuladas para compatibilidade com views existentes
        public ProductComponentHierarchy? Hierarchy { get; set; }
        public CompositeProduct? Product { get; set; }
        public int HierarchyComponentCount { get; set; } = 0;
    }

    /// <summary>
    /// ViewModel para a página de índice/listagem de relações CompositeProductXHierarchy
    /// </summary>
    public class CompositeProductXHierarchyIndexViewModel
    {
        [Display(Name = "Relações")]
        public List<CompositeProductXHierarchyViewModel> Relations { get; set; } = new();

        [Display(Name = "Termo de Busca")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Produto")]
        public string? ProductId { get; set; }

        [Display(Name = "Hierarquia")]
        public string? HierarchyId { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsOptional { get; set; }

        [Display(Name = "Página Atual")]
        public int CurrentPage { get; set; } = 1;

        [Display(Name = "Itens por Página")]
        public int PageSize { get; set; } = 25;

        [Display(Name = "Total de Itens")]
        public int TotalItems { get; set; }

        [Display(Name = "Total de Páginas")]
        public int TotalPages { get; set; }

        [Display(Name = "Ordenar Por")]
        public string SortBy { get; set; } = "AssemblyOrder";

        [Display(Name = "Direção da Ordenação")]
        public string SortDirection { get; set; } = "asc";

        // Propriedades de contexto (quando aplicável)
        public string? ContextProductId { get; set; }
        public string ContextProductName { get; set; } = string.Empty;
        public string? ContextHierarchyId { get; set; }
        public string ContextHierarchyName { get; set; } = string.Empty;

        // Propriedades computadas
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int FirstItemIndex => (CurrentPage - 1) * PageSize + 1;
        public int LastItemIndex => Math.Min(CurrentPage * PageSize, TotalItems);
        public bool HasResults => Relations.Any();
    }

    /// <summary>
    /// Métodos de extensão para mapeamento de ViewModels
    /// </summary>
    public static class CompositeProductXHierarchyMappingExtensions
    {
        /// <summary>
        /// Converte Entity para ViewModel de exibição
        /// </summary>
        public static CompositeProductXHierarchyViewModel ToViewModel(this CompositeProductXHierarchy entity)
        {
            return new CompositeProductXHierarchyViewModel
            {
                Id = entity.Id,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductId = entity.ProductId,
                HierarchyName = entity.ProductComponentHierarchy?.Name ?? "",
                ProductName = entity.Product?.Name ?? "",
                MinQuantity = entity.MinQuantity,
                MaxQuantity = entity.MaxQuantity,
                IsOptional = entity.IsOptional,
                AssemblyOrder = entity.AssemblyOrder,
                Notes = entity.Notes
            };
        }

        /// <summary>
        /// Converte Entity para ViewModel de edição
        /// </summary>
        public static EditCompositeProductXHierarchyViewModel ToEditViewModel(this CompositeProductXHierarchy entity)
        {
            return new EditCompositeProductXHierarchyViewModel
            {
                Id = entity.Id,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductId = entity.ProductId,
                MinQuantity = entity.MinQuantity,
                MaxQuantity = entity.MaxQuantity,
                IsOptional = entity.IsOptional,
                AssemblyOrder = entity.AssemblyOrder,
                Notes = entity.Notes
            };
        }

        /// <summary>
        /// Converte Entity para ViewModel de detalhes
        /// </summary>
        public static CompositeProductXHierarchyDetailsViewModel ToDetailsViewModel(this CompositeProductXHierarchy entity)
        {
            return new CompositeProductXHierarchyDetailsViewModel
            {
                Id = entity.Id,
                ProductComponentHierarchyId = entity.ProductComponentHierarchyId,
                ProductId = entity.ProductId,
                HierarchyName = entity.ProductComponentHierarchy?.Name ?? "",
                ProductName = entity.Product?.Name ?? "",
                MinQuantity = entity.MinQuantity,
                MaxQuantity = entity.MaxQuantity,
                IsOptional = entity.IsOptional,
                AssemblyOrder = entity.AssemblyOrder,
                Notes = entity.Notes
            };
        }
    }

    #region Response Models para Operações CRUD

    /// <summary>
    /// Response padrão para operações CRUD de CompositeProductXHierarchy
    /// </summary>
    public class CompositeProductXHierarchyOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CompositeProductXHierarchyViewModel? Data { get; set; }
        public string Operation { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public int TotalCount { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public CompositeProductXHierarchyOperationResult()
        {
        }

        /// <summary>
        /// Construtor para operação bem-sucedida
        /// </summary>
        public CompositeProductXHierarchyOperationResult(bool success, string message, string operation)
        {
            Success = success;
            Message = message;
            Operation = operation;
        }

        /// <summary>
        /// Construtor para operação com dados
        /// </summary>
        public CompositeProductXHierarchyOperationResult(bool success, string message, string operation, CompositeProductXHierarchyViewModel? data, int totalCount = 0)
        {
            Success = success;
            Message = message;
            Operation = operation;
            Data = data;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Construtor para operação com erro
        /// </summary>
        public CompositeProductXHierarchyOperationResult(bool success, string message, Dictionary<string, string[]>? errors = null)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }
    }

    /// <summary>
    /// Response específico para criação
    /// </summary>
    public class CreateCompositeProductXHierarchyResult : CompositeProductXHierarchyOperationResult
    {
        public CreateCompositeProductXHierarchyResult() : base()
        {
            Operation = "CREATE";
        }

        public CreateCompositeProductXHierarchyResult(bool success, string message) : base(success, message, "CREATE")
        {
        }

        public CreateCompositeProductXHierarchyResult(bool success, string message, CompositeProductXHierarchyViewModel data, int totalCount) 
            : base(success, message, "CREATE", data, totalCount)
        {
        }

        public CreateCompositeProductXHierarchyResult(string message, Dictionary<string, string[]>? errors = null) 
            : base(false, message, errors)
        {
            Operation = "CREATE";
        }
    }

    /// <summary>
    /// Response específico para atualização  
    /// </summary>
    public class UpdateCompositeProductXHierarchyResult : CompositeProductXHierarchyOperationResult
    {
        public UpdateCompositeProductXHierarchyResult() : base()
        {
            Operation = "UPDATE";
        }

        public UpdateCompositeProductXHierarchyResult(bool success, string message) : base(success, message, "UPDATE")
        {
        }

        public UpdateCompositeProductXHierarchyResult(bool success, string message, CompositeProductXHierarchyViewModel data, int totalCount) 
            : base(success, message, "UPDATE", data, totalCount)
        {
        }

        public UpdateCompositeProductXHierarchyResult(string message, Dictionary<string, string[]>? errors = null) 
            : base(false, message, errors)
        {
            Operation = "UPDATE";
        }
    }

    /// <summary>
    /// Response específico para exclusão
    /// </summary>
    public class DeleteCompositeProductXHierarchyResult : CompositeProductXHierarchyOperationResult
    {
        public DeleteCompositeProductXHierarchyResult() : base()
        {
            Operation = "DELETE";
            Data = null; // DELETE não retorna dados
        }

        public DeleteCompositeProductXHierarchyResult(bool success, string message, int totalCount = 0) : base(success, message, "DELETE")
        {
            Data = null; // DELETE não retorna dados
            TotalCount = totalCount;
        }

        public DeleteCompositeProductXHierarchyResult(string message, Dictionary<string, string[]>? errors = null) 
            : base(false, message, errors)
        {
            Operation = "DELETE";
            Data = null;
        }
    }

    #endregion
} 