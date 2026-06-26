using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EcoSENA.Api.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id_usuario")]
        public int Id { get; set; }

        [Column("nombre")]
        [StringLength(50)]
        public required string Nombre { get; set; }

        [Column("apellido")]
        [StringLength(50)]
        public required string Apellido { get; set; }

        [Column("documento")]
        [StringLength(30)]
        public required string Documento { get; set; }

        [Column("contrasenia")]
        [StringLength(250)]
        public required string ContraseñaHash { get; set; }

        [Column("email")]
        [StringLength(50)]
        public required string Correo { get; set; }

        [Column("fecha_nacimiento")]
        public DateOnly? FechaNacimiento { get; set; }

        [Column("foto_perfil")]
        public required string FotoPerfil { get; set; }

        [Column("programa_formacion")]
        public string? ProgramaFormacion { get; set; }

        [Column("ficha")]
        public int? Ficha { get; set; }

        [Column("rol", TypeName ="varchar(20)")]
        [StringLength(20)]
        public required RolUsuario Rol { get; set; }

        [JsonIgnore]
        public ICollection<Infraccion> Infracciones { get; set; } = [];
    }

    public enum RolUsuario
    {
        Aprendiz,
        Administrador,
        Penalizado
    }
}
