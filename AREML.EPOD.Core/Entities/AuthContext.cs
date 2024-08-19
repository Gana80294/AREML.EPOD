using AREML.EPOD.Core.Entities.Client;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Core.Entities.ReverseLogistics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions options) : base(options)
        {
            Database.SetCommandTimeout(Int32.MaxValue);
        }

        public DbSet<ClientDetails> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleMap> UserRoleMaps { get; set; }
        public DbSet<App> Apps { get; set; }
        public DbSet<RoleAppMap> RoleAppMaps { get; set; }

        public DbSet<UserSalesGroupMap> UserSalesGroupMaps { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantGroup> PlantGroups { get; set; }
        public DbSet<PlantGroupPlantMap> PlantGroupPlantMaps { get; set; }
        public DbSet<PlantOrganizationMap> PlantOrganizationMaps { get; set; }
        public DbSet<UserOrganizationMap> UserOrganizationMaps { get; set; }
        public DbSet<UserPlantMap> UserPlantMaps { get; set; }
        public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
        public DbSet<TokenHistory> TokenHistories { get; set; }
        public DbSet<P_INV_HEADER_DETAIL> P_INV_HEADER_DETAIL { get; set; }
        public DbSet<P_INV_ITEM_DETAIL> P_INV_ITEM_DETAIL { get; set; }
        public DbSet<P_INV_ATTACHMENT> P_INV_ATTACHMENT { get; set; }

        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        public DbSet<SLSGroupWithCustomerGroupMap> SLSGroupWithCustomerGroupMaps { get; set; }
        public DbSet<SMSOTPChangePasswordHistory> SMSOTPChnagePasswordHistories { get; set; }

        public DbSet<UserCreationErrorLog> UserCreationErrorLogs { get; set; }
        public DbSet<UserActionHistory> UserActionHistories { get; set; }
        public DbSet<CFAUserMapping> CFAUserMappings { get; set; }

        public DbSet<SalesReturnCreditNoteLog> SalesReturnCreditNoteLogs { get; set; }
        public DbSet<ScrollNotification> ScrollNotifications { get; set; }
        public DbSet<DocumentHistory> DocumentHistories { get; set; }
        public DbSet<UserManualDoc> UserManualDocs { get; set; }
        public DbSet<UserManualDocStore> UserManualDocStores { get; set; }
        public DbSet<RPOD_HEADER> RPOD_HEADER_DETAILS { get; set; }
        public DbSet<RPOD_MATERIAL> RPOD_MATERIAL_DETAILS { get; set; }
        public DbSet<RPOD_LR_DETAIL> RPOD_LR_DETAILS { get; set; }
        public DbSet<RPOD_LR_ATTACHMENT> RPOD_LR_ATTACHMENTS { get; set; }
        public DbSet<WARRANTY_REPLACEMENT> WARRANTY_REPLACEMENT_DETAILS { get; set; }
        public DbSet<ReversePodApprover> ReversePodApprovers { get; set; }
        public DbSet<MobileVersion> MobileVersions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<P_INV_HEADER_DETAIL>()
             .HasIndex(p => new { p.ORGANIZATION, p.DIVISION, p.PLANT, p.INV_NO, p.ODIN, p.INV_DATE, p.CUSTOMER, p.CUSTOMER_NAME, p.LR_NO, p.STATUS });
            modelBuilder.Entity<P_INV_ITEM_DETAIL>()
            .HasIndex(p => new { p.HEADER_ID });
        }

    }
}
