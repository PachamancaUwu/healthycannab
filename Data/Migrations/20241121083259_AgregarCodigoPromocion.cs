using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace healthycannab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoPromocion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CodigoPromocionId",
                table: "pedido",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "codigo_promocion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric", nullable: false),
                    Usado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_codigo_promocion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pedido_CodigoPromocionId",
                table: "pedido",
                column: "CodigoPromocionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_codigo_promocion_Codigo",
                table: "codigo_promocion",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_pedido_codigo_promocion_CodigoPromocionId",
                table: "pedido",
                column: "CodigoPromocionId",
                principalTable: "codigo_promocion",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pedido_codigo_promocion_CodigoPromocionId",
                table: "pedido");

            migrationBuilder.DropTable(
                name: "codigo_promocion");

            migrationBuilder.DropIndex(
                name: "IX_pedido_CodigoPromocionId",
                table: "pedido");

            migrationBuilder.DropColumn(
                name: "CodigoPromocionId",
                table: "pedido");
        }
    }
}
