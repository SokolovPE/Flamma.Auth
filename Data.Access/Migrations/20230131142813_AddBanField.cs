using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flamma.Auth.Data.Access.Migrations
{
    public partial class AddBanField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BannedTill",
                table: "UserData",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannedTill",
                table: "UserData");
        }
    }
}
