using System.Collections.Generic;

namespace GesN.Web.Data.Seeds.IdentitySeeds
{
    public static class IdentityConfiguration
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Atendimento = "Atendimento";
            public const string Producao = "Producao";
            public const string User = "User";
            public const string Financeiro = "Financeiro";

            public static IEnumerable<string> GetAllRoles()
            {
                yield return Admin;
                yield return Atendimento;
                yield return Producao;
                yield return User;
                yield return Financeiro;
            }
        }

        public static class Claims
        {
            public static class Types
            {
                public const string Permissao = "permissao";
            }

            public static class Values
            {
                public const string Criar = "criar";
                public const string Visualizar = "visualizar";
                public const string Editar = "editar";
                public const string Deletar = "deletar";
            }

            public static Dictionary<string, List<string>> GetAdminClaims()
            {
                return new Dictionary<string, List<string>>
                {
                    { Types.Permissao, new List<string> { Values.Criar, Values.Visualizar, Values.Editar, Values.Deletar } }
                };
            }

            public static Dictionary<string, List<string>> GetStandardUserClaims()
            {
                return new Dictionary<string, List<string>>
                {
                    { Types.Permissao, new List<string> { Values.Criar, Values.Visualizar, Values.Editar } }
                };
            }

            public static Dictionary<string, List<string>> GetBasicUserClaims()
            {
                return new Dictionary<string, List<string>>
                {
                    { Types.Permissao, new List<string> { Values.Visualizar } }
                };
            }
        }

        public static class AdminUser
        {
            public const string Email = "igor.gesn@gmail.com";
            public const string Password = "Admin@123";
        }

        public static Dictionary<string, Dictionary<string, List<string>>> GetRoleClaimsMap()
        {
            return new Dictionary<string, Dictionary<string, List<string>>>
            {
                { Roles.Admin, Claims.GetAdminClaims() },
                { Roles.Atendimento, Claims.GetStandardUserClaims() },
                { Roles.Producao, Claims.GetStandardUserClaims() },
                { Roles.User, Claims.GetBasicUserClaims() },
                { Roles.Financeiro, Claims.GetStandardUserClaims() }
            };
        }
    }
} 