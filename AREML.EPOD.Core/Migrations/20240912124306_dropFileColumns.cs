using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AREML.EPOD.Core.Migrations
{
    /// <inheritdoc />
    public partial class dropFileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ATTACHMENT_FILE",
                table: "P_INV_ATTACHMENT");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "DocumentHistories");

            migrationBuilder.AddColumn<string>(
                name: "SHIP_TO_PARTY_CODE",
                table: "P_INV_HEADER_DETAIL",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SHIP_TO_PARTY_CODE",
                table: "P_INV_HEADER_DETAIL");

            migrationBuilder.AddColumn<byte[]>(
                name: "ATTACHMENT_FILE",
                table: "P_INV_ATTACHMENT",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "DocumentHistories",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
