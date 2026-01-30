using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMSApplication.Models
{
    public class LeavesResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string loginMode { get; set; }

        public int TotalLeaves { get; set; }

        public int LeaveDays { get; set; }

        public int TotalDays { get; set; }
    }
}