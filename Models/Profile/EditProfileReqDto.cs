using System.ComponentModel.DataAnnotations;

namespace EcoSENA.Api.Models.Profile
{
    public class EditProfileReqDto
    {
        [EmailAddress]
        public string? Email { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public string? Contraseña { get; set; }

        public string? ConfirmacionContraseña { get; set; }

        public IFormFile? FotoPerfil { get; set; }
    }
}
