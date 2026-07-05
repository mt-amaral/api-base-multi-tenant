using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "IdentityRole",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IdentityRole",
                type: "character varying(180)",
                maxLength: 180,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IdentityRole",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "IdentityRole");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedName",
                table: "IdentityRole",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(180)",
                oldMaxLength: 180,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "IdentityRole",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(180)",
                oldMaxLength: 180,
                oldNullable: true);
        }
    }
}
