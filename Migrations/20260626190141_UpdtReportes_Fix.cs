using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdtReportes_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$M8juu4wWI6tVbS6xDt62K.PsIpxM0G9..1DirLywkdAe79vitYhbe");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$mxHjvlal9murxeM831cwM.xDRgkW/7855DGO37ppH0kAsarxRIlI6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reportes_Ambientes_id_ambiente",
                table: "Reportes");

            migrationBuilder.DropIndex(
                name: "IX_Reportes_id_ambiente",
                table: "Reportes");

            migrationBuilder.DropColumn(
                name: "id_ambiente",
                table: "Reportes");

            migrationBuilder.AddColumn<string>(
                name: "ubicacion",
                table: "Reportes",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

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
    }
}
