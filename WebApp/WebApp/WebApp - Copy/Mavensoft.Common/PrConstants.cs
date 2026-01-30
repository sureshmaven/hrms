
namespace Mavensoft.Common
{
    public static class PrConstants
    {
        //****** Reports *******
        public const string PDF_REPORT_HEADER_COLOUR = "#C8EAFB";
        public const string PDF_REPORT_FOOTER_COLOUR = "#FFEFE6";
        public const string PDF_REPORT_DIVIDE_COLOUR = "#9B870C";
        public const string DEBIT_ADVICE = "DEBIT ADVICE";
        public const string DEBIT_VOCHER = "DEBIT VOCHER";
        public const string CREDIT_ADVICE = "CREDIT ADVICE";
        public const string CREDIT_VOCHER = "CREDIT VOCHER";
        //** Loan & Advances constants
        public const string LOAN_INSTALLMENT = "Installment";
        public const string LOAN_PARTPAYMENT = "Part Pay";
        public const string CD_LOAN_CODE = "CD2";
        public const string EDUCATIONAL_LOAN_CODE = "EDUC";
        public const string FESTIVAL_LOAN_CODE = "FEST";
        public const string HOUSING_LOAN_CODE = "HL2";
        public const string HOUSING_2A_LOAN_CODE = "HL2A";
        public const string HOUSING_2B_2C_LOAN_CODE = "HL2BC";
        public const string HOUSING_ADDL_LOAN_CODE = "HLADD";
        public const string HOUSING_COMMERCIAL_LOAN_CODE = "HLCOM";
        public const string HOUSE_LOAN_MAIN = "HOUS1";
        public const string PF_LOAN1_CODE = "PFL1";
        public const string PF_LOAN2_CODE = "PFL2";
        public const string PF_LOANST1_CODE = "PFHT1";
        public const string PF_LOANST2_CODE = "PFHT2";
        public const string PF_LOANLT1_CODE = "PFLT1";
        public const string PF_LOANLT2_CODE = "PFLT2";
        public const string PF_LOANLT3_CODE = "PFLT3";
        public const string PF_LOANLT4_CODE = "PFLT4";
        public const string VEH_LOANLT5_CODE = "VEH4W";
        public const string VEH_LOANLT6_CODE = "VEH2W";
        public const string PF_LOAN_METHOD = "Interest With Equal Installments";
        public const double PF_ROI = 0.115;

        //payslip constants
        public const string STOP_SALARY = "StopSalary";
        public const string SUSPENDED = "Suspended";
        public const string REGULAR = "Regular";
        public const string ADHOC = "Adhoc";
        public const string ENCASHMENT = "Encashment";
        public const string PROMOTION = "Promotion";

        //payslip basicfields
        public const string BASIC = "Basic";
        public const string TELANAGANA_INCREMENT = "Telangana Increment";
        public const string INTERIM_RELIEF = "Interm Relief";
        public const string LOSS_OF_PAY = "Loss of Pay";
        public const string SPECIAL_PAY = "Special Pay";

        //INCREMENTS
        public const string SPECIAL_INCREMENT = "Special Increment";
        public const string STAGNATION_INCREMENT = "Stagnation Increments";
        public const string ANNUAL_INCREMENT = "Annual Increment";
        public const string JAIIB_INCREMENT = "JAIIB Increment";
        public const string CAIIB_INCREMENT = "CAIIB Increment";

        //ALLOWANCES
        public const string FPIIP = "FPIIP";
        public const string FPA = "Fixed Personal Allowance";
        public const string FPA_HRA_ALLOWANCE = "FPA-HRA Allowanc";
        public const string PERSONAL_QUAL_ALLOWANCE = "Personal Qual Allowance";
        public const string BR_MANAGER_ALLOWANCE = "Br Manager Allowance";
        public const string SPL_CASHIER = "SPL Cashier";
        public const string SPL_WATCHMAN = "SPL Watchman";
        public const string SPL_JAMEDAR = "SPL Jamedar";
        public const string SPL_DAFTER = "SPL Dafter";
        public const string SPL_PERSONAL_PAY = "SPL Personal Pay";
        public const string SPL_ELECTRICIAN = "SPL Electrician";
        public const string SPL_TYPIST = "SPL Typist";
        public const string SPL_STENOGRAPHER = "SPL Stenographer";
        public const string SPL_DUPLICATING_XEROX_MACHINE = "SPL Duplicating/xerox machine";
        public const string SPL_DRIVER = "SPL Driver";
        public const string FACULTY_ALLOWANCE = "FACULTY ALLOWANCE";

        //DEDUCTIONS
        public const string CLUB_SUBSCRIPTION = "Club Subscription";
        public const string TELANAGANA_OFFICERS_ASSC = "TELANGANA OFFICERS ASSN";
        public const string PF_CONTRIBUTION = "PF Contribution";
        public const string VPF_DEDUCTION = "VPF Deduction";
        public const string VPF_PERCENTAGE = "VPF Percentage";
        public const string HFC = "HFC";
        public const string LIC = "LIC";
        public const string IT = "INCOME TAX";
        public const string Provident_Fund = "Provident Fund";
        public const string COD_INS_PRM = "COD_INS_PRM";
         
        //ALLOWANCES
        public const string Medical_Allowance = "Medical Allowance";
        public const string Petrol_Paper = "Petrol & Paper";
        public const string Petrol_Paper1 = "Petrol & Paper 1";

        //code_master 
        public const string Loan_Vendor_Name = "Loan Vendor Name";

        // TDS Employer details

        public const string Name = "TSCAB,Hyderabad";
        public const string PAN = "AAEAT1706F";
        public const string TAN = "HYDT06401D";

        //TDS process
        public const int SECTION_C = 150000;
        public const int SECTION_80DDB = 60000;
        public const int SECTION_80EE = 200000;
        public const int SECTION_24b = 200000;
        public const int SECTION_80D_P = 30000;
        public const int SECTION_80D_F = 25000;
        public const int SECTION_80GPERCENTAGE = 50;
        public const int SECTION_80GGBPERCENTAGE = 100;// 50% PERCENTAGE
        public const int OTHER_SECTION = 150000;
        public const int SECTION_80DD = 125000;
        public const int SECTION_80U = 125000;
        public const int SECTION_80CCD = 50000;
        public const int STANDARD_DEDUCTION = 50000;

        public const int EMP_TAX = 2400;


        //PF_PERKS
        public const double pf_perks = 0.005;
        //PfRepayableloans
        public const double RateofInt = 11.50;

        // Increments_month_end Date

        public const string dtIncrement = "2016-02-01";
    }
}
