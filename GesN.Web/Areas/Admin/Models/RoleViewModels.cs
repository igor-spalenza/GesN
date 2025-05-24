using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "O nome da role é obrigatório")]
        [StringLength(256, ErrorMessage = "O nome da role deve ter no máximo {1} caracteres")]
        [Display(Name = "Nome da Role")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Claims")]
        public List<ClaimViewModel> Claims { get; set; } = new();

        [Display(Name = "Claims Disponíveis")]
        public List<string> AvailableClaimTypes { get; set; } = new()
        {
            "permission.users.read", "permission.users.create", "permission.users.update", "permission.users.delete",
            "permission.roles.read", "permission.roles.create", "permission.roles.update", "permission.roles.delete",
            "permission.claims.read", "permission.claims.create", "permission.claims.update", "permission.claims.delete",
            "permission.admin.access", "permission.reports.read", "permission.settings.manage"
        };
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
        public List<string> AvailableClaimTypes { get; set; } = new()
        {
            "permission.users.read", "permission.users.create", "permission.users.update", "permission.users.delete",
            "permission.roles.read", "permission.roles.create", "permission.roles.update", "permission.roles.delete",
            "permission.claims.read", "permission.claims.create", "permission.claims.update", "permission.claims.delete",
            "permission.admin.access", "permission.reports.read", "permission.settings.manage"
        };

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