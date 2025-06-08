using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Sales
{
    /// <summary>
    /// Entidade Contract (Contrato) do domínio de vendas
    /// </summary>
    public class Contract : Entity
    {
        /// <summary>
        /// Número do contrato
        /// </summary>
        [Required(ErrorMessage = "O número do contrato é obrigatório")]
        [Display(Name = "Número do Contrato")]
        [MaxLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        /// <summary>
        /// Título/Nome do contrato
        /// </summary>
        [Required(ErrorMessage = "O título do contrato é obrigatório")]
        [Display(Name = "Título do Contrato")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do contrato
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Data de início do contrato
        /// </summary>
        [Required(ErrorMessage = "A data de início é obrigatória")]
        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        /// <summary>
        /// Data de fim do contrato
        /// </summary>
        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Valor total do contrato
        /// </summary>
        [Required(ErrorMessage = "O valor do contrato é obrigatório")]
        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Status do contrato
        /// </summary>
        [Required]
        [Display(Name = "Status")]
        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        /// <summary>
        /// ID do cliente (FK)
        /// </summary>
        [Required(ErrorMessage = "O cliente é obrigatório")]
        [Display(Name = "Cliente")]
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Navegação para o cliente
        /// </summary>
        public virtual Customer? Customer { get; set; }

        /// <summary>
        /// Termos e condições do contrato
        /// </summary>
        [Display(Name = "Termos e Condições")]
        [MaxLength(5000)]
        public string? TermsAndConditions { get; set; }

        /// <summary>
        /// Data de assinatura do contrato
        /// </summary>
        [Display(Name = "Data de Assinatura")]
        [DataType(DataType.DateTime)]
        public DateTime? SignedDate { get; set; }

        /// <summary>
        /// Nome de quem assinou pelo cliente
        /// </summary>
        [Display(Name = "Assinado Por")]
        [MaxLength(200)]
        public string? SignedByCustomer { get; set; }

        /// <summary>
        /// Observações do contrato
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Contract() { }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Contract(string title, string customerId, decimal totalValue)
        {
            Title = title;
            CustomerId = customerId;
            TotalValue = totalValue;
            ContractNumber = GenerateContractNumber();
        }

        /// <summary>
        /// Gera número do contrato automaticamente
        /// </summary>
        private static string GenerateContractNumber()
        {
            var year = DateTime.Now.Year;
            var timestamp = DateTime.Now.ToString("MMddHHmmss");
            return $"CONT{year}{timestamp}";
        }

        /// <summary>
        /// Confirma o contrato (sai do status Draft)
        /// </summary>
        public void Confirm(string confirmedBy)
        {
            if (Status == ContractStatus.Draft)
            {
                Status = ContractStatus.Active;
                UpdateModification(confirmedBy);
            }
        }

        /// <summary>
        /// Assina o contrato
        /// </summary>
        public void Sign(string signedBy, DateTime? signDate = null)
        {
            SignedDate = signDate ?? DateTime.Now;
            SignedByCustomer = signedBy;
            
            if (Status == ContractStatus.Active)
            {
                Status = ContractStatus.Signed;
            }
            
            UpdateModification();
        }

        /// <summary>
        /// Suspende o contrato
        /// </summary>
        public void Suspend(string suspendedBy)
        {
            if (Status == ContractStatus.Active || Status == ContractStatus.Signed)
            {
                Status = ContractStatus.Suspended;
                UpdateModification(suspendedBy);
            }
        }

        /// <summary>
        /// Cancela o contrato
        /// </summary>
        public void Cancel(string cancelledBy)
        {
            Status = ContractStatus.Cancelled;
            UpdateModification(cancelledBy);
        }

        /// <summary>
        /// Finaliza o contrato
        /// </summary>
        public void Complete(string completedBy)
        {
            if (Status == ContractStatus.Signed || Status == ContractStatus.Active)
            {
                Status = ContractStatus.Completed;
                UpdateModification(completedBy);
            }
        }

        /// <summary>
        /// Renova o contrato
        /// </summary>
        public void Renew(DateTime newEndDate, string renewedBy)
        {
            if (Status == ContractStatus.Completed || Status == ContractStatus.Active)
            {
                EndDate = newEndDate;
                Status = ContractStatus.Renewed;
                UpdateModification(renewedBy);
            }
        }

        /// <summary>
        /// Verifica se o contrato está ativo
        /// </summary>
        public bool IsActive()
        {
            return Status == ContractStatus.Active || 
                   Status == ContractStatus.Signed ||
                   Status == ContractStatus.Renewed;
        }

        /// <summary>
        /// Verifica se o contrato está vencido
        /// </summary>
        public bool IsExpired()
        {
            return EndDate.HasValue && 
                   EndDate.Value < DateTime.Today &&
                   IsActive();
        }

        /// <summary>
        /// Verifica se o contrato está próximo do vencimento (30 dias)
        /// </summary>
        public bool IsNearExpiration()
        {
            return EndDate.HasValue && 
                   EndDate.Value > DateTime.Today &&
                   EndDate.Value <= DateTime.Today.AddDays(30) &&
                   IsActive();
        }

        /// <summary>
        /// Calcula a duração do contrato em dias
        /// </summary>
        public int GetDurationInDays()
        {
            var endDate = EndDate ?? DateTime.Today;
            return (endDate - StartDate).Days;
        }

        /// <summary>
        /// Obtém o status formatado para exibição
        /// </summary>
        public string GetStatusDisplay()
        {
            return Status switch
            {
                ContractStatus.Draft => "📝 Rascunho",
                ContractStatus.Active => "✅ Ativo",
                ContractStatus.Signed => "📋 Assinado",
                ContractStatus.Suspended => "⏸️ Suspenso",
                ContractStatus.Cancelled => "❌ Cancelado",
                ContractStatus.Completed => "🏁 Finalizado",
                ContractStatus.Renewed => "🔄 Renovado",
                ContractStatus.Expired => "⏰ Expirado",
                _ => Status.ToString()
            };
        }

        /// <summary>
        /// Obtém um resumo do contrato
        /// </summary>
        public string GetContractSummary()
        {
            var summary = $"{ContractNumber} - {Title}";
            
            if (Customer != null)
            {
                summary += $" ({Customer.GetDisplayName()})";
            }
            
            summary += $" - {TotalValue:C} - {GetStatusDisplay()}";
            
            return summary;
        }

        /// <summary>
        /// Override do ToString para exibir resumo do contrato
        /// </summary>
        public override string ToString()
        {
            return GetContractSummary();
        }
    }
} 