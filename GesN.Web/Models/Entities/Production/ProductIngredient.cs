using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade de relacionamento entre Produto e Ingrediente
    /// </summary>
    public class ProductIngredient : Entity
    {
        /// <summary>
        /// ID do produto
        /// </summary>
        [Required(ErrorMessage = "O produto é obrigatório")]
        [Display(Name = "Produto")]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// ID do ingrediente
        /// </summary>
        [Required(ErrorMessage = "O ingrediente é obrigatório")]
        [Display(Name = "Ingrediente")]
        public string IngredientId { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade do ingrediente necessária
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
        /// Indica se o ingrediente é opcional
        /// </summary>
        [Display(Name = "Opcional")]
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Observações sobre o uso do ingrediente
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>
        /// Propriedade navegacional para o produto
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Propriedade navegacional para o ingrediente
        /// </summary>
        public Ingredient? Ingredient { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductIngredient()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductIngredient(string productId, string ingredientId, decimal quantity, ProductionUnit unit = ProductionUnit.Unidades)
        {
            ProductId = productId;
            IngredientId = ingredientId;
            Quantity = quantity;
            Unit = unit;
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
        /// Calcula o custo do ingrediente baseado na quantidade
        /// </summary>
        public decimal CalculateCost()
        {
            if (Ingredient == null) return 0;
            
            return Quantity * Ingredient.CostPerUnit;
        }

        /// <summary>
        /// Verifica se há estoque suficiente do ingrediente
        /// </summary>
        public bool HasSufficientStock()
        {
            if (Ingredient == null) return false;
            
            return Ingredient.CurrentStock >= Quantity;
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(ProductId) &&
                   !string.IsNullOrWhiteSpace(IngredientId) &&
                   Quantity > 0;
        }

        /// <summary>
        /// Override do ToString para exibir resumo do relacionamento
        /// </summary>
        public override string ToString()
        {
            var ingredientName = Ingredient?.Name ?? "Ingrediente";
            return $"{ingredientName}: {GetQuantityDescription()}";
        }
    }
} 