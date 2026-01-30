namespace PayrollModels
{
    public class CommonGetModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public char RowType { get; set; }
        public string Description { get; set; }
        public string remarks { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string Increment_Date_WEF { get; set; }
        public string IncDate { get; set; }
        public string change_inc_date { get; set; }
        public string AccNum { get; set; }
        public string PayType { get; set; }
        public string PayMonths { get; set; }
        public string StartMonth { get; set; }
        public string StopMonth { get; set; }
        public string Amount { get; set; }
        public string Action { get; set; }
        public string plencash { get; set; }
        public string totalpl { get; set; }
        public string UpdatedDate { get; set; }
        public string id { get; set; }
        public string loan_id { get; set; }
        public string loan_description { get; set; }
        public string interest_rate { get; set; }
        public string encashyear { get; set; }
        public string encashfm { get; set; }
        public string encashfy { get; set; }
    }


    public class AllowanceTypes
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class Allowance_model
    {
        public int RowId { get; set; }
        public string SlNo { get; set; }
        public string HRF { get; set; }
        public string grpcol { get; set; }
        public string AllowanceType { get; set; }
        public string from_date { get; set; }
        public string to_date { get; set; }
        public string amount { get; set; }
    }
    public class PfNomineemodel
    {
        public string id { get; set; }
        public string emp_code { get; set; }
        public string membername { get; set; }
        public string gender { get; set; }
        public string relation { get; set; }
        public string dob { get; set; }
        public string age { get; set; }
        public string date { get; set; }
        public string percentage { get; set; }
        public string Action { get; set; }
    }

    public class EmpMaster
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string AccNum { get; set; }
        public string Amount { get; set; }
        public string PayType { get; set; }
        public string PayMonths { get; set; }
        public string StartMonth { get; set; }
        public string StopMonth { get; set; }

    }
    public class allowanereport
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string amount { get; set; }


    }
    public class Getmonthdays
    {
        public string month_days { get; set; }
    }

    public class getUpdate
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DOL { get; set; }
        public string tax_deducted_at_source { get; set; }

        public string tds_per_month { get; set; }

        public string diff { get; set; }

    }
    public class GetIncrementModel
    {
        public string fm { get; set; }
        public string category { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Basic { get; set; }
        public string Increment { get; set; }
        public string increment_type { get; set; }
        public string increment_date { get; set; }
        public string desi_mid { get; set; }
        public string stages { get; set; }
        public string DOJ { get; set; }



    }
    public class GetLoanLedgerModel
    {

        public string amount_issued { get; set; }
        public string sanction_date { get; set; }
        public string interest_rate { get; set; }
        public string loan_opening { get; set; }
        public string loan_repaid { get; set; }
        public string loan_closing { get; set; }
        public string interest_opening { get; set; }
        public string interest_accrued { get; set; }
        public string interest_repaid { get; set; }
        public string interest_closing { get; set; }
        public string installment_repaid { get; set; }

    }
    public class GetIncrementTable
    {
        public string Basic { get; set; }
        public string Increment { get; set; }
        public string Stages { get; set; }
    }

    public class GetStagIncrementTable
    {
        public string Basic { get; set; }
        public string Increment { get; set; }
        public string Stages { get; set; }
        public string Type { get; set; }
    }

    public class loansdata
    {

        // public string active { get; set; }
        //public string trans_id { get; set; }
        //public string Action { get; set; }
    }

    public class LoansAndAdavnceModel
    {
        public int id { get; set; }
        public string priority { get; set; }
        //public string slno { get; set; }
        public string date_disburse { get; set; }
        public string loan_amount { get; set; }
        public string interest_amount_recovered { get; set; }
        public string interest_rate { get; set; }
        public string interest_accured { get; set; }
        public string principal_amount_recovered { get; set; }
        public string Action { get; set; }


    }

    public class loansDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string display { get; set; }
        public string Value { get; set; }
        public string Action { get; set; }
        //public string loan_description { get; set; }
        //public string total_amount { get; set; }
        //public string no_of_installment { get; set; }
        //public string interest_installment { get; set; }
        //public string sanction_date { get; set; }
        //public string method { get; set; }
        //public string interest_rate { get; set; }
        //public string installment_amount { get; set; }
        //public string recovered_amount { get; set; }
        //public string completed_installment { get; set; }
        //public string loan_start_from { get; set; }
        //public string code_master { get; set; }

    }
    public class IncomTaxBankPaymentmodel
    {
        public string Action;
        public string fy { get; set; }
        public string fm { get; set; }
        public string payment_date { get; set; }
        public string bank_name { get; set; }
        public string amount { get; set; }
        public string challan_no { get; set; }
        public string id { get; set; }

    }

    public class PFNONRepayable
    {
        public string pf_no { get; set; }
        public string date_of_join { get; set; }
        public string basic { get; set; }
        public string own_share { get; set; }
        public string bank_share { get; set; }
        public string da { get; set; }
        public string vpf { get; set; }

    }


    public class Form5A
    {

        public int RowId { get; set; }
        public string EmpMonthSalary { get; set; }
        public string NumberofEmployees { get; set; }
        public string TaxperMonth { get; set; }
        public string TaxDeducted { get; set; }
    }


    public class loansTypes
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class loansOptions
    {
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }

    public class payPeriod
    {
        public string date { get; set; }
    }
    public class PfrepayableDetails
    {
        public string basic_da { get; set; }
        public string ploanadv { get; set; }
        public string basic { get; set; }
        public string pf_no { get; set; }
        public string date_of_join { get; set; }
        public decimal own_share { get; set; }
        public decimal bank_share { get; set; }
        public string da_percent { get; set; }
        public int vpf { get; set; }
        public string loantype { get; set; }
        public string purpose_name { get; set; }
        public string rate_of_basic_da { get; set; }
        public string eligibility_amount { get; set; }
        public string total { get; set; }
        public string applyamount1 { get; set; }
        public string applyamount2 { get; set; }
        public string gross { get; set; }
        public string net { get; set; }
        public string pf1 { get; set; }
        public string pf2 { get; set; }
        public string pftype { get; set; }
        public string calm { get; set; }
        public string least { get; set; }
        public string firstinstall { get; set; }
        public string sanctionamt { get; set; }
    }
    public class PfDetails
    {

        public string basic { get; set; }
        public string pf_no { get; set; }
        public string date_of_join { get; set; }
        public int own_share { get; set; }
        public int bank_share { get; set; }
        public string da_percent { get; set; }
        public int vpf { get; set; }

        public string purpose_name { get; set; }
        public double rate_of_basic_da { get; set; }
        public string eligibility_amount { get; set; }
        public string total { get; set; }
        public string amount_applied { get; set; }
        public string gross { get; set; }
        public string net { get; set; }
        public string pf1 { get; set; }
        public string pf2 { get; set; }
        public string purpose_of_advance { get; set; }
        public string month { get; set; }
        public string dor { get; set; }
        public string sanction { get; set; }

    }
    public class PayslipService
    {
        public string Id { get; set; }
        public string run_date_time { get; set; }
        public string total_count { get; set; }
        public string status { get; set; }
        public string process_count { get; set; }
    }
    public class Members_Leaving
    {
        public string Account_No { get; set; }
        public string Name_of_Member { get; set; }
        public string Father_or_Husband_Name { get; set; }
        public string Date_of_Leaving { get; set; }
        public string Reason_of_Leaving { get; set; }
        public string Remarks { get; set; }
    }

    public class certDetails
    {
        public string id { get; set; }
        public string cert_name { get; set; }
        public string status { get; set; }

    }

    public class section80C
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }
        public string ded_amount { get; set; }
        
    }
    public class section80CCC
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }

        public string ded_amount { get; set; }
    }
    public class section80CCD
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string allowance_amount { get; set; }

        public string ded_amount { get; set; }
    }
    public class sectionOther
    {
        public string id { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public string section { get; set; }
        public string allowance_amount { get; set; }
        public string ded_amount { get; set; }
    }
    public class LICReport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Account_No { get; set; }
        public string Amount { get; set; }
        public string Total { get; set; }
        public string Id { get; set; }
        public string id { get; set; }
        public string Type { get; set; }
        public string fY { get; set; }


    }

   public class misearnreport
    {
        public string column_name { get; set; }
    }

   
    public class MISReportPS
    {
        public string id { get; set; }
        public string value { get; set; }
    }
    public class MISReportDeduct
    {
        public string id { get; set; }
        public string value { get; set; }
    }
    public class MISReportTds
    {
        public string id { get; set; }
        public string value { get; set; }
    }
    // public class salarybillofmonth    {
    //  public string Name { get; set; }

    // Some other properties..

    // public int BrId { get; set; }

    // public IEnumerable<SelectListItem> Branches { get; set; }
    //}
    public class ObShare
    {
        public string fund_own { get; set; }
        public string fund_bank { get; set; }
        public string fund_vpf { get; set; }

    }

    public class ObShareIntrst
    {
        public string ist_own { get; set; }
        public string ist_bank { get; set; }
        public string ist_vpf { get; set; }

    }
    public class ObSharePrev
    {
        public string prev_own { get; set; }
        public string prev_bank { get; set; }
        public string prev_vpf { get; set; }

    }
    public class ObSharePres
    {
        public string pres_own { get; set; }
        public string pres_bank { get; set; }
        public string pres_vpf { get; set; }

    }
    public class payslippdfmodel
    {
        public int emp_id { get; set; }
        public int emp_code { get; set; }
        public string emp_name { get; set; }

        public string designation { get; set; }

        public string branch { get; set; }



    }

    public class TDSProcess
    {
        public string option { get; set; }
        public string name { get; set; }

        public string per_address { get; set; }

        public string pan_no { get; set; }
        public string emp_code { get; set; }

        public string designation { get; set; }

        public string sex { get; set; }
        public string fm { get; set; }
        public string employer_name_address { get; set; }
        public string employer_pan { get; set; }
        public string employer_tan { get; set; }

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

        public string values_of_17_2 { get; set; }
        public string values_of_17_3 { get; set; }
        public string paid_by_emp { get; set; }

    }


    public class Payslippdf
    {
        public string Id { get; set; }
        public string emp_code { get; set; }
        public string shortname { get; set; }
        public string Description { get; set; }
        public string gross_amount { get; set; }

        public string deductions_amount { get; set; }

        public string net_amount { get; set; }
        public string spl_type { get; set; }
    }

    public class form16Model1
    {
        public string Id { get; set; }
        public string emp_code { get; set; }
        public string shortname { get; set; }
        public string Description { get; set; }
        public string gross_amount { get; set; }

        public string deductions_amount { get; set; }

        public string net_amount { get; set; }
        public string fy { get; set; }
    }
    public class form7Model1
    {
        public string Id { get; set; }
        public string emp_code { get; set; }
        public string shortname { get; set; }
        public string Description { get; set; }
        public string gross_amount { get; set; }

        public string deductions_amount { get; set; }

        public string net_amount { get; set; }
        public string fy { get; set; }
    }
}