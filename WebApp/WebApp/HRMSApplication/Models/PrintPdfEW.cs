using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public partial class PrintPdfEW
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public int EmpId { get; set; }
        public string bankact { get; set; }
        public int EmpCode { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmpName { get; set; }
        public DateTime Date { get; set; }
        public string monthyear { get; set; }
        public string WorkingDays { get; set; }

        public string WorkDays { get; set; }
        public string panno { get; set; }
        public string EarnedBasic { get; set; }

        public string EarnedHRA { get; set; }

        public string EarnedConveyance { get; set; }

        public string EarnedMedical { get; set; }

        public string EarnedSA { get; set; }

        public string EarnedGross { get; set; }

        public string LOPDays { get; set; }

        public string LOPamount { get; set; }

        public string PF { get; set; }
        public string photo { get; set;}
        public string ESI { get; set; }

        public string PT { get; set; }

        public string TDS { get; set; }

        public string Loan { get; set; }

        public string Others { get; set; }

        public string Totaldeductions { get; set; }

        public string NetSalary { get; set; }

        public bool Active { get; set; }
    }
}