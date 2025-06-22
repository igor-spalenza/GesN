using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Produto simples - implementação concreta da classe Product
    /// </summary>
    public class SimpleProduct : Product
    {
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
        /// Unidade de medida
        /// </summary>
        [Display(Name = "Unidade")]
        public ProductionUnit Unit { get; set; } = ProductionUnit.Unidades;

        /// <summary>
        /// Permite customização do produto
        /// </summary>
        [Display(Name = "Permite Customização")]
        public bool AllowCustomization { get; set; } = false;

        /// <summary>
        /// Tempo de montagem em minutos
        /// </summary>
        [Display(Name = "Tempo de Montagem (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        public int? AssemblyTime { get; set; }

        /// <summary>
        /// Instruções de montagem
        /// </summary>
        [Display(Name = "Instruções de Montagem")]
        [MaxLength(1000)]
        public string? AssemblyInstructions { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public SimpleProduct()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public SimpleProduct(string name, decimal unitPrice, ProductionUnit unit = ProductionUnit.Unidades) : base(name, unitPrice)
        {
            Unit = unit;
        }

        /// <summary>
        /// Verifica se o estoque está baixo
        /// </summary>
        public bool IsLowStock()
        {
            return CurrentStock <= MinStock;
        }

        /// <summary>
        /// Verifica se o produto requer montagem
        /// </summary>
        public bool RequiresAssembly()
        {
            return AssemblyTime > 0;
        }

        /// <summary>
        /// Obtém informações do estoque formatadas
        /// </summary>
        public string GetStockInfo()
        {
            var stockInfo = $"{CurrentStock} {Unit}";
            
            if (IsLowStock())
                stockInfo += " ⚠️ Estoque baixo";
            
            return stockInfo;
        }

        /// <summary>
        /// Obtém o tempo de montagem formatado
        /// </summary>
        public string GetAssemblyTimeInfo()
        {
            if (!AssemblyTime.HasValue || AssemblyTime <= 0)
                return "Sem montagem";
            
            var hours = AssemblyTime.Value / 60;
            var minutes = AssemblyTime.Value % 60;
            
            if (hours > 0)
                return $"{hours}h {minutes}min";
            
            return $"{minutes}min";
        }

        /// <summary>
        /// Override do método HasCompleteData para incluir validações específicas
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData();
        }
    }
} 