using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Classe abstrata base para produtos
    /// </summary>
    public abstract class Product : Entity
    {
        /// <summary>
        /// Tipo do produto (determinado pela classe concreta)
        /// </summary>
        [Display(Name = "Tipo de Produto")]
        public virtual ProductType ProductType { get; protected set; }

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
        /// Estoque mínimo
        /// </summary>
        [Display(Name = "Estoque Mínimo")]
        public int MinStock { get; set; } = 0;

        /// <summary>
        /// Estoque atual
        /// </summary>
        [Display(Name = "Estoque Atual")]
        public int CurrentStock { get; set; } = 0;

        /// <summary>
        /// Unidade de medida
        /// </summary>
        [Display(Name = "Unidade")]
        [MaxLength(10)]
        public string Unit { get; set; } = "UN";

        /// <summary>
        /// Permite customização
        /// </summary>
        [Display(Name = "Permite Customização")]
        public bool AllowCustomization { get; set; } = false;

        /// <summary>
        /// Quantidade mínima de itens necessários (para ProductGroup)
        /// </summary>
        [Display(Name = "Itens Mínimos Necessários")]
        public int MinItemsRequired { get; set; } = 1;

        /// <summary>
        /// Quantidade máxima de itens permitidos (para ProductGroup)
        /// </summary>
        [Display(Name = "Itens Máximos Permitidos")]
        public int? MaxItemsAllowed { get; set; }

        /// <summary>
        /// Tempo de montagem em minutos
        /// </summary>
        [Display(Name = "Tempo de Montagem (min)")]
        public int AssemblyTime { get; set; } = 0;

        /// <summary>
        /// Instruções de montagem
        /// </summary>
        [Display(Name = "Instruções de Montagem")]
        public string? AssemblyInstructions { get; set; }

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