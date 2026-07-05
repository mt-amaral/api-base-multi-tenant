using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class V6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "IdentityRoleClaim");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IdentityRoleClaim",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }
    }
}
