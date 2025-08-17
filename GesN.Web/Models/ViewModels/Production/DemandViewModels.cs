using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GesN.Web.Models.ViewModels.Production
{
    /// <summary>
    /// ViewModel para exibição de demandas na listagem/grid
    /// </summary>
    public class DemandViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;
        
        [Display(Name = "Ordem de Produção")]
        public string? ProductionOrderId { get; set; }
        
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        
        [Display(Name = "Quantidade")]
        public string Quantity { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public DemandStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public string StatusCssClass { get; set; } = string.Empty;
        
        [Display(Name = "Data Prevista")]
        public DateTime? ExpectedDate { get; set; }
        public string ExpectedDateFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Data Início")]
        public DateTime? StartedAt { get; set; }
        
        [Display(Name = "Data Conclusão")]
        public DateTime? CompletedAt { get; set; }
        
        [Display(Name = "Observações")]
        public string Notes { get; set; } = string.Empty;
        
        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; }
        
        // Propriedades calculadas
        public bool IsOverdue { get; set; }
        public int DaysRemaining { get; set; }
        public string ProgressStatus { get; set; } = string.Empty;
        
        // Propriedades para actions
        public bool CanConfirm { get; set; }
        public bool CanMarkAsProduced { get; set; }
        public bool CanMarkAsEnding { get; set; }
        public bool CanMarkAsDelivered { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    /// <summary>
    /// ViewModel para criação de demandas
    /// </summary>
    public class CreateDemandViewModel
    {
        [Required(ErrorMessage = "Item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;
        
        [Display(Name = "Ordem de Produção")]
        public string? ProductionOrderId { get; set; }
        
        [Required(ErrorMessage = "Produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        public string Quantity { get; set; } = string.Empty;
        
        [Display(Name = "Data Prevista")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedDate { get; set; } = DateTime.Today.AddDays(7);
        
        [Display(Name = "Observações")]
        [MaxLength(2000, ErrorMessage = "Observações devem ter no máximo 2000 caracteres")]
        public string Notes { get; set; } = string.Empty;
        
        // Listas para dropdowns
        public List<SelectListItem> AvailableOrderItems { get; set; } = new();
        public List<SelectListItem> AvailableProducts { get; set; } = new();
        public List<SelectListItem> AvailableProductionOrders { get; set; } = new();
        
        // Dados auxiliares
        public string? SelectedOrderItemDisplay { get; set; }
        public string? SelectedProductDisplay { get; set; }
    }

    /// <summary>
    /// ViewModel para edição de demandas
    /// </summary>
    public class EditDemandViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;
        
        [Display(Name = "Ordem de Produção")]
        public string? ProductionOrderId { get; set; }
        
        [Required(ErrorMessage = "Produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        public string Quantity { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public DemandStatus Status { get; set; }
        
        [Display(Name = "Data Prevista")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedDate { get; set; }
        
        [Display(Name = "Observações")]
        [MaxLength(2000, ErrorMessage = "Observações devem ter no máximo 2000 caracteres")]
        public string Notes { get; set; } = string.Empty;
        
        // Dados de controle
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // Listas para dropdowns
        public List<SelectListItem> AvailableOrderItems { get; set; } = new();
        public List<SelectListItem> AvailableProducts { get; set; } = new();
        public List<SelectListItem> AvailableProductionOrders { get; set; } = new();
        public List<SelectListItem> AvailableStatuses { get; set; } = new();
        
        // Dados auxiliares
        public string ProductName { get; set; } = string.Empty;
        public string OrderItemDisplay { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        
        // Validações de negócio
        public bool CanChangeStatus { get; set; }
        public bool CanChangeExpectedDate { get; set; }
        public bool IsReadonly { get; set; }
        
        // Dados de auditoria
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    /// <summary>
    /// ViewModel para exibição detalhada de demandas
    /// </summary>
    public class DemandDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;
        public string OrderItemDisplay { get; set; } = string.Empty;
        
        [Display(Name = "Ordem de Produção")]
        public string? ProductionOrderId { get; set; }
        public string? ProductionOrderDisplay { get; set; }
        
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        
        [Display(Name = "Quantidade")]
        public string Quantity { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public DemandStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public string StatusDescription { get; set; } = string.Empty;
        
        [Display(Name = "Data Prevista")]
        public DateTime? ExpectedDate { get; set; }
        public string ExpectedDateFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Data Início")]
        public DateTime? StartedAt { get; set; }
        public string StartedAtFormatted { get; set; } = string.Empty;
        
        [Display(Name = "Data Conclusão")]
        public DateTime? CompletedAt { get; set; }
        public string CompletedAtFormatted { get; set; } = string.Empty;
        
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
        
        // Informações calculadas
        public bool IsOverdue { get; set; }
        public int DaysRemaining { get; set; }
        public string ProgressPercentage { get; set; } = string.Empty;
        public string TimelineStatus { get; set; } = string.Empty;
        
        // Ações disponíveis
        public bool CanConfirm { get; set; }
        public bool CanMarkAsProduced { get; set; }
        public bool CanMarkAsEnding { get; set; }
        public bool CanMarkAsDelivered { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        
        // Informações relacionadas
        public List<ProductComposition> RelatedCompositions { get; set; } = new();
        public Dictionary<string, object> AdditionalData { get; set; } = new();
    }

    /// <summary>
    /// ViewModel para a página de índice/listagem de demandas
    /// </summary>
    public class DemandIndexViewModel
    {
        public List<DemandViewModel> Demands { get; set; } = new();
        
        // Filtros
        public string? SearchTerm { get; set; }
        public DemandStatus? StatusFilter { get; set; }
        public string? ProductIdFilter { get; set; }
        public DateTime? StartDateFilter { get; set; }
        public DateTime? EndDateFilter { get; set; }
        public bool ShowOverdueOnly { get; set; }
        
        // Paginação
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        
        // Estatísticas do dashboard
        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int ProducedCount { get; set; }
        public int EndingCount { get; set; }
        public int DeliveredCount { get; set; }
        public int OverdueCount { get; set; }
        
        // Listas para filtros
        public List<SelectListItem> AvailableStatuses { get; set; } = new();
        public List<SelectListItem> AvailableProducts { get; set; } = new();
        
        // Configurações de exibição
        public string SortBy { get; set; } = "ExpectedDate";
        public string SortDirection { get; set; } = "asc";
        public bool ShowFilters { get; set; } = false;
    }

    /// <summary>
    /// ViewModel para operações em lote
    /// </summary>
    public class BulkDemandOperationViewModel
    {
        public List<string> SelectedIds { get; set; } = new();
        public string Operation { get; set; } = string.Empty;
        public DemandStatus? NewStatus { get; set; }
        public DateTime? NewExpectedDate { get; set; }
        public string? NewProductionOrderId { get; set; }
        public string? Notes { get; set; }
        
        public List<SelectListItem> AvailableOperations { get; set; } = new();
        public List<SelectListItem> AvailableStatuses { get; set; } = new();
        public List<SelectListItem> AvailableProductionOrders { get; set; } = new();
    }

    /// <summary>
    /// Classe helper para conversão entre entidade e ViewModel
    /// </summary>
    public static class DemandMappingExtensions
    {
        public static DemandViewModel ToViewModel(this Demand entity)
        {
            return new DemandViewModel
            {
                Id = entity.Id,
                OrderItemId = entity.OrderItemId,
                ProductionOrderId = entity.ProductionOrderId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                Status = entity.Status,
                StatusDisplay = entity.GetStatusDisplay(),
                StatusCssClass = GetStatusCssClass(entity.Status),
                ExpectedDate = entity.ExpectedDate,
                ExpectedDateFormatted = entity.ExpectedDate?.ToString("dd/MM/yyyy") ?? "",
                StartedAt = entity.StartedAt,
                CompletedAt = entity.CompletedAt,
                Notes = entity.Notes ?? "",
                CreatedAt = entity.CreatedAt,
                IsOverdue = entity.IsOverdue(),
                DaysRemaining = entity.GetDaysRemaining(),
                ProgressStatus = GetProgressStatus(entity)
            };
        }

        public static DemandDetailsViewModel ToDetailsViewModel(this Demand entity)
        {
            return new DemandDetailsViewModel
            {
                Id = entity.Id,
                OrderItemId = entity.OrderItemId,
                ProductionOrderId = entity.ProductionOrderId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                Status = entity.Status,
                StatusDisplay = entity.GetStatusDisplay(),
                ExpectedDate = entity.ExpectedDate,
                ExpectedDateFormatted = entity.ExpectedDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                StartedAt = entity.StartedAt,
                StartedAtFormatted = entity.StartedAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                CompletedAt = entity.CompletedAt,
                CompletedAtFormatted = entity.CompletedAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Notes = entity.Notes ?? "",
                CreatedAt = entity.CreatedAt,
                CreatedAtFormatted = entity.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                CreatedBy = entity.CreatedBy ?? "",
                LastModifiedAt = entity.LastModifiedAt,
                LastModifiedAtFormatted = entity.LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                LastModifiedBy = entity.LastModifiedBy ?? "",
                IsOverdue = entity.IsOverdue(),
                DaysRemaining = entity.GetDaysRemaining(),
                ProgressPercentage = GetProgressPercentage(entity),
                TimelineStatus = GetTimelineStatus(entity)
            };
        }

        public static Demand ToEntity(this CreateDemandViewModel viewModel)
        {
            return new Demand
            {
                OrderItemId = viewModel.OrderItemId,
                ProductionOrderId = viewModel.ProductionOrderId,
                ProductId = viewModel.ProductId,
                Quantity = viewModel.Quantity,
                ExpectedDate = viewModel.ExpectedDate,
                Notes = viewModel.Notes
            };
        }

        public static void UpdateEntity(this EditDemandViewModel viewModel, Demand entity)
        {
            entity.OrderItemId = viewModel.OrderItemId;
            entity.ProductionOrderId = viewModel.ProductionOrderId;
            entity.ProductId = viewModel.ProductId;
            entity.Quantity = viewModel.Quantity;
            entity.Status = viewModel.Status;
            entity.ExpectedDate = viewModel.ExpectedDate;
            entity.Notes = viewModel.Notes;
        }

        private static string GetStatusCssClass(DemandStatus status)
        {
            return status switch
            {
                DemandStatus.Pending => "badge bg-warning",
                DemandStatus.Confirmed => "badge bg-info",
                DemandStatus.Produced => "badge bg-primary",
                DemandStatus.Ending => "badge bg-secondary",
                DemandStatus.Delivered => "badge bg-success",
                _ => "badge bg-light"
            };
        }

        private static string GetProgressStatus(Demand entity)
        {
            if (entity.IsOverdue()) return "❌ Atrasado";
            
            return entity.Status switch
            {
                DemandStatus.Pending => "⏳ Pendente",
                DemandStatus.Confirmed => "✅ Confirmado",
                DemandStatus.Produced => "🏭 Produzido",
                DemandStatus.Ending => "📦 Finalizando",
                DemandStatus.Delivered => "🚚 Entregue",
                _ => "❓ Indefinido"
            };
        }

        private static string GetProgressPercentage(Demand entity)
        {
            return entity.Status switch
            {
                DemandStatus.Pending => "0%",
                DemandStatus.Confirmed => "25%",
                DemandStatus.Produced => "50%",
                DemandStatus.Ending => "75%",
                DemandStatus.Delivered => "100%",
                _ => "0%"
            };
        }

        private static string GetTimelineStatus(Demand entity)
        {
            var timeline = new List<string>();
            
            if (entity.StartedAt.HasValue)
                timeline.Add($"Iniciado: {entity.StartedAt.Value:dd/MM/yyyy}");
            
            if (entity.CompletedAt.HasValue)
                timeline.Add($"Concluído: {entity.CompletedAt.Value:dd/MM/yyyy}");
            
            return string.Join(" | ", timeline);
        }
    }
} 