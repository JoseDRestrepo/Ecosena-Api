using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoSENA.Api.Migrations.Sofia
{
    /// <inheritdoc />
    public partial class GenerateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    documento = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    correo_contacto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    correo_institucional = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_nacimiento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "documento", "correo_contacto", "correo_institucional", "fecha_nacimiento", "nombre" },
                values: new object[,]
                {
                    { 1, "000001", "lionel.messi@gmail.com", "lionel_andresmessi@sena.edu.co", new DateOnly(1987, 6, 24), "Lionel Andres Messi Cuccittini" },
                    { 2, "000002", "ronaldo.cris@yahoo.com", "cristiano_dsantos@soy.sena.edu.co", new DateOnly(1985, 2, 5), "Cristiano Ronaldo dos Santos Aveiro" },
                    { 3, "000003", "neymar_jr@gmail.com", "lionel_andresmessi@sena.edu.co", new DateOnly(1992, 2, 5), "Neymar da Silva Santos Junior" },
                    { 4, "000004", "lewan.robert@gmail.com", "robert_lewandowski@soy.sena.edu.co", new DateOnly(1988, 8, 21), "Robert Lewandowski" },
                    { 5, "000005", "juanfer10@gmail.com", "juanfer_quintero@sena.edu.co", new DateOnly(1993, 1, 18), "Juan Fernando Quintero Paniagua" },
                    { 6, "000006", "mbappe@yahoo.com", null, new DateOnly(1998, 12, 20), "Kylian Mbappé Lottin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
