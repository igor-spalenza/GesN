using GesN.Web.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.ValueObjects
{
    /// <summary>
    /// Value Object representando dados fiscais de uma empresa/pessoa jurídica
    /// </summary>
    public class FiscalData : ValueObject
    {
        /// <summary>
        /// Número de identificação fiscal (CNPJ/CPF)
        /// </summary>
        [Display(Name = "Número de Identificação Fiscal")]
        [MaxLength(20)]
        public string? TaxNumber { get; set; }

        /// <summary>
        /// Inscrição Estadual
        /// </summary>
        [Display(Name = "Inscrição Estadual")]
        [MaxLength(20)]
        public string? StateRegistration { get; set; }

        /// <summary>
        /// Inscrição Municipal
        /// </summary>
        [Display(Name = "Inscrição Municipal")]
        [MaxLength(20)]
        public string? MunicipalRegistration { get; set; }

        /// <summary>
        /// Razão Social da empresa
        /// </summary>
        [Display(Name = "Razão Social")]
        [MaxLength(200)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Nome Fantasia da empresa
        /// </summary>
        [Display(Name = "Nome Fantasia")]
        [MaxLength(200)]
        public string? TradeName { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public FiscalData() { }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public FiscalData(string? taxNumber, string? companyName = null, string? tradeName = null)
        {
            TaxNumber = taxNumber;
            CompanyName = companyName;
            TradeName = tradeName;
        }

        /// <summary>
        /// Verifica se possui dados fiscais válidos
        /// </summary>
        public bool HasValidFiscalData()
        {
            return !string.IsNullOrWhiteSpace(TaxNumber) ||
                   !string.IsNullOrWhiteSpace(StateRegistration) ||
                   !string.IsNullOrWhiteSpace(MunicipalRegistration);
        }

        /// <summary>
        /// Verifica se é uma empresa (tem dados de pessoa jurídica)
        /// </summary>
        public bool IsCompany()
        {
            return !string.IsNullOrWhiteSpace(CompanyName) ||
                   !string.IsNullOrWhiteSpace(TradeName) ||
                   (TaxNumber?.Length == 14); // CNPJ tem 14 dígitos
        }

        /// <summary>
        /// Obtém o nome para exibição (prioriza Nome Fantasia, depois Razão Social)
        /// </summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(TradeName))
                return TradeName;
            
            if (!string.IsNullOrWhiteSpace(CompanyName))
                return CompanyName;
            
            return "Sem nome informado";
        }

        /// <summary>
        /// Formata o número fiscal (adiciona pontuação para CNPJ/CPF)
        /// </summary>
        public string GetFormattedTaxNumber()
        {
            if (string.IsNullOrWhiteSpace(TaxNumber))
                return string.Empty;

            var cleanNumber = TaxNumber.Replace(".", "").Replace("/", "").Replace("-", "");

            return cleanNumber.Length switch
            {
                11 => FormatCPF(cleanNumber),  // CPF: 000.000.000-00
                14 => FormatCNPJ(cleanNumber), // CNPJ: 00.000.000/0000-00
                _ => TaxNumber
            };
        }

        /// <summary>
        /// Formata CPF
        /// </summary>
        private static string FormatCPF(string cpf)
        {
            if (cpf.Length != 11) return cpf;
            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formata CNPJ
        /// </summary>
        private static string FormatCNPJ(string cnpj)
        {
            if (cnpj.Length != 14) return cnpj;
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        /// <summary>
        /// Obtém resumo dos dados fiscais para exibição
        /// </summary>
        public string GetFiscalSummary()
        {
            var parts = new List<string>();

            var displayName = GetDisplayName();
            if (displayName != "Sem nome informado")
                parts.Add(displayName);

            var formattedTaxNumber = GetFormattedTaxNumber();
            if (!string.IsNullOrWhiteSpace(formattedTaxNumber))
                parts.Add(formattedTaxNumber);

            if (!string.IsNullOrWhiteSpace(StateRegistration))
                parts.Add($"IE: {StateRegistration}");

            return parts.Count > 0 ? string.Join(" - ", parts) : "Dados fiscais não informados";
        }

        /// <summary>
        /// Implementação dos componentes de igualdade
        /// </summary>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return TaxNumber?.Replace(".", "").Replace("/", "").Replace("-", "");
            yield return StateRegistration?.Replace(".", "").Replace("/", "").Replace("-", "");
            yield return MunicipalRegistration?.Replace(".", "").Replace("/", "").Replace("-", "");
            yield return CompanyName?.ToLowerInvariant();
            yield return TradeName?.ToLowerInvariant();
        }

        /// <summary>
        /// Override do ToString para exibir resumo fiscal
        /// </summary>
        public override string ToString()
        {
            return GetFiscalSummary();
        }
    }
} 