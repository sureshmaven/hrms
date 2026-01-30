using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Masters
{
    public class PayslipPdf
    {
        public string Month { get; set; }

        public string EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string grossamount { get; set; }
        public string deductionsamount { get; set; }
        public string netamount { get; set; }
        public string workingdays { get; set; }        
        public IList<PayslipPdfAlwDed> Allowences { get; set; }
        public IList<PayslipPdfAlwDed> Deductions { get; set; }
        public IList<PayslipPdfAlwDed> grossworking { get; set; }
        public IList<PayslipPdfAlwDed> netwrkingList { get; set; }
       
        public string TelanganaIn { get; set; }
        public string Providentfund { get; set; }
        public string Incometax { get; set; }
        public string ClubSubscription { get; set; }
        public string TelangaOfficers { get; set; }
        public string GrossAmount { get; set; }
        public string DeductionAmount { get; set; }
        public string WorkingDays { get; set; }
        public string NetAmount { get; set; }
        public string DOJ { get; set; }
        public string RetirementDate { get; set; }
        public string PfNo { get; set; }

        public string Fm{ get; set; }

        public string PersonalEmailId { get; set; }
    }

    public class PayslipPdfAlwDed
    {
        public string Perticular { get; set; }
        public string Amount { get; set; }
    }
}
