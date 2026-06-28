namespace EcoSENA.Api.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCodigoRecuperacionAsync(string destinatario, string codigo);
    }
}
