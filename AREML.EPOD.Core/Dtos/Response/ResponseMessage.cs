using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.Response
{
    public class ResponseMessage
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
