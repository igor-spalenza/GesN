using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa um ingrediente
    /// </summary>
    public class Ingredient : Entity
    {
        /// <summary>
        /// Nome do ingrediente
        /// </summary>
        [Required(ErrorMessage = "O nome do ingrediente é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do ingrediente
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Unidade de medida
        /// </summary>
        [Required(ErrorMessage = "A unidade de medida é obrigatória")]
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; } = ProductionUnit.Unidades;

        /// <summary>
        /// Custo por unidade
        /// </summary>
        [Display(Name = "Custo por Unidade")]
        [Range(0, double.MaxValue, ErrorMessage = "O custo deve ser maior ou igual a zero")]
        public decimal CostPerUnit { get; set; } = 0;

        /// <summary>
        /// ID do fornecedor padrão
        /// </summary>
        [Display(Name = "Fornecedor")]
        public string? SupplierId { get; set; }

        /// <summary>
        /// Estoque mínimo
        /// </summary>
        [Display(Name = "Estoque Mínimo")]
        [Range(0, double.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
        public decimal MinStock { get; set; } = 0;

        /// <summary>
        /// Estoque atual
        /// </summary>
        [Display(Name = "Estoque Atual")]
        [Range(0, double.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
        public decimal CurrentStock { get; set; } = 0;

        /// <summary>
        /// Dias de validade (para ingredientes perecíveis)
        /// </summary>
        [Display(Name = "Dias de Validade")]
        [Range(0, int.MaxValue, ErrorMessage = "Os dias de validade devem ser maior ou igual a zero")]
        public int? ExpirationDays { get; set; }



        /// <summary>
        /// Propriedade navegacional para o fornecedor
        /// </summary>
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Ingredient()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Ingredient(string name, ProductionUnit unit = ProductionUnit.Unidades, decimal costPerUnit = 0)
        {
            Name = name;
            Unit = unit;
            CostPerUnit = costPerUnit;
        }

        /// <summary>
        /// Verifica se é um ingrediente perecível
        /// </summary>
        public bool IsPerishable()
        {
            return ExpirationDays.HasValue && ExpirationDays.Value > 0;
        }

        /// <summary>
        /// Verifica se o estoque está baixo
        /// </summary>
        public bool IsLowStock()
        {
            return CurrentStock <= MinStock;
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Ingrediente sem nome" : Name;
        }

        /// <summary>
        /// Obtém informações do estoque formatadas
        /// </summary>
        public string GetStockInfo()
        {
            var stockInfo = $"{CurrentStock:N2} {Unit}";
            
            if (IsLowStock())
                stockInfo += " ⚠️ Estoque baixo";
            
            return stockInfo;
        }

        /// <summary>
        /// Obtém informações de custo formatadas
        /// </summary>
        public string GetCostInfo()
        {
            return $"R$ {CostPerUnit:N2} por {Unit}";
        }

        /// <summary>
        /// Verifica se o ingrediente possui dados básicos completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// Obtém um resumo completo do ingrediente
        /// </summary>
        public string GetIngredientSummary()
        {
            var parts = new List<string>
            {
                GetDisplayName(),
                GetStockInfo(),
                GetCostInfo()
            };

            if (IsPerishable())
                parts.Add($"⏱️ {ExpirationDays} dias validade");

            return string.Join(" - ", parts);
        }

        /// <summary>
        /// Override do ToString para exibir resumo do ingrediente
        /// </summary>
        public override string ToString()
        {
            return GetIngredientSummary();
        }
    }
} 