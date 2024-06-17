using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsers1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trip",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "MemberIds",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MemberIds",
                table: "Trip");

            migrationBuilder.RenameTable(
                name: "Trip",
                newName: "Trips");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trips",
                table: "Trips",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trips",
                table: "Trips");

            migrationBuilder.RenameTable(
                name: "Trips",
                newName: "Trip");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "MemberIds",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberIds",
                table: "Trip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trip",
                table: "Trip",
                column: "Id");
        }
    }
}
