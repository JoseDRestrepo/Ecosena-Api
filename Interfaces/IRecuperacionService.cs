namespace EcoSENA.Api.Interfaces
{
    public interface IRecuperacionService
    {
        Task SolicitarRecuperacionAsync(string documento, string email);
        Task<bool> RestablecerContrasenaAsync(string codigo, string nuevaContrasena);
    }
}
