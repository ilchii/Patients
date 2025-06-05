using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patients.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Patronym",
                table: "Doctors",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Doctors",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patronym",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Doctors");
        }
    }
}
