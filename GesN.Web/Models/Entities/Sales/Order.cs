using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Entities.ValueObjects;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Sales
{
    /// <summary>
    /// Entidade Order (Pedido) do domínio de vendas
    /// </summary>
    public class Order : Entity
    {
        /// <summary>
        /// Número sequencial do pedido
        /// </summary>
        [Required(ErrorMessage = "O número do pedido é obrigatório")]
        [Display(Name = "Número do Pedido")]
        [MaxLength(50)]
        public string NumberSequence { get; set; } = string.Empty;

        /// <summary>
        /// Data do pedido
        /// </summary>
        [Required(ErrorMessage = "A data do pedido é obrigatória")]
        [Display(Name = "Data do Pedido")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Data de entrega prevista
        /// </summary>
        [Display(Name = "Data de Entrega")]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// ID do cliente
        /// </summary>
        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Cliente do pedido (navegação)
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// Status do pedido
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        /// <summary>
        /// Tipo do pedido
        /// </summary>
        [Required]
        [Display(Name = "Tipo")]
        public OrderType Type { get; set; }

        /// <summary>
        /// Valor total do pedido
        /// </summary>
        [Required]
        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Subtotal do pedido
        /// </summary>
        [Required]
        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Valor dos impostos
        /// </summary>
        [Required]
        [Display(Name = "Impostos")]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Valor do desconto
        /// </summary>
        [Required]
        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Observações do pedido
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// ID do endereço de entrega
        /// </summary>
        [Display(Name = "Endereço de Entrega")]
        public string? DeliveryAddressId { get; set; }

        /// <summary>
        /// Endereço de entrega (navegação)
        /// </summary>
        public Address? DeliveryAddress { get; set; }

        /// <summary>
        /// Indica se o pedido requer nota fiscal
        /// </summary>
        [Required]
        [Display(Name = "Requer Nota Fiscal")]
        public bool RequiresFiscalReceipt { get; set; }

        /// <summary>
        /// ID dos dados fiscais
        /// </summary>
        [Display(Name = "Dados Fiscais")]
        public string? FiscalDataId { get; set; }

        /// <summary>
        /// Dados fiscais (navegação)
        /// </summary>
        public FiscalData? FiscalData { get; set; }

        /// <summary>
        /// Status de impressão do pedido
        /// </summary>
        [Required]
        [Display(Name = "Status de Impressão")]
        public PrintStatus PrintStatus { get; set; } = PrintStatus.NotPrinted;

        /// <summary>
        /// Número do lote de impressão
        /// </summary>
        [Display(Name = "Lote de Impressão")]
        public int? PrintBatchNumber { get; set; }

        /// <summary>
        /// Itens do pedido (navegação)
        /// </summary>
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Contratos associados ao pedido (navegação)
        /// </summary>
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Order() { }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Order(string customerId, OrderType type)
        {
            CustomerId = customerId;
            Type = type;
            OrderDate = DateTime.Today;
            Status = OrderStatus.Draft;
            PrintStatus = PrintStatus.NotPrinted;
        }

        /// <summary>
        /// Calcula os valores totais do pedido
        /// </summary>
        public void CalculateTotals()
        {
            Subtotal = Items.Sum(item => item.TotalPrice);
            TaxAmount = Items.Sum(item => item.TaxAmount);
            DiscountAmount = Items.Sum(item => item.DiscountAmount);
            TotalAmount = Subtotal + TaxAmount - DiscountAmount;
        }

        /// <summary>
        /// Verifica se o pedido pode ser finalizado
        /// </summary>
        public bool CanBeCompleted()
        {
            return Status != OrderStatus.Completed &&
                   Status != OrderStatus.Cancelled &&
                   Items.Any() &&
                   Items.All(item => item.Quantity > 0);
        }

        /// <summary>
        /// Verifica se o pedido pode ser cancelado
        /// </summary>
        public bool CanBeCancelled()
        {
            return Status != OrderStatus.Completed &&
                   Status != OrderStatus.Cancelled;
        }

        /// <summary>
        /// Verifica se o pedido pode ser impresso
        /// </summary>
        public bool CanBePrinted()
        {
            return Status != OrderStatus.Draft &&
                   Status != OrderStatus.Cancelled &&
                   PrintStatus == PrintStatus.NotPrinted;
        }

        /// <summary>
        /// Verifica se o pedido pode ser editado
        /// </summary>
        public bool CanBeEdited()
        {
            return Status != OrderStatus.Completed &&
                   Status != OrderStatus.Cancelled;
        }
    }
} 