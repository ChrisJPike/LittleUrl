using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleUrl.Infrastructure.EntityFramework.Migrations
{
    public partial class MaxLengthsAndCodeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "LitlUrl",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "LitlUrl",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LitlUrl_Code",
                table: "LitlUrl",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LitlUrl_Code",
                table: "LitlUrl");

            migrationBuilder.AlterColumn<string>(
                name: "LongUrl",
                table: "LitlUrl",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2083)",
                oldMaxLength: 2083);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "LitlUrl",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);
        }
    }
}
