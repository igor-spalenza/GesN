using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GesN.Web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implemente aqui a lógica real de envio de e-mail
            // Por exemplo, usando SMTP, SendGrid, MailKit, etc.
            
            // Por enquanto, apenas logamos as informações
            _logger.LogInformation($"Email para: {email}, Assunto: {subject}");
            _logger.LogInformation($"Mensagem: {htmlMessage}");
            
            return Task.CompletedTask;
        }
    }
} 