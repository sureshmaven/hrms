using System;
using System.Collections.Generic;
using Mavensoft.DAL.Business;
using System.Text;
using System.Threading.Tasks;
using PayrollModels;
using System.Data;
using System.Linq;
using Mavensoft.Common;

namespace PayRollBusiness.Reports
{
   public class JAIIB_CAIIB_Business : BusinessBase
    {
        public JAIIB_CAIIB_Business(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        Helper helper = new Helper();
        //drop down financial year eg:2019-2020
        public async Task<IList<LICReport>> getFy()
        {
            string qryfy = "select fy as fm_fy from pr_month_details where active=1;";
            int fm_fy = await _sha.Run_INS_ExecuteScalar(qryfy);

            IList<LICReport> fyear = new List<LICReport>();

            int fy = fm_fy - 1;
            int Id = 0;

            fyear.Add(new LICReport
            {
                Id = Id.ToString(),
                fY = "Select",
                id= "0",
                Name="All"

            });

            Id++;
            fyear.Add(new LICReport
            {

                Id = Id.ToString(),
                fY = (fy + "-" + (fm_fy)).ToString(),
                id = "1",
                Name = "JAIIB"
            });

            for (int i = 1; i < 6; i++)
            {
                Id++;
                fm_fy--;
                fy--;
                fyear.Add(new LICReport
                {
                    Id = Id.ToString(),
                    fY = (fy + "-" + (fm_fy)).ToString(),

                });
                if (i == 5)
                {
                    fyear.Add(new LICReport
                    {
                        id = "2",
                        Name = "CAIIB"

                    });
                }
            }

            return fyear;

        }
        //IList<CommonReportModel> lst = new List<CommonReportModel>();
        //public async Task<IList<LICReport>> getIncType()
        //{
        //    IList<LICReport> IncType = new List<LICReport>();

        //    IncType.Add(new LICReport
        //    {
        //        Id = "1",
        //        fY = "Select",

        //    });
        //    IncType.Add(new LICReport
        //    {
        //        Id = "1",
        //        fY = "JAIIB",

        //    });
        //    IncType.Add(new LICReport
        //    {
        //        Id = "1",
        //        fY = "CAIIB",

        //    });
        //}

        public async Task<DataTable> GetJAIIB_CAIIB_Data(string empcode, string fYear, string incType)
        {
            string Qry1 = "";
            string cond = "group by emp_code,ShortName,Description,basic_before_inc,incr_incen_type,Incrementamt,incr_incen_type,incr_WEF_date,incrementdate,authorisation";
     


            int Eyear = 0001;
            int Fyear = 0001;
            if (empcode == "dash" && fYear == "dyear")
            {
                Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date, 'dd-MM-yyyy') as incr_WEF_date,  CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation where g.authorisation = 0 group by emp_code,ShortName,Description,basic_before_inc,incr_incen_type,Incrementamt,incr_incen_type,incr_WEF_date,incrementdate,authorisation";
            }
            else
            {
                if (fYear != null && fYear != "^2")
                {
                    string[] split1 = fYear.Split('-');
                    //Console.Write(split[0], split[1]);
                    Eyear = int.Parse(split1[1]);
                    Fyear = int.Parse(split1[1]) - 1;
                }
                if (empcode.Contains("^"))
                {
                    empcode = "0";
                    fYear = "1900";
                }


                if ((empcode == "All" || empcode == "0") && (incType == null || incType.Contains("0")))
                {
                    Qry1 = "  select emp_code,E.ShortName,D.Description,concat(cast(basic_before_inc as nvarchar), '.00') as basic_before_inc," +
                        "incr_incen_type,concat(cast(Incrementamt as nvarchar), '.00') as Incrementamt," +
                        "concat(cast(Sum(basic_before_inc+Incrementamt) as nvarchar), '.00') as Basic_After_Inc," +
                        "format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                        " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                        " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                        " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" + cond;

                   // Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                   //" CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                   //" from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                   //" where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" + cond;

                }
                else if (empcode == "All" && incType != "0")
                {
                    if (incType == "1,2")
                    {
                        Qry1 = "    select emp_code,E.ShortName,D.Description,concat(cast(basic_before_inc as nvarchar), '.00') as basic_before_inc," +
                        "incr_incen_type,concat(cast(Incrementamt as nvarchar), '.00') as Incrementamt," +
                        "concat(cast(Sum(basic_before_inc+Incrementamt) as nvarchar), '.00') as Basic_After_Inc," +
                            "format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                  " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                  " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                  " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" + cond;

                        //Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                        //" CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                        //" from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                        //" where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )" + cond;
                    }
                    if (incType == "1")
                    {
                        Qry1 = "    select emp_code,E.ShortName,D.Description,concat(cast(basic_before_inc as nvarchar), '.00') as basic_before_inc," +
                        "incr_incen_type,concat(cast(Incrementamt as nvarchar), '.00') as Incrementamt," +
                        "concat(cast(Sum(basic_before_inc+Incrementamt) as nvarchar), '.00') as Basic_After_Inc," + 
                        "format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                    " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                    " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                    " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and incr_incen_type='JAIIB'" + cond;

                        //Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                        //" CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                        //" from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                        //" where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and incr_incen_type='JAIIB'" + cond;
                    }

                    if (incType == "2")
                    {
                        Qry1 = "    select emp_code,E.ShortName,D.Description,concat(cast(basic_before_inc as nvarchar), '.00') as basic_before_inc," +
                        "incr_incen_type,concat(cast(Incrementamt as nvarchar), '.00') as Incrementamt," +
                        "concat(cast(Sum(basic_before_inc+Incrementamt) as nvarchar), '.00') as Basic_After_Inc," +
                            "format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                   " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                   " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                   " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and incr_incen_type='CAIIB'" + cond;

                        //Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                        //" CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                        //" from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                        //" where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 ) and incr_incen_type='CAIIB'" + cond;
                    }
                }
                else
                {

                    Qry1 = "    select emp_code,E.ShortName,D.Description,concat(cast(basic_before_inc as nvarchar), '.00') as basic_before_inc," +
                        "incr_incen_type,concat(cast(Incrementamt as nvarchar), '.00') as Incrementamt," +
                        "concat(cast(Sum(basic_before_inc+Incrementamt) as nvarchar), '.00') as Basic_After_Inc," +
                        "format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                        " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                        " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                        " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )";

                    //Qry1 = "  select emp_code,E.ShortName,D.Description,basic_before_inc,incr_incen_type,Incrementamt,Sum(basic_before_inc+Incrementamt) as Basic_After_Inc,format(incrementdate, 'dd-MM-yyyy') as incrementdate,format(incr_WEF_date,'dd-MM-yyyy') as incr_WEF_date, " +
                    //    " CASE WHEN G.authorisation = 1 THEN 'Authorised' else 'Unauthorised'   end as status" +
                    //    " from pr_emp_jaib_caib_general G join Employees E on G.emp_code = E.EmpId join Designations D on D.Id = E.CurrentDesignation" +
                    //    " where fm between DATEFROMPARTS(" + Fyear + ", 04, 01 ) and DATEFROMPARTS(" + Eyear + ", 03, 31 )";
                    if (incType == "1")
                    {
                        Qry1 += " and incr_incen_type='JAIIB'";
                    }

                    if (incType == "2")
                    {
                        Qry1 += " and incr_incen_type='CAIIB'";
                    }
                    Qry1 += " and emp_code in (" + empcode + ") " + cond;
                }
            }
            return await _sha.Get_Table_FromQry(Qry1);
        }
    }
}
