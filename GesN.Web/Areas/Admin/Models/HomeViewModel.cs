using System.ComponentModel.DataAnnotations;

namespace GesN.Web.Areas.Admin.Models
{
    public class AdminHomeViewModel
    {
        [Display(Name = "Total de Usuários")]
        public int TotalUsers { get; set; }

        [Display(Name = "Total de Roles")]
        public int TotalRoles { get; set; }

        [Display(Name = "Total de Claims")]
        public int TotalClaims { get; set; }

        [Display(Name = "Admins Ativos")]
        public int ActiveAdmins { get; set; }

        [Display(Name = "Usuários Ativos (últimos 30 dias)")]
        public int ActiveUsers { get; set; }

        [Display(Name = "Usuários Cadastrados Hoje")]
        public int UsersToday { get; set; }

        [Display(Name = "Claims Únicas")]
        public int UniqueClaims { get; set; }

        [Display(Name = "Roles com Claims")]
        public int RolesWithClaims { get; set; }

        // Estatísticas adicionais para gráficos futuros
        public List<UserStatsViewModel> RecentUsers { get; set; } = new();
        public List<RoleStatsViewModel> RoleDistribution { get; set; } = new();
        public List<ClaimStatsViewModel> TopClaims { get; set; } = new();
    }

    public class UserStatsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int RoleCount { get; set; }
        public int ClaimCount { get; set; }
        public bool IsActive { get; set; }
    }

    public class RoleStatsViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int ClaimCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ClaimStatsViewModel
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public int TotalAssignments => UserCount + RoleCount;
    }
} 