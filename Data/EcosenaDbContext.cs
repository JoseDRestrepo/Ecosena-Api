using BC = BCrypt.Net.BCrypt;
using EcoSENA.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Data
{
    public class EcosenaDbContext(DbContextOptions<EcosenaDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Entrada> Entradas { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Ambiente> Ambientes { get; set; }

        //datos de ejemplo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            const string defaultProfileImage = "https://res.cloudinary.com/denixbxml/image/upload/v1780584233/pfp_yutpht.png";

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Bohorquez",
                    Documento = "0123456789",
                    Correo = "juan_bquez@soy.sena.edu.co",
                    ContraseñaHash = BC.HashPassword(Environment.GetEnvironmentVariable("APRENDIZ_PASSWORD")),
                    FotoPerfil = defaultProfileImage,
                    Ficha = 333333,
                    ProgramaFormacion = "ADSO",
                    Rol = RolUsuario.Aprendiz
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Lamine Yamal",
                    Apellido = "Nasraoui Ebana",
                    Documento = "1111111111",
                    Correo = "lamine_yamal@sena.edu.co",
                    FechaNacimiento = new DateOnly(2007, 07, 13),
                    FotoPerfil = defaultProfileImage,
                    ContraseñaHash = BC.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD")),
                    Rol = RolUsuario.Administrador
                }
            );

            modelBuilder.Entity<Ambiente>().HasData(
                new Ambiente
                {
                    Id = 1,
                    Nombre = "Baño de hombres Piso 1",
                    ReporteActivo = false
                },
                new Ambiente
                {
                    Id = 2,
                    Nombre = "Baño de mujeres Piso 1",
                    ReporteActivo = false
                },
                new Ambiente
                {
                    Id = 3,
                    Nombre = "Renata 3"
                },
                new Ambiente
                {
                    Id = 4,
                    Nombre = "Renata 2"
                },
                new Ambiente
                {
                    Id = 5,
                    Nombre = "Renata 1"
                },
                new Ambiente
                {
                    Id = 6,
                    Nombre = "Automatización Aula 01"
                },
                new Ambiente
                {
                    Id = 7,
                    Nombre = "Automatización Aula 02"
                },
                new Ambiente
                {
                    Id = 8,
                    Nombre = "Automatización Aula 03"
                },
                new Ambiente
                {
                    Id = 9,
                    Nombre = "Automatización Aula 04"
                },
                new Ambiente
                {
                    Id = 10,
                    Nombre = "Entrada Renata"
                },
                new Ambiente
                {
                    Id = 11,
                    Nombre = "Baño de hombres piso 2"
                },
                new Ambiente
                {
                    Id = 12,
                    Nombre = "Baño de mujeres piso 2"
                }
            );
        }
    }
}
