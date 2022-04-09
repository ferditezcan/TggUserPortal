using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tggUserPortal.Migrations
{
    public partial class vr102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PwHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PwSalt",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PwHash",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PwSalt",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
