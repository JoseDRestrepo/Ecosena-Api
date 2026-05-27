using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddReportModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    titulo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ubicacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    foto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_publicacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fecha_revision = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    fecha_solucion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_aprendiz = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reportes", x => x.id);
                    table.ForeignKey(
                        name: "FK_reportes_Usuarios_id_aprendiz",
                        column: x => x.id_aprendiz,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_reporte = table.Column<int>(type: "int", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    leida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    entregada = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_notificaciones_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notificaciones_reportes_id_reporte",
                        column: x => x.id_reporte,
                        principalTable: "reportes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$smZ/JgokRcY8ru7ZO4YDCunwpE7Wq/cDvQ5MZ/xzFU5PkW2hkhGnO");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$qIzyh3o1IBCwXrlmk7I1A.k0ZDQZVdjCEWEIpLh65wbfYUgDJfWFS");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_id_reporte",
                table: "notificaciones",
                column: "id_reporte");

            migrationBuilder.CreateIndex(
                name: "IX_notificaciones_id_usuario",
                table: "notificaciones",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_reportes_id_aprendiz",
                table: "reportes",
                column: "id_aprendiz");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notificaciones");

            migrationBuilder.DropTable(
                name: "reportes");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 1,
                column: "contrasenia",
                value: "$2a$11$e63wLe3jBw5iPoKa4p6R2u5Gjx.u/nFcSDnXGivk90Rq3gQk49UPS");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "id_usuario",
                keyValue: 2,
                column: "contrasenia",
                value: "$2a$11$qig0Hk1ggciu7DIpj3K1Tues1DXci06ycoUPzyYRemrKIqYKt2xKq");
        }
    }
}
