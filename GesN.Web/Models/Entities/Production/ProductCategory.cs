using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa uma categoria de produto
    /// </summary>
    public class ProductCategory : Entity
    {
        /// <summary>
        /// Nome da categoria
        /// </summary>
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da categoria
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(500)]
        public string? Description { get; set; }



        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductCategory()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductCategory(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Categoria sem nome" : Name;
        }

        /// <summary>
        /// Verifica se a categoria possui dados básicos completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// Override do ToString para exibir nome da categoria
        /// </summary>
        public override string ToString()
        {
            return GetDisplayName();
        }
    }
} 