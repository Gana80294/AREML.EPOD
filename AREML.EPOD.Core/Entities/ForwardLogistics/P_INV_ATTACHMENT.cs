using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class P_INV_ATTACHMENT
    {
        [Key]
        public int ATTACHMENT_ID { get; set; }
        public int HEADER_ID { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_ON { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string FILE_PATH { get; set; }
        public string FILE_NAME { get; set; }
        public bool IS_PROCESSED { get; set; }
        public string MESSAGE { get; set; }
    }
}
