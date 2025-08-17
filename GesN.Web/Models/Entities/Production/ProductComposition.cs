using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade ProductComposition - relaciona demandas com componentes específicos
    /// Representa a composição específica de um produto para uma demanda
    /// </summary>
    public class ProductComposition : Entity
    {
        /// <summary>
        /// ID da demanda que originou esta composição
        /// </summary>
        [Required(ErrorMessage = "A demanda é obrigatória")]
        [Display(Name = "Demanda")]
        public string DemandId { get; set; } = string.Empty;

        /// <summary>
        /// ID do componente de produto selecionado
        /// </summary>
        [Required(ErrorMessage = "O componente do produto é obrigatório")]
        [Display(Name = "Componente do Produto")]
        public string ProductComponentId { get; set; } = string.Empty;

        /// <summary>
        /// Nome da hierarquia para organização e referência
        /// </summary>
        [Required(ErrorMessage = "O nome da hierarquia é obrigatório")]
        [Display(Name = "Nome da Hierarquia")]
        [MaxLength(200)]
        public string HierarchyName { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade específica para esta composição
        /// </summary>
        [Display(Name = "Quantidade")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public decimal Quantity { get; set; } = 1;

        /// <summary>
        /// Unidade de medida da quantidade
        /// </summary>
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; } = ProductionUnit.Unidades;

        /// <summary>
        /// Indica se este componente é opcional para esta demanda específica
        /// </summary>
        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Ordem de processamento nesta composição
        /// </summary>
        [Display(Name = "Ordem de Processamento")]
        [Range(0, int.MaxValue, ErrorMessage = "A ordem deve ser maior ou igual a zero")]
        public int ProcessingOrder { get; set; } = 0;

        /// <summary>
        /// Indica se esta composição está ativa
        /// </summary>
        [Display(Name = "Ativa")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Observações específicas sobre esta composição
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Data de início do processamento deste componente
        /// </summary>
        [Display(Name = "Data de Início")]
        [DataType(DataType.DateTime)]
        public DateTime? ProcessingStartedAt { get; set; }

        /// <summary>
        /// Data de conclusão do processamento deste componente
        /// </summary>
        [Display(Name = "Data de Conclusão")]
        [DataType(DataType.DateTime)]
        public DateTime? ProcessingCompletedAt { get; set; }

        // Propriedades de Navegação

        /// <summary>
        /// Demanda que originou esta composição
        /// </summary>
        public Demand? Demand { get; set; }

        /// <summary>
        /// Componente do produto selecionado
        /// </summary>
        public ProductComponent? ProductComponent { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductComposition()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductComposition(string demandId, string productComponentId, string hierarchyName)
        {
            DemandId = demandId;
            ProductComponentId = productComponentId;
            HierarchyName = hierarchyName;
        }

        // Métodos de Negócio

        /// <summary>
        /// Verifica se esta composição está em processamento
        /// </summary>
        public bool IsInProcessing()
        {
            return IsActive && ProcessingStartedAt.HasValue && !ProcessingCompletedAt.HasValue;
        }

        /// <summary>
        /// Verifica se esta composição foi concluída
        /// </summary>
        public bool IsCompleted()
        {
            return ProcessingCompletedAt.HasValue;
        }

        /// <summary>
        /// Inicia o processamento desta composição
        /// </summary>
        public void StartProcessing()
        {
            if (IsActive && !ProcessingStartedAt.HasValue)
            {
                ProcessingStartedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Completa o processamento desta composição
        /// </summary>
        public void CompleteProcessing()
        {
            if (IsInProcessing())
            {
                ProcessingCompletedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Obtém a descrição do status formatada
        /// </summary>
        public string GetStatusDisplay()
        {
            if (!IsActive)
                return "Inativa";
            
            if (ProcessingCompletedAt.HasValue)
                return "Concluída";
            
            if (ProcessingStartedAt.HasValue)
                return "Em Processamento";
            
            return "Pendente";
        }

        /// <summary>
        /// Obtém a descrição da quantidade formatada
        /// </summary>
        public string GetQuantityDisplay()
        {
            var unitDisplay = Unit switch
            {
                ProductionUnit.Unidades => "UN",
                ProductionUnit.Quilogramas => "KG",
                ProductionUnit.Gramas => "G",
                ProductionUnit.Litros => "L",
                ProductionUnit.Mililitros => "ML",
                _ => Unit.ToString()
            };
            
            return $"{Quantity:N2} {unitDisplay}";
        }

        /// <summary>
        /// Obtém informações resumidas da composição
        /// </summary>
        public string GetSummary()
        {
            var componentName = ProductComponent?.Name ?? "Componente";
            var status = GetStatusDisplay();
            return $"{HierarchyName} - {componentName}: {GetQuantityDisplay()} - {status}";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(DemandId) &&
                   !string.IsNullOrWhiteSpace(ProductComponentId) &&
                   !string.IsNullOrWhiteSpace(HierarchyName) &&
                   Quantity > 0;
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