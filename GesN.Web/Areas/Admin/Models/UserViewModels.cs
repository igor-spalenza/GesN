using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GesN.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Funções")]
        public string? Roles { get; set; }

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();
    }

    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação não correspondem.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Funções")]
        public List<string> SelectedRoles { get; set; } = new();

        [Display(Name = "Funções Disponíveis")]
        public List<RoleSelectionViewModel> AvailableRoles { get; set; } = new();

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Funções")]
        public List<string> SelectedRoles { get; set; } = new();

        [Display(Name = "Funções Disponíveis")]
        public List<string> AvailableRoles { get; set; } = new();

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();

        [Display(Name = "Claims Disponíveis")]
        public List<string> AvailableClaimTypes { get; set; } = new()
        {
            "Nome", "CPF", "RG", "DataNascimento", "Endereco",
            "Cidade", "Estado", "CEP", "Departamento", "Cargo",
            "DataAdmissao", "Matricula", "NivelAcesso"
        };
    }

    public class RoleSelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 