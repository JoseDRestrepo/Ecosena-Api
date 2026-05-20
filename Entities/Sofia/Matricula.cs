using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities.Sofia
{
    [Table("Matricula")]
    public class Matricula
    {
        [Key]
        [Column("id")]
        public required int Id { get; set; }

        [Column("id_aprendiz")]
        public required int AprendizId { get; set; }

        [ForeignKey(nameof(AprendizId))]
        public SofiaUsuario Aprendiz { get; set; }

        [Column("id_ficha")]
        public required int FichaId { get; set; }

        [ForeignKey(nameof(FichaId))]
        public Ficha Ficha { get; set; }
    }
}
