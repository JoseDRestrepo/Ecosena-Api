using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EcoSENA.Api.Interfaces;

namespace EcoSENA.Api.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Brevo:ApiKey"]!;
            _fromEmail = configuration["Email:From"]!;
            _fromName = configuration["Email:FromName"] ?? "ECOSENA";
        }

        public async Task EnviarCodigoRecuperacionAsync(string destinatario, string codigo)
        {
            var htmlContent = $"""
                <div style="font-family:Arial,sans-serif;max-width:400px;margin:auto">
                    <h2 style="color:#2e7d32">ECOSENA</h2>
                    <p>Recibimos una solicitud para reestablecer tu contraseña.</p>
                    <p>Tu código de verificación es:</p>
                    <h1 style="letter-spacing:8px;color:#2e7d32">{codigo}</h1>
                    <p>Este código expira en <strong>15 minutos</strong>.</p>
                    <p>Si no solicitaste reestablecer tu contraseña, ignora este mensaje.</p>
                </div>
                """;

            var payload = new
            {
                sender = new { name = _fromName, email = _fromEmail },
                to = new[] { new { email = destinatario } },
                subject = "Reestablecer contraseña - ECOSENA",
                htmlContent
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("api-key", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Error al enviar correo vía Brevo: {response.StatusCode} - {error}");
            }
        }
    }
}