using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JadehRo.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "TripReq",
                newName: "SourcePath");

            migrationBuilder.AddColumn<double>(
                name: "DestinationLatitude",
                table: "TripReq",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DestinationLongitude",
                table: "TripReq",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "DestinationPath",
                table: "TripReq",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SourceLatitude",
                table: "TripReq",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SourceLongitude",
                table: "TripReq",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLatitude",
                table: "TripReq");

            migrationBuilder.DropColumn(
                name: "DestinationLongitude",
                table: "TripReq");

            migrationBuilder.DropColumn(
                name: "DestinationPath",
                table: "TripReq");

            migrationBuilder.DropColumn(
                name: "SourceLatitude",
                table: "TripReq");

            migrationBuilder.DropColumn(
                name: "SourceLongitude",
                table: "TripReq");

            migrationBuilder.RenameColumn(
                name: "SourcePath",
                table: "TripReq",
                newName: "Address");
        }
    }
}
