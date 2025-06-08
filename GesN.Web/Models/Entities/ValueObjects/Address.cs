using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.ValueObjects
{
    /// <summary>
    /// Value Object representando um endereço
    /// </summary>
    public class Address : ValueObject
    {
        /// <summary>
        /// Logradouro (rua, avenida, etc.)
        /// </summary>
        [Required(ErrorMessage = "O logradouro é obrigatório")]
        [Display(Name = "Logradouro")]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço
        /// </summary>
        [Display(Name = "Número")]
        [MaxLength(20)]
        public string? Number { get; set; }

        /// <summary>
        /// Complemento (apartamento, sala, etc.)
        /// </summary>
        [Display(Name = "Complemento")]
        [MaxLength(100)]
        public string? Complement { get; set; }

        /// <summary>
        /// Bairro
        /// </summary>
        [Display(Name = "Bairro")]
        [MaxLength(100)]
        public string? Neighborhood { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [Required(ErrorMessage = "A cidade é obrigatória")]
        [Display(Name = "Cidade")]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Estado/UF
        /// </summary>
        [Required(ErrorMessage = "O estado é obrigatório")]
        [Display(Name = "Estado")]
        [MaxLength(2)]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// CEP
        /// </summary>
        [Display(Name = "CEP")]
        [MaxLength(10)]
        public string? ZipCode { get; set; }

        /// <summary>
        /// País
        /// </summary>
        [Display(Name = "País")]
        [MaxLength(50)]
        public string Country { get; set; } = "Brasil";

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Address() { }

        /// <summary>
        /// Construtor com parâmetros básicos
        /// </summary>
        public Address(string street, string city, string state, string? number = null)
        {
            Street = street;
            City = city;
            State = state;
            Number = number;
        }

        /// <summary>
        /// Obtém o endereço formatado para exibição
        /// </summary>
        public string GetFormattedAddress()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Street))
            {
                var streetPart = Street;
                if (!string.IsNullOrWhiteSpace(Number))
                    streetPart += $", {Number}";
                parts.Add(streetPart);
            }

            if (!string.IsNullOrWhiteSpace(Complement))
                parts.Add(Complement);

            if (!string.IsNullOrWhiteSpace(Neighborhood))
                parts.Add(Neighborhood);

            if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);

            if (!string.IsNullOrWhiteSpace(State))
                parts.Add(State);

            if (!string.IsNullOrWhiteSpace(ZipCode))
                parts.Add($"CEP: {ZipCode}");

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Verifica se o endereço é válido
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Street) &&
                   !string.IsNullOrWhiteSpace(City) &&
                   !string.IsNullOrWhiteSpace(State);
        }

        /// <summary>
        /// Implementação dos componentes de igualdade
        /// </summary>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Street?.ToLowerInvariant();
            yield return Number?.ToLowerInvariant();
            yield return Complement?.ToLowerInvariant();
            yield return Neighborhood?.ToLowerInvariant();
            yield return City?.ToLowerInvariant();
            yield return State?.ToUpperInvariant();
            yield return ZipCode?.Replace("-", "").Replace(".", "");
            yield return Country?.ToLowerInvariant();
        }

        /// <summary>
        /// Override do ToString para exibir endereço formatado
        /// </summary>
        public override string ToString()
        {
            return GetFormattedAddress();
        }
    }
} 