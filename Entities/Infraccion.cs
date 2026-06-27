using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("Infracciones")]
    public class Infraccion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_aprendiz")]
        public int AprendizId { get; set; }
        [ForeignKey(nameof(AprendizId))]
        public Usuario Aprendiz { get; set; }

        [Column("tipo_infraccion")]
        [StringLength(20)]
        public TipoInfraccion Tipo { get; set; }

        [Column("fecha_penalizacion")]
        public DateTime FechaInfraccion { get; set; } = DateTime.UtcNow;

        [Column("fecha_expiracion")]
        public DateTime? FechaExpiracion { get; set; }

        [Column("id_reporte")]
        public int? ReporteId { get; set; }

        [ForeignKey(nameof(ReporteId))]
        public Reporte Reporte { get; set; }
    }

    public enum TipoInfraccion
    {
        Intento,
        PenalizacionAdmin
    }
}
