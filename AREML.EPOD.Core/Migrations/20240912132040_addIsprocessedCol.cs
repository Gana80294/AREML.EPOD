using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AREML.EPOD.Core.Migrations
{
    /// <inheritdoc />
    public partial class addIsprocessedCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IS_PROCESSED",
                table: "P_INV_ATTACHMENT",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MESSAGE",
                table: "P_INV_ATTACHMENT",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "DocumentHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "DocumentHistories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_PROCESSED",
                table: "P_INV_ATTACHMENT");

            migrationBuilder.DropColumn(
                name: "MESSAGE",
                table: "P_INV_ATTACHMENT");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "DocumentHistories");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "DocumentHistories");
        }
    }
}
