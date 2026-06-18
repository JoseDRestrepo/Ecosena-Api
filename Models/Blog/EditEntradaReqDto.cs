using System.ComponentModel.DataAnnotations;

namespace EcoSENA.Api.Models.Blog
{
    public class EditEntradaReqDto
    {
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        public string Contenido { get; set; } = string.Empty;

        public IFormFile? Portada { get; set; }
    }
}
