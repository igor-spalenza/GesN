using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Dapper;
using GesN.Web.Data;

namespace GesN.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleClaimsController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly DapperRoleClaimStore _roleClaimStore;
        private readonly ProjectDataContext _dataContext;

        public RoleClaimsController(
            RoleManager<ApplicationRole> roleManager,
            DapperRoleClaimStore roleClaimStore,
            ProjectDataContext dataContext)
        {
            _roleManager = roleManager;
            _roleClaimStore = roleClaimStore;
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            // Obter roles diretamente do banco de dados em vez de usar _roleManager.Roles
            var roles = await _dataContext.Connection.QueryAsync<ApplicationRole>("SELECT * FROM AspNetRoles");
            var viewModel = new List<RoleClaimsViewModel>();

            foreach (var role in roles)
            {
                var claims = await _roleClaimStore.GetClaimsAsync(role, CancellationToken.None);
                viewModel.Add(new RoleClaimsViewModel
                {
                    Role = role,
                    Claims = claims
                });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var claims = await _roleClaimStore.GetClaimsAsync(role, CancellationToken.None);
            var viewModel = new RoleClaimsViewModel
            {
                Role = role,
                Claims = claims
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AddClaim(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var viewModel = new AddClaimViewModel
            {
                RoleId = roleId,
                RoleName = role.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClaim(AddClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    return NotFound();
                }

                // Verificar se a claim já existe
                var existingClaims = await _roleClaimStore.GetClaimsAsync(role, CancellationToken.None);
                if (existingClaims.Any(c => c.Type == model.ClaimType && c.Value == model.ClaimValue))
                {
                    ModelState.AddModelError("", "Esta claim já existe para esta role.");
                    return View(model);
                }

                // Adicionar a claim
                await _roleClaimStore.AddClaimAsync(role, new Claim(model.ClaimType, model.ClaimValue), CancellationToken.None);

                return RedirectToAction(nameof(Details), new { id = model.RoleId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteClaim(string roleId, string claimType, string claimValue)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            await _roleClaimStore.RemoveClaimAsync(role, new Claim(claimType, claimValue), CancellationToken.None);

            return RedirectToAction(nameof(Details), new { id = roleId });
        }

        [HttpGet]
        public async Task<IActionResult> AssignPermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            var claims = await _roleClaimStore.GetClaimsAsync(role, CancellationToken.None);
            
            // Obter as permissões atuais
            var currentPermissions = claims
                .Where(c => c.Type == "permissao")
                .Select(c => c.Value)
                .ToList();

            var viewModel = new AssignPermissionsViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                Permissions = GetAllPermissions(),
                SelectedPermissions = currentPermissions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPermissions(AssignPermissionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role == null)
                {
                    return NotFound();
                }

                // Remover todas as claims de permissão existentes
                var existingClaims = await _roleClaimStore.GetClaimsAsync(role, CancellationToken.None);
                foreach (var claim in existingClaims.Where(c => c.Type == "permissao").ToList())
                {
                    await _roleClaimStore.RemoveClaimAsync(role, claim, CancellationToken.None);
                }

                // Adicionar as novas permissões selecionadas
                if (model.SelectedPermissions != null)
                {
                    foreach (var permission in model.SelectedPermissions)
                    {
                        await _roleClaimStore.AddClaimAsync(role, new Claim("permissao", permission), CancellationToken.None);
                    }
                }

                return RedirectToAction(nameof(Details), new { id = model.RoleId });
            }

            // Se o ModelState não for válido, recarregar as permissões
            model.Permissions = GetAllPermissions();
            return View(model);
        }

        private List<string> GetAllPermissions()
        {
            // Lista completa de permissões disponíveis no sistema
            return new List<string>
            {
                "usuarios:gerenciar",
                "clientes:visualizar",
                "clientes:gerenciar",
                "pedidos:visualizar",
                "pedidos:gerenciar"
            };
        }
    }

    public class RoleClaimsViewModel
    {
        public ApplicationRole Role { get; set; }
        public IList<Claim> Claims { get; set; }
    }

    public class AddClaimViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }

    public class AssignPermissionsViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> SelectedPermissions { get; set; }
    }
} 