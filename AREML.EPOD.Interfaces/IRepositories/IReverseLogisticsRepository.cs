using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.ReverseLogistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IReverseLogisticsRepository
    {
        Task<List<ReversePodDetails>> GetAllReversePODByCustomer(string customerCode);
        Task<List<ReversePodMaterialDetails>> GetReverseMaterialDetails(int headerId);
        Task<List<RPOD_LR_DETAIL>> GetLRDetailsByHeaderId(int headerId);
        Task<List<ReversePodDetails>> FilterReversePODDetails(ReversePodFilterClass filterClass);
        Task<AttachmentResponse> DownloadRPODDocuments(int attachmentId);
        Task<byte[]> DownloadRPODReport(ReversePodFilterClass filterClass);
        Task<bool> UpdateReversePodApprover(List<Guid> approvers);
        Task<ReversePodApprover> GetIsApprover(Guid UserId);

        Task<List<Guid>> GetDcApprovers();
        Task<bool> ConfirmReversePodDirectly();
        Task<bool> ConfirmReversePod();

    }
}
