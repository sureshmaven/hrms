using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public class CancelConfirm
    {

        public int Id { get; set; }
        public Nullable<int> EmpId { get; set; }

        public string EmpName { get; set; }
        public string ControllingAuthority { get; set; }
        public string SanctioningAuthority { get; set; }
        public string LeaveType { get; set; }
       
        public DateTime StartDate { get; set; }
      
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
      
        public string Reason { get; set; }
       public string fullCancelReason { get; set; }
        public string patialCancelReason { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Status { get; set; }
        public string Stage { get; set; }
        public int LeaveDays { get; set; }
        public int CancelDays { get; set; }
        public int TotalDays { get; set; }
        public DateTime LeaveTimeStamp { get; set; }
        public DateTime Cancelstartdate { get; set; }

        public DateTime Cancelenddate { get; set; }
        public DateTime DateofDelivery { get; set; }
        public int? LeavesYear { get; set; }
        public string canceltype { get; set; }

    }
}