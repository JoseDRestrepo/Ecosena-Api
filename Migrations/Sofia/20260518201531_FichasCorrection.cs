using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoSENA.Api.Migrations.Sofia
{
    /// <inheritdoc />
    public partial class FichasCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_Ficha_id_programa",
                table: "Matricula");

            migrationBuilder.RenameColumn(
                name: "id_programa",
                table: "Matricula",
                newName: "id_ficha");

            migrationBuilder.RenameIndex(
                name: "IX_Matricula_id_programa",
                table: "Matricula",
                newName: "IX_Matricula_id_ficha");

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_Ficha_id_ficha",
                table: "Matricula",
                column: "id_ficha",
                principalTable: "Ficha",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matricula_Ficha_id_ficha",
                table: "Matricula");

            migrationBuilder.RenameColumn(
                name: "id_ficha",
                table: "Matricula",
                newName: "id_programa");

            migrationBuilder.RenameIndex(
                name: "IX_Matricula_id_ficha",
                table: "Matricula",
                newName: "IX_Matricula_id_programa");

            migrationBuilder.AddForeignKey(
                name: "FK_Matricula_Ficha_id_programa",
                table: "Matricula",
                column: "id_programa",
                principalTable: "Ficha",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
