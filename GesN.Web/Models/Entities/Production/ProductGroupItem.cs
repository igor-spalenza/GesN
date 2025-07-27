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
        [Display(Name = "Produto")]
        public string? ProductId { get; set; }

        /// <summary>
        /// ID da categoria de produto que faz parte do grupo
        /// </summary>
        [Display(Name = "Categoria de Produto")]
        public string? ProductCategoryId { get; set; }

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
        /// Propriedade navegacional para a categoria de produto
        /// </summary>
        public ProductCategory? ProductCategory { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroupItem()
        {
        }

        /// <summary>
        /// Construtor com dados básicos - para produto
        /// </summary>
        public ProductGroupItem(string productGroupId, string productId, int quantity = 1)
        {
            ProductGroupId = productGroupId;
            ProductId = productId;
            Quantity = quantity;
        }

        /// <summary>
        /// Construtor com dados básicos - para categoria de produto
        /// </summary>
        public ProductGroupItem(string productGroupId, string productCategoryId, int quantity = 1, bool isCategory = true)
        {
            ProductGroupId = productGroupId;
            ProductCategoryId = productCategoryId;
            Quantity = quantity;
        }

        /// <summary>
        /// Calcula o preço efetivo do item no grupo
        /// </summary>
        public decimal GetEffectivePrice()
        {
            decimal basePrice = 0;

            if (Product != null)
                basePrice = Product.UnitPrice;
            else if (ProductCategory != null)
                basePrice = 0; // Categorias não têm preço direto

            return basePrice + ExtraPrice;
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            if (Product != null)
                return Product.Name;
            
            if (ProductCategory != null)
                return ProductCategory.Name;
            
            return "Item sem nome";
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
        /// Obtém o status de disponibilidade baseado no estado do produto ou categoria
        /// </summary>
        public string GetAvailabilityStatus()
        {
            if (Product != null)
            {
                if (Product.StateCode != ObjectState.Active)
                    return "❌ Produto inativo";
                return "✅ Produto disponível";
            }

            if (ProductCategory != null)
            {
                if (ProductCategory.StateCode != ObjectState.Active)
                    return "❌ Categoria inativa";
                return "✅ Categoria disponível";
            }

            return "❌ Item não encontrado";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductGroupId) &&
                   ((!string.IsNullOrWhiteSpace(ProductId) && string.IsNullOrWhiteSpace(ProductCategoryId)) ||
                    (string.IsNullOrWhiteSpace(ProductId) && !string.IsNullOrWhiteSpace(ProductCategoryId))) &&
                   Quantity > 0;
        }

        /// <summary>
        /// Valida se apenas um tipo de item foi especificado (Produto OU Categoria)
        /// </summary>
        public bool IsValidItemConfiguration()
        {
            bool hasProduct = !string.IsNullOrWhiteSpace(ProductId);
            bool hasCategory = !string.IsNullOrWhiteSpace(ProductCategoryId);

            // Deve ter exatamente um dos dois
            return hasProduct ^ hasCategory;
        }

        /// <summary>
        /// Obtém o tipo do item
        /// </summary>
        public string GetItemType()
        {
            if (!string.IsNullOrWhiteSpace(ProductId))
                return "Produto";
            
            if (!string.IsNullOrWhiteSpace(ProductCategoryId))
                return "Categoria";
            
            return "Indefinido";
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