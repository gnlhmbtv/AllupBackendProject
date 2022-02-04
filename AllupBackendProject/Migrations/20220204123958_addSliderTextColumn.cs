using Microsoft.EntityFrameworkCore.Migrations;

namespace AllupBackendProject.Migrations
{
    public partial class addSliderTextColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MainTitle",
                table: "Sliders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SliderText",
                table: "Sliders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "Sliders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainTitle",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "SliderText",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "Sliders");
        }
    }
}
