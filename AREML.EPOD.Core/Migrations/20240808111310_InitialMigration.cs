using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AREML.EPOD.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Admins",
            //    columns: table => new
            //    {
            //        Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Admins", x => x.Username);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Apps",
            //    columns: table => new
            //    {
            //        AppID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Apps", x => x.AppID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CFAUserMappings",
            //    columns: table => new
            //    {
            //        ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CFAUserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CFAUserMappings", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Clients",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Secret = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        ApplicationType = table.Column<int>(type: "int", nullable: false),
            //        Active = table.Column<bool>(type: "bit", nullable: false),
            //        RefreshTokenLifeTime = table.Column<int>(type: "int", nullable: false),
            //        AllowedOrigin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Clients", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CustomerGroups",
            //    columns: table => new
            //    {
            //        CGID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CustomerGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CustomerGroups", x => x.CGID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DocumentHistories",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        HeaderId = table.Column<int>(type: "int", nullable: false),
            //        FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DocumentHistories", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "MobileVersions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        VersionCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_MobileVersions", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Organizations",
            //    columns: table => new
            //    {
            //        OrganizationCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Organizations", x => x.OrganizationCode);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "P_INV_ATTACHMENT",
            //    columns: table => new
            //    {
            //        ATTACHMENT_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        ATTACHMENT_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DOCUMENT_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ATTACHMENT_FILE = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
            //        CREATED_BY = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CREATED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
            //        FILE_PATH = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FILE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_P_INV_ATTACHMENT", x => x.ATTACHMENT_ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "P_INV_HEADER_DETAILS",
            //    columns: table => new
            //    {
            //        HEADER_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ORGANIZATION = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        DIVISION = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        PLANT = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
            //        PLANT_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        INV_NO = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        ODIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        INV_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        INV_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CUSTOMER = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CUSTOMER_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        CUSTOMER_GROUP = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VEHICLE_NO = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        VEHICLE_CAPACITY = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        POD_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        EWAYBILL_NO = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        EWAYBILL_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        LR_NO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
            //        LR_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        FWD_AGENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CARRIER = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FREIGHT_ORDER = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FREIGHT_ORDER_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        OUTBOUND_DELIVERY = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OUTBOUND_DELIVERY_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ACTUAL_DISPATCH_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PROPOSED_DELIVERY_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        VEHICLE_REPORTED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ACTUAL_DELIVERY_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        TRANSIT_LEAD_TIME = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DISTANCE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CANC_INV_STATUS = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        STATUS = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
            //        STATUS_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ISXMLCREATED = table.Column<bool>(type: "bit", nullable: false),
            //        XMLMOVED_ON = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CREATED_BY = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CREATED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
            //        CUSTOMER_GROUP_DESC = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SECTOR_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CUSTOMER_DESTINATION = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PLANT_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        GROSS_WEIGHT = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DRIVER_CONTACT = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TRACKING_LINK = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TOTAL_TRAVEL_TIME = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        TOTAL_DISTANCE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DELIVERY_DATE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
            //        DELIVERY_TIME = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_P_INV_HEADER_DETAILS", x => x.HEADER_ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "P_INV_ITEM_DETAIL",
            //    columns: table => new
            //    {
            //        ITEM_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ITEM_NO = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        MATERIAL_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MATERIAL_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        RECEIVED_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        QUANTITY_UOM = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        STATUS = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        STATUS_DESCRIPTION = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        REASON = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        REMARKS = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CREATED_BY = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CREATED_ON = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
            //        ITEM_WEIGHT = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_P_INV_ITEM_DETAIL", x => x.ITEM_ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PlantGroupPlantMaps",
            //    columns: table => new
            //    {
            //        PlantGroupId = table.Column<int>(type: "int", nullable: false),
            //        PlantCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PlantGroupPlantMaps", x => new { x.PlantGroupId, x.PlantCode });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PlantGroups",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PlantGroups", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PlantOrganizationMaps",
            //    columns: table => new
            //    {
            //        OrganizationCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        PlantCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PlantOrganizationMaps", x => new { x.OrganizationCode, x.PlantCode });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Plants",
            //    columns: table => new
            //    {
            //        PlantCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Plants", x => x.PlantCode);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Reasons",
            //    columns: table => new
            //    {
            //        ReasonID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Reasons", x => x.ReasonID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ReversePodApprovers",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        IsApprover = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ReversePodApprovers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RoleAppMaps",
            //    columns: table => new
            //    {
            //        RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        AppID = table.Column<int>(type: "int", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RoleAppMaps", x => new { x.RoleID, x.AppID });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Roles",
            //    columns: table => new
            //    {
            //        RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Roles", x => x.RoleID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RPOD_HEADER_DETAILS",
            //    columns: table => new
            //    {
            //        RPOD_HEADER_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        DC_NUMBER = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //        DC_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CLAIM_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CUSTOMER = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CUSTOMER_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PLANT = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PLANT_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        STATUS = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
            //        SLA_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PENDING_DAYS = table.Column<int>(type: "int", nullable: false),
            //        TOTAL_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        BILLED_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        BALANCE_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RPOD_HEADER_DETAILS", x => x.RPOD_HEADER_ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RPOD_LR_ATTACHMENTS",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RPOD_HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        Code = table.Column<int>(type: "int", nullable: false),
            //        FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RPOD_LR_ATTACHMENTS", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RPOD_LR_DETAILS",
            //    columns: table => new
            //    {
            //        LR_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RPOD_HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        LR_NO = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LR_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DC_RECEIEVED_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DC_ACKNOWLEDGEMENT_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CUSTOMER_DOC_ID = table.Column<int>(type: "int", nullable: false),
            //        DC_DOC_ID = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RPOD_LR_DETAILS", x => x.LR_ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SalesReturnCreditNoteLogs",
            //    columns: table => new
            //    {
            //        CRID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreditInvoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        MaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Qty = table.Column<double>(type: "float", nullable: false),
            //        ItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SalesReturnCreditNoteLogs", x => x.CRID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ScrollNotifications",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        Code = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ScrollNotifications", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SLSGroupWithCustomerGroupMaps",
            //    columns: table => new
            //    {
            //        SGID = table.Column<int>(type: "int", nullable: false),
            //        CGID = table.Column<int>(type: "int", nullable: false),
            //        SLSGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CustomerGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SLSGroupWithCustomerGroupMaps", x => new { x.SGID, x.CGID });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SMSOTPChnagePasswordHistories",
            //    columns: table => new
            //    {
            //        OTPID = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        OTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OTPCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        OTPExpiredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        OTPUsedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsOTPUSed = table.Column<bool>(type: "bit", nullable: false),
            //        MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsPasswordChanged = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SMSOTPChnagePasswordHistories", x => x.OTPID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TokenHistories",
            //    columns: table => new
            //    {
            //        TokenHistoryID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        OTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        ExpireOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UsedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        IsUsed = table.Column<bool>(type: "bit", nullable: false),
            //        Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TokenHistories", x => x.TokenHistoryID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserActionHistories",
            //    columns: table => new
            //    {
            //        LogID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TransID = table.Column<int>(type: "int", nullable: false),
            //        Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ChangesDetected = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserActionHistories", x => x.LogID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserCreationErrorLogs",
            //    columns: table => new
            //    {
            //        LogID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LogReson = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserCreationErrorLogs", x => x.LogID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserLoginHistories",
            //    columns: table => new
            //    {
            //        ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserLoginHistories", x => x.ID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserManualDocs",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserManualDocs", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserOrganizationMaps",
            //    columns: table => new
            //    {
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        OrganizationCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserOrganizationMaps", x => new { x.UserID, x.OrganizationCode });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserPlantMaps",
            //    columns: table => new
            //    {
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        PlantCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserPlantMaps", x => new { x.UserID, x.PlantCode });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserRoleMaps",
            //    columns: table => new
            //    {
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        RoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserRoleMaps", x => new { x.UserID, x.RoleID });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SecondLastPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ThirdLastPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FourthLastPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        WrongAttempt = table.Column<int>(type: "int", nullable: false),
            //        IsLocked = table.Column<bool>(type: "bit", nullable: false),
            //        ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        IsCFAMapped = table.Column<bool>(type: "bit", nullable: false),
            //        LastPasswordChangeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CustomerGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.UserID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserSalesGroupMaps",
            //    columns: table => new
            //    {
            //        UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //        SGID = table.Column<int>(type: "int", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserSalesGroupMaps", x => new { x.UserID, x.SGID });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RPOD_MATERIAL_DETAILS",
            //    columns: table => new
            //    {
            //        MATERIAL_ID = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RPOD_HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        MATERIAL_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        HAND_OVERED_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        RECEIVED_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        STATUS = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
            //        REMARKS = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RPOD_MATERIAL_DETAILS", x => x.MATERIAL_ID);
            //        table.ForeignKey(
            //            name: "FK_RPOD_MATERIAL_DETAILS_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
            //            column: x => x.RPOD_HEADER_ID,
            //            principalTable: "RPOD_HEADER_DETAILS",
            //            principalColumn: "RPOD_HEADER_ID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WARRANTY_REPLACEMENT_DETAILS",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RPOD_HEADER_ID = table.Column<int>(type: "int", nullable: false),
            //        MATERIAL_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        TOTAL_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        BILLED_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        BALANCE_QUANTITY = table.Column<double>(type: "float", nullable: true),
            //        INV_NO = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        SUPPLIED_QUANTITY = table.Column<double>(type: "float", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WARRANTY_REPLACEMENT_DETAILS", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_DETAILS_RPOD_HEADER_ID",
            //            column: x => x.RPOD_HEADER_ID,
            //            principalTable: "RPOD_HEADER_DETAILS",
            //            principalColumn: "RPOD_HEADER_ID",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserManualDocStores",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserManualDocId = table.Column<int>(type: "int", nullable: false),
            //        FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        FileContent = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserManualDocStores", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserManualDocStores_UserManualDocs_UserManualDocId",
            //            column: x => x.UserManualDocId,
            //            principalTable: "UserManualDocs",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_P_INV_HEADER_DETAILS_ORGANIZATION_DIVISION_PLANT_INV_NO_ODIN_INV_DATE_CUSTOMER_CUSTOMER_NAME_LR_NO_STATUS",
            //    table: "P_INV_HEADER_DETAILS",
            //    columns: new[] { "ORGANIZATION", "DIVISION", "PLANT", "INV_NO", "ODIN", "INV_DATE", "CUSTOMER", "CUSTOMER_NAME", "LR_NO", "STATUS" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_P_INV_ITEM_DETAIL_HEADER_ID",
            //    table: "P_INV_ITEM_DETAIL",
            //    column: "HEADER_ID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RPOD_MATERIAL_DETAILS_RPOD_HEADER_ID",
            //    table: "RPOD_MATERIAL_DETAILS",
            //    column: "RPOD_HEADER_ID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserManualDocStores_UserManualDocId",
            //    table: "UserManualDocStores",
            //    column: "UserManualDocId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_WARRANTY_REPLACEMENT_DETAILS_RPOD_HEADER_ID",
            //    table: "WARRANTY_REPLACEMENT_DETAILS",
            //    column: "RPOD_HEADER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Apps");

            migrationBuilder.DropTable(
                name: "CFAUserMappings");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "CustomerGroups");

            migrationBuilder.DropTable(
                name: "DocumentHistories");

            migrationBuilder.DropTable(
                name: "MobileVersions");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropTable(
                name: "P_INV_ATTACHMENT");

            migrationBuilder.DropTable(
                name: "P_INV_HEADER_DETAILS");

            migrationBuilder.DropTable(
                name: "P_INV_ITEM_DETAIL");

            migrationBuilder.DropTable(
                name: "PlantGroupPlantMaps");

            migrationBuilder.DropTable(
                name: "PlantGroups");

            migrationBuilder.DropTable(
                name: "PlantOrganizationMaps");

            migrationBuilder.DropTable(
                name: "Plants");

            migrationBuilder.DropTable(
                name: "Reasons");

            migrationBuilder.DropTable(
                name: "ReversePodApprovers");

            migrationBuilder.DropTable(
                name: "RoleAppMaps");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "RPOD_LR_ATTACHMENTS");

            migrationBuilder.DropTable(
                name: "RPOD_LR_DETAILS");

            migrationBuilder.DropTable(
                name: "RPOD_MATERIAL_DETAILS");

            migrationBuilder.DropTable(
                name: "SalesReturnCreditNoteLogs");

            migrationBuilder.DropTable(
                name: "ScrollNotifications");

            migrationBuilder.DropTable(
                name: "SLSGroupWithCustomerGroupMaps");

            migrationBuilder.DropTable(
                name: "SMSOTPChnagePasswordHistories");

            migrationBuilder.DropTable(
                name: "TokenHistories");

            migrationBuilder.DropTable(
                name: "UserActionHistories");

            migrationBuilder.DropTable(
                name: "UserCreationErrorLogs");

            migrationBuilder.DropTable(
                name: "UserLoginHistories");

            migrationBuilder.DropTable(
                name: "UserManualDocStores");

            migrationBuilder.DropTable(
                name: "UserOrganizationMaps");

            migrationBuilder.DropTable(
                name: "UserPlantMaps");

            migrationBuilder.DropTable(
                name: "UserRoleMaps");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserSalesGroupMaps");

            migrationBuilder.DropTable(
                name: "WARRANTY_REPLACEMENT_DETAILS");

            migrationBuilder.DropTable(
                name: "UserManualDocs");

            migrationBuilder.DropTable(
                name: "RPOD_HEADER_DETAILS");
        }
    }
}
