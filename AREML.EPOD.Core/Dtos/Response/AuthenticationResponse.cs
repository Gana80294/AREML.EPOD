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
        public int Role_Id { get; set; }
        public string Password { get; set; }
        public string LastPassword { get; set; }
        public string SecondLastPassword { get; set; }
        public string ThirdLastPassword { get; set; }
        public string FourthLastPassword { get; set; }
        public string ContactNumber { get; set; }
        public string Token { get; set; }
        public List<string> MenuItemLists { get; set; }
        public List<string> Plants {  get; set; }

    }
}
