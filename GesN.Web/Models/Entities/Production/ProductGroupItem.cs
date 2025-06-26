using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade de relacionamento entre grupo de produtos e produtos individuais
    /// </summary>
    public class ProductGroupItem : Entity
    {
        /// <summary>
        /// ID do grupo de produtos
        /// </summary>
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        /// <summary>
        /// ID do produto que faz parte do grupo
        /// </summary>
        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade do item no grupo
        /// </summary>
        [Display(Name = "Quantidade")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Quantidade mínima necessária
        /// </summary>
        [Display(Name = "Quantidade Mínima")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade mínima deve ser maior que zero")]
        public int MinQuantity { get; set; } = 1;

        /// <summary>
        /// Quantidade máxima permitida
        /// </summary>
        [Display(Name = "Quantidade Máxima")]
        public int? MaxQuantity { get; set; }

        /// <summary>
        /// Quantidade padrão sugerida
        /// </summary>
        [Display(Name = "Quantidade Padrão")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade padrão deve ser maior que zero")]
        public int DefaultQuantity { get; set; } = 1;

        /// <summary>
        /// Indica se o item é opcional no grupo
        /// </summary>
        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Preço extra para este item
        /// </summary>
        [Display(Name = "Preço Extra")]
        public decimal ExtraPrice { get; set; } = 0;

        /// <summary>
        /// Propriedade navegacional para o grupo de produtos
        /// </summary>
        public Product? ProductGroup { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroupItem()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductGroupItem(string productGroupId, string productId, int quantity = 1)
        {
            ProductGroupId = productGroupId;
            ProductId = productId;
            Quantity = quantity;
        }

        /// <summary>
        /// Calcula o preço efetivo do item no grupo
        /// </summary>
        public decimal GetEffectivePrice()
        {
            if (Product == null) return ExtraPrice;

            return Product.UnitPrice + ExtraPrice;
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            return Product?.Name ?? "Item sem nome";
        }

        /// <summary>
        /// Obtém informações de preço formatadas
        /// </summary>
        public string GetPriceInfo()
        {
            var effectivePrice = GetEffectivePrice();
            var originalPrice = Product?.UnitPrice ?? 0;

            if (ExtraPrice > 0)
            {
                return $"R$ {effectivePrice:N2} (+R$ {ExtraPrice:N2})";
            }

            return $"R$ {originalPrice:N2}";
        }

        /// <summary>
        /// Verifica se o item tem preço extra
        /// </summary>
        public bool HasExtraPrice()
        {
            return ExtraPrice > 0;
        }

        /// <summary>
        /// Obtém o status de disponibilidade baseado no estoque
        /// </summary>
        public string GetAvailabilityStatus()
        {
            if (Product == null)
                return "❌ Produto não encontrado";

            if (Product.CurrentStock <= 0)
                return "❌ Sem estoque";

            if (Product.CurrentStock < Product.MinStock)
                return "⚠️ Estoque baixo";

            return "✅ Disponível";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductGroupId) &&
                   !string.IsNullOrWhiteSpace(ProductId) &&
                   MaxQuantity > 0;
        }

        /// <summary>
        /// Override do ToString para exibir resumo do item
        /// </summary>
        public override string ToString()
        {
            var name = GetDisplayName();
            var price = GetPriceInfo();
            return $"{name} - {price}";
        }
    }
} 