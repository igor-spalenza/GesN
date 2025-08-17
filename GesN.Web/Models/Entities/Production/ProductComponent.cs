using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa um componente de produto
    /// </summary>
    public class ProductComponent : Entity
    {
        /// <summary>
        /// Nome do componente
        /// </summary>
        [Required(ErrorMessage = "O nome do componente é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do componente
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres")]
        public string? Description { get; set; }

        /// <summary>
        /// ID da hierarquia de componentes à qual este componente pertence
        /// </summary>
        [Required(ErrorMessage = "A hierarquia de componentes é obrigatória")]
        [Display(Name = "Hierarquia de Componentes")]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        /// <summary>
        /// Custo adicional do componente
        /// </summary>
        [Display(Name = "Custo Adicional")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo adicional deve ser maior ou igual a zero")]
        public decimal AdditionalCost { get; set; } = 0;

        /// <summary>
        /// Propriedade navegacional para a hierarquia de componentes
        /// </summary>
        public ProductComponentHierarchy? ProductComponentHierarchy { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductComponent()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductComponent(string name, string productComponentHierarchyId, decimal additionalCost = 0)
        {
            Name = name;
            ProductComponentHierarchyId = productComponentHierarchyId;
            AdditionalCost = additionalCost;
        }

        /// <summary>
        /// Construtor completo
        /// </summary>
        public ProductComponent(string name, string? description, string productComponentHierarchyId, decimal additionalCost)
        {
            Name = name;
            Description = description;
            ProductComponentHierarchyId = productComponentHierarchyId;
            AdditionalCost = additionalCost;
        }

        /// <summary>
        /// Calcula o custo total do componente incluindo custos adicionais
        /// </summary>
        public decimal CalculateTotalCost()
        {
            decimal baseCost = 0;
            
            // Se há uma hierarquia associada, pega o custo base dela
            if (ProductComponentHierarchy != null)
            {
                // Aqui você pode implementar a lógica para calcular o custo base
                // baseado na hierarquia, por exemplo, somando custos de materiais
                baseCost = 0; // Placeholder - implementar conforme regra de negócio
            }
            
            return baseCost + AdditionalCost;
        }

        /// <summary>
        /// Verifica se o componente está ativo
        /// </summary>
        public bool IsActive()
        {
            return StateCode == ObjectState.Active;
        }

        /// <summary>
        /// Obtém informações resumidas do componente
        /// </summary>
        public string GetSummary()
        {
            var summary = Name;
            
            if (!string.IsNullOrWhiteSpace(Description))
                summary += $" - {Description}";
                
            if (AdditionalCost > 0)
                summary += $" (Custo adicional: R$ {AdditionalCost:N2})";
            
            return summary;
        }

        /// <summary>
        /// Obtém o status de ativação formatado
        /// </summary>
        public string GetStatusDisplay()
        {
            return StateCode switch
            {
                ObjectState.Active => "✅ Ativo",
                ObjectState.Inactive => "❌ Inativo",
                _ => "❓ Indefinido"
            };
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(ProductComponentHierarchyId);
        }

        /// <summary>
        /// Obtém o nome da hierarquia associada
        /// </summary>
        public string GetHierarchyName()
        {
            return ProductComponentHierarchy?.Name ?? "Hierarquia não carregada";
        }

        /// <summary>
        /// Valida se o componente está pronto para uso
        /// </summary>
        public bool IsReadyForUse()
        {
            return HasCompleteData() && IsActive();
        }

        /// <summary>
        /// Override do ToString para exibir resumo do componente
        /// </summary>
        public override string ToString()
        {
            return GetSummary();
        }

        // Métodos de compatibilidade (obsoletos)
        
        /// <summary>
        /// Verifica se o componente está disponível (compatibilidade)  
        /// </summary>
        [Obsolete("Use IsActive() - a nova estrutura não tem conceito de disponibilidade separado")]
        public bool IsAvailable()
        {
            return IsActive();
        }


    }
} 