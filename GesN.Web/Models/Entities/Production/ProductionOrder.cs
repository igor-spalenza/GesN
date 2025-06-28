using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa uma ordem de produção
    /// </summary>
    public class ProductionOrder : Entity
    {
        /// <summary>
        /// ID do pedido (OrderId na tabela)
        /// </summary>
        [Required(ErrorMessage = "O pedido é obrigatório")]
        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// ID do item do pedido (OrderItemId na tabela)
        /// </summary>
        [Required(ErrorMessage = "O item do pedido é obrigatório")]
        [Display(Name = "Item do Pedido")]
        public string OrderItemId { get; set; } = string.Empty;

        /// <summary>
        /// ID do produto a ser produzido
        /// </summary>
        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade a ser produzida
        /// </summary>
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; }

        /// <summary>
        /// Status da ordem de produção
        /// </summary>
        [Required(ErrorMessage = "O status é obrigatório")]
        [Display(Name = "Status")]
        public ProductionOrderStatus Status { get; set; } = ProductionOrderStatus.Pending;

        /// <summary>
        /// Prioridade da ordem de produção
        /// </summary>
        [Display(Name = "Prioridade")]
        public ProductionOrderPriority Priority { get; set; } = ProductionOrderPriority.Normal;

        /// <summary>
        /// Data programada de início
        /// </summary>
        [Display(Name = "Início Programado")]
        public DateTime? ScheduledStartDate { get; set; }

        /// <summary>
        /// Data programada de término
        /// </summary>
        [Display(Name = "Término Programado")]
        public DateTime? ScheduledEndDate { get; set; }

        /// <summary>
        /// Data real de início
        /// </summary>
        [Display(Name = "Início Real")]
        public DateTime? ActualStartDate { get; set; }

        /// <summary>
        /// Data real de término
        /// </summary>
        [Display(Name = "Término Real")]
        public DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// Responsável pela produção
        /// </summary>
        [Display(Name = "Responsável")]
        [MaxLength(100)]
        public string? AssignedTo { get; set; }

        /// <summary>
        /// Observações sobre a ordem de produção
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Tempo estimado em minutos
        /// </summary>
        [Display(Name = "Tempo Estimado (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo estimado deve ser maior ou igual a zero")]
        public int? EstimatedTime { get; set; }

        /// <summary>
        /// Tempo real em minutos
        /// </summary>
        [Display(Name = "Tempo Real (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo real deve ser maior ou igual a zero")]
        public int? ActualTime { get; set; }

        /// <summary>
        /// Propriedade navegacional para o pedido
        /// </summary>
        public OrderEntry? Order { get; set; }

        /// <summary>
        /// Propriedade navegacional para o item do pedido
        /// </summary>
        public OrderItem? OrderItem { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductionOrder()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductionOrder(string orderId, string orderItemId, string productId, int quantity)
        {
            OrderId = orderId;
            OrderItemId = orderItemId;
            ProductId = productId;
            Quantity = quantity;
        }

        /// <summary>
        /// Inicia a produção
        /// </summary>
        public void StartProduction(string? assignedTo = null)
        {
            Status = ProductionOrderStatus.InProgress;
            ActualStartDate = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(assignedTo))
                AssignedTo = assignedTo;
        }

        /// <summary>
        /// Pausa a produção
        /// </summary>
        public void PauseProduction()
        {
            if (Status == ProductionOrderStatus.InProgress)
            {
                Status = ProductionOrderStatus.Paused;
            }
        }

        /// <summary>
        /// Retoma a produção
        /// </summary>
        public void ResumeProduction()
        {
            if (Status == ProductionOrderStatus.Paused)
            {
                Status = ProductionOrderStatus.InProgress;
            }
        }

        /// <summary>
        /// Completa a produção
        /// </summary>
        public void CompleteProduction()
        {
            Status = ProductionOrderStatus.Completed;
            ActualEndDate = DateTime.UtcNow;
            
            if (ActualStartDate.HasValue && ActualEndDate.HasValue)
            {
                var duration = ActualEndDate.Value - ActualStartDate.Value;
                ActualTime = (int)duration.TotalMinutes;
            }
        }

        /// <summary>
        /// Cancela a produção
        /// </summary>
        public void CancelProduction()
        {
            Status = ProductionOrderStatus.Cancelled;
        }

        /// <summary>
        /// Marca a produção como falhada
        /// </summary>
        public void FailProduction()
        {
            Status = ProductionOrderStatus.Failed;
            ActualEndDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se a produção está em andamento
        /// </summary>
        public bool IsInProgress()
        {
            return Status == ProductionOrderStatus.InProgress;
        }

        /// <summary>
        /// Verifica se a produção está completa
        /// </summary>
        public bool IsCompleted()
        {
            return Status == ProductionOrderStatus.Completed;
        }

        /// <summary>
        /// Verifica se a produção pode ser iniciada
        /// </summary>
        public bool CanStart()
        {
            return Status == ProductionOrderStatus.Pending || Status == ProductionOrderStatus.Scheduled;
        }

        /// <summary>
        /// Obtém a descrição do status
        /// </summary>
        public string GetStatusDescription()
        {
            return Status switch
            {
                ProductionOrderStatus.Pending => "Pendente",
                ProductionOrderStatus.Scheduled => "Agendado",
                ProductionOrderStatus.InProgress => "Em Andamento",
                ProductionOrderStatus.Paused => "Pausado",
                ProductionOrderStatus.Completed => "Concluído",
                ProductionOrderStatus.Cancelled => "Cancelado",
                ProductionOrderStatus.Failed => "Falhou",
                _ => "Status desconhecido"
            };
        }

        /// <summary>
        /// Obtém a descrição da prioridade
        /// </summary>
        public string GetPriorityDescription()
        {
            return Priority switch
            {
                ProductionOrderPriority.Low => "Baixa",
                ProductionOrderPriority.Normal => "Normal",
                ProductionOrderPriority.High => "Alta",
                ProductionOrderPriority.Urgent => "Urgente",
                _ => "Prioridade desconhecida"
            };
        }

        /// <summary>
        /// Obtém informações de tempo formatadas
        /// </summary>
        public string GetTimeInfo()
        {
            if (EstimatedTime.HasValue && ActualTime.HasValue)
            {
                var estimated = FormatMinutes(EstimatedTime.Value);
                var actual = FormatMinutes(ActualTime.Value);
                return $"Est: {estimated} | Real: {actual}";
            }

            if (EstimatedTime.HasValue)
            {
                return $"Estimado: {FormatMinutes(EstimatedTime.Value)}";
            }

            if (ActualTime.HasValue)
            {
                return $"Real: {FormatMinutes(ActualTime.Value)}";
            }

            return "Tempo não informado";
        }

        /// <summary>
        /// Formata minutos em horas e minutos
        /// </summary>
        private string FormatMinutes(int minutes)
        {
            var hours = minutes / 60;
            var mins = minutes % 60;

            if (hours > 0)
                return $"{hours}h {mins}min";

            return $"{mins}min";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(OrderId) &&
                   !string.IsNullOrWhiteSpace(OrderItemId) &&
                   !string.IsNullOrWhiteSpace(ProductId) &&
                   Quantity > 0;
        }

        /// <summary>
        /// Override do ToString para exibir resumo da ordem
        /// </summary>
        public override string ToString()
        {
            var productName = Product?.Name ?? "Produto";
            var statusDesc = GetStatusDescription();
            return $"{productName} (Qtd: {Quantity}) - {statusDesc}";
        }
    }
} 