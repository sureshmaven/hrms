using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Areas.Payroll.Models
{
    public class Allowance
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Amount { get; set; }

        public List<Allowance> AllowanceList { get; set; }

    }
}