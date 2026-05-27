using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("Reportes")]
    public class Reporte
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("titulo")]
        [StringLength(50)]
        public required string Titulo { get; set; }

        [Column("descripcion")]
        [StringLength(150)]
        public required string Descripcion { get; set; }

        [Column("ubicacion")]
        [StringLength(100)]
        public required string Ubicacion { get; set; }

        [Column("foto")]
        public required string Foto { get; set; }

        [Column("fecha_publicacion")]
        public required DateTime FechaEmision { get; set; }

        [Column("fecha_revision")]
        public DateTime? FechaRevision { get; set; }

        [Column("fecha_solucion")]
        public DateTime? FechaSolucion { get; set; }

        [Column("estado", TypeName ="varchar(20)")]
        [StringLength(20)]
        public required EstadoReporte Estado { get; set; } 

        [Column("id_aprendiz")]
        public int AprendizId { get; set; }

        [ForeignKey(nameof(AprendizId))]
        public Usuario Aprendiz { get; set; }
    }

    public enum EstadoReporte
    {
        Pendiente,
        EnProgreso,
        Resuelto
    }
}
