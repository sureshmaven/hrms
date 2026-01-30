using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSBusiness.Business
{
    public class LoginResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string loginMode { get; set; }

        public string EmployeeImage { get; set; }

        public string FirstName { get; set; }
       public string  ShortName { get; set; }
        public int Designation { get; set; }
        public string desigcode { get; set; }
        public int EmpPkId { get; set; }
        public string EmpId { get; set; }
        public string EmployeeFullName { get; set; }
        public string CurrDesig { get; set; }
        public string LastName { get; set; }

        public DateTime RetirementDate { get; set; }

        public int Branch { get; set; }

        public int Department { get; set; }

        public string BranchName { get; set; }
        public string ControllingAuthority { get; set; }
        public string SanctioningAuthority { get; set; }
        public int Role { get; set; }

        public int Approvals { get; set; }

        public string RolePages { get; set; }

    }
}
