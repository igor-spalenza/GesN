using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Grupo de produtos - produto que representa um conjunto de opções de produtos
    /// Herda de Product e usa ProductType = 'Group'
    /// </summary>
    public class ProductGroup : Product
    {
        /// <summary>
        /// Lista de itens do grupo (relacionamento 1:N com ProductGroupItem)
        /// </summary>
        public ICollection<ProductGroupItem> GroupItems { get; set; } = new List<ProductGroupItem>();

        /// <summary>
        /// Lista de opções de configuração do grupo (relacionamento 1:N com ProductGroupOption)
        /// </summary>
        public ICollection<ProductGroupOption> GroupOptions { get; set; } = new List<ProductGroupOption>();

        /// <summary>
        /// Lista de regras de troca do grupo (relacionamento 1:N com ProductGroupExchangeRule)
        /// </summary>
        public ICollection<ProductGroupExchangeRule> ExchangeRules { get; set; } = new List<ProductGroupExchangeRule>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductGroup()
        {
            ProductType = ProductType.Group;
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductGroup(string name, decimal price) : base(name, price)
        {
            ProductType = ProductType.Group;
        }

        /// <summary>
        /// Calcula o preço com base na seleção de itens
        /// </summary>
        public decimal CalculatePrice(IEnumerable<ProductGroupItem> selectedItems)
        {
            // Se não há itens selecionados, retorna o preço base
            if (!selectedItems?.Any() == true)
                return Price;

            var totalPrice = selectedItems.Sum(item => item.GetEffectivePrice() * item.Quantity);
            
            return totalPrice;
        }

        /// <summary>
        /// Obtém os itens disponíveis para seleção
        /// </summary>
        public IEnumerable<ProductGroupItem> GetAvailableItems()
        {
            return GroupItems.Where(item => !item.IsOptional || item.Product?.StateCode == ObjectState.Active);
        }

        /// <summary>
        /// Verifica se todos os itens estão disponíveis
        /// </summary>
        public bool AreAllItemsAvailable()
        {
            return GroupItems.All(item => item.Product?.StateCode == ObjectState.Active);
        }

        /// <summary>
        /// Obtém informações de disponibilidade
        /// </summary>
        public string GetAvailabilityInfo()
        {
            if (StateCode != ObjectState.Active)
                return "❌ Indisponível para venda";

            var availableCount = GetAvailableItems().Count();
            var totalCount = GroupItems.Count;

            if (availableCount == 0)
                return "❌ Nenhum item disponível";

            if (availableCount < totalCount)
                return $"⚠️ {availableCount}/{totalCount} itens disponíveis";

            return "✅ Todos os itens disponíveis";
        }

        /// <summary>
        /// Obtém informações resumidas do grupo
        /// </summary>
        public string GetGroupInfo()
        {
            var info = GetDisplayName();
            
            if (GroupItems?.Any() == true)
                info += $" ({GroupItems.Count} itens)";
                
            return info;
        }

        /// <summary>
        /// Verifica se o grupo tem itens mínimos
        /// </summary>
        public bool HasMinimumItems()
        {
            return GroupItems?.Count() >= 1;
        }

        /// <summary>
        /// Obtém o total de itens no grupo
        /// </summary>
        public int GetItemsCount()
        {
            return GroupItems?.Count() ?? 0;
        }

        /// <summary>
        /// Override do método HasCompleteData
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData() && HasMinimumItems();
        }
    }
} 