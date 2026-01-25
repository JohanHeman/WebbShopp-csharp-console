using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class adddatetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SoldAt",
                table: "CheckoutProducts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldAt",
                table: "CheckoutProducts");
        }
    }
}
