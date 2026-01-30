using HRMSBusiness.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HRMSApplication.Models
{
    public class MobileDashboardDTO
    {
        //emp details
        public int EmpId { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string BrDept{ get; set; }
        public string EmployeeImage { get; set; }
        public int LeaveId { get; set; }
        public string ControllingAuthority { get; set; }
        public string SanctioningAuthority { get; set; }

        //balance
        //public object LeaveCodeBal { get; set; }   //Caual Leave#5,Personal Leave#2
        public IList<EmpLeavebalanceDTO> LeavesBalance { get; set; }
        //history
        public IList<EmpLeaveHistoryDTO> LeavesHistory { get; set; }
    }

    public class EmpLeaveHistoryDTO
    {
        public string AppliedDate { get; set; }
        public string ReqDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveType { get; set; }
        public string LeaveDays { get; set; }
        public string Status { get; set; }
    }
    public class EmpLeavebalanceDTO
    {
        public string LeaveType { get; set; }
        public string LeaveBalance { get; set; }

    }
}