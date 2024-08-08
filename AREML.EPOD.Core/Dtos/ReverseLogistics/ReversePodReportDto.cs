using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodReportDto
    {
        public string DC_NUMBER { get; set; }
        public DateTime? DC_DATE { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public DateTime? DC_RECEIEVED_DATE { get; set; }
        public DateTime? DC_ACKNOWLEDGEMENT_DATE { get; set; }
        public DateTime? SLA_DATE { get; set; }
        public int PENDING_DAYS { get; set; } = 0;
        public string STATUS { get; set; }
        public string MATERIAL_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public double? HAND_OVERED_QUANTITY { get; set; }
        public double? CUSTOMER_PENDING_QUANTITY { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        public double? DC_PENDING_QUANTITY { get; set; }
        public double? INVOICE_TOTAL_QTY { get; set; }
        public double? INVOICE_BILLED_QTY { get; set; }
        public double? INVOICE_BALANCE_QTY { get; set; }
        public string REMARKS { get; set; }
    }
}
