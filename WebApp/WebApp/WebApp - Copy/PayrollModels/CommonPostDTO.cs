using System.Collections.Generic;
using System;
namespace PayrollModels
{
    public class CommonPostDTO
    {
        public int EntityId { get; set; } // empid = 371
       // public char Action { get; set; }
        public string StringData { get; set; } //N^1=356^~U^3=456^400~U^4=^111
        public string StringData2 { get; set; } //N^1=356^~U^3=456^400~U^4=^111
        public string StringData3 { get; set; }
        public string PersonalEmailId { get; set; }//N^1=356^~U^3=456^400~U^4=^111
        public string Monthdata { get; set; }
        public string Retirementdata { get; set; }
        public string StringData4 { get; set; }
        public string StringData5 { get; set; }
        
        public List<loancols> Loancols { get; set; }
        public List<Emp> object1 { get; set; }
        public List<Emp> object2 { get; set; }
        public List<Emp> object3 { get; set; }
        public List<Emp> object4 { get; set; }
        public List<Emp> object5 { get; set; }
        public List<Emp> object6 { get; set; }
        public List<Emp> object7 { get; set; }
        public List<dates> objdates { get; set; }
        public List<string> Loans { get; set; }
        public List<inc> inc { get; set; }
        public List<multiGrid> multiObject { get; set; }
        public List<multiGrid> multiObject2 { get; set; }
        public List<LoansMaster> mstrloanobject { get; set; }
        public List<IncomTaxBankPaymentmodel1> IncomeTaxBank { get; set; }
        public List<LoansAdavncechild> objectLD { get; set; }
        public List<PE> objPE { get; set; }
        public List<prsnldeductiondata> objpd { get; set; }
        public List<prsnldeductiondataNew> objperdednew { get; set; }
        public List<allowancemasterdataNew> objallowancemasterdatanew { get; set; }
        public List<allowancemasterdata> objallowancemasterdata { get; set; }
        public List<PfNominee> Objpfnominee{ get; set; }
        public List<string> pdftypes { get; set; }
        public List<PfInterest> PfIntest { get; set; }
        public List<form16> form16data { get; set; }


    }
    //public class AddPaymonthModal
    //{
      
    //    //public string fy { get; set; }
    //    //public string fm { get; set; }
    //    public string week_holidays { get; set; }
    //    public string paid_holidays { get; set; }
    //    public string payment_date { get; set; }
    //    public string da_slabs { get; set; }
    //    public string da_points { get; set; }
    //    public string da_percent { get; set; }
      


    //}
    //public class month
    //{
    //    public List<AddPaymonthModal> monthdata { get; set; }

    //}
    public class allowancemasterdata
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Action { get; set; }
    }
    
        public class PfInterest
    {
        public string Id { get; set; } 
        public string Value { get; set; }
    }
    public class allowancemasterdataNew
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
    }
    public class IncomTaxBankPaymentmodel1
    {
        
        public string amount { get; set; }
        public string bank_name { get; set; }
        public string challan_no { get; set; }
        public string id { get; set; }
        public string payment_date { get; set; }
        public string Action { get; set; }

    }
    public class Emp
    {
        public string Id { get; set; } 
        public string Value { get; set; }
       
    }

    public class loancols
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
    }
    public class PE
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
    }
    public class inc 
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string Increment_Date_WEF { get; set; }
        public string IncDate { get; set; }
        public string change_incr_date { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }
    }
    public class dates
    {
        public string EntityId { get; set; }
        public string id { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string action { get; set; }
        public string amount { get; set; }
        public string name { get; set; }
    }
    public class multiGrid
    {
        public string Id { get; set; }
        public string AccNum { get; set; }
        public string Action { get; set; }
        public string Amount { get; set; }
        public string PayMonths { get; set; }
        public string PayType { get; set; }
        public string StartMonth { get; set; }
        public string StopMonth { get; set; }
    }


    public class prsnldeductiondata
        {
        public int id { get; set; }
        public string amount { get; set; }
        public string section { get; set; }
        public string action { get; set; }
        public int mid { get; set; }
        }

    public class prsnldeductiondataNew
    {
        public string id { get; set; }
        public string amount { get; set; }
        public string section { get; set; }
        public string action { get; set; }
        public string name { get; set; }
    }

    public class LoansMaster
    {
        public string id { get; set; }
        public string loan_id { get; set; }
        public string loan_description { get; set; }
        public string interest_rate { get; set; }
        public string active { get; set; }
        public string trans_id { get; set; }
        public string Action { get; set; }
    }

    public class PfNominee
    {
        public string id { get; set; }
        public string membername { get; set; }
        public string gender { get; set; }
        public string relation { get; set; }
        public string dob { get; set; }
        public string age { get; set; }
        public string date { get; set; }
        public string percentage { get; set; }
        public string Action { get; set; }
    }


    public class LoansAdavncechild
    {
        public int id { get; set; }
        public string slno { get; set; }
        public string date_disburse { get; set; }
        public string loan_amount { get; set; }
        public string interest_amount_recovered { get; set; }
        public string interest_rate { get; set; }
        public string interest_accured { get; set; }
        public string principal_amount_recovered { get; set; }
        public string priority { get; set; }

    }

    
    public class PFNonPayable
    {
        
        public int EntityId { get; set; }
        public string pf_no { get; set; }
        public string basic_da { get; set; }
        public string purposeType { get; set; }
        public List<string> certificates { get; set; }
        public string apply_amount { get; set; }
        public string own_share { get; set; }
        public string bank_share { get; set; }
        public string vpn_share { get; set; }
        public string total { get; set; }
        public string elg_amount { get; set; }
        public string sanction { get; set; }
        public string sactiondate { get; set; }
        public string processdate { get; set; }
    }
    public class PFPayable
    {
        public int EntityId { get; set; }
        public string pf_no { get; set; }
        public string basic_da { get; set; }
        public string purposeType { get; set; }
        public string pfloansid { get; set; }
        public string apply_amount { get; set; }
        public string rate_of_interest { get; set; }
        public string amount_applied_for_2 { get; set; }
        public string calculating_months { get; set; }
        public string netownsharenetbankshare { get; set; }
        public string least_of_3 { get; set; }
        public string gross_salary { get; set; }
        public string net_salary { get; set; }
        public string net_minus_pf { get; set; }
        public string rdofgrosssalary { get; set; }
        public string totaloutstandingloan { get; set; }
        public string amountrecommendedforsanction { get; set; }
      public string sanctamt { get; set; }
        public string sactiondate { get; set; }
        public string processdate { get; set; }
    }

    public class authorise
    {
        public int EntityId { get; set; }
        public string loantype { get; set; }

    }

    public class TDS
    {
        public general general { get; set; }
        public List<cc> cc { get; set; }
        public List<ccc> ccc { get; set; }

        public List<ccd> ccd { get; set; }

        public List<other> other { get; set; }

        public int empId { get; set; }
    }

    public class TDSUpdate
    {
        public List<update> update { get; set; }
    }

    public class update
    {
        public string id { get; set; }
        public string amount { get; set; }
    }

    public class general
    {

        public string name { get; set; }

        public string per_address { get; set; }

        public string pan_no { get; set; }
        public string emp_code { get; set; }

        public string designation { get; set; }

        public string sex { get; set; }
        public string fm { get; set; }

        public string basic { get; set; }

        public string da_percent { get; set; }
        public string per_allowance { get; set; }
        public string int_ref { get; set; }

        public string t_inc { get; set; }

        public string fpa_allow { get; set; }
        public string fpiip { get; set; }
        public string pf_perk { get; set; }

        public string loan_perk { get; set; }

        public string incentives { get; set; }
        public string spl_allowance { get; set; }
        public string spl_da { get; set; }

        public string hra { get; set; }

        public string cca { get; set; }
        public string gross { get; set; }
        public string standard_deduction { get; set; }
        public string ent_allowance { get; set; }
        public string emp_tax { get; set; }
        public string aggregate_of_4 { get; set; }

        public string Income_chargable { get; set; }
        public string other { get; set; }
        public string houseloan_allownace { get; set; }

        public string total_gross_income { get; set; }

        public string section_cd { get; set; }
        public string section_cd_amount { get; set; }

        public string loan_instl_amt { get; set; }

        public string section_ccc { get; set; }
        public string section_ccd { get; set; }

        public string edu_allowance { get; set; }

        public string deductible { get; set; }
        public string total_income { get; set; }
        public string income { get; set; }
        public string edu_cess { get; set; }
        public string tax_payable { get; set; }
        public string tax_till { get; set; }
        public string tax_balance { get; set; }
        public string balance_month { get; set; }
        public string tds_per_month { get; set; }
        public string vpf { get; set; }
        public string section_vpf_amount { get; set; }
        public string section_ccc_amount { get; set; }
        public string section_ccd_amount { get; set; }
        public string house_rent_allowance { get; set; }
        public string total_section { get; set; }
        public string balance { get; set; }
        public string round_income { get; set; }
        public string section_87a { get; set; }

    }

    public class cc
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }
        public string ded_amount { get; set; }

    }
    public class ccc
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }

        public string ded_amount { get; set; }
    }
    public class ccd
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }

        public string ded_amount { get; set; }
    }
    public class other
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }
        public string ded_amount { get; set; }
    }
    public class TDSForAll
    {
        public bool final { get; set; }
        public bool flag { get; set; }
       
    }

    public class Form12BA
    {
        public int EntityId { get; set; }
        public List<form12ba> form12ba { get; set; }
    }

    public class form12ba
    {
        public string id { get; set; }
        public string preq { get; set; }
        public string rec { get; set; }
        public string tax { get; set; }
        public string action { get; set; }
        
    }
    public class OtherTDSDeduction
    {
        public string stringData { get; set; }
        public int EntityId { get; set; }

    }

    public class GetIDs_fromPayslipGrid
    {
        public List<PayslipIds> psid { get; set; }
    }

    public class PayslipIds
    {
        public string id { get; set; }
       
    }

    public class PayslipDownload
    {
        public List<string> psid { get; set; }

    }

    public class EmpSearch
    {
        public int empId { get; set; }

    }

    public class PayslipStructure
    {
        public List<PayslipStr> payslip { get; set; }
    }

    public class PayslipStr
    {
        public string id { get; set; }

        public string type { get; set; }

        public string status { get; set; }

        public string action { get; set; }

    }

    public class EncashmentStructure
    {
        public List<EncashEarn> enashEarn { get; set; }
        public List<EncashDed> enashDed { get; set; }
    }

    public class EncashEarn
    {
        public string id { get; set; }

        public string type { get; set; }

        public string status { get; set; }

        public string action { get; set; }

    }

    public class EncashDed
    {
        public string id { get; set; }

        public string type { get; set; }

        public string status { get; set; }

        public string action { get; set; }

    }
    public class retermaintemp
    {
        
        public string EmpId { get; set; }
        public string pfcaldate { get; set; }
        
        public string RelivingDate { get; set; }
        public decimal inst_rates { get; set; }
       
    }
    public class form16
    {

        public string fy { get; set; }
        public string bsrcode { get; set; }
        public string challan_no { get; set; }
        public string id { get; set; }
        public string payment_date { get; set; }
        public string Action { get; set; }

    }

}
