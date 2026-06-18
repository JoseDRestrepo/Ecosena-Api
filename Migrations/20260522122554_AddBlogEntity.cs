using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "fecha_nacimiento",
                table: "Usuarios",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titulo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    contenido = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    portada = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_publicacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    id_redactor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.id);
                    table.ForeignKey(
                        name: "FK_Entradas_Usuarios_id_redactor",
                        column: x => x.id_redactor,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "fecha_nacimiento" },
                values: new object[] { "$2a$11$e63wLe3jBw5iPoKa4p6R2u5Gjx.u/nFcSDnXGivk90Rq3gQk49UPS", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$qig0Hk1ggciu7DIpj3K1Tues1DXci06ycoUPzyYRemrKIqYKt2xKq");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_id_redactor",
                table: "Entradas",
                column: "id_redactor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "fecha_nacimiento",
                table: "Usuarios",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "fecha_nacimiento" },
                values: new object[] { "$2a$11$XIEjdiSl7htcAYpTvQo.YuHLFCKhlPIbx9mDXN70Jr0xUqax4qKAC", new DateOnly(1, 1, 1) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$lAt1.J6Au.3Rs2y4EnBwPeGq3kwhh/A6EBCN1QjCuk/7O5bO2MTGy");
        }
    }
}
