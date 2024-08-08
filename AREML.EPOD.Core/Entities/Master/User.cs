using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string LastPassword { get; set; }
        public string SecondLastPassword { get; set; }
        public string ThirdLastPassword { get; set; }
        public string FourthLastPassword { get; set; }
        public int WrongAttempt { get; set; }
        public bool IsLocked { get; set; }
        public string ContactNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsCFAMapped { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public string CustomerGroupCode { get; set; }
    }
}
