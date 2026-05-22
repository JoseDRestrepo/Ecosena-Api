using BC = BCrypt.Net.BCrypt;
using EcoSENA.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Data
{
    public class EcosenaDbContext(DbContextOptions<EcosenaDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Entrada> Entradas { get; set; }

        //datos de ejemplo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Bohorquez",
                    Documento = "0123456789",
                    Correo = "juan_bquez@soy.sena.edu.co",
                    ContraseñaHash = BC.HashPassword(Environment.GetEnvironmentVariable("APRENDIZ_PASSWORD")),
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
                    ContraseñaHash = BC.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD")),
                    Rol = RolUsuario.Administrador
                }
            );
        }
    }
}
