using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Configurations
{
    public class NetworkCredentials
    {
        public string ForwardAttachmentsPath { get; set; }
        public string ReverseAttachmentsPath { get; set; }
        public string SharedFolderUserName { get; set; }
        public string SharedFolderPassword { get; set; }
        public string SharedFolderDomain { get; set; }
    }
}
