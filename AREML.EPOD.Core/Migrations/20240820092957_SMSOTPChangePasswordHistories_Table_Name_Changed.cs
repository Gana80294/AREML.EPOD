using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AREML.EPOD.Core.Migrations
{
    /// <inheritdoc />
    public partial class SMSOTPChangePasswordHistories_Table_Name_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_SMSOTPChnagePasswordHistories",
            //    table: "SMSOTPChnagePasswordHistories");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_P_INV_HEADER_DETAILS",
            //    table: "P_INV_HEADER_DETAILS");

            //migrationBuilder.RenameTable(
            //    name: "SMSOTPChnagePasswordHistories",
            //    newName: "SMSOTPChangePasswordHistories");

            //migrationBuilder.RenameTable(
            //    name: "P_INV_HEADER_DETAILS",
            //    newName: "P_INV_HEADER_DETAIL");

            //migrationBuilder.RenameIndex(
            //    name: "IX_P_INV_HEADER_DETAILS_ORGANIZATION_DIVISION_PLANT_INV_NO_ODIN_INV_DATE_CUSTOMER_CUSTOMER_NAME_LR_NO_STATUS",
            //    table: "P_INV_HEADER_DETAIL",
            //    newName: "IX_P_INV_HEADER_DETAIL_ORGANIZATION_DIVISION_PLANT_INV_NO_ODIN_INV_DATE_CUSTOMER_CUSTOMER_NAME_LR_NO_STATUS");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_SMSOTPChangePasswordHistories",
            //    table: "SMSOTPChangePasswordHistories",
            //    column: "OTPID");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_P_INV_HEADER_DETAIL",
            //    table: "P_INV_HEADER_DETAIL",
            //    column: "HEADER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SMSOTPChangePasswordHistories",
                table: "SMSOTPChangePasswordHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_P_INV_HEADER_DETAIL",
                table: "P_INV_HEADER_DETAIL");

            migrationBuilder.RenameTable(
                name: "SMSOTPChangePasswordHistories",
                newName: "SMSOTPChnagePasswordHistories");

            migrationBuilder.RenameTable(
                name: "P_INV_HEADER_DETAIL",
                newName: "P_INV_HEADER_DETAILS");

            migrationBuilder.RenameIndex(
                name: "IX_P_INV_HEADER_DETAIL_ORGANIZATION_DIVISION_PLANT_INV_NO_ODIN_INV_DATE_CUSTOMER_CUSTOMER_NAME_LR_NO_STATUS",
                table: "P_INV_HEADER_DETAILS",
                newName: "IX_P_INV_HEADER_DETAILS_ORGANIZATION_DIVISION_PLANT_INV_NO_ODIN_INV_DATE_CUSTOMER_CUSTOMER_NAME_LR_NO_STATUS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SMSOTPChnagePasswordHistories",
                table: "SMSOTPChnagePasswordHistories",
                column: "OTPID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_P_INV_HEADER_DETAILS",
                table: "P_INV_HEADER_DETAILS",
                column: "HEADER_ID");
        }
    }
}
