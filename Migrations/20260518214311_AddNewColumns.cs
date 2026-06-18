using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "email",
                keyValue: null,
                column: "email",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Usuarios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ficha",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "foto_perfil",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "programa_formacion",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "fecha_nacimiento", "ficha", "foto_perfil", "programa_formacion" },
                values: new object[] { "$2a$11$XIEjdiSl7htcAYpTvQo.YuHLFCKhlPIbx9mDXN70Jr0xUqax4qKAC", new DateOnly(1, 1, 1), 333333, null, "ADSO" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                columns: new[] { "contrasenia", "fecha_nacimiento", "ficha", "foto_perfil", "programa_formacion" },
                values: new object[] { "$2a$11$lAt1.J6Au.3Rs2y4EnBwPeGq3kwhh/A6EBCN1QjCuk/7O5bO2MTGy", new DateOnly(2007, 7, 13), null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ficha",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "foto_perfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "programa_formacion",
                table: "Usuarios");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "fecha_nacimiento",
                table: "Usuarios",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Usuarios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "fecha_nacimiento" },
                values: new object[] { "$2a$11$HAIVXy/ZrpdKxyAYXKCCWONFepbcdAKS7dbkBNtAjwYbRF6yqaA9G", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                columns: new[] { "contrasenia", "fecha_nacimiento" },
                values: new object[] { "$2a$11$nCSDSRsItuH66SvPre25vO67DULhdyLBD5WoXGN5PupfRxPNGF2Qy", null });
        }
    }
}
