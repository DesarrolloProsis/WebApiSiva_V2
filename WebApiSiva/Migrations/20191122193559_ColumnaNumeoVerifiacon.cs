using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiSiva.Migrations
{
    public partial class ColumnaNumeoVerifiacon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroVerificacion",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroVerificacion",
                table: "Users");
        }
    }
}
