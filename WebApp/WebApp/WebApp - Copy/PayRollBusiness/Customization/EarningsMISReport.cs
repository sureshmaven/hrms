using Mavensoft.DAL.Business;
using Mavensoft.DAL.Db;
using PayrollModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Mavensoft.Common;
using Newtonsoft.Json;

namespace PayRollBusiness.Customization
{
    public class EarningsMISReport : BusinessBase
    {
        public EarningsMISReport(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        IList<misearnreportscolumns> lst = new List<misearnreportscolumns>();

        public async Task<IList<misearnreportscolumns>> GetEarningsMIS_Data(string emp_code, string mnth, string payslip)
         {
            if (emp_code.Contains("^"))
            {
                emp_code = "0";
                mnth = "01-01-01";
                payslip = "";
            }
            DateTime str = Convert.ToDateTime(mnth);
            string str1 = str.ToString("yyyy-MM-dd");
            string[] datearr = str1.Split('-');
            if (payslip != "")
            {
                payslip = payslip.Replace(",", "','");
            }

            string qry = "";
            DataTable Loans = new DataTable();
            string qrysel1 = "";
            DataTable pftotal = new DataTable();
            int slno = 0;
            if (emp_code != "All")
            {
                qry = "SELECT * from AllowanceFact where emp_code="+ emp_code + " and fm='" + str1 + "'";
                Loans = await _sha.Get_Table_FromQry(qry);

            }
            else
            {
                qry = "SELECT * from AllowanceFact where  fm='" + str1 + "'";
                Loans = await _sha.Get_Table_FromQry(qry);

            }
            try
            {
                if (emp_code != "0")
                {
                    foreach (DataRow dr in Loans.Rows)
                    {

                        lst.Add(new misearnreportscolumns
                        {
                            RowId = slno++,
                            column1 = dr["emp_code"].ToString(),
                            allowance_type = dr["allowance_type"].ToString(),
                            dim_tid_ref_allow = dr["dim_tid_ref_allow"].ToString(),
                            ABHLFHYD = dr["ABHLFHYD"].ToString(),
                            ANNUALINCREMENT = dr["ANNUAL INCREMENT"].ToString(),
                            APCCADBEMP = dr["APCCADBEMP"].ToString(),
                            APCOBEDLHNR = dr["APCOBEDLHNR"].ToString(),
                            APCOBEDLHO = dr["APCOBEDLHO"].ToString(),
                            APCOBHFCHO = dr["APCOBHFCHO"].ToString(),
                            APCOBHFCLT = dr["APCOBHFCLT"].ToString(),
                            APSCBLTASSN = dr["APSCBLTASSN"].ToString(),
                            ARR_GR_AMT = dr["ARR_GR_AMT"].ToString(),
                            ARREARGROSSAMOUNT = dr["ARREAR GROSS AMOUNT"].ToString(),
                            BASIC = dr["BASIC"].ToString(),
                            BRMANAGERALLOWANCE = dr["BR MANAGER ALLOWANCE"].ToString(),
                            BR_MGR = dr["BR_MGR"].ToString(),
                            CAIIB = dr["CAIIB"].ToString(),
                            CAIIBINCREMENT = dr["CAIIB INCREMENT"].ToString(),
                            CCA = dr["CCA"].ToString(),
                            CCSAP = dr["CCSAP"].ToString(),
                            CCSHYD = dr["CCSHYD"].ToString(),
                            CD2 = dr["CD2"].ToString(),
                            CEOALLOWANCE = dr["CEO ALLOWANCE"].ToString(),
                            CEOALLW = dr["CEOALLW"].ToString(),
                            //CHILDRENEDUCATIONALLOWANCE = dr["CHILDREN EDUCATION ALLOWANCE"].ToString(),
                            CODINSPRM = dr["CODINSPRM"].ToString(),
                            CODINSPRM1 = dr["CODINSPRM1"].ToString(),
                            CODPERKS = dr["CODPERKS"].ToString(),
                            COURT = dr["COURT"].ToString(),
                            COURTAB = dr["COURTAB"].ToString(),
                            COURTDED = dr["COURTDED"].ToString(),
                            COURTHYD = dr["COURTHYD"].ToString(),
                            COURTKADAPA = dr["COURTKADAPA"].ToString(),
                            COURTSECBAD = dr["COURTSECBAD"].ToString(),
                            COURTSLRPET = dr["COURTSLRPET"].ToString(),
                            COURTTANUKU = dr["COURTTANUKU"].ToString(),
                            COURTVIZAG = dr["COURTVIZAG"].ToString(),
                            DA = dr["DA"].ToString(),
                            DCCB = dr["DCCB"].ToString(),
                            DEPU = dr["DEPU"].ToString(),
                            DEPUTATIONALLOWANCE = dr["DEPUTATION ALLOWANCE"].ToString(),
                            DRCCSVIZAG = dr["DRCCSVIZAG"].ToString(),
                            EASSNSUB = dr["EASSNSUB"].ToString(),
                            EDUC = dr["EDUC"].ToString(),
                            EMPLOYEETDS = dr["EMPLOYEE TDS"].ToString(),
                            ENCASHMENT = dr["ENCASHMENT"].ToString(),
                            ESI = dr["ESI"].ToString(),
                            ESTT = dr["ESTT"].ToString(),
                            EXGRATIA = dr["EXGRATIA"].ToString(),
                            FA = dr["FA"].ToString(),
                            FACULTYALLOWANCE = dr["FACULTY ALLOWANCE"].ToString(),
                            FAELURU = dr["FAELURU"].ToString(),
                            FAMEDHAK = dr["FAMEDHAK"].ToString(),
                            FAONGOLE = dr["FAONGOLE"].ToString(),
                            FCLTYALLW = dr["FCLTYALLW"].ToString(),
                            FEST = dr["FEST"].ToString(),
                            FESTADVANCE = dr["FEST. ADVANCE"].ToString(),
                            FGPRM = dr["FGPRM"].ToString(),
                            FIXEDALLOWANCE = dr["FIXED ALLOWANCE"].ToString(),
                            FIXEDPERSONALALLOWANCE = dr["FIXED PERSONAL ALLOWANCE"].ToString(),
                            FPINCENTIVERECOVERY = dr["FP INCENTIVE RECOVERY"].ToString(),
                            FPAHRAALLOWANCE = dr["FPA-HRA ALLOWANCE"].ToString(),
                            FPIIP = dr["FPIIP"].ToString(),
                            FPIR = dr["FPIR"].ToString(),
                            FXDALLW = dr["FXDALLW"].ToString(),
                            GSLI = dr["GSLI"].ToString(),
                            HDCF = dr["HDCF"].ToString(),
                            HFC = dr["HFC"].ToString(),
                            HL2 = dr["HL2"].ToString(),
                            HL2A = dr["HL2A"].ToString(),
                            HL2BC = dr["HL2BC"].ToString(),
                            HLADD = dr["HLADD"].ToString(),
                            HLCOM = dr["HLCOM"].ToString(),
                            HLPLT = dr["HLPLT"].ToString(),
                            HOUINSUPRE = dr["HOUINSUPRE"].ToString(),
                            HOUPROPINS = dr["HOUPROPINS"].ToString(),
                            HOUS1 = dr["HOUS1"].ToString(),
                            HOUS2 = dr["HOUS2"].ToString(),
                            HOUS3 = dr["HOUS3"].ToString(),
                            HOUSI = dr["HOUSI"].ToString(),
                            HRA = dr["HRA"].ToString(),
                            HRADIFFAUG10 = dr["HRA DIFF-AUG-10"].ToString(),
                            HYDCOURT = dr["HYDCOURT"].ToString(),
                            INCENTIVE = dr["INCENTIVE"].ToString(),
                            INCENTIVEDIFF = dr["INCENTIVE DIFF"].ToString(),
                            INCREMENT = dr["INCREMENT"].ToString(),
                            INTERESTONNSC = dr["INTEREST ON NSC (EARNING)"].ToString(),
                            INTERIM = dr["INTERIM"].ToString(),
                            INTERIMALLOWANCE = dr["INTERIM ALLOWANCE"].ToString(),
                            INTERIMRELIEF = dr["INTERIM RELIEF"].ToString(),
                            INTERMRELIEF = dr["INTERM RELIEF"].ToString(),
                            JAIIB = dr["JAIIB"].ToString(),
                            JAIIBINCREMENT = dr["JAIIB INCREMENT"].ToString(),
                            JEEVAN = dr["JEEVAN"].ToString(),
                            KADAPADCCB = dr["KADAPADCCB"].ToString(),
                            KMLCOOPBANK = dr["KMLCOOPBANK"].ToString(),
                            LEAVEENCASHMENT = dr["LEAVE ENCASHMENT"].ToString(),
                            LIC = dr["LIC"].ToString(),
                            LICPREMIUM = dr["LIC PREMIUM"].ToString(),
                            LICMACHILI = dr["LICMACHILI"].ToString(),
                            LICPENSION = dr["LICPENSION"].ToString(),
                            LIFEINS = dr["LIFEINS"].ToString(),
                            LOANPERKS = dr["LOANPERKS"].ToString(),
                            LOSSOFPAY = dr["LOSS OF PAY"].ToString(),
                            LTPFLOAN = dr["LT PF LOAN"].ToString(),
                            LTASSNLEVY = dr["LTASSNLEVY"].ToString(),
                            LTC = dr["LTC"].ToString(),
                            LTCENCASHMENT = dr["LTC ENCASHMENT"].ToString(),
                            LTCADVREC = dr["LTCADVREC"].ToString(),
                            MARR = dr["MARR"].ToString(),
                            MEDICAL = dr["MEDICAL"].ToString(),
                            MEDICALAID = dr["MEDICAL AID"].ToString(),
                            MEDICALALLOWANCE = dr["MEDICAL ALLOWANCE"].ToString(),
                            MEDICALADVANCE = dr["MEDICALADVANCE"].ToString(),
                            MISCDED = dr["MISCDED"].ToString(),
                            MISCEARN = dr["MISCEARN"].ToString(),
                            MISCELLAN = dr["MISCELLAN"].ToString(),
                            NPSGALLOWANCE = dr["NPSG ALLOWANCE"].ToString(),
                            NPSGA = dr["NPSGA"].ToString(),
                            OFFASSN = dr["OFFASSN"].ToString(),
                            OFFASSNC = dr["OFFASSNC"].ToString(),
                            OFFI = dr["OFFI"].ToString(),
                            OFFICIATINGALLOWANCE = dr["OFFICIATING ALLOWANCE"].ToString(),
                            ONDAYSAL = dr["ONDAYSAL"].ToString(),
                            ONEDAYSAL = dr["ONEDAYSAL"].ToString(),
                            OTWAGES = dr["OT WAGES"].ToString(),
                            PERNLON1 = dr["PERNLON1"].ToString(),
                            PERPAY = dr["PERPAY"].ToString(),
                            PERQPAY = dr["PERQPAY"].ToString(),
                            PERS = dr["PERS"].ToString(),
                            PERSONALPAY = dr["PERSONAL PAY"].ToString(),
                            PERSONALQUALALLOWANCE = dr["PERSONAL QUAL ALLOWANCE"].ToString(),
                            PETROLPAPER = dr["PETROL & PAPER"].ToString(),
                            PETROLPAPER1 = dr["PETROL & PAPER 1"].ToString(),
                            PF = dr["PF"].ToString(),
                            PF5 = dr["PF5"].ToString(),
                            PFHT1 = dr["PFHT1"].ToString(),
                            PFHT2 = dr["PFHT2"].ToString(),
                            PFLT1 = dr["PFLT1"].ToString(),
                            PFLT2 = dr["PFLT2"].ToString(),
                            PFLT3 = dr["PFLT3"].ToString(),
                            PFLT4 = dr["PFLT4"].ToString(),
                            PFPERKS = dr["PFPERKS"].ToString(),
                            PH = dr["PH"].ToString(),
                            PHONE = dr["PHONE"].ToString(),
                            PHYSICALLYHANDICAPPED = dr["PHYSICALLY HANDICAPPED"].ToString(),
                            RESATTENDERSALLOWANCE = dr["RES. ATTENDERS ALLOWANCE"].ToString(),
                            RESATTN = dr["RESATTN"].ToString(),
                            SBF = dr["SBF"].ToString(),
                            SBFLN = dr["SBFLN"].ToString(),
                            SHIFTDUTYALLOWANCE = dr["SHIFT DUTY ALLOWANCE"].ToString(),
                            SOCIE = dr["SOCIE"].ToString(),
                            SP_ACSTI = dr["SP_ACSTI"].ToString(),
                            SP_ARREAR = dr["SP_ARREAR"].ToString(),
                            SP_BILLCOLL = dr["SP_BILLCOLL"].ToString(),
                            SP_CARETAKE = dr["SP_CARETAKE"].ToString(),
                            SP_CASHCAD = dr["SP_CASHCAD"].ToString(),
                            SP_CASHIER = dr["SP_CASHIER"].ToString(),
                            SP_CONVEY = dr["SP_CONVEY"].ToString(),
                            SP_DAFTARI = dr["SP_DAFTARI"].ToString(),
                            SP_DESPATCH = dr["SP_DESPATCH"].ToString(),
                            SP_DRIVER = dr["SP_DRIVER"].ToString(),
                            SP_ELEC = dr["SP_ELEC"].ToString(),
                            SP_INCENTIVE = dr["SP_INCENTIVE"].ToString(),
                            SP_JAMEDAR = dr["SP_JAMEDAR"].ToString(),
                            SP_KEY = dr["SP_KEY"].ToString(),
                            SP_LIBRARY = dr["SP_LIBRARY"].ToString(),
                            SP_LIFT = dr["SP_LIFT"].ToString(),
                            SP_NONPROM = dr["SP_NONPROM"].ToString(),
                            SP_PERPAY = dr["SP_PERPAY"].ToString(),
                            SP_RECASST = dr["SP_RECASST"].ToString(),
                            SP_RECEPTION = dr["SP_RECEPTION"].ToString(),
                            SP_RECSUB = dr["SP_RECSUB"].ToString(),
                            SP_SD_AWARD = dr["SP_SD_AWARD"].ToString(),
                            SP_SD_MGR = dr["SP_SD_MGR"].ToString(),
                            SP_SHFTDUTY = dr["SP_SHFTDUTY"].ToString(),
                            SP_STENO = dr["SP_STENO"].ToString(),
                            SP_TELEPHONE = dr["SP_TELEPHONE"].ToString(),
                            SP_TYPIST = dr["SP_TYPIST"].ToString(),
                            SP_WATCHMAN = dr["SP_WATCHMAN"].ToString(),
                            SP_XEROX = dr["SP_XEROX"].ToString(),
                            SPCLDA = dr["SPCL. DA"].ToString(),
                            SPECIALINCREMENT = dr["SPECIAL INCREMENT"].ToString(),
                            SPECIALPAY = dr["SPECIAL PAY"].ToString(),
                            SPLARREARINCENTIVE = dr["SPL ARREAR INCENTIVE"].ToString(),
                            SPLBILLCOLLECTOR = dr["SPL BILL COLLECTOR"].ToString(),
                            SPLCARETAKER = dr["SPL CARE TAKER"].ToString(),
                            SPLCASHCABIN = dr["SPL CASH CABIN"].ToString(),
                            SPLCASHIER = dr["SPL CASHIER"].ToString(),
                            SPLCONVEYANCE = dr["SPL CONVEYANCE"].ToString(),
                            SPLDAFTAR = dr["SPL DAFTAR"].ToString(),
                            SPLDAFTER = dr["SPL DAFTER"].ToString(),
                            SPLDESPATCH = dr["SPL DESPATCH"].ToString(),
                            SPLDRIVER = dr["SPL DRIVER"].ToString(),
                            SPLDUPLICATINGXEROXMACHINE = dr["SPL DUPLICATING/XEROX MACHINE"].ToString(),
                            SPLELECTRICIAN = dr["SPL ELECTRICIAN"].ToString(),
                            SPLINCENTIVE = dr["SPL INCENTIVE"].ToString(),
                            SPLJAMEDAR = dr["SPL JAMEDAR"].ToString(),
                            SPLKEY = dr["SPL KEY"].ToString(),
                            SPLLIBRARY = dr["SPL LIBRARY"].ToString(),
                            SPLLIFTOPERATOR = dr["SPL LIFT OPERATOR"].ToString(),
                            SPLNONPROMOTIONAL = dr["SPL NON PROMOTIONAL"].ToString(),
                            SPLPERSONALPAY = dr["SPL PERSONAL PAY"].ToString(),
                            SPLRECEPTIONISTALLOWANCE = dr["SPL RECEPTIONIST ALLOWANCE"].ToString(),
                            SPLRECORDROOMASSTALLOWANCE = dr["SPL RECORD ROOM ASST ALLOWANCE"].ToString(),
                            SPLRECORDROOMSUBSTAFFALL = dr["SPL RECORD ROOM SUB STAFF ALL"].ToString(),
                            SPLRECORDROOMUBSTAFFALLOWANCE = dr["SPL RECORD ROOM SUB.STAFF ALLOWANCE"].ToString(),
                            SPLSPLALWACSTI = dr["SPL SPL.ALW.ACSTI"].ToString(),
                            SPLSPLITDUTYMANAGERS = dr["SPL SPLIT DUTY - MANAGERS"].ToString(),
                            SPLSPLITDUTYAWARDSTAFF = dr["SPL SPLIT DUTY -AWARD STAFF"].ToString(),
                            SPLSTENOGRAPHER = dr["SPL STENOGRAPHER"].ToString(),
                            SPLTELEPHONEOPERATOR = dr["SPL TELEPHONE OPERATOR"].ToString(),
                            SPLTYPIST = dr["SPL TYPIST"].ToString(),
                            SPLWATCHMAN = dr["SPL WATCHMAN"].ToString(),
                            SPLALLOW = dr["SPL. ALLOW"].ToString(),
                            SPLINCR = dr["SPLINCR"].ToString(),
                            SPLPAY = dr["SPLPAY"].ToString(),
                            STAGALLOW = dr["STAGALLOW"].ToString(),
                            STAGNATIONINCREMENTS = dr["STAGNATION INCREMENTS"].ToString(),
                            SUBCLUB = dr["SUBCLUB"].ToString(),
                            SUBLT = dr["SUBLT"].ToString(),
                            SUBST = dr["SUBST"].ToString(),
                            SUBUNION = dr["SUBUNION"].ToString(),
                            SYSADMN = dr["SYSADMN"].ToString(),
                            SYSTEMADMINISTRATORALLOWANCE = dr["SYSTEM ADMINISTRATOR ALLOWANCE"].ToString(),
                            TDS = dr["TDS"].ToString(),
                            TEACH = dr["TEACH"].ToString(),
                            TEACHINGALLOWANCE = dr["TEACHING ALLOWANCE"].ToString(),
                            TELANGANAINCREMENT = dr["TELANGANA INCREMENT"].ToString(),
                            TGASSN = dr["TGASSN"].ToString(),
                            TGUNION = dr["TGUNION"].ToString(),
                            TOFA = dr["TOFA"].ToString(),
                            TSCABOA = dr["TSCABOA"].ToString(),
                            UNIONLEVY = dr["UNIONLEVY"].ToString(),
                            VEH2W = dr["VEH2W"].ToString(),
                            VEH4W = dr["VEH4W"].ToString(),
                            VEHINS = dr["VEHINS"].ToString(),
                            VEHMACHILI = dr["VEHMACHILI"].ToString(),
                            VIJAYA = dr["VIJAYA"].ToString(),
                            VISAKHA = dr["VISAKHA"].ToString(),
                            VPF = dr["VPF"].ToString(),
                            WASHALLW = dr["WASHALLW"].ToString(),
                            WASHINGALLOWANCE = dr["WASHING ALLOWANCE"].ToString(),


                            //column3 = dr["loan_id"].ToString(),
                            //column4 = dr["adv_total_amount"].ToString(),
                            //column5 = dr["adv_total_installment"].ToString(),
                            //column6 = dr["adv_completed_installment"].ToString(),
                            //column7 = dr["adv_remaining_installment"].ToString(),
                            //column8 = dr["adv_installment_amount"].ToString(),
                            //column9 = dr["os_principal_amount"].ToString(),
                            //column10 = dr["child_os_interest_amount"].ToString(),
                            //column11 = dr["adjust_fm"].ToString(),
                        });
                     
               
                        }
                    //foreach (DataRow dr1 in pftotal.Rows)
                    //{
                    //    lst.Add(new CommonReportModel
                    //    {
                    //        RowId = slno++,
                    //        column1 = "Grand Total",
                    //        column2 = "",
                    //        column3 = dr1["own_share"].ToString(),
                    //        column4 = dr1["bank_share"].ToString(),
                    //        column5 = dr1["bank_share"].ToString(),
                    //        column6 = dr1["pension_open"].ToString(),
                    //    });
                    //}
                }
            }
            catch (Exception ex)
            {
            }
            return lst;
        }



        public async Task<IList<misearnreport>> GetEarningsMIS_Datadropdown()
        {
            string qryfy = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'AllowanceFact' " +
                "and column_name not in ('Emp_code', 'fm', 'allowance_type', 'ps_type', 'dim_tid_ref_allow')";
            DataTable dtearnings = new DataTable();
            dtearnings = await _sha.Get_Table_FromQry(qryfy);

            IList<misearnreport> lstearnings = new List<misearnreport>();
         
            foreach(DataRow dr in dtearnings.Rows)
            {
                lstearnings.Add(new misearnreport
                {

                    column_name = dr["column_name"].ToString()

                });

            }



            return lstearnings;

        }

        //public async Task<string> GetEarningsMIS_Data()
        //{
        //    string query = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'AllowanceFact' " +
        //        "and column_name not in ('Emp_code', 'fm', 'allowance_type', 'ps_type', 'dim_tid_ref_allow')";

        //    DataTable dtallowancetypes = new DataTable();
        //    var dtEarnfields = dtallowancetypes;
        //    var ltjson = JsonConvert.SerializeObject(dtEarnfields);

        //    ltjson = ltjson.Replace("null", "''");

        //    var javaScriptSerializer = new JavaScriptSerializer();
        //    var ltDetails = javaScriptSerializer.DeserializeObject(ltjson);

        //    var resultJson = javaScriptSerializer.Serialize(new { LTDetails = ltDetails });

        //    return JsonConvert.SerializeObject(resultJson);
        //}
    }

}
