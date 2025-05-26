using Dapper;
using GesN.Web.Areas.Identity.Data.Models;
using GesN.Web.Areas.Identity.Data.Stores;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GesN.Web.Data
{
    public class SeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly DapperRoleClaimStore _roleClaimStore;
        private readonly ProjectDataContext _dataContext;

        public SeedData(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            DapperRoleClaimStore roleClaimStore,
            ProjectDataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleClaimStore = roleClaimStore;
            _dataContext = dataContext;
        }

        public async Task Initialize()
        {
            // Criar roles padrão
            await CreateRoles();
            
            // Criar usuário administrador padrão
            await CreateAdminUser();
            
            // Atribuir permissões às roles
            await SetupRolePermissions();
        }

        private async Task CreateRoles()
        {
            // Lista de roles padrão
            string[] roleNames = { "Admin", "Gerente", "Vendedor", "Cliente" };

            foreach (var roleName in roleNames)
            {
                // Verificar se a role já existe
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    // Criar a role
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    };

                    await _roleManager.CreateAsync(role);
                    Console.WriteLine($"Role '{roleName}' criada com sucesso.");
                }
                else
                {
                    Console.WriteLine($"Role '{roleName}' já existe.");
                }
            }
        }

        private async Task CreateAdminUser()
        {
            // Dados do usuário administrador padrão
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "igor.spalenza94@gmail.com",
                EmailConfirmed = true,
                FirstName = "Administrador",
                LastName = "Sistema",
                PhoneNumber = "31975092916",
                PhoneNumberConfirmed = true
            };

            // Verificar se o usuário já existe
            var user = await _userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                // Criar o usuário com senha padrão
                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    Console.WriteLine($"Usuário administrador '{adminUser.Email}' criado com sucesso.");
                    
                    // Adicionar o usuário à role Admin
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"Usuário '{adminUser.Email}' adicionado à role 'Admin'.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Erro ao criar usuário administrador: {errors}");
                }
            }
            else
            {
                Console.WriteLine($"Usuário administrador '{adminUser.Email}' já existe.");
                
                // Garantir que o usuário está na role Admin
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine($"Usuário '{user.Email}' adicionado à role 'Admin'.");
                }
                
                // Atualizar as propriedades FirstName e LastName se necessário
                bool needsUpdate = false;
                
                if (string.IsNullOrEmpty(user.FirstName))
                {
                    user.FirstName = "Administrador";
                    needsUpdate = true;
                }
                
                if (string.IsNullOrEmpty(user.LastName))
                {
                    user.LastName = "Sistema";
                    needsUpdate = true;
                }
                
                if (needsUpdate)
                {
                    await _userManager.UpdateAsync(user);
                    Console.WriteLine($"Informações do usuário '{user.Email}' atualizadas.");
                }
            }
        }

        private async Task SetupRolePermissions()
        {
            // Configurar permissões para a role Admin
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                // Admin tem todas as permissões
                var adminPermissions = new Dictionary<string, List<string>>
                {
                    { "permissao", new List<string> { 
                        "usuarios:gerenciar", 
                        "clientes:visualizar", 
                        "clientes:gerenciar", 
                        "pedidos:visualizar", 
                        "pedidos:gerenciar" 
                    }}
                };
                
                await _roleClaimStore.SetRoleClaimsAsync(adminRole.Id, adminPermissions);
                Console.WriteLine("Permissões da role 'Admin' configuradas com sucesso.");
            }

            // Configurar permissões para a role Gerente
            var gerenteRole = await _roleManager.FindByNameAsync("Gerente");
            if (gerenteRole != null)
            {
                // Gerente tem permissões de visualização e gerenciamento, exceto para usuários
                var gerentePermissions = new Dictionary<string, List<string>>
                {
                    { "permissao", new List<string> { 
                        "clientes:visualizar", 
                        "clientes:gerenciar", 
                        "pedidos:visualizar", 
                        "pedidos:gerenciar" 
                    }}
                };
                
                await _roleClaimStore.SetRoleClaimsAsync(gerenteRole.Id, gerentePermissions);
                Console.WriteLine("Permissões da role 'Gerente' configuradas com sucesso.");
            }

            // Configurar permissões para a role Vendedor
            var vendedorRole = await _roleManager.FindByNameAsync("Vendedor");
            if (vendedorRole != null)
            {
                // Vendedor só tem permissões de visualização
                var vendedorPermissions = new Dictionary<string, List<string>>
                {
                    { "permissao", new List<string> { 
                        "clientes:visualizar", 
                        "pedidos:visualizar"
                    }}
                };
                
                await _roleClaimStore.SetRoleClaimsAsync(vendedorRole.Id, vendedorPermissions);
                Console.WriteLine("Permissões da role 'Vendedor' configuradas com sucesso.");
            }
        }
    }
} 