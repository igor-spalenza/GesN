using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Areas.Identity.Data.Models.Users
{
    public class UserDetailsModel
    {
        public string Id { get; set; }

        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }

        [Display(Name = "Nome Completo")]
        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Funções")]
        public string Roles { get; set; }

        [Display(Name = "Data de Cadastro")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Último Acesso")]
        public DateTime? LastAccess { get; set; }
    }
} 