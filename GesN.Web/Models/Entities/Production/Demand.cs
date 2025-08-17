using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade Demand - representa uma demanda de produção específica
    /// Conecta OrderItems com ProductionOrders e define os requisitos de produção
    /// </summary>
    public class Demand : Entity
    {
        /// <summary>
        /// ID do item do pedido que originou esta demanda
        /// </summary>
        [Required(ErrorMessage = "O item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;

        /// <summary>
        /// ID da ordem de produção associada (opcional até que seja criada)
        /// </summary>
        [Display(Name = "Ordem de Produção")]
        public string? ProductionOrderId { get; set; }

        /// <summary>
        /// ID do produto a ser produzido
        /// </summary>
        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade demandada para produção
        /// Armazenado como TEXT no banco para flexibilidade (pode incluir unidades)
        /// </summary>
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [MaxLength(50)]
        public string Quantity { get; set; } = "1";

        /// <summary>
        /// Observações sobre a demanda
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Status da demanda
        /// </summary>
        [Display(Name = "Status")]
        public DemandStatus Status { get; set; } = DemandStatus.Pending;

        /// <summary>
        /// Data prevista para conclusão
        /// </summary>
        [Display(Name = "Data Prevista")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedDate { get; set; }

        /// <summary>
        /// Data de início da produção
        /// </summary>
        [Display(Name = "Data de Início")]
        [DataType(DataType.DateTime)]
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Data de conclusão da produção
        /// </summary>
        [Display(Name = "Data de Conclusão")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedAt { get; set; }

        // Propriedades de Navegação

        /// <summary>
        /// Item do pedido que originou esta demanda
        /// </summary>
        public OrderItem? OrderItem { get; set; }

        /// <summary>
        /// Ordem de produção associada
        /// </summary>
        public ProductionOrder? ProductionOrder { get; set; }

        /// <summary>
        /// Produto a ser produzido
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Composições de produtos relacionadas a esta demanda
        /// </summary>
        public ICollection<ProductComposition> ProductCompositions { get; set; } = new List<ProductComposition>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Demand()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Demand(string orderItemId, string productId, string quantity)
        {
            OrderItemId = orderItemId;
            ProductId = productId;
            Quantity = quantity;
            ExpectedDate = DateTime.Today.AddDays(7); // Padrão de 7 dias
        }

        // Métodos de Negócio

        /// <summary>
        /// Obtém a quantidade como número inteiro (conversão simples)
        /// </summary>
        public int GetQuantityAsInt()
        {
            if (int.TryParse(Quantity.Trim(), out int result))
                return result;
            return 1; // Valor padrão
        }

        /// <summary>
        /// Verifica se a demanda está em produção
        /// </summary>
        public bool IsInProduction()
        {
            return Status == DemandStatus.Produced;
        }

        /// <summary>
        /// Verifica se a demanda foi entregue
        /// </summary>
        public bool IsDelivered()
        {
            return Status == DemandStatus.Delivered && CompletedAt.HasValue;
        }

        /// <summary>
        /// Verifica se a demanda está atrasada
        /// </summary>
        public bool IsOverdue()
        {
            return ExpectedDate.HasValue && 
                   ExpectedDate.Value < DateTime.Today && 
                   !IsDelivered();
        }

        /// <summary>
        /// Calcula os dias restantes até a data prevista
        /// </summary>
        public int GetDaysRemaining()
        {
            if (!ExpectedDate.HasValue || IsDelivered())
                return 0;

            return (ExpectedDate.Value - DateTime.Today).Days;
        }

        /// <summary>
        /// Confirma a demanda
        /// </summary>
        public void Confirm()
        {
            if (Status == DemandStatus.Pending)
            {
                Status = DemandStatus.Confirmed;
                StartedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Marca a demanda como produzida
        /// </summary>
        public void MarkAsProduced()
        {
            if (Status == DemandStatus.Confirmed)
            {
                Status = DemandStatus.Produced;
            }
        }

        /// <summary>
        /// Marca a demanda como finalizando
        /// </summary>
        public void MarkAsEnding()
        {
            if (Status == DemandStatus.Produced)
            {
                Status = DemandStatus.Ending;
            }
        }

        /// <summary>
        /// Marca a demanda como entregue
        /// </summary>
        public void MarkAsDelivered()
        {
            if (Status == DemandStatus.Ending)
            {
                Status = DemandStatus.Delivered;
                CompletedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Obtém a descrição de status formatada
        /// </summary>
        public string GetStatusDisplay()
        {
            return Status switch
            {
                DemandStatus.Pending => "Pendente",
                DemandStatus.Confirmed => "Confirmado",
                DemandStatus.Produced => "Produzido",
                DemandStatus.Ending => "Finalizando",
                DemandStatus.Delivered => "Entregue",
                _ => Status.ToString()
            };
        }

        /// <summary>
        /// Obtém informações resumidas da demanda
        /// </summary>
        public string GetSummary()
        {
            var productName = Product?.Name ?? "Produto";
            var status = GetStatusDisplay();
            return $"{productName} - Qtd: {Quantity} - Status: {status}";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(OrderItemId) &&
                   !string.IsNullOrWhiteSpace(ProductId) &&
                   !string.IsNullOrWhiteSpace(Quantity);
        }

        /// <summary>
        /// Override do ToString para exibir resumo
        /// </summary>
        public override string ToString()
        {
            return GetSummary();
        }
    }
} 