using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GesN.Web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            
            // Ler configurações do appsettings.json
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.example.com";
            _smtpPort = int.TryParse(_configuration["EmailSettings:SmtpPort"], out int port) ? port : 587;
            _enableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out bool ssl) ? ssl : true;
            _smtpUsername = _configuration["EmailSettings:Username"] ?? "";
            _smtpPassword = _configuration["EmailSettings:Password"] ?? "";
            _senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "noreply@example.com";
            _senderName = _configuration["EmailSettings:SenderName"] ?? "GesN Sistema";
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                // Verificar se estamos em modo de desenvolvimento
                bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                
                if (isDevelopment)
                {
                    // Em desenvolvimento, apenas logamos o email
                    _logger.LogInformation($"[DEV] Email para: {email}, Assunto: {subject}");
                    _logger.LogInformation($"[DEV] Mensagem: {htmlMessage}");
                    return;
                }
                
                // Criar a mensagem
                var message = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email));

                // Configurar o cliente SMTP
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = _enableSsl;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    
                    // Enviar o email
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Email enviado com sucesso para {email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao enviar email para {email}: {ex.Message}");
                // Não lançamos a exceção para não interromper o fluxo da aplicação
            }
        }
    }
} 