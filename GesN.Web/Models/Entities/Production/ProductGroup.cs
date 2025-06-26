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
        public ProductGroup(string name, decimal unitPrice) : base(name, unitPrice)
        {
            // Define o tipo do produto como Group
        }

        /// <summary>
        /// Obtém a quantidade mínima de itens (usa campo herdado MinItemsRequired)
        /// </summary>
        public int GetMinSelection()
        {
            return MinItemsRequired ?? 0;
        }

        /// <summary>
        /// Obtém a quantidade máxima de itens (usa campo herdado MaxItemsAllowed)
        /// </summary>
        public int? GetMaxSelection()
        {
            return MaxItemsAllowed;
        }

        /// <summary>
        /// Calcula o preço com base na seleção de itens
        /// </summary>
        public decimal CalculatePrice(IEnumerable<ProductGroupItem> selectedItems)
        {
            // Se não há itens selecionados, retorna o preço base
            if (!selectedItems?.Any() == true)
                return UnitPrice;

            var totalPrice = selectedItems.Sum(item => item.GetEffectivePrice() * item.Quantity);
            
            return totalPrice;
        }

        /// <summary>
        /// Verifica se uma seleção de itens é válida
        /// </summary>
        public bool IsValidSelection(IEnumerable<ProductGroupItem> selectedItems)
        {
            var count = selectedItems?.Count() ?? 0;
            
            if (count < GetMinSelection())
                return false;

            if (MaxItemsAllowed.HasValue && count > MaxItemsAllowed.Value)
                return false;

            return selectedItems?.All(item => GroupItems.Contains(item)) ?? true;
        }

        /// <summary>
        /// Obtém os itens disponíveis para seleção
        /// </summary>
        public IEnumerable<ProductGroupItem> GetAvailableItems()
        {
            return GroupItems.Where(item => !item.IsOptional || item.Product?.IsActive == true);
        }

        /// <summary>
        /// Obtém a descrição do tipo de seleção
        /// </summary>
        public string GetSelectionDescription()
        {
            if (!MaxItemsAllowed.HasValue)
                return $"Selecione pelo menos {GetMinSelection()} {(GetMinSelection() == 1 ? "item" : "itens")}";

            if (GetMinSelection() == MaxItemsAllowed.Value)
                return $"Selecione {GetMinSelection()} {(GetMinSelection() == 1 ? "item" : "itens")}";

            return $"Selecione entre {GetMinSelection()} e {MaxItemsAllowed.Value} itens";
        }

        /// <summary>
        /// Verifica se todos os itens estão disponíveis
        /// </summary>
        public bool AreAllItemsAvailable()
        {
            return GroupItems.All(item => item.Product?.IsActive == true);
        }

        /// <summary>
        /// Obtém informações de disponibilidade
        /// </summary>
        public string GetAvailabilityInfo()
        {
            if (!IsActive)
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
        /// Override do método HasCompleteData
        /// </summary>
        public override bool HasCompleteData()
        {
            return base.HasCompleteData() && 
                   GroupItems.Any() && 
                   GetMinSelection() > 0 &&
                   (!MaxItemsAllowed.HasValue || GetMinSelection() <= MaxItemsAllowed.Value);
        }

        /// <summary>
        /// Número mínimo de itens obrigatórios
        /// </summary>
        [Display(Name = "Min. Itens Obrigatórios")]
        [Range(0, int.MaxValue, ErrorMessage = "O número mínimo deve ser maior ou igual a 0")]
        public int? MinItemsRequired { get; set; }
    }
} 