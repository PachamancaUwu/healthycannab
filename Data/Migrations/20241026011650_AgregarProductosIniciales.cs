using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace healthycannab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProductosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "producto",
                columns: new[] { "Id", "Descripcion", "ImagenUrl", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, "Aceite de cannabis 100% puro", "/img/a.png", "Producto 1", 150m },
                    { 2, "Ungüento de cannabis", "/img/b.jpg", "Producto 2", 120m },
                    { 3, "Cannabis CBD cannabidiol", "/img/canabis3.png", "Producto 3", 95m },
                    { 4, "Cannabis To you", "/img/canabis4.png", "Producto 4", 105m },
                    { 5, "Cannabis Gotas", "/img/cannabisGotas.jpg", "Producto 5", 99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "producto",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "producto",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "producto",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "producto",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "producto",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
