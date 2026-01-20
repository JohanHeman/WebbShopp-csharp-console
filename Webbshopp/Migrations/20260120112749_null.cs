using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class @null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Addresses_AdressId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Checkouts_CheckOutId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CheckOutId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_AdressId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "AdressId",
                table: "Checkouts");

            migrationBuilder.AlterColumn<string>(
                name: "ExpirationDate",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CheckOutId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CardholderName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CardLastFour",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "Checkouts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CheckOutId",
                table: "Payments",
                column: "CheckOutId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_AddressId",
                table: "Checkouts",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Addresses_AddressId",
                table: "Checkouts",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Checkouts_CheckOutId",
                table: "Payments",
                column: "CheckOutId",
                principalTable: "Checkouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Addresses_AddressId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Checkouts_CheckOutId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CheckOutId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Checkouts_AddressId",
                table: "Checkouts");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Checkouts");

            migrationBuilder.AlterColumn<string>(
                name: "ExpirationDate",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CheckOutId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CardholderName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CardLastFour",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CartId",
                table: "Checkouts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdressId",
                table: "Checkouts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CheckOutId",
                table: "Payments",
                column: "CheckOutId",
                unique: true,
                filter: "[CheckOutId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_AdressId",
                table: "Checkouts",
                column: "AdressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Addresses_AdressId",
                table: "Checkouts",
                column: "AdressId",
                principalTable: "Addresses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkouts_Carts_CartId",
                table: "Checkouts",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Checkouts_CheckOutId",
                table: "Payments",
                column: "CheckOutId",
                principalTable: "Checkouts",
                principalColumn: "Id");
        }
    }
}
