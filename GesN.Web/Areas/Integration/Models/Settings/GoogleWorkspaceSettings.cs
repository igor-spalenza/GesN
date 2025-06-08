namespace GesN.Web.Areas.Integration.Models.Settings
{
    /// <summary>
    /// Configurações para integração com Google Workspace
    /// </summary>
    public class GoogleWorkspaceSettings
    {
        /// <summary>
        /// Nome da seção no appsettings.json
        /// </summary>
        public const string SectionName = "GoogleWorkspace";

        /// <summary>
        /// Client ID da aplicação Google
        /// </summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>
        /// Client Secret da aplicação Google
        /// </summary>
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Domínio do Google Workspace
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Caminho para o arquivo de credenciais de serviço
        /// </summary>
        public string ServiceAccountKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// Email da conta de serviço para impersonalização
        /// </summary>
        public string ServiceAccountEmail { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário administrador para impersonalização
        /// </summary>
        public string ImpersonateUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Indica se a integração está habilitada
        /// </summary>
        public bool IsEnabled { get; set; } = false;

        /// <summary>
        /// Timeout para chamadas à API (em segundos)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Indica se deve criar contatos automaticamente no Google
        /// </summary>
        public bool AutoCreateContacts { get; set; } = true;

        /// <summary>
        /// Indica se deve sincronizar automaticamente com o Google
        /// </summary>
        public bool AutoSync { get; set; } = true;
    }
} 