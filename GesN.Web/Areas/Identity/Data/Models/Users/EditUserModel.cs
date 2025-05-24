using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Areas.Identity.Data.Models.Users
{
    public class EditUserModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Nome de Usuário")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "O sobrenome é obrigatório")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "Telefone inválido")]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Funções")]
        public List<string> SelectedRoles { get; set; } = new();
        public List<string> AvailableRoles { get; set; } = new();
    }
} 