using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.DTOs
{
    /// <summary>
    /// DTO para mapeamento de dados do banco de dados para a entidade Product
    /// Usado pelo Dapper para instanciar dados antes de converter para as classes específicas (SimpleProduct, CompositeProduct, ProductGroup)
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Identificador único do produto
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Nome do produto
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição do produto
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Preço do produto
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Preço baseado em quantidade
        /// </summary>
        public int QuantityPrice { get; set; }

        /// <summary>
        /// Preço unitário do produto
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Custo do produto
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// ID da categoria do produto
        /// </summary>
        public string? CategoryId { get; set; }

        /// <summary>
        /// Nome da categoria do produto
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Código SKU do produto
        /// </summary>
        public string? SKU { get; set; }

        /// <summary>
        /// URL da imagem do produto
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Notas sobre o produto
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Tempo de montagem em minutos
        /// </summary>
        public int AssemblyTime { get; set; }

        /// <summary>
        /// Instruções de montagem
        /// </summary>
        public string? AssemblyInstructions { get; set; }

        /// <summary>
        /// Tipo do produto (Dapper converte automaticamente string ↔ enum)
        /// Valores no banco: 'Simple', 'Composite', 'Group'
        /// </summary>
        public ProductType ProductType { get; set; } = ProductType.Simple;

        /// <summary>
        /// Código do estado do objeto (ativo/inativo)
        /// </summary>
        public int StateCode { get; set; }

        /// <summary>
        /// Data de criação
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Usuário que criou o registro
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Data da última modificação
        /// </summary>
        public DateTime LastModifiedAt { get; set; }

        /// <summary>
        /// Usuário que fez a última modificação
        /// </summary>
        public string LastModifiedBy { get; set; } = string.Empty;
    }
} 