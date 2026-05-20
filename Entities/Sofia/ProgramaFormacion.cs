using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities.Sofia
{
    [Table("ProgramasFormacion")]
    public class ProgramaFormacion
    {
        [Key]
        [Column("id")]
        public required int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        public List<Ficha> Fichas { get; set; } = new List<Ficha>();
    }
}
