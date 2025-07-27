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
        public CompositeProduct(string name, decimal price) : base(name, price)
        {
            ProductType = ProductType.Composite;
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
        /// Obtém informações resumidas do produto composto
        /// </summary>
        public string GetCompositeInfo()
        {
            var info = GetDisplayName();
            
            if (Components?.Any() == true)
                info += $" ({Components.Count} componentes)";
                
            return info;
        }

        /// <summary>
        /// Verifica se o produto tem todos os componentes básicos
        /// </summary>
        public bool HasMinimumComponents()
        {
            return Components?.Count() >= 1;
        }

        /// <summary>
        /// Obtém o total de componentes
        /// </summary>
        public int GetComponentsCount()
        {
            return Components?.Count() ?? 0;
        }

        /// <summary>
        /// Calcula tempo total de montagem incluindo componentes
        /// </summary>
        public int CalculateTotalAssemblyTime()
        {
            var baseTime = AssemblyTime;
            // Na nova estrutura, usar tempo da hierarquia dos componentes (campo removido, retorna 0)
            var componentsTime = Components?.Where(c => c.StateCode == ObjectState.Active)
                .Sum(c => c.ProductComponentHierarchy?.CalculateTotalProcessingTime() ?? 0) ?? 0;
            return baseTime + componentsTime;
        }

        /// <summary>
        /// Obtém informações completas de montagem
        /// </summary>
        public string GetCompleteAssemblyInfo()
        {
            var totalTime = CalculateTotalAssemblyTime();
            
            if (totalTime <= 0)
                return "Produto não requer montagem";
                
            var hours = totalTime / 60;
            var minutes = totalTime % 60;
            
            var timeInfo = hours > 0 ? $"{hours}h {minutes}min" : $"{minutes}min";
            
            if (!string.IsNullOrWhiteSpace(AssemblyInstructions))
                timeInfo += " - Com instruções detalhadas";
                
            return timeInfo;
        }

        /// <summary>
        /// Override do método HasCompleteData
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData() && HasMinimumComponents();
        }


    }
} 