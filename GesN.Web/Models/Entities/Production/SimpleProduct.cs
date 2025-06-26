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
        /// Construtor padrão
        /// </summary>
        public SimpleProduct()
        {
            ProductType = ProductType.Simple;
        }

        /// <summary>
        /// Estoque atual do produto simples
        /// </summary>
        [Display(Name = "Estoque Atual")]
        [Range(0, double.MaxValue, ErrorMessage = "O estoque atual deve ser maior ou igual a zero")]
        public new decimal CurrentStock { get; set; } = 0;

        /// <summary>
        /// Estoque mínimo para alertas
        /// </summary>
        [Display(Name = "Estoque Mínimo")]
        [Range(0, double.MaxValue, ErrorMessage = "O estoque mínimo deve ser maior ou igual a zero")]
        public new decimal MinStock { get; set; } = 0;

        /// <summary>
        /// Preço unitário do produto simples
        /// </summary>
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public new decimal? UnitPrice { get; set; }



        /// <summary>
        /// Permite customização do produto
        /// </summary>
        [Display(Name = "Permite Customização")]
        public new bool AllowCustomization { get; set; } = false;

        /// <summary>
        /// Tempo de montagem em minutos
        /// </summary>
        [Display(Name = "Tempo de Montagem (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a zero")]
        public new int? AssemblyTime { get; set; }

        /// <summary>
        /// Instruções de montagem
        /// </summary>
        [Display(Name = "Instruções de Montagem")]
        [MaxLength(1000)]
        public new string? AssemblyInstructions { get; set; }

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