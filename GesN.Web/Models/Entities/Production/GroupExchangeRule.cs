using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa regras de troca entre produtos em grupos
    /// Tabela: ProductGroupExchangeRule
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
        /// ID do produto original (que será trocado)
        /// </summary>
        [Required(ErrorMessage = "O produto original é obrigatório")]
        [Display(Name = "Produto Original")]
        public string OriginalProductId { get; set; } = string.Empty;

        /// <summary>
        /// ID do produto de troca (ExchangeProductId na tabela)
        /// </summary>
        [Required(ErrorMessage = "O produto de troca é obrigatório")]
        [Display(Name = "Produto de Troca")]
        public string ExchangeProductId { get; set; } = string.Empty;

        /// <summary>
        /// Proporção da troca (ExchangeRatio na tabela)
        /// </summary>
        [Display(Name = "Proporção de Troca")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A proporção deve ser maior que zero")]
        public decimal ExchangeRatio { get; set; } = 1;

        /// <summary>
        /// Custo adicional da troca (AdditionalCost na tabela)
        /// </summary>
        [Display(Name = "Custo Adicional")]
        public decimal AdditionalCost { get; set; } = 0;

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
        /// Propriedade navegacional para o produto original
        /// </summary>
        public Product? OriginalProduct { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto de troca
        /// </summary>
        public Product? ExchangeProduct { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroupExchangeRule()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductGroupExchangeRule(string productGroupId, string originalProductId, string exchangeProductId)
        {
            ProductGroupId = productGroupId;
            OriginalProductId = originalProductId;
            ExchangeProductId = exchangeProductId;
        }

        /// <summary>
        /// Calcula o custo total da troca
        /// </summary>
        public decimal CalculateExchangeCost()
        {
            return AdditionalCost * ExchangeRatio;
        }

        /// <summary>
        /// Verifica se a troca pode ser aplicada
        /// </summary>
        public bool CanApplyExchange()
        {
            if (!IsActive)
                return false;

            // Verifica se o produto de troca está disponível
            if (ExchangeProduct != null)
            {
                return ExchangeProduct.IsActive;
            }

            return false;
        }

        /// <summary>
        /// Obtém a descrição da proporção de troca
        /// </summary>
        public string GetExchangeRatioDescription()
        {
            if (ExchangeRatio == 1)
                return "1:1";

            return $"1:{ExchangeRatio:N2}";
        }

        /// <summary>
        /// Obtém informações sobre o custo adicional
        /// </summary>
        public string GetAdditionalCostInfo()
        {
            if (AdditionalCost == 0)
                return "Sem custo adicional";

            if (AdditionalCost > 0)
                return $"+R$ {AdditionalCost:N2}";

            return $"-R$ {Math.Abs(AdditionalCost):N2}";
        }

        /// <summary>
        /// Obtém o status da regra
        /// </summary>
        public string GetRuleStatus()
        {
            if (!IsActive)
                return "❌ Inativa";

            if (!CanApplyExchange())
                return "⚠️ Produto de troca indisponível";

            return "✅ Ativa e disponível";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductGroupId) &&
                   !string.IsNullOrWhiteSpace(OriginalProductId) &&
                   !string.IsNullOrWhiteSpace(ExchangeProductId) &&
                   ExchangeRatio > 0 &&
                   OriginalProductId != ExchangeProductId;
        }

        /// <summary>
        /// Override do ToString para exibir resumo da regra
        /// </summary>
        public override string ToString()
        {
            var ratio = GetExchangeRatioDescription();
            var costInfo = GetAdditionalCostInfo();
            return $"Troca ({ratio}) - {costInfo}";
        }
    }
} 