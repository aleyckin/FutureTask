using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DeletedAdministratorAtAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Administrators_AdministratorId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AdministratorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "UserRole",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorId",
                table: "Projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Password = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AdministratorId",
                table: "Projects",
                column: "AdministratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Administrators_AdministratorId",
                table: "Projects",
                column: "AdministratorId",
                principalTable: "Administrators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
