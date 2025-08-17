using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade CompositeProductXHierarchy - relaciona produtos compostos com hierarquias de componentes
    /// Define como uma hierarquia específica é aplicada a um produto composto
    /// </summary>
    public class CompositeProductXHierarchy
    {
        /// <summary>
        /// Identificador único da relação (auto-incremental)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// ID da hierarquia de componentes
        /// </summary>
        [Required]
        public string ProductComponentHierarchyId { get; set; } = string.Empty;

        /// <summary>
        /// Hierarquia de componentes (propriedade de navegação)
        /// </summary>
        public virtual ProductComponentHierarchy? ProductComponentHierarchy { get; set; }

        /// <summary>
        /// ID do produto composto
        /// </summary>
        [Required]
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Produto composto (propriedade de navegação)
        /// </summary>
        public virtual CompositeProduct? Product { get; set; }

        /// <summary>
        /// Quantidade mínima necessária desta hierarquia
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade mínima deve ser maior que zero")]
        public int MinQuantity { get; set; } = 1;

        /// <summary>
        /// Quantidade máxima permitida desta hierarquia (0 = ilimitado)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade máxima não pode ser negativa")]
        public int MaxQuantity { get; set; } = 0;

        /// <summary>
        /// Indica se esta hierarquia é opcional no produto
        /// </summary>
        public bool IsOptional { get; set; } = false;

        /// <summary>
        /// Ordem de montagem/produção desta hierarquia no produto
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ordem de montagem deve ser maior que zero")]
        public int AssemblyOrder { get; set; } = 1;

        /// <summary>
        /// Observações adicionais sobre esta relação
        /// </summary>
        public string? Notes { get; set; }

        // Métodos de validação e lógica de negócio

        /// <summary>
        /// Valida se a quantidade está dentro dos limites estabelecidos
        /// </summary>
        public bool IsQuantityValid(int quantity)
        {
            if (quantity < MinQuantity)
                return false;

            if (MaxQuantity > 0 && quantity > MaxQuantity)
                return false;

            return true;
        }

        /// <summary>
        /// Verifica se a relação está válida para produção
        /// </summary>
        public bool IsValidForProduction()
        {
            return ProductComponentHierarchy?.IsActive() == true &&
                   Product?.IsActive == true;
        }

        /// <summary>
        /// Verifica se a relação está ativa (sempre retorna true por compatibilidade)
        /// </summary>
        public bool IsActive()
        {
            // CompositeProductXHierarchy não possui mais propriedade IsActive
            // por não herdar de Entity, então sempre retorna true por compatibilidade
            return true;
        }

        /// <summary>
        /// Ativa a relação
        /// </summary>
        public void Activate()
        {
            // Método mantido por compatibilidade, mas sem funcionalidade
        }

        /// <summary>
        /// Desativa a relação
        /// </summary>
        public void Deactivate()
        {
            // Método mantido por compatibilidade, mas sem funcionalidade
        }

        /// <summary>
        /// Representação em string da relação
        /// </summary>
        public override string ToString()
        {
            return $"Hierarquia {ProductComponentHierarchy?.Name} no Produto {Product?.Name} (Qtd: {MinQuantity}-{(MaxQuantity == 0 ? "∞" : MaxQuantity.ToString())})";
        }
    }
} 