using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Admin.Models;
using System.Data;
using GesN.Web.Data;
using GesN.Web.Infrastructure.Data;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public HomeController(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new AdminHomeViewModel();
                model.TotalUsers = await GetTotalUsersAsync();
                model.TotalRoles = await GetTotalRolesAsync();
                model.TotalClaims = await GetTotalClaimsAsync();
                model.ActiveAdmins = await GetActiveAdminsAsync();
                model.ActiveUsers = await GetActiveUsersAsync();
                model.UniqueClaims = await GetUniqueClaimsAsync();
                model.RolesWithClaims = await GetRolesWithClaimsAsync();

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
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = "SELECT COUNT(*) FROM AspNetUsers";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetTotalRolesAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = "SELECT COUNT(*) FROM AspNetRoles";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetTotalClaimsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM AspNetUserClaims) + 
                    (SELECT COUNT(*) FROM AspNetRoleClaims) AS TotalClaims";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetActiveAdminsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT COUNT(DISTINCT u.Id)
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = 'Admin'";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetActiveUsersAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            // Por enquanto, consideramos todos os usuários como ativos
            // No futuro, pode ser implementado baseado em LastLoginDate
            const string query = "SELECT COUNT(*) FROM AspNetUsers";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetUniqueClaimsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT COUNT(*) FROM (
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetUserClaims
                    UNION
                    SELECT DISTINCT ClaimType, ClaimValue FROM AspNetRoleClaims
                ) AS DistinctClaims";
            return await connection.QuerySingleAsync<int>(query);
        }

        private async Task<int> GetRolesWithClaimsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT COUNT(DISTINCT RoleId) 
                FROM AspNetRoleClaims";
            return await connection.QuerySingleAsync<int>(query);
        }

        // Métodos auxiliares para estatísticas futuras (não utilizados atualmente)
        
        /*
        private async Task<List<UserStatsViewModel>> GetRecentUsersAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT 
                    u.Id as UserId,
                    u.UserName,
                    u.Email,
                    datetime('now') as CreatedDate,
                    COUNT(DISTINCT ur.RoleId) as RoleCount,
                    COUNT(DISTINCT uc.Id) as ClaimCount
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetUserClaims uc ON u.Id = uc.UserId
                GROUP BY u.Id, u.UserName, u.Email
                ORDER BY u.UserName
                LIMIT 5";

            var result = await connection.QueryAsync<UserStatsViewModel>(query);
            return result.ToList();
        }

        private async Task<List<RoleStatsViewModel>> GetRoleDistributionAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            const string query = @"
                SELECT 
                    r.Id as RoleId,
                    r.Name as RoleName,
                    COUNT(DISTINCT ur.UserId) as UserCount,
                    COUNT(DISTINCT rc.Id) as ClaimCount,
                    datetime('now') as CreatedDate
                FROM AspNetRoles r
                LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                GROUP BY r.Id, r.Name
                ORDER BY UserCount DESC";

            var result = await connection.QueryAsync<RoleStatsViewModel>(query);
            return result.ToList();
        }

        private async Task<List<ClaimStatsViewModel>> GetTopClaimsAsync()
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
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

            var result = await connection.QueryAsync<ClaimStatsViewModel>(query);
            return result.ToList();
        }
        */
    }
} 