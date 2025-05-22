using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Areas.Identity.Data.Models
{
    /// <summary>
    /// Representa uma claim atribuída a uma role
    /// </summary>
    /// <typeparam name="TKey">O tipo de chave da role</typeparam>
    public class ApplicationRoleClaim<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// ID único da claim da role
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID da role associada a esta claim
        /// </summary>
        public TKey RoleId { get; set; }

        /// <summary>
        /// Tipo da claim (ex: "permission", "function", "resource", etc.)
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Valor da claim (ex: "create", "read", "update", "delete", etc.)
        /// </summary>
        public string ClaimValue { get; set; }
    }
} 