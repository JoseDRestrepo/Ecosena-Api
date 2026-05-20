using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("Entradas")]
    public class Entrada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("titulo")]
        [StringLength(50)]
        public required string Titulo { get; set; }

        [Column("contenido")]
        public required string Contenido { get; set; }

        [Column("portada")]
        public string? Portada { get; set; }

        [Column("fecha_publicacion")]
        public required DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;
    }
}
