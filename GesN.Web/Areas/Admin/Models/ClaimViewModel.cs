using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GesN.Web.Areas.Admin.Models
{
    public class ClaimViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "O tipo da claim é obrigatório")]
        [Display(Name = "Tipo")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor da claim é obrigatório")]
        [Display(Name = "Valor")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "Usuários")]
        public string? Users { get; set; }

        [Display(Name = "Roles")]
        public string? Roles { get; set; }

        [Display(Name = "Quantidade de Usuários")]
        public int UserCount { get; set; }

        [Display(Name = "Quantidade de Roles")]
        public int RoleCount { get; set; }

        // Propriedades adicionais para compatibilidade com views
        public List<UserSelectionViewModel> UsersWithClaim { get; set; } = new();
        public List<RoleSelectionClaimViewModel> RolesWithClaim { get; set; } = new();
    }

    public class CreateClaimViewModel
    {
        [Required(ErrorMessage = "O tipo da claim é obrigatório")]
        [StringLength(256, ErrorMessage = "O tipo da claim deve ter no máximo {1} caracteres")]
        [Display(Name = "Tipo da Claim")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor da claim é obrigatório")]
        [StringLength(256, ErrorMessage = "O valor da claim deve ter no máximo {1} caracteres")]
        [Display(Name = "Valor da Claim")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "Usuários Selecionados")]
        public List<string> SelectedUsers { get; set; } = new();

        [Display(Name = "Usuários Disponíveis")]
        public List<UserSelectionViewModel> AvailableUsers { get; set; } = new();

        [Display(Name = "Roles Selecionadas")]
        public List<string> SelectedRoles { get; set; } = new();

        [Display(Name = "Roles Disponíveis")]
        public List<RoleSelectionClaimViewModel> AvailableRoles { get; set; } = new();

        // Propriedades adicionais para compatibilidade com views
        [Display(Name = "IDs de Usuários Selecionados")]
        public List<string> SelectedUserIds { get; set; } = new();

        [Display(Name = "IDs de Roles Selecionadas")]
        public List<string> SelectedRoleIds { get; set; } = new();

        [Display(Name = "Tipos Comuns de Claims")]
        public List<string> CommonClaimTypes { get; set; } = new()
        {
            "permission.users.read", "permission.users.create", "permission.users.update", "permission.users.delete",
            "permission.roles.read", "permission.roles.create", "permission.roles.update", "permission.roles.delete",
            "permission.claims.read", "permission.claims.create", "permission.claims.update", "permission.claims.delete",
            "permission.admin.access", "permission.reports.read", "permission.settings.manage",
            "Nome", "CPF", "RG", "DataNascimento", "Endereco", "Cidade", "Estado", "CEP", 
            "Departamento", "Cargo", "DataAdmissao", "Matricula", "NivelAcesso"
        };

        [Display(Name = "Tipos Disponíveis de Claims")]
        public List<string> AvailableClaimTypes => CommonClaimTypes;
    }

    public class EditClaimViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo da claim é obrigatório")]
        [StringLength(256, ErrorMessage = "O tipo da claim deve ter no máximo {1} caracteres")]
        [Display(Name = "Tipo da Claim")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor da claim é obrigatório")]
        [StringLength(256, ErrorMessage = "O valor da claim deve ter no máximo {1} caracteres")]
        [Display(Name = "Valor da Claim")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "Usuários Associados")]
        public List<UserSelectionViewModel> AssociatedUsers { get; set; } = new();

        [Display(Name = "Roles Associadas")]
        public List<RoleSelectionClaimViewModel> AssociatedRoles { get; set; } = new();

        // Propriedades adicionais para compatibilidade com views
        [Display(Name = "Usuários Disponíveis")]
        public List<UserSelectionViewModel> AvailableUsers { get; set; } = new();

        [Display(Name = "Roles Disponíveis")]
        public List<RoleSelectionClaimViewModel> AvailableRoles { get; set; } = new();

        [Display(Name = "IDs de Usuários Selecionados")]
        public List<string> SelectedUserIds { get; set; } = new();

        [Display(Name = "IDs de Roles Selecionadas")]
        public List<string> SelectedRoleIds { get; set; } = new();

        [Display(Name = "Tipos Comuns de Claims")]
        public List<string> CommonClaimTypes { get; set; } = new()
        {
            "permission.users.read", "permission.users.create", "permission.users.update", "permission.users.delete",
            "permission.roles.read", "permission.roles.create", "permission.roles.update", "permission.roles.delete",
            "permission.claims.read", "permission.claims.create", "permission.claims.update", "permission.claims.delete",
            "permission.admin.access", "permission.reports.read", "permission.settings.manage",
            "Nome", "CPF", "RG", "DataNascimento", "Endereco", "Cidade", "Estado", "CEP", 
            "Departamento", "Cargo", "DataAdmissao", "Matricula", "NivelAcesso"
        };

        [Display(Name = "Tipos Disponíveis de Claims")]
        public List<string> AvailableClaimTypes => CommonClaimTypes;
    }

    public class RoleSelectionClaimViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public class ClaimDetailViewModel
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public List<UserSelectionViewModel> UsersWithClaim { get; set; } = new();
        public List<RoleSelectionClaimViewModel> RolesWithClaim { get; set; } = new();
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }

        // Propriedades de compatibilidade
        public int UserCount => TotalUsers;
        public int RoleCount => TotalRoles;
    }
} 