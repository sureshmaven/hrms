using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.Common;
using Mavensoft.DAL.Business;
using PayRollBusiness.PayrollService;
using PayrollModels;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Data;
using Newtonsoft.Json;

namespace PayRollBusiness.Process
{

    public class CalculatePFforRetirementBusiness : BusinessBase
    {
        public CalculatePFforRetirementBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
            LoginCredential lCredentials = null;
        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();

        #region getRetirementDate

        public async Task<string> SearchEmployee(string EmpCode)
        {
            List<EmployeeSearchResult> retList = new List<EmployeeSearchResult>();
            string emp_option = "";
            string str_qryempcode = "";
            //List <string>retList = new List<string>();
            //string ECode = "",EName = "",EDesignation = "",EBranch = "",EDoj = "",EDRetire = "",EDoc = "", EselOpt="";

            string strQry = "select e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
            + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
            + " join Designations d on e.CurrentDesignation = d.Id"
            + " join Branches b on e.Branch = b.Id"
            + " join Departments dep on e.Department = dep.Id where empid =" + EmpCode + "";

            DataTable dtEmpDetails = null;
            try
            {
                dtEmpDetails = await _sha.Get_Table_FromQry(strQry);
            }
            catch
            {
            }

            if (dtEmpDetails == null || dtEmpDetails.Rows.Count == 0)
            {
                strQry = "select e.empid, e.EmpId,CONCAT(e.FirstName,' ',e.LastName) as Name,convert(varchar,e.DOJ,105) as DOJ,convert(varchar,e.RetirementDate,105) as RetirementDate,d.Name as Designation,"
                + " case when b.Name = 'OtherBranch' then dep.name  else b.Name end deptbranch from employees e"
                + " join Designations d on e.CurrentDesignation = d.Id "
                + " join Branches b on e.Branch = b.Id join Departments dep on e.Department = dep.Id ";

                int ecode = 0;
                if (int.TryParse(EmpCode, out ecode))
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and empid LIKE '" + EmpCode + "%'; ";
                }
                else
                {
                    strQry += " where  e.RetirementDate>=CAST(CAST(GETDATE() AS DATE) AS DATETIME) and  FirstName LIKE '%" + EmpCode + "%' OR LastName LIKE '%" + EmpCode + "%'; ";
                }

                dtEmpDetails = await _sha.Get_Table_FromQry(strQry);
            }
            DateTime curdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //int curFY = _LoginCredential.FY;
            string[] arrFm = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM").Split('-');
            int curFY = Convert.ToInt32(arrFm[0]);
            int curFM = Convert.ToInt32(arrFm[1]);

            str_qryempcode = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + EmpCode + ";";
            DataTable dt_empopt = await _sha.Get_Table_FromQry(str_qryempcode);

            //int i_fy = Convert.ToInt32(FM);
            if (dt_empopt.Rows.Count > 0)
            {
                if (dt_empopt.Rows[0]["Option"].ToString() == "2")
                {
                    emp_option = "Option 2";
                }
                else if (dt_empopt.Rows[0]["Option"].ToString() == "1")
                {
                    emp_option = "Option 1";
                }
            }
            else
            {
                emp_option = "No Option";
            }
            foreach (DataRow drEDet in dtEmpDetails.Rows)
            {
                DateTime retdate = Convert.ToDateTime(drEDet["RetirementDate"]);
                int reFm = retdate.Month;
                int retFy = retdate.Year;

                retList.Add(new EmployeeSearchResult
                {
                    ECode = drEDet["empid"].ToString(),
                    EName = drEDet["Name"].ToString(),
                    EDesignation = drEDet["Designation"].ToString(),
                    EBranch = drEDet["deptbranch"].ToString(),
                    EDoj = drEDet["DOJ"].ToString(),
                    EDRetire = drEDet["RetirementDate"].ToString(),
                    EDoc = Convert.ToDateTime(drEDet["DOJ"].ToString()).AddMonths(12).ToString("dd-MM-yyyy"),
                    EselOpt = emp_option,
                });


            }

            return JsonConvert.SerializeObject(retList);

        }

        public async Task<DataTable> getRetirementDate(string empid)
        {
            string qry = "select convert(varchar(20),RetirementDate,105)as RetirementDate from employees where empid=" + empid + " ";
            return await _sha.Get_Table_FromQry(qry);
        }
        #endregion
        #region getinstrate
        public async Task<DataTable> getinstrate()
        {
            //string qry = "Select top 1 interest ,fy from pr_pf_year_rate where interest>0 order by fy ";
            string qry = "SELECT top 1 interest,fy FROM pr_pf_year_rate ORDER BY fy desc  ";
            return await _sha.Get_Table_FromQry(qry);
        }
        #endregion

        #region insertRetirementDate
        public async Task<string> insertRetirementDate(string empid, string RelivingDate, decimal interest, string pfcaldate)
        {
            string msg = "";
            string emp_code = "";

            string ob_share = "";
            string Eyear = "0";
            int Fyear = 0;

            string retdate = "select format(RetirementDate,'dd-MM-yyyy') as RetirementDate,empid from Employees  where empid= " + empid + " ";
            DataTable Rdate = await _sha.Get_Table_FromQry(retdate);
            string RetirementDate = Rdate.Rows[0]["RetirementDate"].ToString();

            DateTime ret = Convert.ToDateTime(RelivingDate);
            string rtdate1 = ""; string rtdate2 = ""; string rtdate3 = "";
            DateTime RelivingDates = Convert.ToDateTime(RelivingDate); //Relieving date 
            DateTime pfcaldates = Convert.ToDateTime(pfcaldate); //calculate date
            //pfcaldates
            //DateTime pfcaldate1 = Convert.ToDateTime(pfcaldate);
            string pfdate = pfcaldates.ToString("yyyy-MM-dd");
            string[] pdate = pfdate.Split('-');
            string pfdateY = pdate[0]; //year
            string pfdateM = pdate[1]; //month
            
            string getmonth = "select fy from pr_month_details where active=1";
            DataTable getmonths = await _sha.Get_Table_FromQry(getmonth);
            Eyear = getmonths.Rows[0]["fy"].ToString();
            Fyear = Convert.ToInt32(Eyear) - 1;
            string topmonth = "select top 1 fm from pr_ob_share where  emp_code = " + empid + " and fy =" + Eyear + " order by fm desc";
            DataTable top_month = await _sha.Get_Table_FromQry(topmonth);
            DateTime lastmontnh =Convert.ToDateTime( top_month.Rows[0]["fm"].ToString()); //lastmontnh
            string lastmontnh1 = lastmontnh.ToString("yyyy-MM-dd");
            string[] lastmontnh2 = lastmontnh1.Split('-');
            string lastmontnhY = lastmontnh2[0]; //year
            string lastmontnhM = lastmontnh2[1]; //month

            if ( pfdateY == lastmontnhY && pfdateM == lastmontnhM)
            {
                //if (pfcaldate == RelivingDate || pfdate1 == Rtdate1 && pfdate2 == Rtdate2)
                //{
                if (pfcaldate != null)
                {
                    if (RelivingDate != null)
                    {
                        DateTime dt = Convert.ToDateTime(RelivingDate);
                        string rdate = dt.ToString("yyyy-MM-dd");
                        string[] rtdate = rdate.Split('-');
                        rtdate1 = rtdate[0]; //year
                        rtdate2 = rtdate[1]; //month
                        rtdate3 = rtdate[2]; //date
                    }
                    else
                    {
                        if (RetirementDate != null)
                        {
                            DateTime dt = Convert.ToDateTime(RetirementDate);
                            string rdate = dt.ToString("yyyy-MM-dd");
                            string[] rtdate = rdate.Split('-');
                            rtdate1 = rtdate[0]; //year
                            rtdate2 = rtdate[1]; //month
                            rtdate3 = rtdate[2]; //date
                        }
                    }
                    
                    //ob share data  
                    //string qry = "select emp_code,fm,own_share,bank_share,vpf from pr_ob_share where emp_code=" + empid + " and fy=(select fy from pr_month_details where active=1) order by fm asc";
                    string qry = "select fm,emp_code,ISNULL(own_share_intrst_open,0) as own_share_intrst_open,ISNULL(bank_share_intrst_open,0) as bank_share_intrst_open,ISNULL(vpf_intrst_open,0) as vpf_intrst_open from pr_ob_share ob " +
         " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and " +
         "  DATEFROMPARTS(" + rtdate1 + ", " + rtdate2 + ", " + rtdate3 + " ) and " +
         " ob.emp_code = " + empid + " ";
                    DataTable dts = await _sha.Get_Table_FromQry(qry);

                    string opbal = "select os_open as own ,vpf_open as vpf,bs_open as bank, os_open+vpf_open + bs_open as opbal from pr_pf_open_bal_year where emp_code = " + empid + " and fy = " + Fyear + " ";
                    DataTable op_bals = await _sha.Get_Table_FromQry(opbal);
                    string oldmonth = "select top 1 fm from pr_ob_share where  emp_code = " + empid + " and fy =" + Eyear + " order by fm desc";
                    DataTable old_month = await _sha.Get_Table_FromQry(oldmonth);
                    string fm = old_month.Rows[0]["fm"].ToString();
                    //string opbalmonth = "select own_share + bank_share + vpf as monthdata from pr_ob_share " +
                    //    "where  emp_code = " + empid + " and fm <= '"+ fm +"' and fy =2020 ";
                    string opbalmonth = "select distinct h.fm1,h.own_share + ( case when(select count(c1.own_share) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.own_share from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+ ( case when(select count(c1.own_share) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.own_share) ELSE 0 END from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) own_share,h.bank_share + ( case when(select count(c1.bank_share) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+( case when(select count(c1.bank_share) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.bank_share) ELSE 0 END from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) bank_share,h.vpf + ( case when(select count(c1.vpf) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.vpf from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +( case when(select count(c1.vpf) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.vpf) ELSE 0 END from pr_ob_share_adhoc c1 " +
                        "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) vpf " +
         //"--h.own_share_total + ( case when(select count(c1.own_share_total) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.own_share_total from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +( case when(select count(c1.own_share_total) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.own_share_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)  own_share_total,h.bank_share_total + ( case when(select count(c1.bank_share_total) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share_total from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +( case when(select count(c1.bank_share_total) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) bank_share_total,h.vpf_total + ( case when(select count(c1.vpf_total) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.vpf_total from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+(case when(select count(c1.vpf_total) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.vpf_total from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) vpf_total,h.ownbal,h.bankbal,h.vpfbal, h.os_open_int,h.bs_open_int,h.vpf_open_int,h.active,h.pf_return,h.vpf_return,h.bank_return,h.os_cur_int,h.vpf_cur_int,h.bs_cur_int,h.op_bal_inst,
         //--h.op_bal_inst_year,h.op_bal_inst_NRloan
         " from(select ob.fm as fm1, ob.emp_code, e.ShortName, pay.pf_no, pay.uan_no, REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm, own_share, bank_share, vpf, own_share_total, bank_share_total, vpf_total, pfbal.os_open AS ownbal, pfbal.bs_open as bankbal, pfbal.vpf_open as vpfbal, pfbal.os_open_int, pfbal.bs_open_int, pfbal.vpf_open_int, active = 1, pfbal.pf_return, pfbal.vpf_return, pfbal.bank_return, pfbal.os_cur_int, pfbal.vpf_cur_int, pfbal.bs_cur_int, pfbal.op_bal_inst, pfbal.op_bal_inst_year, pfbal.op_bal_inst_NRloan from pr_ob_share ob join pr_emp_general pay  on ob.emp_code = pay.emp_code join Employees e on e.EmpId = ob.emp_code JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId " +
         "where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31) " +
         "and ob.fm < '" + fm + "' and ob.emp_code in (" + empid + " ) and pay.active = 1 and pfbal.fy = " + Fyear + " ) as h ";
                    DataTable opbal_month = await _sha.Get_Table_FromQry(opbalmonth);
                    //ob bal 
                    string inst_opbal = "select bs_open_int+os_open_int+vpf_open_int as inst_opbal , " +
                        "bs_open_int as bank_inst_opbal,os_open_int as own_inst_opbal,vpf_open_int as vpf_inst_opbal, " +
                        " fy,pfbal.os_open AS ownbal, pfbal.bs_open as bankbal,pfbal.vpf_open as vpfbal from pr_pf_open_bal_year pfbal " +
                           "where emp_code=" + empid + " and fy=" + Fyear + " ";
                    DataTable inst_op_bal = await _sha.Get_Table_FromQry(inst_opbal);
                    string qry2 = "select count(own_share) as totmonths from pr_ob_share ob " +
                        " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and " +
                        " ob.emp_code = " + empid + " ";
                    DataTable dts2 = await _sha.Get_Table_FromQry(qry2);

                    StringBuilder sbqry = new StringBuilder();
                    decimal op_bal_inst = 0; decimal totalmonthdata = 0; decimal no_of_month = 0; decimal opbals = 0; decimal totop_month = 0;
                    string f_month = ""; decimal totmonthdata = 0; decimal totmonth = 0;
                    decimal won_totalmonthdata = 0; decimal bank_totalmonthdata = 0; decimal vpf_totalmonthdata = 0;
                    decimal won_totmonthdata = 0; decimal bank_totmonthdata = 0; decimal vpf_totmonthdata = 0;
                    decimal won_totmonth = 0; decimal bank_totmonth = 0; decimal vpf_totmonth = 0;
                    decimal won_totop_month = 0; decimal bank_totop_month = 0; decimal vpf_totop_month = 0;
                    decimal own_opbals = 0; decimal vpf_opbals = 0; decimal bank_opbals = 0;
                    try
                    {  
                        //1. trans_id
                        sbqry.Append(GenNewTransactionString());
                        //decimal tot_month = 0;
                        //foreach (DataRow dr in dts.Rows)
                        //{
                        if (!op_bals.Rows[0].IsNull("opbal"))
                        {
                            opbals = Convert.ToDecimal(op_bals.Rows[0]["opbal"]);
                            //own ,bank,vpf
                            own_opbals = Convert.ToDecimal(op_bals.Rows[0]["own"]);
                            bank_opbals = Convert.ToDecimal(op_bals.Rows[0]["bank"]);
                            vpf_opbals = Convert.ToDecimal(op_bals.Rows[0]["vpf"]);
                        }
                        else { opbals = 0; }
                        //total months count
                        if (!dts2.Rows[0].IsNull("totmonths"))
                        {
                            no_of_month = Convert.ToDecimal(dts2.Rows[0]["totmonths"]);
                        }
                        else { no_of_month = 0; }
                        foreach (DataRow dr1 in opbal_month.Rows)
                        {
                            //totalmonthdata += Convert.ToDecimal(dr1["monthdata"]);
                            if (!dr1.IsNull("own_share"))
                            {
                                totalmonthdata += Convert.ToDecimal(dr1["own_share"]);
                                won_totalmonthdata += Convert.ToDecimal(dr1["own_share"]);
                            }
                            else { totalmonthdata += 0; won_totalmonthdata += 0; }
                            if (!dr1.IsNull("bank_share"))
                            {
                                totalmonthdata += Convert.ToDecimal(dr1["bank_share"]);
                                bank_totalmonthdata += Convert.ToDecimal(dr1["bank_share"]);
                            }
                            else { totalmonthdata += 0; bank_totalmonthdata += 0; }
                            if (!dr1.IsNull("vpf"))
                            {
                                totalmonthdata += Convert.ToDecimal(dr1["vpf"]);
                                vpf_totalmonthdata += Convert.ToDecimal(dr1["vpf"]);
                            }
                            else { totalmonthdata += 0; vpf_totalmonthdata += 0; }
                            //totalmonthdata += opbals;
                            totmonthdata = totalmonthdata + opbals;
                            won_totmonthdata = won_totalmonthdata + own_opbals;
                            bank_totmonthdata = bank_totalmonthdata + bank_opbals;
                            vpf_totmonthdata = vpf_totalmonthdata + vpf_opbals;
                            
                            totmonth += totmonthdata;    
                            // won,bank,vpf indivedual inst cal
                            won_totmonth += won_totmonthdata;    
                            bank_totmonth += bank_totmonthdata;    
                            vpf_totmonth += vpf_totmonthdata;

                        }

                        //accured amount calculation
                        totop_month = opbals + totmonth; // totalmonthdata + opbals ;
                     
                        // own bank vpf 
                        won_totop_month = own_opbals + won_totmonth;                           
                        bank_totop_month = bank_opbals + bank_totmonth;
                        vpf_totop_month = vpf_opbals + vpf_totmonth;
                         
                        decimal op_bal_own = won_totop_month; //total_opbal + own_share ;
                        decimal op_bal_bank = bank_totop_month; //total_opbal + bank_share ;
                        decimal op_bal_vpf = vpf_totop_month; //total_opbal + vpf_share ;

                       decimal op_bal_inst_own = (op_bal_own) * interest / 100; //12 months
                        op_bal_inst_own = op_bal_inst_own / 12;
                        op_bal_inst_own = Math.Round(op_bal_inst_own, MidpointRounding.AwayFromZero);

                        decimal op_bal_inst_bank = (op_bal_bank) * interest / 100; //12 months
                        //decimal op_bal_inst_own = (op_bal_own) * interest / 100; //12 months
                        op_bal_inst_bank = op_bal_inst_bank / 12;
                        op_bal_inst_bank = Math.Round(op_bal_inst_bank, MidpointRounding.AwayFromZero);

                        decimal op_bal_inst_vpf = (op_bal_vpf) * interest / 100; //12 months
                        op_bal_inst_vpf = op_bal_inst_vpf / 12;
                        op_bal_inst_vpf = Math.Round(op_bal_inst_vpf, MidpointRounding.AwayFromZero);

                        decimal totown_bank_vpf = op_bal_inst_own + op_bal_inst_bank + op_bal_inst_vpf;
                        emp_code = dts.Rows[0]["emp_code"].ToString();
                        //string fm = Convert.ToDateTime(dts.Rows[0]["fm"]).ToString("yyy/MM/dd"); //dr["fm"].ToString();
                        //tot_month = Convert.ToDecimal(dts2.Rows[0]["totmonths"]);
                        ///interest = 8.85M;
                        decimal op_bal = totop_month; //total_opbal + own_share + bank_share + Vpf;
                        op_bal_inst = (op_bal) * interest / 100; //12 months
                        op_bal_inst = op_bal_inst / 12;
                        op_bal_inst = Math.Round(op_bal_inst, MidpointRounding.AwayFromZero);
                        f_month = dts.Rows[0]["fm"].ToString();
                        decimal getinsrt_opbals = 0;
                        decimal getinsrt_own_opbals = 0;decimal getinsrt_bank_opbals = 0;decimal getinsrt_vpf_opbals = 0;
                        if (!inst_op_bal.Rows[0].IsNull("inst_opbal"))
                        {
                            getinsrt_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["inst_opbal"]); 
                            //own bank vpf 
                            getinsrt_own_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["own_inst_opbal"]);
                            getinsrt_bank_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["bank_inst_opbal"]);
                            getinsrt_vpf_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["vpf_inst_opbal"]);
                        }
                        else
                        {
                            getinsrt_opbals = 0; 
                            getinsrt_own_opbals = 0;
                            getinsrt_bank_opbals = 0; getinsrt_vpf_opbals = 0;
                        }
                        //total 
                        getinsrt_opbals = getinsrt_opbals * 12;
                        decimal insrt_opbal =  getinsrt_opbals * interest / 100;

                        insrt_opbal = insrt_opbal / 12;
                        //insrt_opbal = Math.Round(insrt_opbal, MidpointRounding.AwayFromZero);
                        //month multiplication //un comment
                        insrt_opbal = insrt_opbal * no_of_month;
                        insrt_opbal = insrt_opbal / 12;
                        // own , bank,vpf
                        getinsrt_own_opbals = getinsrt_own_opbals * 12;
                        decimal insrt_own_opbal_year = getinsrt_own_opbals * interest / 100;

                        insrt_own_opbal_year = insrt_own_opbal_year / 12;                     
                        insrt_own_opbal_year = insrt_own_opbal_year * no_of_month;
                        insrt_own_opbal_year = insrt_own_opbal_year / 12;
                        //bank
                        getinsrt_bank_opbals = getinsrt_bank_opbals * 12;
                        decimal insrt_bank_opbal_year = getinsrt_bank_opbals * interest / 100;

                        insrt_bank_opbal_year = insrt_bank_opbal_year / 12;                        
                        insrt_bank_opbal_year = insrt_bank_opbal_year * no_of_month;
                        insrt_bank_opbal_year = insrt_bank_opbal_year / 12;
                        //vpf
                        getinsrt_vpf_opbals = getinsrt_vpf_opbals * 12;
                        decimal insrt_vpf_opbal_year = getinsrt_vpf_opbals * interest / 100;
             
                        insrt_vpf_opbal_year = insrt_vpf_opbal_year / 12;
                        insrt_vpf_opbal_year = insrt_vpf_opbal_year * no_of_month ;
                       insrt_vpf_opbal_year = insrt_vpf_opbal_year / 12;

                        decimal tot_getinsrt_opbals = insrt_own_opbal_year + insrt_bank_opbal_year + insrt_vpf_opbal_year;
                        
                        insrt_opbal = Math.Round(insrt_opbal, MidpointRounding.AwayFromZero);
                        ob_share += "update pr_pf_open_bal_year set op_bal_inst_year = '" + insrt_opbal + "' , op_bal_inst = '" + op_bal_inst + "' " +
                            " , RelievingDate ='"+ RelivingDate +"' , PFcaldate ='" + pfcaldate + "' " +
                            " ,op_bal_own_inst ='" + op_bal_inst_own + "' ,op_bal_bank_inst ='" + op_bal_inst_bank + "' ,op_bal_vpf_inst ='" + op_bal_inst_vpf + "' " +
                            " ,op_bal_inst_own_year ='" + insrt_own_opbal_year + "' ,op_bal_inst_bank_year ='" + insrt_bank_opbal_year + "'" +
                            " ,op_bal_inst_vpf_year ='" + insrt_vpf_opbal_year + "' " +
                            " where emp_code = " + empid + " and fy= (select fy-1 from pr_month_details where active=1) ; ";
                        // total op bal and inst op bal
                        decimal totop_inst = insrt_opbal + op_bal_inst;
                        // added by Sowjanya
                        string finacialyear = "select year(fm) as year,fy from pr_month_details where active=1";
                        DataTable finacalDT = await _sha.Get_Table_FromQry(finacialyear);
                        string finayear = finacalDT.Rows[0]["year"].ToString();
                        string nextfinayear = finacalDT.Rows[0]["fy"].ToString();
                        if (finayear == nextfinayear)
                        {
                            finayear = (Convert.ToInt32(finayear) - 1).ToString();
                        }

                        //decimal NRLoanCF = 00;
                        string previyearCF = "select pf_return as own_share, vpf_return as vpf, bank_return as bank_share from pr_pf_open_bal_year where emp_code = " + empid + " and fy = (select fy-1 from pr_month_details where active=1)";
                        DataTable NRLoanCF = await _sha.Get_Table_FromQry(previyearCF);
                                          
                        // added by indhu
                        // during year
                        string duringyearCF = "select total as nrtotal,own_share as nrown_share ,vpf as nrvpf,bank_share as nrbank_share from pr_emp_pf_nonrepayable_loan " +
                        "where emp_code = " + empid + " and sanction_date between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31)   order by process_date desc; ";
                        DataTable duringNRLoanCF = await _sha.Get_Table_FromQry(duringyearCF);
                        string nr_tot_amt = ""; decimal nrown_share =0;decimal nrvpf = 0;decimal nrbank_share = 0;
                        if (duringNRLoanCF.Rows.Count >0)
                        {
                             nr_tot_amt = duringNRLoanCF.Rows[0]["nrtotal"].ToString();
                            nrown_share =Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrown_share"].ToString());
                            nrvpf =Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrvpf"].ToString());
                            nrbank_share =Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrbank_share"].ToString());
                        }
                        else
                        {
                            nr_tot_amt = "0"; nrown_share = 0; nrvpf = 0; nrbank_share = 0;
                        }
                                                

                        //get during months
                        string duringmonths = "select top(1) datediff(month,sanction_date,(select Retirementdate from Employees where empid=" + empid + "))+1 as nrtotmonths from pr_emp_pf_nonrepayable_loan where emp_code = " + empid + " and sanction_date between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 )   order by process_date desc; ";
                        DataTable duringNRmonths = await _sha.Get_Table_FromQry(duringmonths);
                      
                        int tot_months = 0;
                        if (duringNRmonths.Rows.Count > 0)
                        {
                            tot_months = Convert.ToInt32(duringNRmonths.Rows[0]["nrtotmonths"]);//.ToString();
                            //tot_months = tot_months - 1;
                        }
                        else
                        {
                            tot_months = 0;
                        }
                        int own_cf = 0; int bank_cf = 0; int vpf_cf = 0; int cf = 0;
                        if (NRLoanCF.Rows.Count > 0)
                        {
                            // own ,bank,vpf
                            own_cf = Convert.ToInt32(NRLoanCF.Rows[0]["own_share"]);
                            bank_cf = Convert.ToInt32(NRLoanCF.Rows[0]["bank_share"]);
                            vpf_cf = Convert.ToInt32(NRLoanCF.Rows[0]["vpf"]);
                            cf = Convert.ToInt32(NRLoanCF.Rows[0]["own_share"]) + Convert.ToInt32(NRLoanCF.Rows[0]["bank_share"]) + Convert.ToInt32(NRLoanCF.Rows[0]["vpf"]);//, cfMonth = 1;

                        }
                        else
                        {
                            own_cf = 0;
                            bank_cf = 0;
                            vpf_cf = 0;
                            cf = own_cf+ bank_cf+ vpf_cf;
                        }
                        
                        //cfMonth = 1;


                        decimal tota1 = cf * no_of_month;
                        //added by indhu
                        //nr loan inst cal
                        decimal nrown_share_inst = nrown_share * Convert.ToDecimal(tot_months);
                        decimal nrown_share_inst1 = Math.Round(nrown_share_inst * (interest / 100) / 12, 2);

                        decimal nrvpf_inst = nrvpf * Convert.ToDecimal(tot_months);
                        decimal nrvpf_inst1 = Math.Round(nrvpf_inst * (interest / 100) / 12, 2);

                        decimal nrbank_share_inst = nrbank_share* Convert.ToDecimal(tot_months);
                        decimal nrbank_share_inst1 = Math.Round(nrbank_share_inst * (interest / 100) / 12, 2);

                        int nr_loan_inst =Convert.ToInt32(nr_tot_amt) * Convert.ToInt32(tot_months);
                        decimal nr_loan_inst_cal = Math.Round(nr_loan_inst * (interest / 100) / 12, 2);
                        decimal nr_second_loan_inst_cal = nr_loan_inst_cal;
                        //own,bank,vpf
                        decimal own_tota1 = own_cf * no_of_month; decimal bank_tota1 = bank_cf * no_of_month; 
                        decimal vpf_tota1 = vpf_cf * no_of_month;
                        //decimal tota1 = cf * cfMonth + q1cum * q1m + q2cum * q2m + q3cum * q3m + q4cum * q4m + q5cum * q5m + q6cum * q6m + q7cum * q7m + q8cum * q8m + q9cum * q9m + q10cum * q10m + q11cum * q11m;
                        decimal intrestNR = Math.Round(tota1 * (interest / 100) / 12, 2);
                        decimal nrinst_instnr = Math.Round(nr_loan_inst_cal + intrestNR);
                        //own,bank,vpf
                        decimal intrestNR_own = Math.Round(own_tota1 * (interest / 100) / 12, 2);
                        intrestNR_own = intrestNR_own + nrown_share_inst1;
                        decimal intrestNR_bank = Math.Round(bank_tota1 * (interest / 100) / 12, 2); intrestNR_bank = intrestNR_bank + nrbank_share_inst1;
                        decimal intrestNR_vpf = Math.Round(vpf_tota1 * (interest / 100) / 12, 2); intrestNR_vpf = intrestNR_vpf + nrvpf_inst1;
                        decimal tot_own_bank_vpf = intrestNR_own + intrestNR_bank + intrestNR_vpf;
                        intrestNR = Math.Round(intrestNR, MidpointRounding.AwayFromZero);
                        // total Intrest Acrued on PF Contribution
                        //nsrt_opbal ,op_bal_inst  ( add nsrt_opbal,op_bal_inst and substract nrinst_instnr)
                        decimal inst_Acrued_pf_con = insrt_opbal+ op_bal_inst - nrinst_instnr;
                        //intrestNR = totop_inst - intrestNR;
                        ob_share += "update pr_pf_open_bal_year set op_bal_inst_NRloan = '" + nrinst_instnr + "' " +
                            " ,op_bal_inst_nrloan_own = '" + intrestNR_own + "',op_bal_inst_nrloan_bank = '" + intrestNR_bank + "' " +
                            " ,op_bal_inst_nrloan_vpf = '" + intrestNR_vpf + "' ,inst_Acrued_pf_con = '" + inst_Acrued_pf_con + "'  " +
                            " where emp_code = " + empid + " and fy=(select fy-1 from pr_month_details where active=1) ; ";
                        //ob_share += "update pr_pf_open_bal_year set op_bal_inst_NRloan = '" + intrest + "' " +
                        //    " where emp_code = " + empid + " and fy=(select fy-1 from pr_month_details where active=1) ; ";

                        sbqry.Append(ob_share);

                        if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                        {
                            msg = "I# Interest Retirement# PF interest calculated Successfully.";
                        }
                        else
                        {
                            msg = "E# Interest Retirement# Interest Calculation is Not Done ,Please Re Calculate";
                        }

                    }
                    catch (Exception e)
                    {
                        msg = e.Message;
                    }
                }
            }

            // before RetirementDate
            else
            {
                TimeSpan remaindays =new TimeSpan();
                if (pfcaldate != null)
                {                    
                    DateTime pfcaldate1 = Convert.ToDateTime(pfcaldate);
                    string todate = pfcaldate1.ToString("yyyy-MM-dd");
                    string[] to_sa = todate.Split('-');
                    rtdate1 = to_sa[0]; //year
                    rtdate2 = to_sa[1]; //month
                    rtdate3 = to_sa[2];
                     remaindays = pfcaldate1 - RelivingDates;
                    int months = (int)(Math.Floor(((pfcaldate1 - RelivingDates).TotalDays / 30.4)));
                    string getmonth3 = "select fy from pr_month_details where active=1";
                    DataTable getmonths3 = await _sha.Get_Table_FromQry(getmonth3);
                    Eyear = getmonths3.Rows[0]["fy"].ToString();
                    Fyear = Convert.ToInt32(Eyear) - 1;
                }
                else
                {
                    if (RelivingDate != null)
                    {
                        DateTime pfcaldate1 = Convert.ToDateTime(pfcaldate);
                        string todate = pfcaldate1.ToString("yyyy-MM-dd");
                        string[] to_sa = todate.Split('-');
                        rtdate1 = to_sa[0]; //year
                        rtdate2 = to_sa[1]; //month
                        rtdate3 = to_sa[2];
                         remaindays = pfcaldate1 - RelivingDates;
                        int months = (int)(Math.Floor(((pfcaldate1 - RelivingDates).TotalDays / 30.4)));
                        string getmonth1 = "select fy from pr_month_details where active=1";
                        DataTable getmonths1 = await _sha.Get_Table_FromQry(getmonth1);
                        Eyear = getmonths1.Rows[0]["fy"].ToString();
                        Fyear = Convert.ToInt32(Eyear) - 1;
                    }
                    else
                    {
                        if (RetirementDate != null)
                            {
                            DateTime RetirementDates = Convert.ToDateTime(RetirementDate);
                            DateTime pfcaldate1 = Convert.ToDateTime(pfcaldate);
                            string todate = pfcaldate1.ToString("yyyy-MM-dd");
                            string[] to_sa = todate.Split('-');
                            rtdate1 = to_sa[0]; //year
                            rtdate2 = to_sa[1]; //month
                            rtdate3 = to_sa[2];
                             remaindays = pfcaldate1 - RetirementDates;
                            int months = (int)(Math.Floor(((pfcaldate1 - RelivingDates).TotalDays / 30.4)));
                            string getmonth2 = "select fy from pr_month_details where active=1";
                            DataTable getmonths2 = await _sha.Get_Table_FromQry(getmonth2);
                            Eyear = getmonths2.Rows[0]["fy"].ToString();
                            Fyear = Convert.ToInt32(Eyear) - 1;
                        }
                    }
                }
                //ob share data  
                //string qry = "select emp_code,fm,own_share,bank_share,vpf from pr_ob_share where emp_code=" + empid + " and fy=(select fy from pr_month_details where active=1) order by fm asc";
                string qry = "select fm,emp_code,ISNULL(own_share_intrst_open,0) as own_share_intrst_open,ISNULL(bank_share_intrst_open,0) as bank_share_intrst_open,ISNULL(vpf_intrst_open,0) as vpf_intrst_open from pr_ob_share ob " +
     " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and " +
     "  DATEFROMPARTS(" + rtdate1 + ", " + rtdate2 + ", " + rtdate3 + " ) and " +
     " ob.emp_code = " + empid + " ";
                DataTable dts = await _sha.Get_Table_FromQry(qry);

                string opbal = "select os_open as own ,vpf_open as vpf,bs_open as bank, os_open+vpf_open + bs_open as opbal from pr_pf_open_bal_year where emp_code = " + empid + " and fy = " + Fyear + " ";
                DataTable op_bals = await _sha.Get_Table_FromQry(opbal);
                string oldmonth = "select top 1 fm from pr_ob_share where  emp_code = " + empid + " and fy =" + Eyear + " order by fm desc";
                DataTable old_month = await _sha.Get_Table_FromQry(oldmonth);
                string fm = old_month.Rows[0]["fm"].ToString(); //lastmontnh
              
                string opbalmonth = "select distinct h.fm1,h.own_share + ( case when(select count(c1.own_share) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.own_share from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+ ( case when(select count(c1.own_share) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.own_share) ELSE 0 END from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) own_share,h.bank_share + ( case when(select count(c1.bank_share) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.bank_share from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end)+( case when(select count(c1.bank_share) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.bank_share) ELSE 0 END from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) bank_share,h.vpf + ( case when(select count(c1.vpf) from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select c1.vpf from pr_ob_share_encashment c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) +( case when(select count(c1.vpf) from pr_ob_share_adhoc c1 where c1.emp_code = h.emp_code and h.fm1 = c1.fm) = 0 then 0 else (select CASE WHEN COUNT(1) > 0 THEN sum(c1.vpf) ELSE 0 END from pr_ob_share_adhoc c1 " +
                    "where c1.emp_code = h.emp_code and h.fm1 = c1.fm) end) vpf " +

     " from(select ob.fm as fm1, ob.emp_code, e.ShortName, pay.pf_no, pay.uan_no, REPLACE(RIGHT(CONVERT(VARCHAR(11), ob.fm, 106), 8), ' ', '-') as fm, own_share, bank_share, vpf, own_share_total, bank_share_total, vpf_total, pfbal.os_open AS ownbal, pfbal.bs_open as bankbal, pfbal.vpf_open as vpfbal, pfbal.os_open_int, pfbal.bs_open_int, pfbal.vpf_open_int, active = 1, pfbal.pf_return, pfbal.vpf_return, pfbal.bank_return, pfbal.os_cur_int, pfbal.vpf_cur_int, pfbal.bs_cur_int, pfbal.op_bal_inst, pfbal.op_bal_inst_year, pfbal.op_bal_inst_NRloan from pr_ob_share ob join pr_emp_general pay  on ob.emp_code = pay.emp_code join Employees e on e.EmpId = ob.emp_code JOIN pr_pf_open_bal_year pfbal on pfbal.emp_code = e.EmpId " +
     "where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31) " +
     //"and ob.fm < '" + fm + "' " +
     "and ob.emp_code in (" + empid + " ) and pay.active = 1 and pfbal.fy = " + Fyear + " ) as h ";
                DataTable opbal_month = await _sha.Get_Table_FromQry(opbalmonth);
                //ob bal 
                string inst_opbal = "select bs_open_int+os_open_int+vpf_open_int as inst_opbal , " +
                     "bs_open_int as bank_inst_opbal,os_open_int as own_inst_opbal,vpf_open_int as vpf_inst_opbal, " +
                    " fy,pfbal.os_open AS ownbal, pfbal.bs_open as bankbal,pfbal.vpf_open as vpfbal from pr_pf_open_bal_year pfbal " +
                       "where emp_code=" + empid + " and fy=" + Fyear + " ";
                DataTable inst_op_bal = await _sha.Get_Table_FromQry(inst_opbal);
                string qry2 = "select count(own_share) as totmonths from pr_ob_share ob " +
                    " where ob.fm between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and " +
                    " ob.emp_code = " + empid + " ";
                DataTable dts2 = await _sha.Get_Table_FromQry(qry2);

                StringBuilder sbqry = new StringBuilder();
                decimal op_bal_inst = 0; decimal totalmonthdata = 0; decimal no_of_month = 0; decimal opbals = 0; decimal totop_month = 0;
                decimal totamt = 0; string f_month = ""; decimal totmonthdata = 0; decimal totalmonthdata1 = 0; decimal totmonth = 0;

                decimal won_totalmonthdata = 0; decimal bank_totalmonthdata = 0; decimal vpf_totalmonthdata = 0;
                decimal won_totmonthdata = 0; decimal bank_totmonthdata = 0; decimal vpf_totmonthdata = 0;
                decimal won_totmonth = 0; decimal bank_totmonth = 0; decimal vpf_totmonth = 0;
                decimal won_totop_month = 0; decimal bank_totop_month = 0; decimal vpf_totop_month = 0;
                decimal own_opbals = 0; decimal vpf_opbals = 0; decimal bank_opbals = 0;
                try
                {
                    //1. trans_id
                    sbqry.Append(GenNewTransactionString());
                    //decimal tot_month = 0;
                    //foreach (DataRow dr in dts.Rows)
                    //{
                    if (!op_bals.Rows[0].IsNull("opbal"))
                    {
                        opbals = Convert.ToDecimal(op_bals.Rows[0]["opbal"]);
                        //own ,bank,vpf
                        own_opbals = Convert.ToDecimal(op_bals.Rows[0]["own"]);
                        bank_opbals = Convert.ToDecimal(op_bals.Rows[0]["bank"]);
                        vpf_opbals = Convert.ToDecimal(op_bals.Rows[0]["vpf"]);
                    }
                    else { opbals = 0; }
                    //total months count
                    if (!dts2.Rows[0].IsNull("totmonths"))
                    {
                        no_of_month = Convert.ToDecimal(dts2.Rows[0]["totmonths"]);
                    }
                    else { no_of_month = 0; }
                    foreach (DataRow dr1 in opbal_month.Rows)
                    {
                        //totalmonthdata += Convert.ToDecimal(dr1["monthdata"]);
                        if (!dr1.IsNull("own_share"))
                        {
                            totalmonthdata += Math.Round(Convert.ToDecimal(dr1["own_share"]));
                            won_totalmonthdata += Convert.ToDecimal(dr1["own_share"]);
                        }
                        else { totalmonthdata += 0; won_totalmonthdata += 0; }
                        if (!dr1.IsNull("bank_share"))
                        {
                            totalmonthdata += Math.Round(Convert.ToDecimal(dr1["bank_share"]));
                            bank_totalmonthdata += Convert.ToDecimal(dr1["bank_share"]);
                        }
                        else { totalmonthdata += 0; bank_totalmonthdata += 0; }
                        if (!dr1.IsNull("vpf"))
                        {
                            totalmonthdata += Convert.ToDecimal(Convert.ToDecimal(dr1["vpf"]));
                            vpf_totalmonthdata += Convert.ToDecimal(dr1["vpf"]);
                        }
                        else { totalmonthdata += 0; vpf_totalmonthdata += 0; }
                        //totalmonthdata += opbals;
                        totmonthdata = totalmonthdata + opbals;

                        won_totmonthdata = won_totalmonthdata + own_opbals;
                        bank_totmonthdata = bank_totalmonthdata + bank_opbals;
                        vpf_totmonthdata = vpf_totalmonthdata + vpf_opbals;

                        totmonth += totmonthdata;

                        // won,bank,vpf indivedual inst cal
                        won_totmonth += won_totmonthdata;
                        bank_totmonth += bank_totmonthdata;
                        vpf_totmonth += vpf_totmonthdata;
                    }
                    decimal own_totalmonthdata1 = 0; decimal bank_totalmonthdata1 = 0;decimal vpf_totalmonthdata1 = 0;
                    decimal own_tot_totalmonthdata1 = 0; decimal bank_tot_totalmonthdata1 = 0; decimal vpf_tot_totalmonthdata1 = 0;
                    decimal op_bal_own1 = 0; decimal op_bal_bank1 = 0; decimal op_bal_vpf1 = 0;
                    //get last month data 
                    foreach (DataRow dr2 in opbal_month.Rows)
                    {
                        if (!dr2.IsNull("own_share"))
                        {
                            totalmonthdata1 += Math.Round(Convert.ToDecimal(dr2["own_share"]));
                            own_totalmonthdata1 += Math.Round(Convert.ToDecimal(dr2["own_share"]));
                            won_totop_month = Math.Round(Convert.ToDecimal(dr2["own_share"]));
                        }
                        else { totalmonthdata1 += 0; own_totalmonthdata1 = +0; }
                        if (!dr2.IsNull("bank_share")) 
                        {
                            totalmonthdata1 += Math.Round(Convert.ToDecimal(dr2["bank_share"]));
                            bank_totalmonthdata1 += Math.Round(Convert.ToDecimal(dr2["bank_share"]));
                            bank_totop_month = Math.Round(Convert.ToDecimal(dr2["bank_share"]));
                        }
                        else { totalmonthdata1 += 0; bank_totalmonthdata1 += 0; }
                        if (!dr2.IsNull("vpf"))
                        {
                            totalmonthdata1 += Convert.ToDecimal(Convert.ToDecimal(dr2["vpf"]));
                            vpf_totalmonthdata1 += Convert.ToDecimal(Convert.ToDecimal(dr2["vpf"]));
                            vpf_totop_month = Math.Round(Convert.ToDecimal(dr2["vpf"]));
                        }
                        else { totalmonthdata1 += 0; vpf_totalmonthdata1 += 0; }

                        //totalmonthdata += opbals;
                        totamt = totalmonthdata1 + opbals;
                        own_tot_totalmonthdata1 = own_totalmonthdata1 + own_opbals;
                        bank_tot_totalmonthdata1 = bank_totalmonthdata1 + bank_opbals;
                        vpf_tot_totalmonthdata1 = vpf_totalmonthdata1 + vpf_opbals;
                        op_bal_own1 = won_totop_month;
                        op_bal_bank1 = bank_totop_month;
                        op_bal_vpf1 = vpf_totop_month;
                    }
                    //totamt += totalmonthdata1+ opbals;
                    //interest cal for extra months                                     
                    totop_month = totmonth + opbals;

                    // own bank vpf 
                    won_totop_month = own_opbals + won_totmonth;
                    bank_totop_month = bank_opbals + bank_totmonth;
                    vpf_totop_month = vpf_opbals + vpf_totmonth;

                    //decimal op_bal_own1 = won_totop_month; //total_opbal + own_share ;
                    //decimal op_bal_bank1 = bank_totop_month; //total_opbal + bank_share ;
                    //decimal op_bal_vpf1 = vpf_totop_month; //total_opbal + vpf_share ;

                

                    emp_code = dts.Rows[0]["emp_code"].ToString();
                    // own,bank,vpf , op bal
                    ///interest = 8.85M;
                    
                    // late  settilment                    
                    DateTime pfcaldate2 = Convert.ToDateTime(pfcaldate);
                    DateTime RetirementDates = Convert.ToDateTime(lastmontnh);
                    int totmonts = 0;
                    int interest_months = (int)(Math.Floor(((pfcaldate2 - RetirementDates).TotalDays / 30)));
                    decimal pfsetmtamt =0; decimal pf_own_setmtamt = 0; decimal pf_banksetmtamt = 0; decimal pf_vpfsetmtamt = 0;
                    if (pfdateY == lastmontnhY && pfdateM == lastmontnhM)
                    {
                        //totmonts = interest_months + Convert.ToInt32(no_of_month);
                        totmonts = interest_months;
                        pfsetmtamt = totop_month;
                    }
                    else
                    {
                        //totmonts = interest_months + Convert.ToInt32(no_of_month);
                        totmonts = interest_months - 1;
                        
                        if(totmonts ==1)
                        {
                            pfsetmtamt += totop_month + totamt;

                            pf_own_setmtamt += won_totop_month + own_tot_totalmonthdata1;
                            pf_banksetmtamt += bank_totop_month + bank_tot_totalmonthdata1;
                            pf_vpfsetmtamt += vpf_totop_month + vpf_tot_totalmonthdata1;
                        }
                        else
                        {
                            decimal pfamtsett=  totamt * totmonts;
                            pfsetmtamt += totop_month + pfamtsett;

                            //
                            decimal pf_own_amtsett = own_tot_totalmonthdata1 * totmonts;
                            pf_own_setmtamt += won_totop_month + pf_own_amtsett;
                            
                            decimal pf_bank_amtsett = bank_tot_totalmonthdata1 * totmonts;
                            pf_banksetmtamt += bank_totop_month + pf_bank_amtsett; 

                            decimal pf_vpf_amtsett = vpf_tot_totalmonthdata1 * totmonts;
                            pf_vpfsetmtamt += vpf_totop_month + pf_vpf_amtsett;
                        }
                    }
                    decimal pfsetmt = 0; decimal pf_own_setmt = 0; decimal pf_bank_setmt = 0; decimal pf_vpf_setmt = 0;
                    pfsetmt = pfsetmtamt;//totop_month * totmonts;
                    pf_own_setmt = pf_own_setmtamt; pf_bank_setmt = pf_banksetmtamt; pf_vpf_setmt = pf_vpfsetmtamt;
                    //interest amount and extra amount  calculation
                    decimal op_bal = 0; decimal op_bal_own = 0; decimal op_bal_bank = 0; decimal op_bal_vpf = 0;
                    op_bal = pfsetmt; //total_opbal + own_share + bank_share + Vpf;

                    op_bal_own = pf_own_setmt; op_bal_bank = pf_bank_setmt; op_bal_vpf = pf_vpf_setmt;

                    op_bal_inst = Math.Round((op_bal) * interest / 100); //12 months
                    op_bal_inst = Math.Round(op_bal_inst / 12, 2);

                    // 
                    decimal op_bal_inst_own = (op_bal_own) * interest / 100; //12 months
                    op_bal_inst_own = op_bal_inst_own / 12;
                    op_bal_inst_own = Math.Round(op_bal_inst_own, MidpointRounding.AwayFromZero);

                    decimal op_bal_inst_bank = (op_bal_bank) * interest / 100; //12 months
                                                                               //decimal op_bal_inst_own = (op_bal_own) * interest / 100; //12 months
                    op_bal_inst_bank = op_bal_inst_bank / 12;
                    op_bal_inst_bank = Math.Round(op_bal_inst_bank, MidpointRounding.AwayFromZero);

                    decimal op_bal_inst_vpf = (op_bal_vpf) * interest / 100; //12 months
                    op_bal_inst_vpf = op_bal_inst_vpf / 12;
                    op_bal_inst_vpf = Math.Round(op_bal_inst_vpf, MidpointRounding.AwayFromZero);

                    decimal totown_bank_vpf = op_bal_inst_own + op_bal_inst_bank + op_bal_inst_vpf;
                    //inst op bal
                    f_month = dts.Rows[0]["fm"].ToString();
                    decimal getinsrt_opbals = 0;
                    decimal getinsrt_own_opbals = 0; decimal getinsrt_bank_opbals = 0; decimal getinsrt_vpf_opbals = 0;

                    if (!inst_op_bal.Rows[0].IsNull("inst_opbal"))
                    {
                        getinsrt_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["inst_opbal"]);
                        //own bank vpf 
                        getinsrt_own_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["own_inst_opbal"]);
                        getinsrt_bank_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["bank_inst_opbal"]);
                        getinsrt_vpf_opbals = Convert.ToDecimal(inst_op_bal.Rows[0]["vpf_inst_opbal"]);
                    }
                    else
                    {
                        getinsrt_opbals = 0;
                        getinsrt_own_opbals = 0;
                        getinsrt_bank_opbals = 0; getinsrt_vpf_opbals = 0;
                    }
                                        
                    //if (RetirementDates != pfcaldate2) {
                    
                    //no_of_month =Convert.ToDecimal(totmonts);
                    getinsrt_opbals = getinsrt_opbals * 12;
                    decimal insrt_opbal = getinsrt_opbals * interest / 100;
                    insrt_opbal = Math.Round(insrt_opbal / 12, 2);
                    insrt_opbal = insrt_opbal * Convert.ToDecimal(no_of_month) / 12;
                    insrt_opbal = Math.Round(insrt_opbal, MidpointRounding.AwayFromZero);

                    // own , bank,vpf
                    getinsrt_own_opbals = getinsrt_own_opbals * 12;
                    decimal insrt_own_opbal_year = getinsrt_own_opbals * interest / 100;

                    insrt_own_opbal_year = insrt_own_opbal_year / 12;
                    insrt_own_opbal_year = insrt_own_opbal_year * no_of_month;
                    insrt_own_opbal_year = insrt_own_opbal_year / 12;
                    //bank
                    getinsrt_bank_opbals = getinsrt_bank_opbals * 12;
                    decimal insrt_bank_opbal_year = getinsrt_bank_opbals * interest / 100;

                    insrt_bank_opbal_year = insrt_bank_opbal_year / 12;
                    insrt_bank_opbal_year = insrt_bank_opbal_year * no_of_month;
                    insrt_bank_opbal_year = insrt_bank_opbal_year / 12;
                    //vpf
                    getinsrt_vpf_opbals = getinsrt_vpf_opbals * 12;
                    decimal insrt_vpf_opbal_year = getinsrt_vpf_opbals * interest / 100;

                    insrt_vpf_opbal_year = insrt_vpf_opbal_year / 12;
                    insrt_vpf_opbal_year = insrt_vpf_opbal_year * no_of_month;
                    insrt_vpf_opbal_year = insrt_vpf_opbal_year / 12;

                    decimal tot_getinsrt_opbals = insrt_own_opbal_year + insrt_bank_opbal_year + insrt_vpf_opbal_year;

                    //ob_share += "update pr_pf_open_bal_year set op_bal_inst_year = '" + insrt_opbal + "' , op_bal_inst = '" + op_bal_inst + "' " +
                    //    " , RelievingDate ='" + RelivingDate + "' , PFcaldate ='" + pfcaldate + "' " +
                    //    " where emp_code = " + empid + " and fy= (select fy-1 from pr_month_details where active=1) ; ";
                    ob_share += "update pr_pf_open_bal_year set op_bal_inst_year = '" + insrt_opbal + "' , op_bal_inst = '" + op_bal_inst + "' " +
                           " , RelievingDate ='" + RelivingDate + "' , PFcaldate ='" + pfcaldate + "' " +
                           " ,op_bal_own_inst ='" + op_bal_inst_own + "' ,op_bal_bank_inst ='" + op_bal_inst_bank + "' ,op_bal_vpf_inst ='" + op_bal_inst_vpf + "' " +
                           " ,op_bal_inst_own_year ='" + insrt_own_opbal_year + "' ,op_bal_inst_bank_year ='" + insrt_bank_opbal_year + "'" +
                           " ,op_bal_inst_vpf_year ='" + insrt_vpf_opbal_year + "' " +
                           " where emp_code = " + empid + " and fy= (select fy-1 from pr_month_details where active=1) ; ";

                    // total op bal and inst op bal
                    decimal totop_inst = insrt_opbal + op_bal_inst;
                    // added by Sowjanya
                    string finacialyear = "select year(fm) as year,fy from pr_month_details where active=1";
                    DataTable finacalDT = await _sha.Get_Table_FromQry(finacialyear);
                    string finayear = finacalDT.Rows[0]["year"].ToString();
                    string nextfinayear = finacalDT.Rows[0]["fy"].ToString();
                    if (finayear == nextfinayear)
                    {
                        finayear = (Convert.ToInt32(finayear) - 1).ToString();
                    }

                    //decimal NRLoanCF = 00;
                    string previyearCF = "select pf_return as own_share, vpf_return as vpf, bank_return as bank_share from pr_pf_open_bal_year where emp_code = " + empid + " and fy = (select fy-1 from pr_month_details where active=1)";
                    DataTable NRLoanCF = await _sha.Get_Table_FromQry(previyearCF);

                    // added by indhu
                    // during year                   
                    string duringyearCF = "select total as nrtotal,own_share as nrown_share ,vpf as nrvpf,bank_share as nrbank_share from pr_emp_pf_nonrepayable_loan " +
                        "where emp_code = " + empid + " and sanction_date between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31)   order by process_date desc; ";
                    DataTable duringNRLoanCF = await _sha.Get_Table_FromQry(duringyearCF);
                    string nr_tot_amt = ""; decimal nrown_share = 0; decimal nrvpf = 0; decimal nrbank_share = 0;
                    if (duringNRLoanCF.Rows.Count > 0)
                    {
                        nr_tot_amt = duringNRLoanCF.Rows[0]["nrtotal"].ToString();
                        nrown_share = Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrown_share"].ToString());
                        nrvpf = Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrvpf"].ToString());
                        nrbank_share = Convert.ToDecimal(duringNRLoanCF.Rows[0]["nrbank_share"].ToString());
                    }
                    else
                    {
                        nr_tot_amt = "0"; nrown_share = 0; nrvpf = 0; nrbank_share = 0;
                    }
                    //get during months
                    string duringmonths = "select top(1) datediff(month,sanction_date,(select Retirementdate from Employees where empid= " + empid + "))+1 as nrtotmonths from pr_emp_pf_nonrepayable_loan where emp_code =  " + empid + " and sanction_date between DATEFROMPARTS(" + Fyear + ", 04, 01)  and DATEFROMPARTS(" + Eyear + ", 03, 31 )   order by process_date desc; ";
                    DataTable duringNRmonths = await _sha.Get_Table_FromQry(duringmonths);
                    //string tot_months = duringNRmonths.Rows[0]["nrtotmonths"].ToString();
                    string tot_months = "";
                    if (duringNRmonths.Rows.Count > 0)
                    {
                        tot_months = duringNRmonths.Rows[0]["nrtotmonths"].ToString();
                    }
                    else
                    {
                        tot_months = "0";
                    }
                                   

                    int own_cf = 0; int bank_cf = 0; int vpf_cf = 0; int cf = 0;
                    if (NRLoanCF.Rows.Count > 0)
                    {
                        // own ,bank,vpf
                        own_cf = Convert.ToInt32(NRLoanCF.Rows[0]["own_share"]);
                        bank_cf = Convert.ToInt32(NRLoanCF.Rows[0]["bank_share"]);
                        vpf_cf = Convert.ToInt32(NRLoanCF.Rows[0]["vpf"]);
                        cf = Convert.ToInt32(NRLoanCF.Rows[0]["own_share"]) + Convert.ToInt32(NRLoanCF.Rows[0]["bank_share"]) + Convert.ToInt32(NRLoanCF.Rows[0]["vpf"]);//, cfMonth = 1;

                    }
                    else
                    {
                        own_cf = 0;
                        bank_cf = 0;
                        vpf_cf = 0;
                        cf = own_cf + bank_cf + vpf_cf;
                    }
                                       
                    decimal tota1 = cf * no_of_month;

                    //added by indhu
                    //nr loan inst cal
                    decimal nrown_share_inst = nrown_share * Convert.ToDecimal(tot_months);
                    decimal nrown_share_inst1 = Math.Round(nrown_share_inst * (interest / 100) / 12, 2);

                    decimal nrvpf_inst = nrvpf * Convert.ToDecimal(tot_months);
                    decimal nrvpf_inst1 = Math.Round(nrvpf_inst * (interest / 100) / 12, 2);

                    decimal nrbank_share_inst = nrbank_share * Convert.ToDecimal(tot_months);
                    decimal nrbank_share_inst1 = Math.Round(nrbank_share_inst * (interest / 100) / 12, 2);

                    int nr_loan_inst = Convert.ToInt32(nr_tot_amt) * Convert.ToInt32(tot_months);
                    decimal nr_loan_inst_cal = Math.Round(nr_loan_inst * (interest / 100) / 12, 2);
                    decimal nr_second_loan_inst_cal = nr_loan_inst_cal;
                    //own,bank,vpf
                    decimal own_tota1 = own_cf * no_of_month; decimal bank_tota1 = bank_cf * no_of_month;
                    decimal vpf_tota1 = vpf_cf * no_of_month;
                    //decimal tota1 = cf * cfMonth + q1cum * q1m + q2cum * q2m + q3cum * q3m + q4cum * q4m + q5cum * q5m + q6cum * q6m + q7cum * q7m + q8cum * q8m + q9cum * q9m + q10cum * q10m + q11cum * q11m;
                    decimal intrestNR = Math.Round(tota1 * (interest / 100) / 12, 2);
                    // add two loans
                    decimal nrinst_instnr = Math.Round(nr_loan_inst_cal + intrestNR);
                    //own,bank,vpf
                    decimal intrestNR_own = Math.Round(own_tota1 * (interest / 100) / 12, 2); intrestNR_own = intrestNR_own + nrown_share_inst1;
                    decimal intrestNR_bank = Math.Round(bank_tota1 * (interest / 100) / 12, 2); intrestNR_bank = intrestNR_bank + nrbank_share_inst1;
                    decimal intrestNR_vpf = Math.Round(vpf_tota1 * (interest / 100) / 12, 2); intrestNR_vpf = intrestNR_vpf + nrvpf_inst1;
                    decimal tot_own_bank_vpf = intrestNR_own + intrestNR_bank + intrestNR_vpf;
                    intrestNR = Math.Round(intrestNR, MidpointRounding.AwayFromZero);

                    //decimal tota1 = cf * cfMonth + q1cum * q1m + q2cum * q2m + q3cum * q3m + q4cum * q4m + q5cum * q5m + q6cum * q6m + q7cum * q7m + q8cum * q8m + q9cum * q9m + q10cum * q10m + q11cum * q11m;
                    // decimal intrestNR = Math.Round(tota1 * (interest / 100) / 12, 2);
                    // total Intrest Acrued on PF Contribution
                    //nsrt_opbal ,op_bal_inst  ( add nsrt_opbal,op_bal_inst and substract nrinst_instnr)
                    decimal inst_Acrued_pf_con = insrt_opbal + op_bal_inst - nrinst_instnr;
                    //intrestNR = totop_inst - intrestNR;=
                    ob_share += "update pr_pf_open_bal_year set op_bal_inst_NRloan = '" + nrinst_instnr + "' " +
                          " ,op_bal_inst_nrloan_own = '" + intrestNR_own + "',op_bal_inst_nrloan_bank = '" + intrestNR_bank + "' " +
                            " ,op_bal_inst_nrloan_vpf = '" + intrestNR_vpf + "' ,inst_Acrued_pf_con = '" + inst_Acrued_pf_con + "' " +
                        " where emp_code = " + empid + " and fy=(select fy-1 from pr_month_details where active=1) ; ";
                    //ob_share += "update pr_pf_open_bal_year set op_bal_inst_NRloan = '" + intrest + "' " +
                    //    " where emp_code = " + empid + " and fy=(select fy-1 from pr_month_details where active=1) ; ";

                    sbqry.Append(ob_share);

                    if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
                    {
                        msg = "I# Interest Retirement# PF interest calculated Successfully.";
                    }
                    else
                    {
                        msg = "E# Interest Retirement# Interest Calculation is Not Done ,Please Re Calculate";
                    }

                }
                catch (Exception e)
                {
                    msg = e.Message;
                }
            }

            return msg;
        }
        #endregion
    }
}
