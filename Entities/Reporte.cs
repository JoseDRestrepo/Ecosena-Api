using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("reportes")]
    public class Reporte
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("titulo")]
        public required string Titulo { get; set; }

        [Column("descripcion")]
        public required string Descripcion { get; set; }

        [Column("ubicacion")]
        public required string Ubicacion { get; set; }

        [Column("fecha_publicacion")]
        public required DateOnly FechaEmision { get; set; }

        [Column("fecha_revision")]
        public required DateOnly FechaRevision { get; set; }

        [Column("fecha_solucion")]
        public required DateOnly FechaSolucion { get; set; }

        [Column("estado")]
        public required string Estado { get; set; } = "Pendiente";

        [Column("id_aprendiz")]
        public int AprendizId { get; set; }

        [ForeignKey(nameof(AprendizId))]
        public Usuario Aprendiz { get; set; }
    }
}
