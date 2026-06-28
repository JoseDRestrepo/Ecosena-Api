using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenRecuperacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tokens_recuperacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    codigo_hash = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expira_en = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    usado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens_recuperacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_tokens_recuperacion_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$4k7uNz2u5F2HQsd4jOLU3Ob4v/D0VYHg7ERw3I5cGnytG.8wg33ri");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$4LzKzOBtneCh6QAB6f1BvuAwFgQ2WK6jukLv2GrXCr825V3FqbPWm");

            migrationBuilder.CreateIndex(
                name: "IX_tokens_recuperacion_id_usuario",
                table: "tokens_recuperacion",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tokens_recuperacion");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$RcBhlIcVD0O8R1iDW.MOoelDwPQWmhaZXK6ACMyPtfefHJPNUD4wW");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$zlk6og0HbbZDoLUQvhmBIOXNoy3zy074xah16Gc9AZoJ8wptHjolW");
        }
    }
}
