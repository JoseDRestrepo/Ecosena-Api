using EcoSENA.Api.Entities.Sofia;
using Microsoft.EntityFrameworkCore;

namespace EcoSENA.Api.Data
{
    public class SofiaDbContext(DbContextOptions<SofiaDbContext> options) : DbContext(options)
    {
        public DbSet<SofiaUsuario> SofiaUsuarios { get; set; }

        //datos para simular usuarios 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SofiaUsuario>().HasData(
                new SofiaUsuario
                {
                    Id = 1,
                    Documento = "000001",
                    Nombre = "Lionel Andres Messi Cuccittini",
                    EmailPersonal = "lionel.messi@gmail.com",
                    EmailSena = "lionel_andresmessi@sena.edu.co",
                    FechaNacimiento = new DateOnly(1987, 06, 24)
                },
                new SofiaUsuario
                {
                    Id = 2,
                    Documento = "000002",
                    Nombre = "Cristiano Ronaldo dos Santos Aveiro",
                    EmailPersonal = "ronaldo.cris@yahoo.com",
                    EmailSena = "cristiano_dsantos@soy.sena.edu.co",
                    FechaNacimiento = new DateOnly(1985, 02, 05)
                },
                new SofiaUsuario
                {
                    Id = 3,
                    Documento = "000003",
                    Nombre = "Neymar da Silva Santos Junior",
                    EmailPersonal = "neymar_jr@gmail.com",
                    EmailSena = "lionel_andresmessi@sena.edu.co",
                    FechaNacimiento = new DateOnly(1992, 02, 05)
                },
                new SofiaUsuario
                {
                    Id = 4,
                    Documento = "000004",
                    Nombre = "Robert Lewandowski",
                    EmailPersonal = "lewan.robert@gmail.com",
                    EmailSena = "robert_lewandowski@soy.sena.edu.co",
                    FechaNacimiento = new DateOnly(1988, 08, 21)
                },
                new SofiaUsuario
                {
                    Id = 5,
                    Documento = "000005",
                    Nombre = "Juan Fernando Quintero Paniagua",
                    EmailPersonal = "juanfer10@gmail.com",
                    EmailSena = "juanfer_quintero@sena.edu.co",
                    FechaNacimiento = new DateOnly(1993, 01, 18)
                },
                new SofiaUsuario 
                { 
                    Id = 6,
                    Documento = "000006",
                    Nombre = "Kylian Mbappé Lottin",
                    EmailPersonal = "mbappe@yahoo.com",
                    FechaNacimiento = new DateOnly(1998, 12, 20)
                }
            );
        }
    }
}
