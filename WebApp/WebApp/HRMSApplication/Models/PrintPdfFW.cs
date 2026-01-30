using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public partial class PrintPdfFW
    {
        public int Id { get; set; }
        public int empid { get; set; }
        public string EmpName { get; set; }
        public int EmpCode { get; set; }
        public DateTime Date { get; set; }
        public string Branch { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmpEmailId { get; set; }
        public string FixedGross { get; set; }
        public string FixedBasic { get; set; }
        public string FixedHRA { get; set; }
        public string FixedConveyance { get; set; }
        public string FixedMedical { get; set; }
        public string FixedSA { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }

        public string PFEmployer { get; set; }
        public string ESIEmployer { get; set; }
        public string CTC { get; set; }

        public int Active { get; set; }



        public string EmpPANno { get; set; }
        public string EmpBankAccountno { get; set; }
        public string EmpUAN_PFno { get; set; }
        public string EmpESIno { get; set; }
    }
}