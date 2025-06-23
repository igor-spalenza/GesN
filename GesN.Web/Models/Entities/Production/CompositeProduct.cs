using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Produto composto - produto que é formado por outros produtos/componentes
    /// Herda de Product e usa ProductType = 'Composite'
    /// </summary>
    public class CompositeProduct : Product
    {
        /// <summary>
        /// Lista de componentes do produto composto
        /// </summary>
        public ICollection<ProductComponent> Components { get; set; } = new List<ProductComponent>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public CompositeProduct()
        {
            ProductType = ProductType.Composite;
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public CompositeProduct(string name, decimal unitPrice) : base(name, unitPrice)
        {
            // Define o tipo do produto como Composite
        }

        /// <summary>
        /// Calcula o custo total dos componentes
        /// </summary>
        public decimal CalculateComponentsCost()
        {
            return Components?.Sum(c => c.CalculateTotalCost()) ?? 0;
        }

        /// <summary>
        /// Verifica se todos os componentes estão disponíveis
        /// </summary>
        public bool AreAllComponentsAvailable()
        {
            return Components?.All(c => c.IsAvailable()) ?? true;
        }

        /// <summary>
        /// Verifica se o estoque está baixo (usa campos herdados MinStock e CurrentStock)
        /// </summary>
        public bool IsLowStock()
        {
            return CurrentStock <= MinStock;
        }

        /// <summary>
        /// Obtém o tempo de montagem formatado (usa campo herdado AssemblyTime)
        /// </summary>
        public string GetFormattedAssemblyTime()
        {
            if (AssemblyTime <= 0)
                return "Não informado";

            var hours = AssemblyTime / 60;
            var minutes = AssemblyTime % 60;

            if (hours > 0)
                return $"{hours}h {minutes}min";

            return $"{minutes}min";
        }

        /// <summary>
        /// Obtém informações do estoque
        /// </summary>
        public string GetStockInfo()
        {
            var info = $"{CurrentStock} {Unit}";
            if (IsLowStock())
                info += " ⚠️ Estoque baixo";
            return info;
        }

        /// <summary>
        /// Override do método HasCompleteData
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData() && Components.Any();
        }

        /// <summary>
        /// Tempo de montagem em minutos
        /// </summary>
        [Display(Name = "Tempo de Montagem (min)")]
        [Range(0, int.MaxValue, ErrorMessage = "O tempo de montagem deve ser maior ou igual a 0")]
        public int? AssemblyTime { get; set; }
    }
} 