using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa regras de troca entre itens de grupos de produtos
    /// Tabela: ProductGroupExchangeRule
    /// Nova modelagem: relaciona itens de grupo (ProductGroupItem) com pesos para trocas flexíveis
    /// </summary>
    public class ProductGroupExchangeRule : Entity
    {
        /// <summary>
        /// ID do grupo de produtos
        /// </summary>
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        /// <summary>
        /// ID do item de grupo origem (que será trocado)
        /// </summary>
        [Required(ErrorMessage = "O item de grupo origem é obrigatório")]
        [Display(Name = "Item de Grupo Origem")]
        public string SourceGroupItemId { get; set; } = string.Empty;

        /// <summary>
        /// Peso do item de grupo origem na troca
        /// </summary>
        [Required(ErrorMessage = "O peso do item origem é obrigatório")]
        [Display(Name = "Peso do Item Origem")]
        [Range(1, int.MaxValue, ErrorMessage = "O peso deve ser maior que zero")]
        public int SourceGroupItemWeight { get; set; } = 1;

        /// <summary>
        /// ID do item de grupo destino (que será recebido)
        /// </summary>
        [Required(ErrorMessage = "O item de grupo destino é obrigatório")]
        [Display(Name = "Item de Grupo Destino")]
        public string TargetGroupItemId { get; set; } = string.Empty;

        /// <summary>
        /// Peso do item de grupo destino na troca
        /// </summary>
        [Required(ErrorMessage = "O peso do item destino é obrigatório")]
        [Display(Name = "Peso do Item Destino")]
        [Range(1, int.MaxValue, ErrorMessage = "O peso deve ser maior que zero")]
        public int TargetGroupItemWeight { get; set; } = 1;

        /// <summary>
        /// Proporção da troca (ExchangeRatio na tabela)
        /// </summary>
        [Display(Name = "Proporção de Troca")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A proporção deve ser maior que zero")]
        public decimal ExchangeRatio { get; set; } = 1;

        /// <summary>
        /// Indica se a regra está ativa
        /// </summary>
        [Display(Name = "Ativa")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Propriedade navegacional para o grupo de produtos
        /// </summary>
        public Product? ProductGroup { get; set; }

        /// <summary>
        /// Propriedade navegacional para o item de grupo origem
        /// </summary>
        public ProductGroupItem? SourceGroupItem { get; set; }

        /// <summary>
        /// Propriedade navegacional para o item de grupo destino
        /// </summary>
        public ProductGroupItem? TargetGroupItem { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroupExchangeRule()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductGroupExchangeRule(string productGroupId, string sourceGroupItemId, string targetGroupItemId)
        {
            ProductGroupId = productGroupId;
            SourceGroupItemId = sourceGroupItemId;
            TargetGroupItemId = targetGroupItemId;
        }

        /// <summary>
        /// Construtor com dados completos
        /// </summary>
        public ProductGroupExchangeRule(string productGroupId, string sourceGroupItemId, int sourceWeight, string targetGroupItemId, int targetWeight, decimal exchangeRatio = 1)
        {
            ProductGroupId = productGroupId;
            SourceGroupItemId = sourceGroupItemId;
            SourceGroupItemWeight = sourceWeight;
            TargetGroupItemId = targetGroupItemId;
            TargetGroupItemWeight = targetWeight;
            ExchangeRatio = exchangeRatio;
        }

        /// <summary>
        /// Calcula a proporção efetiva da troca considerando os pesos
        /// </summary>
        public decimal CalculateEffectiveRatio()
        {
            if (TargetGroupItemWeight == 0) return 0;
            
            return ExchangeRatio * (decimal)SourceGroupItemWeight / TargetGroupItemWeight;
        }

        /// <summary>
        /// Verifica se a troca pode ser aplicada
        /// </summary>
        public bool CanApplyExchange()
        {
            if (!IsActive)
                return false;

            // Verifica se os itens de grupo estão disponíveis
            if (SourceGroupItem?.Product != null && TargetGroupItem?.Product != null)
            {
                return SourceGroupItem.Product.IsActive && TargetGroupItem.Product.IsActive;
            }

            return false;
        }

        /// <summary>
        /// Obtém a descrição da proporção de troca incluindo pesos
        /// </summary>
        public string GetExchangeRatioDescription()
        {
            var effectiveRatio = CalculateEffectiveRatio();
            
            if (SourceGroupItemWeight == 1 && TargetGroupItemWeight == 1 && ExchangeRatio == 1)
                return "1:1";

            return $"{SourceGroupItemWeight}:{TargetGroupItemWeight} (Taxa {ExchangeRatio:N2})";
        }

        /// <summary>
        /// Obtém informações sobre os pesos dos itens
        /// </summary>
        public string GetWeightInfo()
        {
            if (SourceGroupItemWeight == 1 && TargetGroupItemWeight == 1)
                return "Pesos equilibrados";

            return $"Origem: {SourceGroupItemWeight} | Destino: {TargetGroupItemWeight}";
        }

        /// <summary>
        /// Obtém o status da regra
        /// </summary>
        public string GetRuleStatus()
        {
            if (!IsActive)
                return "❌ Inativa";

            if (!CanApplyExchange())
                return "⚠️ Itens de grupo indisponíveis";

            return "✅ Ativa e disponível";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductGroupId) &&
                   !string.IsNullOrWhiteSpace(SourceGroupItemId) &&
                   !string.IsNullOrWhiteSpace(TargetGroupItemId) &&
                   SourceGroupItemWeight > 0 &&
                   TargetGroupItemWeight > 0 &&
                   ExchangeRatio > 0 &&
                   SourceGroupItemId != TargetGroupItemId;
        }

        /// <summary>
        /// Obtém o nome do produto do item origem
        /// </summary>
        public string GetSourceProductName()
        {
            return SourceGroupItem?.Product?.Name ?? "Item não encontrado";
        }

        /// <summary>
        /// Obtém o nome do produto do item destino
        /// </summary>
        public string GetTargetProductName()
        {
            return TargetGroupItem?.Product?.Name ?? "Item não encontrado";
        }

        /// <summary>
        /// Obtém a descrição completa da regra de troca
        /// </summary>
        public string GetCompleteDescription()
        {
            var sourceName = GetSourceProductName();
            var targetName = GetTargetProductName();
            var ratio = GetExchangeRatioDescription();
            
            return $"{sourceName} → {targetName} ({ratio})";
        }

        /// <summary>
        /// Override do ToString para exibir resumo da regra
        /// </summary>
        public override string ToString()
        {
            var ratio = GetExchangeRatioDescription();
            var weightInfo = GetWeightInfo();
            return $"Troca ({ratio}) - {weightInfo}";
        }
    }
} 