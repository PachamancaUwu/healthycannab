using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace healthycannab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacionesBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comentario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comentario_producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comentario_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pedido_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_precio",
                columns: table => new
                {
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_precio", x => new { x.PedidoId, x.ProductoId });
                    table.ForeignKey(
                        name: "FK_detalle_precio_pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalle_precio_producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comentario_ProductoId",
                table: "comentario",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_comentario_UsuarioId",
                table: "comentario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_precio_ProductoId",
                table: "detalle_precio",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_UsuarioId",
                table: "pedido",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentario");

            migrationBuilder.DropTable(
                name: "detalle_precio");

            migrationBuilder.DropTable(
                name: "pedido");
        }
    }
}
