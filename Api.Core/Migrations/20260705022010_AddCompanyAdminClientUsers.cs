using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyAdminClientUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserType",
                table: "IdentityUser",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Admin");

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUser_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdminUser_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUser",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUser_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientUser_IdentityUser_UserId",
                        column: x => x.UserId,
                        principalTable: "IdentityUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_CompanyId",
                table: "AdminUser",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_UserId",
                table: "AdminUser",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_CompanyId",
                table: "ClientUser",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_UserId",
                table: "ClientUser",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUser");

            migrationBuilder.DropTable(
                name: "ClientUser");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "IdentityUser");
        }
    }
}
