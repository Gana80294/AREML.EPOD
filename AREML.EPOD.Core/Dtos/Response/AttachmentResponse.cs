using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.Response
{
    public class AttachmentResponse
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string Extension { get; set; }
    }
}
