using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Sales
{
    public class OrderViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Número")]
        public string NumberSequence { get; set; } = string.Empty;

        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public string CustomerId { get; set; } = string.Empty;

        [Display(Name = "Nome do Cliente")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "A data do pedido é obrigatória")]
        [Display(Name = "Data do Pedido")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "A data de entrega é obrigatória")]
        [Display(Name = "Data de Entrega")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "O tipo de pedido é obrigatório")]
        [Display(Name = "Tipo")]
        public OrderType Type { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        [Display(Name = "Status de Impressão")]
        public PrintStatus PrintStatus { get; set; } = PrintStatus.NotPrinted;

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Impostos")]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Observações")]
        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Status")]
        public string StatusDisplay => Status switch
        {
            OrderStatus.Draft => "Rascunho",
            OrderStatus.Confirmed => "Confirmado",
            OrderStatus.InProduction => "Em Produção",
            OrderStatus.ReadyForDelivery => "Pronto para Entrega",
            OrderStatus.InDelivery => "Em Entrega",
            OrderStatus.Delivered => "Entregue",
            OrderStatus.Cancelled => "Cancelado",
            OrderStatus.Completed => "Concluído",
            _ => Status.ToString()
        };

        [Display(Name = "Tipo")]
        public string TypeDisplay => Type switch
        {
            OrderType.Order => "Pedido",
            OrderType.Event => "Evento",
            _ => Type.ToString()
        };

        [Display(Name = "Status de Impressão")]
        public string PrintStatusDisplay => PrintStatus switch
        {
            PrintStatus.NotPrinted => "Não Impresso",
            PrintStatus.Printed => "Impresso",
            _ => PrintStatus.ToString()
        };

        [Display(Name = "Data de Criação")]
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        [Display(Name = "Última Modificação")]
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";

        [Display(Name = "Data do Pedido")]
        public string FormattedOrderDate => OrderDate.ToString("dd/MM/yyyy");

        [Display(Name = "Data de Entrega")]
        public string FormattedDeliveryDate => DeliveryDate.ToString("dd/MM/yyyy");
    }

    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de pedido é obrigatório")]
        [Display(Name = "Tipo")]
        public OrderType Type { get; set; }

        // Propriedades auxiliares para o formulário
        [Display(Name = "Buscar Cliente")]
        public string CustomerSearchTerm { get; set; } = string.Empty;

        [Display(Name = "Tipos de Pedido Disponíveis")]
        public List<OrderTypeSelectionViewModel> AvailableOrderTypes { get; set; } = new()
        {
            new() { Value = OrderType.Order, Text = "Pedido", IsSelected = false },
            new() { Value = OrderType.Event, Text = "Evento", IsSelected = false }
        };
    }

    public class EditOrderViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Número")]
        public string NumberSequence { get; set; } = string.Empty;

        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public string CustomerId { get; set; } = string.Empty;

        [Display(Name = "Nome do Cliente")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "A data do pedido é obrigatória")]
        [Display(Name = "Data do Pedido")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "A data de entrega é obrigatória")]
        [Display(Name = "Data de Entrega")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "O tipo de pedido é obrigatório")]
        [Display(Name = "Tipo")]
        public OrderType Type { get; set; }

        [Display(Name = "Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        [Display(Name = "Status de Impressão")]
        public PrintStatus PrintStatus { get; set; } = PrintStatus.NotPrinted;

        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Impostos")]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Observações")]
        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Tipos de Pedido Disponíveis")]
        public List<OrderTypeSelectionViewModel> AvailableOrderTypes { get; set; } = new()
        {
            new() { Value = OrderType.Order, Text = "Pedido", IsSelected = false },
            new() { Value = OrderType.Event, Text = "Evento", IsSelected = false }
        };

        // Lista de itens do pedido
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NumberSequence { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; }
        public PrintStatus PrintStatus { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        public string StatusDisplay => Status switch
        {
            OrderStatus.Draft => "Rascunho",
            OrderStatus.Confirmed => "Confirmado",
            OrderStatus.InProduction => "Em Produção",
            OrderStatus.ReadyForDelivery => "Pronto para Entrega",
            OrderStatus.InDelivery => "Em Entrega",
            OrderStatus.Delivered => "Entregue",
            OrderStatus.Cancelled => "Cancelado",
            OrderStatus.Completed => "Concluído",
            _ => Status.ToString()
        };

        public string TypeDisplay => Type switch
        {
            OrderType.Order => "Pedido",
            OrderType.Event => "Evento",
            _ => Type.ToString()
        };

        public string PrintStatusDisplay => PrintStatus switch
        {
            PrintStatus.NotPrinted => "Não Impresso",
            PrintStatus.Printed => "Impresso",
            _ => PrintStatus.ToString()
        };

        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
        public string FormattedOrderDate => OrderDate.ToString("dd/MM/yyyy");
        public string FormattedDeliveryDate => DeliveryDate.ToString("dd/MM/yyyy");

        // Lista de itens do pedido
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderIndexViewModel
    {
        public List<OrderViewModel> Orders { get; set; } = new();
        public OrderStatisticsViewModel Statistics { get; set; } = new();
        public OrderSearchViewModel Search { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalOrders { get; set; }
    }

    public class OrderStatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public int DraftOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int InProductionOrders { get; set; }
        public int PendingDeliveryOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalOrdersValue { get; set; }
        public int NewOrdersThisMonth { get; set; }
    }

    public class OrderSearchViewModel
    {
        [Display(Name = "Termo de Busca")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Status")]
        public OrderStatus? Status { get; set; }

        [Display(Name = "Tipo")]
        public OrderType? Type { get; set; }

        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Cliente")]
        public string? CustomerId { get; set; }

        public List<OrderStatusSelectionViewModel> GetAvailableStatuses()
        {
            return new List<OrderStatusSelectionViewModel>
            {
                new() { Value = null, Text = "Todos", IsSelected = Status == null },
                new() { Value = OrderStatus.Draft, Text = "Rascunho", IsSelected = Status == OrderStatus.Draft },
                new() { Value = OrderStatus.Confirmed, Text = "Confirmado", IsSelected = Status == OrderStatus.Confirmed },
                new() { Value = OrderStatus.InProduction, Text = "Em Produção", IsSelected = Status == OrderStatus.InProduction },
                new() { Value = OrderStatus.ReadyForDelivery, Text = "Pronto para Entrega", IsSelected = Status == OrderStatus.ReadyForDelivery },
                new() { Value = OrderStatus.InDelivery, Text = "Em Entrega", IsSelected = Status == OrderStatus.InDelivery },
                new() { Value = OrderStatus.Delivered, Text = "Entregue", IsSelected = Status == OrderStatus.Delivered },
                new() { Value = OrderStatus.Cancelled, Text = "Cancelado", IsSelected = Status == OrderStatus.Cancelled },
                new() { Value = OrderStatus.Completed, Text = "Concluído", IsSelected = Status == OrderStatus.Completed }
            };
        }

        public List<OrderTypeSelectionViewModel> GetAvailableTypes()
        {
            return new List<OrderTypeSelectionViewModel>
            {
                new() { Value = null, Text = "Todos", IsSelected = Type == null },
                new() { Value = OrderType.Order, Text = "Pedido", IsSelected = Type == OrderType.Order },
                new() { Value = OrderType.Event, Text = "Evento", IsSelected = Type == OrderType.Event }
            };
        }
    }

    public class OrderItemViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        [Display(Name = "Nome do Produto")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "O preço unitário é obrigatório")]
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "O preço unitário não pode ser negativo")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "O desconto não pode ser negativo")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Impostos")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Os impostos não podem ser negativos")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Observações")]
        [StringLength(1000, ErrorMessage = "As observações devem ter no máximo {1} caracteres")]
        public string? Notes { get; set; }

        // Propriedades calculadas
        [Display(Name = "Subtotal")]
        public decimal Subtotal => Quantity * UnitPrice;

        [Display(Name = "Total")]
        public decimal TotalPrice => Subtotal + TaxAmount - DiscountAmount;
    }

    public class OrderTypeSelectionViewModel
    {
        public OrderType? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class OrderStatusSelectionViewModel
    {
        public OrderStatus? Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 