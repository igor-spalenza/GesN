using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade ProductComponentHierarchy - define hierarquias de componentes para produtos compostos
    /// Permite organizar componentes em estruturas hierárquicas flexíveis
    /// </summary>
    public class ProductComponentHierarchy : Entity
    {
        /// <summary>
        /// Nome da hierarquia
        /// </summary>
        [Required(ErrorMessage = "O nome da hierarquia é obrigatório")]
        [Display(Name = "Nome da Hierarquia")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da hierarquia
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Observações sobre a hierarquia
        /// </summary>
        [Display(Name = "Observações")]
        [MaxLength(2000)]
        public string? Notes { get; set; }

        // Propriedades de Navegação

        /// <summary>
        /// Componentes que fazem parte desta hierarquia
        /// </summary>
        public ICollection<ProductComponent> Components { get; set; } = new List<ProductComponent>();

        /// <summary>
        /// Relacionamentos com produtos compostos
        /// </summary>
        public ICollection<CompositeProductXHierarchy> CompositeProductRelations { get; set; } = new List<CompositeProductXHierarchy>();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ProductComponentHierarchy()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public ProductComponentHierarchy(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }

        // Métodos de Negócio

        /// <summary>
        /// Obtém o número total de componentes na hierarquia
        /// </summary>
        public int GetTotalComponents()
        {
            return Components?.Count() ?? 0;
        }

        /// <summary>
        /// Obtém o número de componentes ativos
        /// </summary>
        public int GetActiveComponentsCount()
        {
            return Components?.Count(c => c.StateCode == ObjectState.Active) ?? 0;
        }

        /// <summary>
        /// Obtém o número de componentes inativos
        /// </summary>
        public int GetInactiveComponentsCount()
        {
            return Components?.Count(c => c.StateCode == ObjectState.Inactive) ?? 0;
        }

        /// <summary>
        /// Calcula o custo adicional total da hierarquia 
        /// </summary>
        public decimal CalculateAdditionalCost()
        {
            return Components?.Where(c => c.StateCode == ObjectState.Active).Sum(c => c.AdditionalCost) ?? 0;
        }

        /// <summary>
        /// Verifica se a hierarquia tem componentes ativos
        /// </summary>
        public bool HasActiveComponents()
        {
            return Components?.Any(c => c.StateCode == ObjectState.Active) == true;
        }

        /// <summary>
        /// Obtém componentes ordenados por nome
        /// </summary>
        public IEnumerable<ProductComponent> GetComponentsByName()
        {
            return Components?.Where(c => c.StateCode == ObjectState.Active).OrderBy(c => c.Name) ?? Enumerable.Empty<ProductComponent>();
        }

        /// <summary>
        /// Verifica se todos os componentes estão ativos
        /// </summary>
        public bool AreAllComponentsActive()
        {
            return Components?.All(c => c.StateCode == ObjectState.Active) ?? true;
        }

        /// <summary>
        /// Verifica se a hierarquia está ativa
        /// </summary>
        public bool IsActive()
        {
            return StateCode == ObjectState.Active;
        }

        /// <summary>
        /// Obtém informações de disponibilidade
        /// </summary>
        public string GetAvailabilityInfo()
        {
            if (!IsActive())
                return "❌ Hierarquia Inativa";

            if (!Components?.Any() == true)
                return "⚠️ Sem Componentes";

            if (AreAllComponentsActive())
                return $"✅ Disponível ({GetTotalComponents()} componentes)";

            return "⚠️ Alguns componentes inativos";
        }

        /// <summary>
        /// Obtém o status de completude da hierarquia
        /// </summary>
        public string GetCompletenessStatus()
        {
            var total = GetTotalComponents();
            var active = GetActiveComponentsCount();
            var inactive = GetInactiveComponentsCount();

            if (total == 0)
                return "Vazia";

            if (active == 0 && inactive > 0)
                return "Apenas Inativos";

            if (active > 0 && inactive == 0)
                return "Apenas Ativos";

            return "Completa";
        }

        /// <summary>
        /// Clona a hierarquia (versão simplificada)
        /// </summary>
        public ProductComponentHierarchy Clone()
        {
            return new ProductComponentHierarchy
            {
                Name = Name,
                Description = Description,
                Notes = Notes
            };
        }

        /// <summary>
        /// Obtém informações resumidas da hierarquia
        /// </summary>
        public string GetSummary()
        {
            var status = IsActive() ? "Ativa" : "Inativa";
            var components = GetTotalComponents();
            return $"{Name} - {components} componentes - {status}";
        }

        /// <summary>
        /// Verifica se os dados estão completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// Obtém o nome para exibição
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(Name) ? "Hierarquia sem nome" : Name;
        }

        /// <summary>
        /// Override do ToString para exibir resumo
        /// </summary>
        public override string ToString()
        {
            return GetSummary();
        }

        // Métodos de compatibilidade (obsoletos) - mantidos para evitar quebrar código existente

        /// <summary>
        /// Verifica se todos os componentes estão disponíveis (compatibilidade)
        /// </summary>
        [Obsolete("Use AreAllComponentsActive() - a nova estrutura usa StateCode")]
        public bool AreAllComponentsAvailable()
        {
            return AreAllComponentsActive();
        }

        /// <summary>
        /// Calcula o tempo total estimado de processamento (compatibilidade)
        /// </summary>
        [Obsolete("EstimatedProcessingTime não existe mais na nova estrutura")]
        public int CalculateTotalProcessingTime()
        {
            // Retorna 0 pois campo não existe mais
            return 0;
        }

        /// <summary>
        /// Verifica se tem componentes ativos (compatibilidade com HasRequiredComponents)
        /// </summary>
        [Obsolete("Use HasActiveComponents() - a nova estrutura não tem conceito de componentes obrigatórios")]
        public bool HasRequiredComponents()
        {
            return HasActiveComponents();
        }

        /// <summary>
        /// Calcula custo estimado (compatibilidade com CalculateEstimatedCost)
        /// </summary>
        [Obsolete("Use CalculateAdditionalCost() - a nova estrutura usa custo adicional")]
        public decimal CalculateEstimatedCost()
        {
            return CalculateAdditionalCost();
        }
    }
} 