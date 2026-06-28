namespace EcoSENA.Api.Models.Recover
{
    public class ResetPasswordReq
    {
        public required string Codigo { get; set; }
        public required string NuevaContraseña { get; set; }
        public required string ConfirmacionContraseña { get; set; }
    }
}
