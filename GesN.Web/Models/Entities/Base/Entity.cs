using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Base
{
    /// <summary>
    /// Classe abstrata base para todas as entidades do domínio
    /// Contém propriedades comuns de auditoria e controle de estado
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Identificador único da entidade (GUID como string)
        /// </summary>
        [Key]
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Data e hora de criação da entidade
        /// </summary>
        [Required]
        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identificador do usuário que criou a entidade
        /// </summary>
        [Required]
        [Display(Name = "Criado Por")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora da última modificação (opcional)
        /// </summary>
        [Display(Name = "Data de Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// Identificador do usuário que fez a última modificação (opcional)
        /// </summary>
        [Display(Name = "Modificado Por")]
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Estado do objeto (Active/Inactive)
        /// Armazenado como INTEGER no banco de dados
        /// </summary>
        [Required]
        [Display(Name = "Estado")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;

        /// <summary>
        /// Verifica se a entidade está ativa
        /// </summary>
        public bool IsActive => StateCode == ObjectState.Active;

        /// <summary>
        /// Ativa a entidade
        /// </summary>
        public virtual void Activate()
        {
            StateCode = ObjectState.Active;
            UpdateModification();
        }

        /// <summary>
        /// Desativa a entidade
        /// </summary>
        public virtual void Deactivate()
        {
            StateCode = ObjectState.Inactive;
            UpdateModification();
        }

        /// <summary>
        /// Atualiza os campos de modificação
        /// Deve ser chamado sempre que a entidade for modificada
        /// </summary>
        public virtual void UpdateModification(string? modifiedBy = null)
        {
            LastModifiedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(modifiedBy))
            {
                LastModifiedBy = modifiedBy;
            }
        }

        /// <summary>
        /// Define quem criou a entidade
        /// Deve ser chamado na criação da entidade
        /// </summary>
        public virtual void SetCreatedBy(string createdBy)
        {
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Override do Equals para comparação por Id
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Override do GetHashCode baseado no Id
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Operador de igualdade
        /// </summary>
        public static bool operator ==(Entity? left, Entity? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operador de desigualdade
        /// </summary>
        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }
    }
} 