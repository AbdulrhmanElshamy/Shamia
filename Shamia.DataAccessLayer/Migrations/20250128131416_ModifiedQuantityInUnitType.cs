using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shamia.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedQuantityInUnitType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity_In_Unit",
                table: "ProductsOptions",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Quantity_In_Unit",
                table: "ProductsOptions",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
