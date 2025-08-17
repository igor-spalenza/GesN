using GesN.Web.Models.Entities.Base;
using GesN.Web.Models.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Models.Entities.Production
{
    /// <summary>
    /// Entidade que representa um fornecedor
    /// </summary>
    public class Supplier : Entity
    {
        /// <summary>
        /// Nome do fornecedor
        /// </summary>
        [Required(ErrorMessage = "O nome do fornecedor é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Razão social (para empresas)
        /// </summary>
        [Display(Name = "Razão Social")]
        [MaxLength(200)]
        public string? CompanyName { get; set; }

        /// <summary>
        /// Número do documento (CPF/CNPJ)
        /// </summary>
        [Display(Name = "Documento")]
        [MaxLength(20)]
        public string? DocumentNumber { get; set; }

        /// <summary>
        /// Tipo de documento
        /// </summary>
        [Display(Name = "Tipo de Documento")]
        public DocumentType? DocumentType { get; set; }

        /// <summary>
        /// Email de contato
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Telefone de contato
        /// </summary>
        [Display(Name = "Telefone")]
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// ID do endereço (referência para tabela Address)
        /// </summary>
        public string? AddressId { get; set; }



        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Supplier()
        {
        }

        /// <summary>
        /// Construtor com dados básicos
        /// </summary>
        public Supplier(string name, string? email = null, string? phone = null)
        {
            Name = name;
            Email = email;
            Phone = phone;
        }

        /// <summary>
        /// Verifica se o fornecedor é pessoa jurídica
        /// </summary>
        public bool IsCompany()
        {
            return DocumentType == Enumerators.DocumentType.CNPJ;
        }

        /// <summary>
        /// Obtém o nome para exibição (prioriza nome fantasia sobre razão social)
        /// </summary>
        public string GetDisplayName()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return Name;
            
            if (!string.IsNullOrWhiteSpace(CompanyName))
                return CompanyName;
            
            return "Fornecedor sem nome";
        }

        /// <summary>
        /// Obtém informações de contato formatadas
        /// </summary>
        public string GetContactInfo()
        {
            var contacts = new List<string>();

            if (!string.IsNullOrWhiteSpace(Email))
                contacts.Add($"📧 {Email}");

            if (!string.IsNullOrWhiteSpace(Phone))
                contacts.Add($"📞 {Phone}");

            return contacts.Count > 0 ? string.Join(" | ", contacts) : "Sem contato informado";
        }

        /// <summary>
        /// Verifica se o fornecedor possui dados básicos completos
        /// </summary>
        public bool HasCompleteData()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   (!string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(Phone));
        }

        /// <summary>
        /// Obtém um resumo completo do fornecedor
        /// </summary>
        public string GetSupplierSummary()
        {
            var parts = new List<string>
            {
                GetDisplayName()
            };

            if (!string.IsNullOrWhiteSpace(DocumentNumber))
            {
                var docType = DocumentType?.ToString() ?? "Doc";
                parts.Add($"{docType}: {DocumentNumber}");
            }

            var contactInfo = GetContactInfo();
            if (contactInfo != "Sem contato informado")
                parts.Add(contactInfo);

            return string.Join(" - ", parts);
        }

        /// <summary>
        /// Override do ToString para exibir resumo do fornecedor
        /// </summary>
        public override string ToString()
        {
            return GetSupplierSummary();
        }
    }
} 