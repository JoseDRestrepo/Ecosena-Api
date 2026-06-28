using EcoSENA.Api.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EcoSENA.Api.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly string _from = configuration["Email:From"]!;
        private readonly string _password = configuration["Email:Password"]!;

        public async Task EnviarCodigoRecuperacionAsync(string destinatario, string codigo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(MailboxAddress.Parse(_from));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = "Reestablecer contraseña - ECOSENA";
            mensaje.Body = new TextPart("html")
            {
                Text = $"""
                    <div style="font-family:Arial,sans-serif;max-width:400px;margin:auto">
                        <h2 style="color:#2e7d32">ECOSENA</h2>
                        <p>Recibimos una solicitud para reestablecer tu contraseña.</p>
                        <p>Tu código de verificación es:</p>
                        <h1 style="letter-spacing:8px;color:#2e7d32">{codigo}</h1>
                        <p>Este código expira en <strong>15 minutos</strong>.</p>
                        <p>Si no solicitaste reestablecer tu contraseña, ignora este mensaje.</p>
                    </div>
                    """
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_from, _password);
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);
        }
    }
}