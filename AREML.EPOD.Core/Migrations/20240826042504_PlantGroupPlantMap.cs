using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AREML.EPOD.Core.Migrations
{
    /// <inheritdoc />
    public partial class PlantGroupPlantMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
            //    table: "WARRANTY_REPLACEMENT_DETAILS");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_WARRANTY_REPLACEMENT_DETAILS",
            //    table: "WARRANTY_REPLACEMENT_DETAILS");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_PlantGroupPlantMaps",
            //    table: "PlantGroupPlantMaps");

            //migrationBuilder.RenameTable(
            //    name: "WARRANTY_REPLACEMENT_DETAILS",
            //    newName: "WARRANTY_REPLACEMENT");

            //migrationBuilder.RenameTable(
            //    name: "PlantGroupPlantMaps",
            //    newName: "PlantGroupPlantMap");

            //migrationBuilder.RenameIndex(
            //    name: "IX_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_ID",
            //    table: "WARRANTY_REPLACEMENT",
            //    newName: "IX_WARRANTY_REPLACEMENT_RPOD_HEADER_ID");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_WARRANTY_REPLACEMENT",
            //    table: "WARRANTY_REPLACEMENT",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_PlantGroupPlantMap",
            //    table: "PlantGroupPlantMap",
            //    columns: new[] { "PlantGroupId", "PlantCode" });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_WARRANTY_REPLACEMENT_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
            //    table: "WARRANTY_REPLACEMENT",
            //    column: "RPOD_HEADER_ID",
            //    principalTable: "RPOD_HEADER_DETAILS",
            //    principalColumn: "RPOD_HEADER_ID",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WARRANTY_REPLACEMENT_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
                table: "WARRANTY_REPLACEMENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WARRANTY_REPLACEMENT",
                table: "WARRANTY_REPLACEMENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlantGroupPlantMap",
                table: "PlantGroupPlantMap");

            migrationBuilder.RenameTable(
                name: "WARRANTY_REPLACEMENT",
                newName: "WARRANTY_REPLACEMENT_DETAILS");

            migrationBuilder.RenameTable(
                name: "PlantGroupPlantMap",
                newName: "PlantGroupPlantMaps");

            migrationBuilder.RenameIndex(
                name: "IX_WARRANTY_REPLACEMENT_RPOD_HEADER_ID",
                table: "WARRANTY_REPLACEMENT_DETAILS",
                newName: "IX_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WARRANTY_REPLACEMENT_DETAILS",
                table: "WARRANTY_REPLACEMENT_DETAILS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlantGroupPlantMaps",
                table: "PlantGroupPlantMaps",
                columns: new[] { "PlantGroupId", "PlantCode" });

            migrationBuilder.AddForeignKey(
                name: "FK_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
                table: "WARRANTY_REPLACEMENT_DETAILS",
                column: "RPOD_HEADER_ID",
                principalTable: "RPOD_HEADER_DETAILS",
                principalColumn: "RPOD_HEADER_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
