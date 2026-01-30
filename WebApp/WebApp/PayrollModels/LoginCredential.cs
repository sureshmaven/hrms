
using System;

namespace PayrollModels
{
    public class LoginCredential
    {
        public int EmpCode { get; set; }
        public string EmpShortName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public string AppEnvironment { get; set; }
        public DateTime FinancialMonthDate { get; set; }
        public int FY { get; set; }
        public int FM { get; set; }

        public static LoginCredential GetCurrentUserForPR()
        {
            throw new NotImplementedException();
        }

        //public string EmpPkId { get; set; }
        // public string EmpFullName { get; set; }

        //public string LoginMode { get; set; }

        //public string EmpImage { get; set; }

        //public string Branch { get; set; }
        //public string Designation { get; set; }
        //public string BranchName { get; set; }

        //public string Department { get; set; }

        //public string Role { get; set; }

        //public string RolePages { get; set; }

        //public string Approvals { get; set; }

    }
}