namespace EcoSENA.Api.Interfaces
{
    public interface IPenalizacionService
    {
        public Task RegistrarIntento(int aprendizId);
        public Task PenalizacionAdmin(int id, int reporteId);
        public Task<(bool Bloq, DateTime? Expiracion)> VerificarPenalizacion(int aprendizId);
    }
}
