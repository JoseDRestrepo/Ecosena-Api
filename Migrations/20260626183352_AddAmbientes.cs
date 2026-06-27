using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAmbientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ambientes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    reporte_activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ambientes", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Ambientes",
                columns: new[] { "id", "nombre", "reporte_activo" },
                values: new object[,]
                {
                    { 1, "Baño de hombres Piso 1", false },
                    { 2, "Baño de mujeres Piso 1", false },
                    { 3, "Renata 3", false },
                    { 4, "Renata 2", false },
                    { 5, "Renata 1", false },
                    { 6, "Automatización Aula 01", false },
                    { 7, "Automatización Aula 02", false },
                    { 8, "Automatización Aula 03", false },
                    { 9, "Automatización Aula 04", false },
                    { 10, "Entrada Renata", false },
                    { 11, "Baño de hombres piso 2", false },
                    { 12, "Baño de mujeres piso 2", false }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$ivaGxTI5XyzWYMgWzkBh0OdsZeEMa.gf653l5QpZo7n3aZtjfGPJu");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$z9sruJmhgtA7qYdugJroweZ3VyvNLALMvW5TgobIHoGmwL0XRIDXm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ambientes");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$FRnXpBy8v3wzeteLFtOxuORpqSzTm3zsRYiLJSKdx98/e9zkyJA8W");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$Dcxvo2hDC.wg/H9gC0induOFQcnW4HJQ5HF9V.EmX2/m5qNdzkU9a");
        }
    }
}
