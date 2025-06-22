using GesN.Web.Models.Entities.Sales;

namespace GesN.Web.Areas.Integration.Services
{
    /// <summary>
    /// Interface para operações com Google People API
    /// </summary>
    public interface IGooglePeopleService
    {
        /// <summary>
        /// Indica se a integração está habilitada e configurada
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Cria um contato no Google Contacts
        /// </summary>
        /// <param name="customer">Dados do cliente</param>
        /// <returns>ID do contato criado no Google ou null se falhou</returns>
        Task<string?> CreateContactAsync(Customer customer);

        /// <summary>
        /// Atualiza um contato no Google Contacts
        /// </summary>
        /// <param name="customer">Dados atualizados do cliente</param>
        /// <param name="googleContactId">ID do contato no Google</param>
        /// <returns>True se atualizou com sucesso</returns>
        Task<bool> UpdateContactAsync(Customer customer, string googleContactId);

        /// <summary>
        /// Remove um contato do Google Contacts
        /// </summary>
        /// <param name="googleContactId">ID do contato no Google</param>
        /// <returns>True se removeu com sucesso</returns>
        Task<bool> DeleteContactAsync(string googleContactId);

        /// <summary>
        /// Busca um contato no Google Contacts por email
        /// </summary>
        /// <param name="email">Email para buscar</param>
        /// <returns>ID do contato encontrado ou null</returns>
        Task<string?> FindContactByEmailAsync(string email);

        /// <summary>
        /// Busca um contato no Google Contacts por ID
        /// </summary>
        /// <param name="googleContactId">ID do contato no Google</param>
        /// <returns>Dados do contato ou null se não encontrado</returns>
        Task<GoogleContactData?> GetContactAsync(string googleContactId);

        /// <summary>
        /// Sincroniza um cliente com Google Contacts (cria se não existe, atualiza se existe)
        /// </summary>
        /// <param name="customer">Dados do cliente</param>
        /// <returns>ID do contato no Google ou null se falhou</returns>
        Task<string?> SyncContactAsync(Customer customer);

        /// <summary>
        /// Valida se as credenciais estão funcionando
        /// </summary>
        /// <returns>True se as credenciais são válidas</returns>
        Task<bool> ValidateCredentialsAsync();
    }

    /// <summary>
    /// Dados de um contato do Google
    /// </summary>
    public class GoogleContactData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? LastModified { get; set; }
    }
} 