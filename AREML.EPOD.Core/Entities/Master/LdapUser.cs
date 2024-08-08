using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class LdapUser
    {
        public Guid UserID { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Path { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
