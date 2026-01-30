using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMSBusiness.Models
{
    public class YrLeaveBal
    {
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string BrDept { get; set; }
        public string Year { get; set; }
        //cl
        public string CarryForwardFLeaves { get; set; }
        public string CreditLeaves { get; set; }
        public string Debits { get; set; }
        public string LeaveBalance { get; set; }
        //ml
        
        public string TotalMedicalSickLeave { get; set; }
        public string ConsumedML { get; set; }
        public string RemainingML { get; set; }
        public string CarryForwardML { get; set; }
        
        //public string CarryForwardML { get; set; }
        //public string ConsumedML { get; set; }
        //public string RemainingML { get; set; }
        //public string TotalMedicalSickLeave { get; set; }
        //pl
        public string TotalPrivilegeLeave { get; set; }
        public string ConsumedPL { get; set; }
        public string RemainingPL { get; set; }
        public string CarryForwardPL { get; set; }

        //mtl
        public string TotalMaternityLeave { get; set; }
        public string ConsumedMTL { get; set; }
        public string RemainingMTL { get; set; }
        public string CarryForwardMTL { get; set; }

        //ptl
        public string TotalPaternityLeave { get; set; }
        public string ConsumedPTL { get; set; }
        public string RemainingPTL { get; set; }
        public string CarryForwardPTL { get; set; }

        //eol
        public string TotalExtraordinaryLeave { get; set; }
        public string ConsumedEOL { get; set; }
        public string RemainingEOL { get; set; }
        public string CarryForwardEOL { get; set; }
        
        //scl
        public string TotalSpecialCasualLeave { get; set; }
        public string ConsumedSCL { get; set; }
        public string RemainingSCL { get; set; }
        public string CarryForwardSCL { get; set; }
        //c-off
        //scl
        public string TotalCOFFLeave { get; set; }
        public string ConsumedCOFF { get; set; }
        public string RemainingCOFF { get; set; }
        public string CarryForwardCOFF { get; set; }
        //LOP
        //scl
        public string TotalLOPLeave { get; set; }
        public string ConsumedLOP{ get; set; }
        public string RemainingLOP{ get; set; }
        public string CarryForwardLOP { get; set; }
    }
}
