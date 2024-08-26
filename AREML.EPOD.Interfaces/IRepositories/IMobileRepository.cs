using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IMobileRepository
    {
        Task<InvoiceStatusCount> FilterInvoicesStatusCount(FilterClass filterClass);

        Task<DeliveryCount> FilterDeliverysCount(FilterClass filterClass);

        Task<List<int>> FilterLeadTimeCount(FilterClass filterClass);

        Task<InvoiceStatusCount> FilterInvoiceStatusCountByUser(FilterClass filterClass);

        Task<DeliveryCount> FilterDeliveryCountByUser(FilterClass filterClass);

        Task<List<int>> FilterLeadTimeCountByUser(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterInvoices(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterInvoicesByUser(FilterClass filterClass);

        Task<AttachmentResponse> DowloandPODDocument(int HeaderID, int AttachmentID);

        Task<MobileVersion> GetMobileAppVersion();
        Task<MobileVersion> UpdateMobileVersion(string version);
    }
}
