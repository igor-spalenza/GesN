using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Classe abstrata base para produtos
    /// </summary>
    public abstract class Product : Entity
    {
        /// <summary>
        /// Nome do produto
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "Nome")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do produto
        /// </summary>
        [StringLength(500)]
        [Display(Name = "Descrição")]
        public string? Description { get; set; }

        /// <summary>
        /// Código/SKU do produto
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Código")]
        public string? Code { get; set; }

        /// <summary>
        /// Preço unitário do produto
        /// </summary>
        [Display(Name = "Preço")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Custo do produto
        /// </summary>
        [Display(Name = "Custo")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        public decimal Cost { get; set; }

        /// <summary>
        /// Categoria do produto
        /// </summary>
        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        /// <summary>
        /// Fornecedor do produto
        /// </summary>
        [Display(Name = "Fornecedor")]
        public string? SupplierId { get; set; }



        /// <summary>
        /// Propriedade navegacional para categoria
        /// </summary>
        public ProductCategory? Category { get; set; }

        /// <summary>
        /// Propriedade navegacional para fornecedor
        /// </summary>
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Product(string name, decimal unitPrice)
        {
            Name = name;
            UnitPrice = unitPrice;
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Produto sem nome" : Name;
        }

        /// <summary>
        /// Obtém informações de preço formatadas
        /// </summary>
        public string GetPriceInfo()
        {
            return $"R$ {UnitPrice:N2}";
        }

        /// <summary>
        /// Verifica se o produto possui dados básicos completos
        /// </summary>
        public virtual bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name) && UnitPrice >= 0;
        }

        /// <summary>
        /// Override do ToString para exibir resumo do produto
        /// </summary>
        public override string ToString()
        {
            return $"{GetDisplayName()} - {GetPriceInfo()}";
        }
    }
} 