namespace EcoSENA.Api.Interfaces
{
    public interface ICensorshipService
    {
        bool EsSoez(string texto);
        string? PrimerMatch(string texto);
    }
}
