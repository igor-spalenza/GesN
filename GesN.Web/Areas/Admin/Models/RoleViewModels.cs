using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace GesN.Web.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O nome da role é obrigatório")]
        [Display(Name = "Nome da Role")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Nome Normalizado")]
        public string? NormalizedName { get; set; }

        [Display(Name = "Usuários")]
        public string? Users { get; set; }

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();

        [Display(Name = "Quantidade de Usuários")]
        public int UserCount { get; set; }
    }

    public class CreateRoleViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "O nome da role é obrigatório")]
        [StringLength(256, ErrorMessage = "O nome da role deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome da Role")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();

        [Display(Name = "Claims Disponíveis")]
        public List<string> AvailableClaimTypes { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Regra de negócio: uma role deve ter pelo menos 1 claim
            var validClaims = Claims?.Where(c => !string.IsNullOrWhiteSpace(c.Type) && !string.IsNullOrWhiteSpace(c.Value)).ToList();
            
            if (validClaims == null || !validClaims.Any())
            {
                yield return new ValidationResult(
                    "Uma role deve ter pelo menos uma claim associada. Adicione pelo menos uma claim antes de salvar.",
                    new[] { nameof(Claims) });
            }
        }
    }

    public class EditRoleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome da role é obrigatório")]
        [StringLength(256, ErrorMessage = "O nome da role deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome da Role")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Nome Normalizado")]
        public string? NormalizedName { get; set; }

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();

        [Display(Name = "Claims Disponíveis")]
        public List<string> AvailableClaimTypes { get; set; } = new();

        [Display(Name = "Usuários Associados")]
        public List<UserSelectionViewModel> AssociatedUsers { get; set; } = new();
    }

    public class UserSelectionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
} 