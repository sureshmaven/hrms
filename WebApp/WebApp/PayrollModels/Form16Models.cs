using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels
{
    public class Form16Model
    {
        public string Month { get; set; }

        public string EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string fy { get; set; }
        public string city { get; set; }
        public string PAN { get; set; }

        public string Quarter1 { get; set; }
        public string Quarter2 { get; set; }
        public string Quarter3 { get; set; }
        public string Quarter4 { get; set; }

        public string gross_Salaryaspercontainedsec17 { get; set; }
        public string Valueofperquisitiesu17 { get; set; }
        public string Profitslieuofsalary17 { get; set; }
        public string HouseRentAllowance { get; set; }
        public string TotalSection1017 { get; set; }
        public string Balance1_2 { get; set; }
        public string StandardDeduction { get; set; }
        public string Tax_on_Employment { get; set; }
        public string Aggregate { get; set; }
        public string Income_Charg_Under_Salaries { get; set; }
        public string Any_Other_Income { get; set; }
        public string Reported_by_Employee { get; set; }
        public string Income_from_House_Property_Interest_on_Housing_Loan { get;set; }

        public string Gross_Total_Income { get; set; }


        public string sect2_grss { get; set; }
        public string sect2_qual { get; set; }
        public string sect2_dedu { get; set; }

        public string sect3_grss { get; set; }
        public string sect3_qual { get; set; }
        public string sect3_dedu { get; set; }

        public string sect4_grss { get; set; }
        public string sect4_qual { get; set; }
        public string sect4_dedu { get; set; }
        public string Aggregate_amount_Under_ChapterVIA { get; set; }
        public string TotalIncome8_10 { get; set; }
        public string Tax_on_Total_Income { get; set; }
        public string Section87A { get; set; }
        public string EducationCESS { get; set; }
        public string Tax_payable { get; set; }
        public string LessReliefUnderSection89 { get; set; }
        public string Tax_payable1 { get; set; }
        public string Tax_deducted_Source { get; set; }
        public string Tax_paid_employer { get; set; }
        public string Tax_Payable_Refundable { get; set; }

        public string sanctName { get; set; }
        public string sanctFantherName { get; set; }
        public string sanctDesignation { get; set; }
        public string sanctDate { get; set; }
        public string sanctpalce { get; set; }
        public string sancPAN { get; set; }

        public string bsrcode { get; set; }
        public string paymentdate { get; set; }
        public string challanno { get; set; }
        public IList<monthwiseDeductions> deductions { get; set; }
        public IList<section80Cform16> sect1 { get; set; }
    }
    public class monthwiseDeductions
    {

        public string fm { get; set; }
        public string dedu_amount { get; set; }

    }

    public class section80Cform16
    { 
    
        public string type { get; set; }
    public string sect1_grss { get; set; }
    public string sect1_qual { get; set; }
    public string sect1_dedu { get; set; }
}

    public class Form16ModelAll
    {
        public string Perticular { get; set; }
        public string Amount { get; set; }
    }
}
