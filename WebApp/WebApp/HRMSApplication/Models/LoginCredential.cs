using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public class LoginCredential
    {
        public string EmpId { get; set; }

        public string LoginMode { get; set; }

        public string EmpImage { get; set; }

        public string EmpFullName { get; set; }
       public string EmpShortName { get; set; }
        public string EmpPkId { get; set; }
        public string CurrDesig { get; set; }
        public string Branch { get; set; }
        public string Designation { get; set; }
        public string BranchName { get; set; }

        public string Department { get; set; }

        public string Role { get; set; }

        public string RolePages { get; set; }

        public string Approvals { get; set; }

    }
}