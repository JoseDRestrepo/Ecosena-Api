using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_reporte")]
        public int ReporteId { get; set; }

        [ForeignKey(nameof(ReporteId))]
        public Reporte Reporte { get; set; }

        [Column("id_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; }

        [Column("fecha_envio")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Column("leida")]
        public bool Leida { get; set; }

        [Column("entregada")]
        public bool Entregada { get; set; } = false;
    }
}
