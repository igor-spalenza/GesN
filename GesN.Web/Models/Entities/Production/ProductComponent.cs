using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade de relacionamento entre produto composto e seus componentes
    /// </summary>
    public class ProductComponent : Entity
    {
        /// <summary>
        /// ID do produto composto pai
        /// </summary>
        [Required(ErrorMessage = "O produto composto é obrigatório")]
        [Display(Name = "Produto Composto")]
        public string CompositeProductId { get; set; } = string.Empty;

        /// <summary>
        /// ID do produto componente (filho)
        /// </summary>
        [Required(ErrorMessage = "O produto componente é obrigatório")]
        [Display(Name = "Produto Componente")]
        public string ComponentProductId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade necessária do componente
        /// </summary>
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Display(Name = "Quantidade")]
        [Range(0.001, double.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Unidade de medida da quantidade
        /// </summary>
        [Required(ErrorMessage = "A unidade é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; } = ProductionUnit.Unidades;

        /// <summary>
        /// Indica se o componente é opcional
        /// </summary>
        [Display(Name = "Componente Opcional")]
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Ordem de montagem (para sequência de montagem)
        /// </summary>
        [Display(Name = "Ordem de Montagem")]
        [Range(0, int.MaxValue, ErrorMessage = "A ordem deve ser maior ou igual a zero")]
        public int AssemblyOrder { get; set; } = 0;

        /// <summary>
        /// Observações sobre o componente
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto composto
        /// </summary>
        public Product? CompositeProduct { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto componente
        /// </summary>
        public Product? ComponentProduct { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductComponent()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductComponent(string compositeProductId, string componentProductId, decimal quantity, ProductionUnit unit = ProductionUnit.Unidades)
        {
            CompositeProductId = compositeProductId;
            ComponentProductId = componentProductId;
            Quantity = quantity;
            Unit = unit;
        }

        /// <summary>
        /// Calcula o custo total deste componente
        /// </summary>
        public decimal CalculateTotalCost()
        {
            if (ComponentProduct == null) return 0;
            
            return ComponentProduct.Cost * Quantity;
        }

        /// <summary>
        /// Verifica se o componente está disponível
        /// </summary>
        public bool IsAvailable()
        {
            if (ComponentProduct == null) return false;
            
            // Verifica estoque baseado nos campos MinStock/CurrentStock da tabela Product
            return ComponentProduct.CurrentStock >= Quantity;
        }

        /// <summary>
        /// Obtém a descrição formatada da quantidade
        /// </summary>
        public string GetQuantityDescription()
        {
            var description = $"{Quantity:N2} {Unit}";
            
            if (IsOptional)
                description += " (Opcional)";
            
            return description;
        }

        /// <summary>
        /// Obtém informações de disponibilidade
        /// </summary>
        public string GetAvailabilityInfo()
        {
            if (IsAvailable())
                return "✅ Disponível";
            
            return "❌ Indisponível";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(CompositeProductId) &&
                   !string.IsNullOrWhiteSpace(ComponentProductId) &&
                   Quantity > 0;
        }

        /// <summary>
        /// Override do ToString para exibir resumo do componente
        /// </summary>
        public override string ToString()
        {
            var productName = ComponentProduct?.Name ?? "Produto";
            return $"{productName}: {GetQuantityDescription()}";
        }
    }
} 