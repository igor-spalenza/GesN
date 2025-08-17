using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Entities.Production;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Sales
{
    /// <summary>
    /// Entidade OrderItem (Item do Pedido) do domínio de vendas
    /// </summary>
    public class OrderItem : Entity
    {
        /// <summary>
        /// ID do pedido
        /// </summary>
        [Required]
        [Display(Name = "Pedido")]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// Pedido (navegação)
        /// </summary>
        public OrderEntry? Order { get; set; }

        /// <summary>
        /// ID do produto
        /// </summary>
        [Required]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Produto (navegação)
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Quantidade do item
        /// </summary>
        [Required]
        [Display(Name = "Quantidade")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantity { get; set; }

        /// <summary>
        /// Preço unitário do item
        /// </summary>
        [Required]
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Desconto aplicado ao item
        /// </summary>
        [Required]
        [Display(Name = "Desconto")]
        [DataType(DataType.Currency)]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Valor dos impostos do item
        /// </summary>
        [Required]
        [Display(Name = "Impostos")]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Observações do item
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public OrderItem() { }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public OrderItem(string orderId, string productId, int quantity, decimal unitPrice)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        /// <summary>
        /// Preço total do item (sem impostos e descontos)
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;

        /// <summary>
        /// Preço total do item (com impostos e descontos)
        /// </summary>
        public decimal TotalPrice => Subtotal + TaxAmount - DiscountAmount;

        /// <summary>
        /// Atualiza a quantidade do item
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero", nameof(newQuantity));

            Quantity = newQuantity;
            UpdateModification();
        }

        /// <summary>
        /// Atualiza o preço unitário do item
        /// </summary>
        public void UpdateUnitPrice(decimal newUnitPrice)
        {
            if (newUnitPrice < 0)
                throw new ArgumentException("O preço unitário não pode ser negativo", nameof(newUnitPrice));

            UnitPrice = newUnitPrice;
            UpdateModification();
        }

        /// <summary>
        /// Atualiza o desconto do item
        /// </summary>
        public void UpdateDiscount(decimal newDiscount)
        {
            if (newDiscount < 0)
                throw new ArgumentException("O desconto não pode ser negativo", nameof(newDiscount));

            if (newDiscount > Subtotal)
                throw new ArgumentException("O desconto não pode ser maior que o subtotal", nameof(newDiscount));

            DiscountAmount = newDiscount;
            UpdateModification();
        }

        /// <summary>
        /// Atualiza o valor dos impostos do item
        /// </summary>
        public void UpdateTax(decimal newTax)
        {
            if (newTax < 0)
                throw new ArgumentException("O valor dos impostos não pode ser negativo", nameof(newTax));

            TaxAmount = newTax;
            UpdateModification();
        }
    }
} 