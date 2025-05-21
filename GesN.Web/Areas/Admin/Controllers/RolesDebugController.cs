using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesDebugController : Controller
    {
        private readonly IDbConnection _dbConnection;

        public RolesDebugController(ProjectDataContext context)
        {
            _dbConnection = context.Connection;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                ViewBag.ErrorMessage = "O nome da role é obrigatório.";
                return View("Index");
            }

            try
            {
                // Verificar se a role já existe
                var existingRole = await _dbConnection.QueryFirstOrDefaultAsync<ApplicationRole>(
                    "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName",
                    new { NormalizedName = roleName.ToUpper() });

                if (existingRole != null)
                {
                    ViewBag.ErrorMessage = $"Role '{roleName}' já existe.";
                    return View("Index");
                }

                // Criar a role
                var roleId = Guid.NewGuid().ToString();
                var query = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                await _dbConnection.ExecuteAsync(query, new
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                ViewBag.SuccessMessage = $"Role '{roleName}' criada com sucesso usando Dapper.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exceção ao criar role: {ex.Message}";
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                // Verificar se a tabela AspNetRoles existe
                var tableCheck = await _dbConnection.QueryAsync<string>(@"
                    SELECT name FROM sqlite_master 
                    WHERE type='table' AND name='AspNetRoles'");
                
                if (!tableCheck.Any())
                {
                    ViewBag.DbStatus = "A tabela AspNetRoles não existe!";
                    return View("Index");
                }

                // Verificar a estrutura da tabela
                var columns = await _dbConnection.QueryAsync<string>(@"
                    PRAGMA table_info(AspNetRoles)");
                
                ViewBag.TableStructure = string.Join(", ", columns);
                
                // Contar registros
                var count = await _dbConnection.ExecuteScalarAsync<int>(@"
                    SELECT COUNT(*) FROM AspNetRoles");
                
                ViewBag.DbStatus = $"A tabela AspNetRoles existe e contém {count} registros.";

                // Listar todas as roles
                var roles = await _dbConnection.QueryAsync<ApplicationRole>("SELECT * FROM AspNetRoles");
                ViewBag.Roles = roles;
            }
            catch (Exception ex)
            {
                ViewBag.DbStatus = $"Erro ao verificar banco de dados: {ex.Message}";
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> TestDirectCreate()
        {
            try
            {
                // Criar a role diretamente
                var roleId = Guid.NewGuid().ToString();
                var roleName = "TestRole_" + DateTime.Now.Ticks;
                
                var query = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                await _dbConnection.ExecuteAsync(query, new
                {
                    Id = roleId,
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });

                ViewBag.SuccessMessage = $"Role '{roleName}' criada com sucesso via teste direto.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Exceção ao criar role via teste direto: {ex.Message}";
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ConnectionTest()
        {
            try
            {
                ViewBag.ConnectionState = "Testando conexão...";
                
                // Testar se a conexão está funcionando
                var version = await _dbConnection.ExecuteScalarAsync<string>("SELECT sqlite_version()");
                
                ViewBag.ConnectionState = $"Conexão OK! Versão SQLite: {version}";
            }
            catch (Exception ex)
            {
                ViewBag.ConnectionState = $"Erro na conexão: {ex.Message}";
            }

            return View("Index");
        }
    }
} 