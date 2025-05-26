using System.Collections.Generic;

namespace GesN.Web.Areas.Admin.Constants
{
    public static class SystemClaims
    {
        public static class Types
        {
            public const string Permission = "Permission";
            public const string Department = "Department";
            public const string AccessLevel = "AccessLevel";
            public const string Function = "Function";
        }

        public static class Permissions
        {
            // Usuários
            public const string ViewUsers = "ViewUsers";
            public const string CreateUsers = "CreateUsers";
            public const string EditUsers = "EditUsers";
            public const string DeleteUsers = "DeleteUsers";
            
            // Roles
            public const string ViewRoles = "ViewRoles";
            public const string CreateRoles = "CreateRoles";
            public const string EditRoles = "EditRoles";
            public const string DeleteRoles = "DeleteRoles";
            
            // Claims
            public const string ManageClaims = "ManageClaims";
        }

        public static class Departments
        {
            public const string IT = "TI";
            public const string HR = "RH";
            public const string Finance = "Financeiro";
            public const string Sales = "Vendas";
        }

        public static class AccessLevels
        {
            public const string Basic = "Básico";
            public const string Advanced = "Avançado";
            public const string Admin = "Administrador";
        }

        public static Dictionary<string, string[]> GetAvailableClaimValues()
        {
            return new Dictionary<string, string[]>
            {
                [Types.Permission] = new[]
                {
                    Permissions.ViewUsers,
                    Permissions.CreateUsers,
                    Permissions.EditUsers,
                    Permissions.DeleteUsers,
                    Permissions.ViewRoles,
                    Permissions.CreateRoles,
                    Permissions.EditRoles,
                    Permissions.DeleteRoles,
                    Permissions.ManageClaims
                },
                [Types.Department] = new[]
                {
                    Departments.IT,
                    Departments.HR,
                    Departments.Finance,
                    Departments.Sales
                },
                [Types.AccessLevel] = new[]
                {
                    AccessLevels.Basic,
                    AccessLevels.Advanced,
                    AccessLevels.Admin
                }
            };
        }
    }
} 