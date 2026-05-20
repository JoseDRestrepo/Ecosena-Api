namespace EcoSENA.Api.Models
{
    public class EntradaResDto
    {
        public int Id { get; set; }

        public required string Titulo { get; set; }

        public required string Contenido { get; set; }

        public string? Portada { get; set; }

        public DateTime FechaPublicacion { get; set; }
    }
}
