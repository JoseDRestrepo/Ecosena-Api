using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities
{
    [Table("tokens_recuperacion")]
    public class TokenRecuperacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_usuario")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }

        [Column("codigo_hash")]
        [StringLength(250)]
        public required string CodigoHash { get; set; }

        [Column("expira_en")]
        public DateTime ExpiraEn { get; set; }

        [Column("usado")]
        public bool Usado { get; set; } = false;
    }
}
