using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.Response
{
    public class AuthenticationResponse
    {
        public Guid UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public Guid Role_Id { get; set; }
        public string ContactNumber { get; set; }
        public string Token { get; set; }
        public List<string> MenuItemLists { get; set; }
        public List<string> Plants {  get; set; }

        public string IsChangePasswordRequired {  get; set; }

    }
}
