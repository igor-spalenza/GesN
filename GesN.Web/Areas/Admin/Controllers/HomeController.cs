using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Admin.Models;
using GesN.Web.Interfaces;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new AdminHomeViewModel();

                // Total de usuários
                model.TotalUsers = await GetTotalUsersAsync();

                // Total de roles
                model.TotalRoles = await GetTotalRolesAsync();

                // Total de claims distintas
                model.TotalClaims = await GetTotalClaimsAsync();

                // Admins ativos
                model.ActiveAdmins = await GetActiveAdminsAsync();

                // Usuários ativos (últimos 30 dias) - baseado em LastLoginDate se existir
                model.ActiveUsers = await GetActiveUsersAsync();

                // Usuários cadastrados hoje
                model.UsersToday = await GetUsersTodayAsync();

                // Claims únicas (distintas por tipo + valor)
                model.UniqueClaims = await GetUniqueClaimsAsync();

                // Roles que possuem claims
                model.RolesWithClaims = await GetRolesWithClaimsAsync();

                // Dados adicionais para futuras expansões
                model.RecentUsers = await GetRecentUsersAsync();
                model.RoleDistribution = await GetRoleDistributionAsync();
                model.TopClaims = await GetTopClaimsAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                // Log do erro
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar estatísticas da Home Admin: {ex.Message}");
                
                // Retornar modelo vazio em caso de erro
                return View(new AdminHomeViewModel());
            }
        }

        private async Task<int> GetTotalUsersAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetUsers";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetTotalRolesAsync()
        {
            const string query = "SELECT COUNT(*) FROM AspNetRoles";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetTotalClaimsAsync()
        {
            const string query = @"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetUserClaims
                    UNION
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetRoleClaims
                ) AS DistinctClaims";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetActiveAdminsAsync()
        {
            const string query = @"
                SELECT COUNT(DISTINCT u.Id)
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = 'Admin'";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetActiveUsersAsync()
        {
            // Por enquanto, consideramos todos os usuários como ativos
            // No futuro, pode ser implementado baseado em LastLoginDate
            const string query = "SELECT COUNT(*) FROM AspNetUsers";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetUsersTodayAsync()
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM AspNetUsers 
                WHERE DATE(CreatedDate) = DATE('now')";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetUniqueClaimsAsync()
        {
            const string query = @"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetUserClaims
                    UNION
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetRoleClaims
                ) AS DistinctClaims";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<int> GetRolesWithClaimsAsync()
        {
            const string query = @"
                SELECT COUNT(DISTINCT RoleId) 
                FROM AspNetRoleClaims";
            return await _unitOfWork.Connection.QuerySingleAsync<int>(query, transaction: _unitOfWork.Transaction);
        }

        private async Task<List<UserStatsViewModel>> GetRecentUsersAsync()
        {
            const string query = @"
                SELECT  
                    u.Id as UserId,
                    u.UserName,
                    u.Email,
                    u.CreatedDate,
                    COUNT(DISTINCT ur.RoleId) as RoleCount,
                    COUNT(DISTINCT uc.Id) as ClaimCount
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                GROUP BY u.Id, u.UserName, u.Email, u.CreatedDate
                ORDER BY u.CreatedDate DESC
                LIMIT 5";

            var result = await _unitOfWork.Connection.QueryAsync<UserStatsViewModel>(query, transaction: _unitOfWork.Transaction);
            return result.ToList();
        }

        private async Task<List<RoleStatsViewModel>> GetRoleDistributionAsync()
        {
            const string query = @"
                SELECT 
                    r.Id as RoleId,
                    r.Name as RoleName,
                    COUNT(DISTINCT ur.UserId) as UserCount,
                    COUNT(DISTINCT rc.Id) as ClaimCount,
                    r.CreatedDate
                FROM AspNetRoles r
                LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                GROUP BY r.Id, r.Name, r.CreatedDate
                ORDER BY UserCount DESC";

            var result = await _unitOfWork.Connection.QueryAsync<RoleStatsViewModel>(query, transaction: _unitOfWork.Transaction);
            return result.ToList();
        }

        private async Task<List<ClaimStatsViewModel>> GetTopClaimsAsync()
        {
            const string query = @"
                SELECT 
                    ClaimType as Type,
                    ClaimValue as Value,
                    UserCount,
                    RoleCount
                FROM (
                    SELECT 
                        ClaimType,
                        ClaimValue,
                        COUNT(DISTINCT CASE WHEN UserId IS NOT NULL THEN UserId END) as UserCount,
                        COUNT(DISTINCT CASE WHEN RoleId IS NOT NULL THEN RoleId END) as RoleCount
                    FROM (
                        SELECT ClaimType, ClaimValue, UserId, NULL as RoleId FROM AspNetUserClaims
                        UNION ALL
                        SELECT ClaimType, ClaimValue, NULL as UserId, RoleId FROM AspNetRoleClaims
                    ) AS AllClaims
                    GROUP BY ClaimType, ClaimValue
                ) AS ClaimStats
                ORDER BY (UserCount + RoleCount) DESC
                LIMIT 5";

            var result = await _unitOfWork.Connection.QueryAsync<ClaimStatsViewModel>(query, transaction: _unitOfWork.Transaction);
            return result.ToList();
        }
    }
} 