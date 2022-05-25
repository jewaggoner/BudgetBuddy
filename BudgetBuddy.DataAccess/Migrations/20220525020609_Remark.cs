using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetBuddy.DataAccess.Migrations
{
    public partial class Remark : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItem_AspNetUsers_UserId",
                table: "LineItem");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LineItem",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "LineItem",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItem_AspNetUsers_UserId",
                table: "LineItem",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItem_AspNetUsers_UserId",
                table: "LineItem");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "LineItem");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "LineItem",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LineItem_AspNetUsers_UserId",
                table: "LineItem",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
