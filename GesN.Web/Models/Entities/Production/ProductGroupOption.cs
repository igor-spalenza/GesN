using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa opções de configuração para grupos de produtos
    /// </summary>
    public class ProductGroupOption : Entity
    {
        /// <summary>
        /// ID do grupo de produtos
        /// </summary>
        [Required(ErrorMessage = "O grupo de produtos é obrigatório")]
        [Display(Name = "Grupo de Produtos")]
        public string ProductGroupId { get; set; } = string.Empty;

        /// <summary>
        /// Nome da opção
        /// </summary>
        [Required(ErrorMessage = "O nome da opção é obrigatório")]
        [Display(Name = "Nome da Opção")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da opção
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Tipo da opção (seleção única ou múltipla)
        /// </summary>
        [Required(ErrorMessage = "O tipo da opção é obrigatório")]
        [Display(Name = "Tipo")]
        public ProductGroupOptionType OptionType { get; set; } = ProductGroupOptionType.Single;

        /// <summary>
        /// Indica se a opção é obrigatória
        /// </summary>
        [Display(Name = "Obrigatória")]
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Ordem de apresentação
        /// </summary>
        [Display(Name = "Ordem")]
        [Range(1, int.MaxValue, ErrorMessage = "A ordem deve ser maior que zero")]
        public int DisplayOrder { get; set; } = 1;

        /// <summary>
        /// Propriedade navegacional para o grupo de produtos
        /// </summary>
        public Product? ProductGroup { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroupOption()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductGroupOption(string productGroupId, string name, ProductGroupOptionType optionType = ProductGroupOptionType.Single)
        {
            ProductGroupId = productGroupId;
            Name = name;
            OptionType = optionType;
        }

        /// <summary>
        /// Obtém a descrição do tipo de opção
        /// </summary>
        public string GetOptionTypeDescription()
        {
            return OptionType switch
            {
                ProductGroupOptionType.Single => "Seleção única",
                ProductGroupOptionType.Multiple => "Seleção múltipla",
                _ => "Não definido"
            };
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductGroupId) &&
                   !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// Override do ToString para exibir resumo da opção
        /// </summary>
        public override string ToString()
        {
            var typeInfo = GetOptionTypeDescription();
            var requiredInfo = IsRequired ? " (Obrigatória)" : "";
            return $"{Name} - {typeInfo}{requiredInfo}";
        }
    }
} 