using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class DownloadUserModel
    {
        public Guid Role { get; set; }
        public int[] SGID { get; set; }
        public bool isAmUser { get; set; }
    }
}
