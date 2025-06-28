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
        /// Construtor com dados básicos
        /// </summary>
        public SimpleProduct(string name, decimal price) : base(name, price)
        {
            ProductType = ProductType.Simple;
        }



        /// <summary>
        /// Override do método HasCompleteData para incluir validações específicas
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData() && !string.IsNullOrWhiteSpace(SKU);
        }

        /// <summary>
        /// Obtém informações resumidas do produto
        /// </summary>
        public string GetSummaryInfo()
        {
            var summary = GetDisplayName();
            
            if (!string.IsNullOrWhiteSpace(SKU))
                summary += $" (SKU: {SKU})";
                
            return summary;
        }

        /// <summary>
        /// Verifica se o produto tem imagem
        /// </summary>
        public bool HasImage()
        {
            return !string.IsNullOrWhiteSpace(ImageUrl);
        }

        /// <summary>
        /// Obtém a URL da imagem ou uma URL padrão
        /// </summary>
        public string GetImageUrl()
        {
            return !string.IsNullOrWhiteSpace(ImageUrl) ? ImageUrl : "/img/no-image.png";
        }

        /// <summary>
        /// Verifica se possui instruções de montagem
        /// </summary>
        public bool HasAssemblyInstructions()
        {
            return !string.IsNullOrWhiteSpace(AssemblyInstructions);
        }

        /// <summary>
        /// Obtém resumo da montagem
        /// </summary>
        public string GetAssemblySummary()
        {
            if (!RequiresAssembly())
                return "Produto não requer montagem";
                
            var summary = GetAssemblyTimeInfo();
            
            if (HasAssemblyInstructions())
                summary += " - Com instruções";
                
            return summary;
        }
    }
} 