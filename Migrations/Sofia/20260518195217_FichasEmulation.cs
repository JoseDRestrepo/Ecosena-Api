using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoSENA.Api.Migrations.Sofia
{
    /// <inheritdoc />
    public partial class FichasEmulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramasFormacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasFormacion", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ficha",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    numero = table.Column<int>(type: "int", nullable: false),
                    id_programa_formacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ficha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ficha_ProgramasFormacion_id_programa_formacion",
                        column: x => x.id_programa_formacion,
                        principalTable: "ProgramasFormacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Matricula",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_aprendiz = table.Column<int>(type: "int", nullable: false),
                    id_programa = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matricula", x => x.id);
                    table.ForeignKey(
                        name: "FK_Matricula_Ficha_id_programa",
                        column: x => x.id_programa,
                        principalTable: "Ficha",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matricula_Usuarios_id_aprendiz",
                        column: x => x.id_aprendiz,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ProgramasFormacion",
                columns: new[] { "id", "nombre" },
                values: new object[,]
                {
                    { 1, "ADSO" },
                    { 2, "Electricidad Industrial" },
                    { 3, "Animación 3D" }
                });

            migrationBuilder.InsertData(
                table: "Ficha",
                columns: new[] { "Id", "numero", "id_programa_formacion" },
                values: new object[,]
                {
                    { 1, 333333, 1 },
                    { 2, 333334, 2 },
                    { 3, 333344, 3 }
                });

            migrationBuilder.InsertData(
                table: "Matricula",
                columns: new[] { "id", "id_aprendiz", "id_programa" },
                values: new object[,]
                {
                    { 1, 2, 2 },
                    { 3, 4, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ficha_id_programa_formacion",
                table: "Ficha",
                column: "id_programa_formacion");

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_id_aprendiz",
                table: "Matricula",
                column: "id_aprendiz");

            migrationBuilder.CreateIndex(
                name: "IX_Matricula_id_programa",
                table: "Matricula",
                column: "id_programa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matricula");

            migrationBuilder.DropTable(
                name: "Ficha");

            migrationBuilder.DropTable(
                name: "ProgramasFormacion");
        }
    }
}
