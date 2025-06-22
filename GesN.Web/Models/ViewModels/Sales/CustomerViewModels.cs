using System.ComponentModel.DataAnnotations;
using GesN.Web.Models.Entities.Sales;
using GesN.Web.Models.Enumerators;

namespace GesN.Web.Models.ViewModels.Sales
{
    public class CustomerViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O primeiro nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O primeiro nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Primeiro Nome")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo {1} caracteres")]
        [Display(Name = "Sobrenome")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo {1} caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O documento é obrigatório")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo {1} caracteres")]
        [Display(Name = "Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        [Display(Name = "Tipo de Documento")]
        public DocumentType DocumentType { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo {1} caracteres")]
        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; } = ObjectState.Active;

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        [Display(Name = "Nome Completo")]
        public string FullName => string.IsNullOrWhiteSpace(FirstName) ? "Cliente sem nome" : $"{FirstName} {LastName ?? ""}".Trim();

        [Display(Name = "Iniciais")]
        public string Initials => string.IsNullOrWhiteSpace(FirstName) ? "?" : FirstName.Substring(0, 1).ToUpper();

        [Display(Name = "Tipo de Documento")]
        public string DocumentTypeDisplay => DocumentType == GesN.Web.Models.Enumerators.DocumentType.CPF ? "CPF" : "CNPJ";

        [Display(Name = "Status")]
        public string StateCodeDisplay => StateCode == ObjectState.Active ? "Ativo" : "Inativo";
        
        [Display(Name = "Tipo de Cliente")]
        public string CustomerTypeDisplay => DocumentType == GesN.Web.Models.Enumerators.DocumentType.CPF ? "Pessoa Física" : "Pessoa Jurídica";
    }

    public class CreateCustomerViewModel
    {
        [Required(ErrorMessage = "O primeiro nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O primeiro nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Primeiro Nome")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo {1} caracteres")]
        [Display(Name = "Sobrenome")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo {1} caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O documento é obrigatório")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo {1} caracteres")]
        [Display(Name = "Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        [Display(Name = "Tipo de Documento")]
        public DocumentType DocumentType { get; set; } = GesN.Web.Models.Enumerators.DocumentType.CPF;

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo {1} caracteres")]
        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "Tipos de Documento Disponíveis")]
        public List<DocumentTypeSelectionViewModel> AvailableDocumentTypes { get; set; } = new()
        {
            new() { Value = GesN.Web.Models.Enumerators.DocumentType.CPF, Text = "Pessoa Física (CPF)", IsSelected = true },
            new() { Value = GesN.Web.Models.Enumerators.DocumentType.CNPJ, Text = "Pessoa Jurídica (CNPJ)", IsSelected = false }
        };
    }

    public class EditCustomerViewModel
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O primeiro nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O primeiro nome deve ter no máximo {1} caracteres")]
        [Display(Name = "Primeiro Nome")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O sobrenome deve ter no máximo {1} caracteres")]
        [Display(Name = "Sobrenome")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(200, ErrorMessage = "O email deve ter no máximo {1} caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O documento é obrigatório")]
        [StringLength(20, ErrorMessage = "O documento deve ter no máximo {1} caracteres")]
        [Display(Name = "Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        [Display(Name = "Tipo de Documento")]
        public DocumentType DocumentType { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo {1} caracteres")]
        [Display(Name = "Telefone")]
        public string? Phone { get; set; }

        [Display(Name = "Status")]
        public ObjectState StateCode { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Última Modificação")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Tipos de Documento Disponíveis")]
        public List<DocumentTypeSelectionViewModel> AvailableDocumentTypes { get; set; } = new()
        {
            new() { Value = GesN.Web.Models.Enumerators.DocumentType.CPF, Text = "Pessoa Física (CPF)", IsSelected = false },
            new() { Value = GesN.Web.Models.Enumerators.DocumentType.CNPJ, Text = "Pessoa Jurídica (CNPJ)", IsSelected = false }
        };
    }

    public class CustomerDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
        public string? Phone { get; set; }
        public ObjectState StateCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Propriedades calculadas
        public string FullName => string.IsNullOrWhiteSpace(FirstName) ? "Cliente sem nome" : $"{FirstName} {LastName ?? ""}".Trim();
        public string Initials => string.IsNullOrWhiteSpace(FirstName) ? "?" : FirstName.Substring(0, 1).ToUpper();
        public string DocumentTypeDisplay => DocumentType == GesN.Web.Models.Enumerators.DocumentType.CPF ? "CPF" : "CNPJ";
        public string StateCodeDisplay => StateCode == ObjectState.Active ? "Ativo" : "Inativo";
        public string CustomerTypeDisplay => DocumentType == GesN.Web.Models.Enumerators.DocumentType.CPF ? "Pessoa Física" : "Pessoa Jurídica";
        public string FormattedCreatedAt => CreatedAt.ToString("dd/MM/yyyy HH:mm");
        public string FormattedLastModifiedAt => LastModifiedAt?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }

    public class CustomerIndexViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new();
        public CustomerStatisticsViewModel Statistics { get; set; } = new();
        public CustomerSearchViewModel Search { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCustomers { get; set; }
    }

    public class CustomerStatisticsViewModel
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int IndividualCustomers { get; set; }
        public int CorporateCustomers { get; set; }
        public int NewCustomersThisMonth { get; set; }
    }

    public class CustomerSearchViewModel
    {
        [StringLength(100, ErrorMessage = "A busca deve ter no máximo {1} caracteres")]
        [Display(Name = "Buscar")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Tipo de Documento")]
        public DocumentType? DocumentType { get; set; }

        [Display(Name = "Status")]
        public ObjectState? StateCode { get; set; }

        public List<DocumentTypeSelectionViewModel> GetAvailableDocumentTypes()
        {
            return new List<DocumentTypeSelectionViewModel>
            {
                new() { Value = GesN.Web.Models.Enumerators.DocumentType.CPF, Text = "CPF", IsSelected = false },
                new() { Value = GesN.Web.Models.Enumerators.DocumentType.CNPJ, Text = "CNPJ", IsSelected = false }
            };
        }

        public List<StateSelectionViewModel> GetAvailableStates()
        {
            return new List<StateSelectionViewModel>
            {
                new() { Value = ObjectState.Active, Text = "Ativo", IsSelected = false },
                new() { Value = ObjectState.Inactive, Text = "Inativo", IsSelected = false }
            };
        }
    }

    public class DocumentTypeSelectionViewModel
    {
        public DocumentType Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class StateSelectionViewModel
    {
        public ObjectState Value { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 