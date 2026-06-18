namespace EcoSENA.Api.Models.Profile
{
    public class ProfileResDto
    {
        public string? FotoPerfil { get; set; }
        public required string Nombre { get; set; }
        public int? Ficha { get; set; }
        public string? Programa { get; set; }
        public required string Email { get; set; }
        public required DateOnly FechaNacimiento { get; set; }
    }
}
