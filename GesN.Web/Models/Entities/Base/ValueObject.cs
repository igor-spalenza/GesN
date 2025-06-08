using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Base
{
    /// <summary>
    /// Classe abstrata base para Value Objects
    /// Value Objects são objetos imutáveis identificados pelos seus valores, não por ID
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Identificador único do Value Object (GUID como string)
        /// Usado apenas para persistência no banco de dados
        /// </summary>
        [Key]
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Obtém os componentes de igualdade do Value Object
        /// Deve ser implementado pelas classes filhas para definir quais propriedades
        /// determinam a igualdade entre dois Value Objects
        /// </summary>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        /// <summary>
        /// Override do Equals para comparação por valores
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is null || GetType() != obj.GetType())
                return false;

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Override do GetHashCode baseado nos componentes de igualdade
        /// </summary>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Where(x => x != null)
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + obj.GetHashCode();
                    }
                });
        }

        /// <summary>
        /// Operador de igualdade
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
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
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Cria uma cópia do Value Object
        /// </summary>
        public virtual T Copy<T>() where T : ValueObject
        {
            return (T)MemberwiseClone();
        }
    }
} 