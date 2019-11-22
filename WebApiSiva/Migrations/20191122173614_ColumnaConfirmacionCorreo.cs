using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiSiva.Migrations
{
    public partial class ColumnaConfirmacionCorreo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Validado",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Validado",
                table: "Users");
        }
    }
}
