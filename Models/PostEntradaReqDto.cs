using System.ComponentModel.DataAnnotations;

namespace EcoSENA.Api.Models
{
    public class PostEntradaReqDto
    {
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        public string Contenido { get; set; } = string.Empty;

        public string? Portada { get; set; }
    }
}
