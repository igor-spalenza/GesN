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
        /// Preço do produto
        /// </summary>
        [Display(Name = "Preço")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero")]
        public decimal Price { get; set; }

        /// <summary>
        /// Preço baseado em quantidade
        /// </summary>
        [Display(Name = "Preço por Quantidade")]
        public int QuantityPrice { get; set; } = 0;

        /// <summary>
        /// Preço unitário do produto
        /// </summary>
        [Display(Name = "Preço Unitário")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço unitário deve ser maior ou igual a zero")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Custo do produto
        /// </summary>
        [Display(Name = "Custo")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        public decimal Cost { get; set; }

        /// <summary>
        /// ID da categoria do produto
        /// </summary>
        [Display(Name = "Categoria")]
        public string? CategoryId { get; set; }

        /// <summary>
        /// Nome da categoria do produto
        /// </summary>
        [Display(Name = "Nome da Categoria")]
        public string? Category { get; set; }

        /// <summary>
        /// Código SKU do produto
        /// </summary>
        [StringLength(50)]
        [Display(Name = "SKU")]
        public string? SKU { get; set; }

        /// <summary>
        /// URL da imagem do produto
        /// </summary>
        [Display(Name = "Imagem")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Notas sobre o produto
        /// </summary>
        [Display(Name = "Observações")]
        public string? Note { get; set; }

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
        public ProductCategory? CategoryNavigation { get; set; }

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
            return $"R$ {Price:N2}";
        }

        /// <summary>
        /// Obtém informações de preço unitário formatadas
        /// </summary>
        public string GetUnitPriceInfo()
        {
            return $"R$ {UnitPrice:N2}";
        }

        /// <summary>
        /// Obtém informações do tempo de montagem formatadas
        /// </summary>
        public string GetAssemblyTimeInfo()
        {
            if (AssemblyTime <= 0)
                return "Sem montagem";
            
            var hours = AssemblyTime / 60;
            var minutes = AssemblyTime % 60;
            
            if (hours > 0)
                return $"{hours}h {minutes}min";
            
            return $"{minutes}min";
        }

        /// <summary>
        /// Verifica se o produto requer montagem
        /// </summary>
        public bool RequiresAssembly()
        {
            return AssemblyTime > 0;
        }

        /// <summary>
        /// Verifica se o produto possui dados básicos completos
        /// </summary>
        public virtual bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name) && Price >= 0;
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