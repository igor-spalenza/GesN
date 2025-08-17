using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Production
{
    public class ProductionOrderViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        [Display(Name = "Número do Pedido")]
        public string? OrderNumber { get; set; }

        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;

        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto")]
        public string? ProductName { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantity { get; set; }

        [Display(Name = "Status")]
        public ProductionOrderStatus Status { get; set; }

        [Display(Name = "Prioridade")]
        public ProductionOrderPriority Priority { get; set; }

        [Display(Name = "Início Programado")]
        public DateTime? ScheduledStartDate { get; set; }

        [Display(Name = "Término Programado")]
        public DateTime? ScheduledEndDate { get; set; }

        [Display(Name = "Início Real")]
        public DateTime? ActualStartDate { get; set; }

        [Display(Name = "Término Real")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Responsável")]
        public string? AssignedTo { get; set; }

        [Display(Name = "Observações")]
        public string? Notes { get; set; }

        [Display(Name = "Tempo Estimado (min)")]
        public int? EstimatedTime { get; set; }

        [Display(Name = "Tempo Real (min)")]
        public int? ActualTime { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Status")]
        public string StatusDisplay => Status switch
        {
            ProductionOrderStatus.Pending => "Pendente",
            ProductionOrderStatus.InProgress => "Em Progresso",
            ProductionOrderStatus.Paused => "Pausado",
            ProductionOrderStatus.Completed => "Concluído",
            ProductionOrderStatus.Cancelled => "Cancelado",
            ProductionOrderStatus.Failed => "Falhado",
            _ => Status.ToString()
        };

        [Display(Name = "Prioridade")]
        public string PriorityDisplay => Priority switch
        {
            ProductionOrderPriority.Low => "Baixa",
            ProductionOrderPriority.Normal => "Normal",
            ProductionOrderPriority.High => "Alta",
            ProductionOrderPriority.Urgent => "Urgente",
            _ => Priority.ToString()
        };

        [Display(Name = "Início Programado")]
        public string FormattedScheduledStartDate => ScheduledStartDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Término Programado")]
        public string FormattedScheduledEndDate => ScheduledEndDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Início Real")]
        public string FormattedActualStartDate => ActualStartDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Término Real")]
        public string FormattedActualEndDate => ActualEndDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [Display(Name = "Última Modificação")]
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Progresso")]
        public string ProgressIndicator => Status switch
        {
            ProductionOrderStatus.Pending => "0%",
            ProductionOrderStatus.InProgress => "50%",
            ProductionOrderStatus.Paused => "50%",
            ProductionOrderStatus.Completed => "100%",
            ProductionOrderStatus.Cancelled => "0%",
            ProductionOrderStatus.Failed => "0%",
            _ => "0%"
        };

        public bool CanStart => Status == ProductionOrderStatus.Pending;
        public bool CanPause => Status == ProductionOrderStatus.InProgress;
        public bool CanResume => Status == ProductionOrderStatus.Paused;
        public bool CanComplete => Status == ProductionOrderStatus.InProgress || Status == ProductionOrderStatus.Paused;
        public bool CanCancel => Status != ProductionOrderStatus.Completed && Status != ProductionOrderStatus.Cancelled;
    }

    public class CreateProductionOrderViewModel
    {
        [Required(ErrorMessage = "O pedido é obrigatório")]
        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Prioridade")]
        public ProductionOrderPriority Priority { get; set; } = ProductionOrderPriority.Normal;

        [Display(Name = "Início Programado")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledStartDate { get; set; }

        [Display(Name = "Término Programado")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledEndDate { get; set; }

        [Display(Name = "Responsável")]
        [MaxLength(100, ErrorMessage = "O nome do responsável deve ter no máximo {1} caracteres")]
        public string? AssignedTo { get; set; }

        [Display(Name = "Observações")]
        [MaxLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }

        [Display(Name = "Tempo Estimado (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo estimado deve ser maior ou igual a zero")]
        public int? EstimatedTime { get; set; }

        // Dropdowns
        [Display(Name = "Pedidos Disponíveis")]
        public List<OrderSelectionViewModel> AvailableOrders { get; set; } = new();

        [Display(Name = "Produtos Disponíveis")]
        public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();

        [Display(Name = "Prioridades Disponíveis")]
        public List<PrioritySelectionViewModel> AvailablePriorities { get; set; } = new()
        {
            new() { Value = ProductionOrderPriority.Low, Text = "Baixa", IsSelected = false },
            new() { Value = ProductionOrderPriority.Normal, Text = "Normal", IsSelected = true },
            new() { Value = ProductionOrderPriority.High, Text = "Alta", IsSelected = false },
            new() { Value = ProductionOrderPriority.Urgent, Text = "Urgente", IsSelected = false }
        };
    }

    public class EditProductionOrderViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O pedido é obrigatório")]
        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        [Display(Name = "Número do Pedido")]
        public string? OrderNumber { get; set; }

        [Required(ErrorMessage = "O item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [Display(Name = "Quantidade")]
        public int Quantity { get; set; } = 1;

        [Display(Name = "Status")]
        public ProductionOrderStatus Status { get; set; }

        [Display(Name = "Prioridade")]
        public ProductionOrderPriority Priority { get; set; } = ProductionOrderPriority.Normal;

        [Display(Name = "Início Programado")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledStartDate { get; set; }

        [Display(Name = "Término Programado")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledEndDate { get; set; }

        [Display(Name = "Início Real")]
        [DataType(DataType.DateTime)]
        public DateTime? ActualStartDate { get; set; }

        [Display(Name = "Término Real")]
        [DataType(DataType.DateTime)]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Responsável")]
        [MaxLength(100, ErrorMessage = "O nome do responsável deve ter no máximo {1} caracteres")]
        public string? AssignedTo { get; set; }

        [Display(Name = "Observações")]
        [MaxLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }

        [Display(Name = "Tempo Estimado (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo estimado deve ser maior ou igual a zero")]
        public int? EstimatedTime { get; set; }

        [Display(Name = "Tempo Real (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo real deve ser maior ou igual a zero")]
        public int? ActualTime { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        // Dropdowns
        [Display(Name = "Status Disponíveis")]
        public List<StatusSelectionViewModel> AvailableStatuses { get; set; } = new()
        {
            new() { Value = ProductionOrderStatus.Pending, Text = "Pendente", IsSelected = false },
            new() { Value = ProductionOrderStatus.InProgress, Text = "Em Progresso", IsSelected = false },
            new() { Value = ProductionOrderStatus.Paused, Text = "Pausado", IsSelected = false },
            new() { Value = ProductionOrderStatus.Completed, Text = "Concluído", IsSelected = false },
            new() { Value = ProductionOrderStatus.Cancelled, Text = "Cancelado", IsSelected = false },
            new() { Value = ProductionOrderStatus.Failed, Text = "Falhado", IsSelected = false }
        };

        [Display(Name = "Prioridades Disponíveis")]
        public List<PrioritySelectionViewModel> AvailablePriorities { get; set; } = new()
        {
            new() { Value = ProductionOrderPriority.Low, Text = "Baixa", IsSelected = false },
            new() { Value = ProductionOrderPriority.Normal, Text = "Normal", IsSelected = false },
            new() { Value = ProductionOrderPriority.High, Text = "Alta", IsSelected = false },
            new() { Value = ProductionOrderPriority.Urgent, Text = "Urgente", IsSelected = false }
        };
    }

    public class ProductionOrderDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string? OrderNumber { get; set; }
        public string OrderItemId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public ProductionOrderStatus Status { get; set; }
        public ProductionOrderPriority Priority { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? AssignedTo { get; set; }
        public string? Notes { get; set; }
        public int? EstimatedTime { get; set; }
        public int? ActualTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        public string StatusDisplay => Status switch
        {
            ProductionOrderStatus.Pending => "Pendente",
            ProductionOrderStatus.InProgress => "Em Progresso",
            ProductionOrderStatus.Paused => "Pausado",
            ProductionOrderStatus.Completed => "Concluído",
            ProductionOrderStatus.Cancelled => "Cancelado",
            ProductionOrderStatus.Failed => "Falhado",
            _ => Status.ToString()
        };

        public string PriorityDisplay => Priority switch
        {
            ProductionOrderPriority.Low => "Baixa",
            ProductionOrderPriority.Normal => "Normal",
            ProductionOrderPriority.High => "Alta",
            ProductionOrderPriority.Urgent => "Urgente",
            _ => Priority.ToString()
        };

        public string FormattedScheduledStartDate => ScheduledStartDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedScheduledEndDate => ScheduledEndDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedActualStartDate => ActualStartDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedActualEndDate => ActualEndDate?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        public bool CanStart => Status == ProductionOrderStatus.Pending;
        public bool CanPause => Status == ProductionOrderStatus.InProgress;
        public bool CanResume => Status == ProductionOrderStatus.Paused;
        public bool CanComplete => Status == ProductionOrderStatus.InProgress || Status == ProductionOrderStatus.Paused;
        public bool CanCancel => Status != ProductionOrderStatus.Completed && Status != ProductionOrderStatus.Cancelled;
    }

    public class ProductionOrderIndexViewModel
    {
        public List<ProductionOrderViewModel> ProductionOrders { get; set; } = new();
        public List<ProductionOrderViewModel> Orders { get; set; } = new();
        public ProductionOrderStatisticsViewModel Statistics { get; set; } = new();
        public ProductionOrderSearchViewModel Search { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalOrders { get; set; }
    }

    public class ProductionOrderStatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int InProgressOrders { get; set; }
        public int PausedOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int FailedOrders { get; set; }
        public int OverdueOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal AverageCompletionTime { get; set; }
        public decimal EfficiencyRate { get; set; }
    }

    public class ProductionOrderSearchViewModel
    {
        [Display(Name = "Termo de Busca")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Status")]
        public ProductionOrderStatus? Status { get; set; }

        [Display(Name = "Prioridade")]
        public ProductionOrderPriority? Priority { get; set; }

        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Responsável")]
        public string? AssignedTo { get; set; }

        public List<StatusSelectionViewModel> GetAvailableStatuses()
        {
            return new List<StatusSelectionViewModel>
            {
                new() { Value = null, Text = "Todos", IsSelected = true },
                new() { Value = ProductionOrderStatus.Pending, Text = "Pendente", IsSelected = false },
                new() { Value = ProductionOrderStatus.InProgress, Text = "Em Progresso", IsSelected = false },
                new() { Value = ProductionOrderStatus.Paused, Text = "Pausado", IsSelected = false },
                new() { Value = ProductionOrderStatus.Completed, Text = "Concluído", IsSelected = false },
                new() { Value = ProductionOrderStatus.Cancelled, Text = "Cancelado", IsSelected = false },
                new() { Value = ProductionOrderStatus.Failed, Text = "Falhado", IsSelected = false }
            };
        }

        public List<PrioritySelectionViewModel> GetAvailablePriorities()
        {
            return new List<PrioritySelectionViewModel>
            {
                new() { Value = null, Text = "Todas", IsSelected = true },
                new() { Value = ProductionOrderPriority.Low, Text = "Baixa", IsSelected = false },
                new() { Value = ProductionOrderPriority.Normal, Text = "Normal", IsSelected = false },
                new() { Value = ProductionOrderPriority.High, Text = "Alta", IsSelected = false },
                new() { Value = ProductionOrderPriority.Urgent, Text = "Urgente", IsSelected = false }
            };
        }
    }

    public class OrderSelectionViewModel
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }



    public class StatusSelectionViewModel
    {
        public ProductionOrderStatus? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class PrioritySelectionViewModel
    {
        public ProductionOrderPriority? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 