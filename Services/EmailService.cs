using EcoSENA.Api.Interfaces;
using Resend;

namespace EcoSENA.Api.Services
{
    public class EmailService(IResend resend, IConfiguration configuration) : IEmailService
    {
        public async Task EnviarCodigoRecuperacionAsync(string destinatario, string codigo)
        {
            var mensaje = new EmailMessage
            {
                From = configuration["Email:From"]!,
                To = ["joserestrepo.student@gmail.com"],
                Subject = "Reestablecer contraseña - ECOSENA",
                HtmlBody = $"""
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

            await resend.EmailSendAsync(mensaje);
        }
    }
}
