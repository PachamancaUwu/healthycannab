using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace healthycannab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRebuildComentario3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comentario_usuario_UsuarioId1",
                table: "comentario");

            migrationBuilder.DropIndex(
                name: "IX_comentario_UsuarioId1",
                table: "comentario");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "comentario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId1",
                table: "comentario",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comentario_UsuarioId1",
                table: "comentario",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_comentario_usuario_UsuarioId1",
                table: "comentario",
                column: "UsuarioId1",
                principalTable: "usuario",
                principalColumn: "Id");
        }
    }
}
