using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class CloudinaryUpdt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "foto_perfil",
                keyValue: null,
                column: "foto_perfil",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "foto_perfil",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "foto_perfil" },
                values: new object[] { "$2a$11$FRnXpBy8v3wzeteLFtOxuORpqSzTm3zsRYiLJSKdx98/e9zkyJA8W", "https://res.cloudinary.com/denixbxml/image/upload/v1780584233/pfp_yutpht.png" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                columns: new[] { "contrasenia", "foto_perfil" },
                values: new object[] { "$2a$11$Dcxvo2hDC.wg/H9gC0induOFQcnW4HJQ5HF9V.EmX2/m5qNdzkU9a", "https://res.cloudinary.com/denixbxml/image/upload/v1780584233/pfp_yutpht.png" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "foto_perfil",
                table: "Usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                columns: new[] { "contrasenia", "foto_perfil" },
                values: new object[] { "$2a$11$smZ/JgokRcY8ru7ZO4YDCunwpE7Wq/cDvQ5MZ/xzFU5PkW2hkhGnO", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                columns: new[] { "contrasenia", "foto_perfil" },
                values: new object[] { "$2a$11$qIzyh3o1IBCwXrlmk7I1A.k0ZDQZVdjCEWEIpLh65wbfYUgDJfWFS", null });
        }
    }
}
