using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFailedLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "failed_login_attempts",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Infracciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_aprendiz = table.Column<int>(type: "int", nullable: false),
                    tipo_infraccion = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    fecha_penalizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fecha_expiracion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    id_reporte = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Infracciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Infracciones_Reportes_id_reporte",
                        column: x => x.id_reporte,
                        principalTable: "Reportes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Infracciones_Usuarios_id_aprendiz",
                        column: x => x.id_aprendiz,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "failed_login_attempts", "lockout_end" },
                values: new object[] { "$2a$11$RcBhlIcVD0O8R1iDW.MOoelDwPQWmhaZXK6ACMyPtfefHJPNUD4wW", 0, null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                columns: new[] { "contrasenia", "failed_login_attempts", "lockout_end" },
                values: new object[] { "$2a$11$zlk6og0HbbZDoLUQvhmBIOXNoy3zy074xah16Gc9AZoJ8wptHjolW", 0, null });

            migrationBuilder.CreateIndex(
                name: "IX_Infracciones_id_aprendiz",
                table: "Infracciones",
                column: "id_aprendiz");

            migrationBuilder.CreateIndex(
                name: "IX_Infracciones_id_reporte",
                table: "Infracciones",
                column: "id_reporte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Infracciones");

            migrationBuilder.DropColumn(
                name: "failed_login_attempts",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "Usuarios");

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
    }
}
