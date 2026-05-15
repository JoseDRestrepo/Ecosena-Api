using System.ComponentModel.DataAnnotations.Schema;

namespace EcoSENA.Api.Entities.Sofia
{
    [Table("Usuarios")]
    public class SofiaUsuario
    {
        [Column("Id")]
        public required int Id { get; set; }

        [Column("nombre")]
        public required string Nombre { get; set; }

        [Column("documento")]
        public required string Documento { get; set; }

        [Column("correo_contacto")]
        public required string EmailPersonal { get; set; }

        [Column("correo_institucional")]
        public string? EmailSena { get; set; }

        [Column("fecha_nacimiento")]
        public required DateOnly FechaNacimiento { get; set; }
    }
}
