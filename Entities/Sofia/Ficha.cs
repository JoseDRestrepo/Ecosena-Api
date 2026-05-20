using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities.Sofia
{
    [Table("Ficha")]
    public class Ficha
    {
        [Key]
        [Column("Id")]
        public required int Id { get; set; }

        [Column("numero")]
        public required int Numero { get; set; }

        [Column("id_programa_formacion")]
        public required int ProgramaFormacionId { get; set; }

        [ForeignKey(nameof(ProgramaFormacionId))]
        public ProgramaFormacion ProgramaFormacion { get; set; }

        public List<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
