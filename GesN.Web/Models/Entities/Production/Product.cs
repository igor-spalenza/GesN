using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa um produto
    /// </summary>
    public class Product : Entity
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
        public decimal Price { get; set; }

        /// <summary>
        /// Indica se o produto está ativo
        /// </summary>
        [Display(Name = "Ativo")]
        public new bool IsActive { get; set; } = true;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Product()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
} 