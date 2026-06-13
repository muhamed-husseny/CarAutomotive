using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAutomotive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMechanicProfilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MechanicService_MechanicProfile_MechanicProfileId",
                table: "MechanicService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MechanicProfile",
                table: "MechanicProfile");

            migrationBuilder.RenameTable(
                name: "MechanicProfile",
                newName: "MechanicProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_MechanicProfile_Location",
                table: "MechanicProfiles",
                newName: "IX_MechanicProfiles_Location");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MechanicProfiles",
                table: "MechanicProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MechanicService_MechanicProfiles_MechanicProfileId",
                table: "MechanicService",
                column: "MechanicProfileId",
                principalTable: "MechanicProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MechanicService_MechanicProfiles_MechanicProfileId",
                table: "MechanicService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MechanicProfiles",
                table: "MechanicProfiles");

            migrationBuilder.RenameTable(
                name: "MechanicProfiles",
                newName: "MechanicProfile");

            migrationBuilder.RenameIndex(
                name: "IX_MechanicProfiles_Location",
                table: "MechanicProfile",
                newName: "IX_MechanicProfile_Location");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MechanicProfile",
                table: "MechanicProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MechanicService_MechanicProfile_MechanicProfileId",
                table: "MechanicService",
                column: "MechanicProfileId",
                principalTable: "MechanicProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
