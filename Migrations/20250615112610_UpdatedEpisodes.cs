using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patients.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEpisodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClinicalStatus",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ConditionSeverity",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DiagnosisICD10",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DiagnosisICPC2",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscoveryDate",
                table: "Episodes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DiseaseStage",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DiseaseType",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EpisodeType",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReliabilityStatus",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Symptoms",
                table: "Episodes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClinicalStatus",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "ConditionSeverity",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "DiagnosisICD10",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "DiagnosisICPC2",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "DiscoveryDate",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "DiseaseStage",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "DiseaseType",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "EpisodeType",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "ReliabilityStatus",
                table: "Episodes");

            migrationBuilder.DropColumn(
                name: "Symptoms",
                table: "Episodes");
        }
    }
}
