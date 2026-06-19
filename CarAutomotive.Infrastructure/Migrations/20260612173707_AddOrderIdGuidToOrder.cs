using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarAutomotive.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIdGuidToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Orders");
        }
    }
}
