using HRMSBusiness.Db;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace HRMSBusiness.Business
{
    public class LoginBus
    {
        SqlHelper sh = new SqlHelper();
        

        public LoginResult ValidateUnmPwdForMobile(string empid, string password)
        {
            LoginResult lresult = new LoginResult();
            string Pwd = Encrypt(password);
            // DateTime lcurrentDate = GetCurrentTime(DateTime.Now).Date;
            DateTime lcurrentDate = DateTime.Today.AddDays(1);
            string qry = " Select ControllingAuthority=(select  top 1 shortname from employees where id=e.ControllingAuthority),SanctioningAuthority = (select  top 1 shortname from employees where id = e.sanctioningauthority), e.EmpId,d.Name as Designation,  e.Id as EmpPK,e.FirstName,e.LastName,e.ShortName,e.Password,e.Role,e.UploadPhoto,e.Branch,e.Department," +
                " case when br.Name='OtherBranch' then dept.Name else br.Name + ' Br' end as BrDept,d.Name as CurrentDesignation,e.RetirementDate,e.LoginMode,(select count(*) FROM View_ChangingAuthority where ControllingEmpId=" + empid + " or SanctioningEmpID=" + empid + ") as Count "
                + " FROM Employees e join Designations d on e.CurrentDesignation = d.Id" +
                " join Branches br on e.Branch=br.Id" +
                " join Departments dept on e.Department=dept.Id" +
                " where e.EmpId='" + empid + "' AND e.Password='" + Pwd +  "' ";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                string loginmode = (string)dt.Rows[0]["LoginMode"].ToString();
                DateTime dbRetirementDate = (DateTime)dt.Rows[0]["RetirementDate"];
                if (dbRetirementDate >= lcurrentDate)
                {
                    lresult.Success = true;
                    lresult.EmpPkId = (int)dt.Rows[0]["EmpPk"];
                    lresult.EmpId = (string)dt.Rows[0]["EmpId"];
                    //lresult.EmployeeImage = (string)dt.Rows[0]["UploadPhoto"].ToString();
                    lresult.EmployeeFullName = (string)dt.Rows[0]["FirstName"].ToString() + " " + (string)dt.Rows[0]["LastName"].ToString();
                    //lresult.Branch = (int)dt.Rows[0]["Branch"];
                    //lresult.Department = (int)dt.Rows[0]["Department"];
                    //lresult.Role = (int)dt.Rows[0]["Role"];
                    //lresult.BranchName = (string)dt.Rows[0]["BrDept"];
                    //lresult.Approvals = (int)dt.Rows[0]["Count"];
                    //lresult.loginMode = loginmode;
                    //lresult.ControllingAuthority = (string)dt.Rows[0]["ControllingAuthority"];
                    //lresult.SanctioningAuthority = (string)dt.Rows[0]["SanctioningAuthority"];
                    lresult.desigcode = (string)dt.Rows[0]["CurrentDesignation"];
                }
                else
                {
                    lresult.Success = false;
                    lresult.Message = "Sorry, your Account is inactive Please Contact administrator";
                }
            }
            else
            {
                lresult.Success = false;
                lresult.Message = "Invalid Username or Password";
            }
            return lresult;
        }


        public LoginResult getLoginInformationmobileapi(int empid)
        {
            LoginResult lresult = new LoginResult();
            //string Pwd = Encrypt(password);
            DateTime lcurrentDate = GetCurrentTime(DateTime.Now).Date;
            string qry = " Select ControllingAuthority=(select  top 1 shortname from employees where id=e.ControllingAuthority),SanctioningAuthority = (select  top 1 shortname from employees where id = e.sanctioningauthority), e.EmpId,d.Name as Designation,  e.Id as EmpPK,e.FirstName,e.LastName,e.ShortName,e.Password,e.Role,e.UploadPhoto,e.Branch,e.Department," +
                " case when br.Name='OtherBranch' then dept.Name else br.Name + ' Br' end as BrDept,d.Name as CurrentDesignation,e.RetirementDate,e.LoginMode,(select count(*) FROM View_ChangingAuthority where ControllingEmpId=" + empid + " or SanctioningEmpID=" + empid + ") as Count "
                + " FROM Employees e join Designations d on e.CurrentDesignation = d.Id" +
                " join Branches br on e.Branch=br.Id" +
                " join Departments dept on e.Department=dept.Id" +
                " where e.EmpId=" + empid + " ";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                string loginmode = (string)dt.Rows[0]["LoginMode"].ToString();
                DateTime dbRetirementDate = (DateTime)dt.Rows[0]["RetirementDate"];
                if (dbRetirementDate >= lcurrentDate)
                {
                    lresult.Success = true;
                    lresult.EmpPkId = (int)dt.Rows[0]["EmpPk"];
                    lresult.EmpId = (string)dt.Rows[0]["EmpId"];
                    lresult.EmployeeImage = (string)dt.Rows[0]["UploadPhoto"].ToString();
                    lresult.EmployeeFullName = (string)dt.Rows[0]["FirstName"].ToString() + " " + (string)dt.Rows[0]["LastName"].ToString();
                    lresult.Branch = (int)dt.Rows[0]["Branch"];
                    lresult.Department = (int)dt.Rows[0]["Department"];
                    lresult.Role = (int)dt.Rows[0]["Role"];
                    lresult.BranchName = (string)dt.Rows[0]["BrDept"];
                    lresult.Approvals = (int)dt.Rows[0]["Count"];
                    lresult.loginMode = loginmode;
                    lresult.ControllingAuthority = (string)dt.Rows[0]["ControllingAuthority"];
                    lresult.SanctioningAuthority = (string)dt.Rows[0]["SanctioningAuthority"];
                    lresult.desigcode = (string)dt.Rows[0]["CurrentDesignation"];
                }
                else
                {
                    lresult.Success = false;
                    lresult.Message = "Sorry, your Account is inactive Please Contact administrator";
                }
            }
            else
            {
                lresult.Success = false;
                lresult.Message = "Invalid Username or Password";
            }
            return lresult;
        }

        public LoginResult getLoginInformation(string empid, string password)
        {
            LoginResult lresult = new LoginResult(); 
            string loginmode = "";
            string basedate = "1900-Jan-01 00:00:00.000";
            DateTime DefaultRelievingDate = DateTime.Parse(basedate);
            string Pwd = Encrypt(password);
              DateTime lcurrentDate = GetCurrentTime(DateTime.Now).Date;
            //DateTime lcurrentDate = DateTime.Today.AddDays(1); 
            string qry = "Select e.EmpId,d.Name as Designation, d.Name as currdes,  e.Id as EmpPK,e.FirstName,e.LastName,e.ShortName,e.Password,e.Role,e.UploadPhoto,e.Branch,e.Department," +
                " case when br.Name='OtherBranch' then dept.Name else br.Name + ' Br' end as BrDept,e.CurrentDesignation,e.RetirementDate,e.LoginMode,(select count(*) FROM View_ChangingAuthority where ControllingEmpId=" + empid + " or SanctioningEmpID=" + empid + ") as Count "
                + " FROM Employees e join Designations d on e.CurrentDesignation = d.Id" +
                " join Branches br on e.Branch=br.Id" +
                " join Departments dept on e.Department=dept.Id" +
                " where e.EmpId='" + empid + "' And e.Password='" + Pwd + "'";
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                loginmode = (string)dt.Rows[0]["LoginMode"].ToString();
                DateTime dbRetirementDate = (DateTime)dt.Rows[0]["RetirementDate"];
                if (dbRetirementDate >= lcurrentDate) 
                {
                    lresult.Success = true;
                    lresult.EmpPkId = (int)dt.Rows[0]["EmpPk"];
                    lresult.EmpId = (string)dt.Rows[0]["EmpId"];
                    lresult.EmployeeImage = (string)dt.Rows[0]["UploadPhoto"].ToString();
                    lresult.EmployeeFullName = (string)dt.Rows[0]["FirstName"].ToString() + " " + (string)dt.Rows[0]["LastName"].ToString();
                    lresult.Branch = (int)dt.Rows[0]["Branch"];
                    lresult.Department = (int)dt.Rows[0]["Department"];
                    lresult.Role = (int)dt.Rows[0]["Role"];
                    lresult.BranchName = (string)dt.Rows[0]["BrDept"];
                    lresult.Approvals = (int)dt.Rows[0]["Count"];
                    lresult.ShortName = (string)dt.Rows[0]["ShortName"].ToString();
                    // lresult.ControllingAuthority= (int)dt.Rows[0]["ControllingAuthority"];
                    lresult.loginMode = loginmode;
                    lresult.Designation = (int)dt.Rows[0]["CurrentDesignation"];
                    lresult.CurrDesig = (string)dt.Rows[0]["currdes"].ToString();
                }
                else
                {
                    lresult.Success = false;
                    lresult.Message = "Sorry, your Account is inactive Please Contact administrator";
                }
            }
            else
            {
                lresult.Success = false;
                lresult.Message = "Invalid Username or Password";
            }
            return lresult;
        }

        public DataTable GetLeaveBalance(int emppkid)
        {
            return sh.Get_Table_FromQry("select * from [V_EmpLeaveBalance] where EmpId=" + emppkid);
        }

        public string getUserPages(string EmpId, string[] LeavesSanctioning, int roleid, int approve, int desig, string lcontrolling, string lsanction,int dept)
        {
            //string retStr = "";

            
            string retStr = "";
            string[] LeavesSanctionings = ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',');
            List<string> list = LeavesSanctionings.ToList();

            if (ConfigurationManager.AppSettings["LeavesSanctioning"].Split(',').Contains(EmpId) || roleid==4)
                
            {
                    retStr = "MemoSuper,MemoMain,LVLAP,EDMain,Payslip,MPMain,RPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDMain,LVMLV,ODOAL,LTCAL,PLPAL,WDIAR,EMPREP,COVID";  
            }
            else
            {

                if (roleid == 1 && (desig != 1 && desig != 3 && desig != 2))
                {
                    retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                        "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                        "MAODY,MABPD,MANES,WDIAR,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                        "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                        "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";


                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip,COVID";
                        }
                    }

                    else if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,RPCASAE,Payslip,COVID";
                    }

                }
                if (roleid == 1 && (desig == 1 || desig == 3 || desig == 2 || desig == 4))
                {
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDAPLMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                              "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                              "MAODY,MABPD,MANES,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                              "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                              "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDSHYMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                             "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                             "MAODY,MABPD,MANES,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                             "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                             "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null && dept != 16)
                        {
                            retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDAPLMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                             "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                             "MAODY,MABPD,MANES,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                             "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                             "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null && dept==16)
                        {
                            retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDAPLMain,TsheetApp,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                             "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                             "MAODY,MABPD,MANES,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                             "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                             "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";
                        }

                    }

                    else
                    {
                        retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                                               "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                                               "MAODY,MABPD,MANES,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                                               "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                                               "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID";
                    }
                    //if (approve > 0 && approve<2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL";
                    //}
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip,COVID";
                        }
                    }
                    else if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,Payslip,COVID";
                    }

                }
                else if (roleid == 2 && (desig == 1 || desig == 2 || desig == 3 || desig == 4))
                {
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,TsheetMain,MPMain,LEMain,ODMain,LTMain,PLMain,POMain,MANews,WDAPLMain,RPMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,RPWDY,EMPREP,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,MANews,WDSHYMain,RPMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,RPWDY,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,MPMain,Payslip,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,MANews,WDAPLMain,RPMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,RPWDY,EMPREP,COVID";
                        }

                    }
                    else
                    {
                        retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,LEMain,TsheetMain,ODMain,LTMain,PLMain,POMain,MANews,RPMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,RPWDY,COVID";
                    }

                    //if (approve > 0 && approve < 2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL,RPWDY";
                    //}
                    if (approve > 0)
                    {

                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,RPWDY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,RPWDY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,RPWDY,Payslip,COVID";
                        }

                    }
                    else if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,RPWDY,Payslip,COVID";
                    }

                }
                else if (roleid == 2 && (desig == 1 || desig == 3 || desig == 4))
                {
                    retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,MANews,WDMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,RPWDY,EMPREP,COVID";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,RPWDY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,RPWDY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,RPWDY,Payslip,COVID";
                        }

                    }
                }
                else if (roleid == 2)
                {

                    retStr = "MemoSuper,MemoMain,DBMain,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,MANews,WDMain,LVMLV,ODOAL,LTCAL,PLPAL,MANES,WDIAR,EMPREP,Payslip,COVID";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip,COVID";
                        }

                    }
                }
        else if (roleid == 3 && (desig == 1 || desig == 2 || desig == 3 || desig == 4))
                {

                    if (approve > 0)
                    {

                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,LVMLV,ODOAL,RPMain,LTCAL,PLPAL,EMPREP,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,LEMain,TsheetMain,ODMain,LTMain,PLMain,POMain,WDSHYMain,LVMLV,RPMain,ODOAL,LTCAL,PLPAL,EMPREP,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,LEMain,TsheetMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,RPMain,LVMLV,ODOAL,LTCAL,PLPAL,EMPREP,COVID";
                        }


                    }
                    else
                    {
                        retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,LVMLV,ODOAL,RPMain,EMPREP,LTCAL,PLPAL,COVID";
                    }

                    //if (approve > 0 && approve < 2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL";
                    //}
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDSHY,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,WDSHY,COVID";
                        }

                    }
                    else if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,Payslip,COVID";
                    }

                }
                else if (roleid == 3)
                {
                    retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,RPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDMain,WDIAR,LVMLV,ODOAL,LTCAL,PLPAL,EMPREP,COVID";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip,COVID";
                        }

                    }
                }
                else if (roleid == 4 && (desig == 1 || desig == 2 || desig == 3 || desig == 4))
                {
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoSuper,MemoMain,EDMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,LVMLV,ODOAL,LTCAL,PLPAL,EMPREP,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoSuper,MemoMain,EDMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDSHYMain,LVMLV,ODOAL,LTCAL,PLPAL,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr = "MemoSuper,MemoMain,EDMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,LVMLV,ODOAL,LTCAL,PLPAL,COVID";
                        }

                    }
                    else
                    {
                        retStr = "MemoSuper,MemoMain,EDMain,Payslip,MPMain,RPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,LVMLV,ODOAL,LTCAL,PLPAL,EMPREP,COVID";

                    }


                    //if (approve > 0 && approve < 2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL";
                    //}
                    if (approve == 0)
                    {
                        retStr += "LVLAP,Payslip,LVMLA,ODOAV,COVID";
                    }
                    else if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDSHY,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,WDSHY,COVID";
                        }

                    }

                }
                else if (roleid == 4  || roleid == 7)
                {
                    retStr = "MemoSuper,MemoMain,EDMain,Payslip,MPMain,RPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDMain,LVMLV,ODOAL,LTCAL,PLPAL,WDIAR,EMPREP,COVID";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDSHY,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,Payslip,LVMLA,ODOAV,WDAPL,WDSHY,COVID";
                        }


                    }
                }
                else if (roleid == 5 && (desig == 1 || desig == 2 || desig == 3 || desig == 4))
                {
                    if (approve > 0)
                    {

                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,MABlock,MABPD,MANES,ODOAL,LVMLV,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA," +
                                  "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDSHYMain,MABlock,MABPD,MANES,ODOAL,LVMLV,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA," +
                                  "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr = "MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,WDAPLMain,MABlock,MABPD,MANES,ODOAL,LVMLV,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA," +
                                  "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }

                    }
                    else
                    {
                        retStr = "MemoMain,DBMain,MPMain,Payslip,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,MABlock,MABPD,MANES,ODOAL,LVMLV,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA," +
                                    "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                    }



                    //if (approve > 0 && approve < 2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL";
                    //}
                    if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,Payslip";
                    }
                    else if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip";
                        }

                    }

                }
                else if (roleid == 5)
                {
                    retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,LEMain,ODMain,TsheetMain,LTMain,PLMain,POMain,WDMain,MABlock,MABPD,MANES,WDIAR,ODOAL,LVMLV,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA," +
                             "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip";
                        }

                    }
                }
                else if (roleid == 6 && (desig == 1 || desig == 2 || desig == 3 || desig == 4))
                {
                    if (approve > 0)
                    {

                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDAPLMain,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY,MAODY," +
                                 "MABPD,MANES,ODOAL,LVMLV,LVELB,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA,RPEMP,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA" +
                                 "RPKOF,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPRET,RPSEN,RPSMA,RPSPL,RPLTC,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDSHYMain,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY,MAODY," +
                                "MABPD,MANES,ODOAL,LVMLV,LVELB,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA,RPEMP,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA" +
                                "RPKOF,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPRET,RPSEN,RPSMA,RPTXH,RPSPL,RPLTC,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDAPLMain,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY,MAODY," +
                                "MABPD,MANES,ODOAL,LVMLV,LVELB,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA,RPEMP,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA" +
                                "RPKOF,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPRET,RPSEN,RPSMA,RPTXH,RPSPL,RPLTC,RPTTS,RPTMT,RPWDY,RPYLB,COVID";
                        }

                    }
                    else
                    {
                        retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY,MAODY," +
                                 "MABPD,MANES,ODOAL,LVMLV,LVELB,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA,RPEMP,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA" +
                                 "RPKOF,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPRET,RPSEN,RPSMA,RPSPL,RPLTC,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB";

                    }

                    //if (approve > 0 && approve < 2)
                    //{
                    //    retStr += "LVLAP,LVMLA,ODOAV,LTCAV,PLPAV,WDAPL";
                    //}
                    if (approve == 0)
                    {
                        retStr += "LVLAP,LVMLA,ODOAV,Payslip";
                    }
                    else if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip";
                        }

                    }

                }
                else if (roleid == 6)
                {
                    retStr = "MemoSuper,MemoMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDMain,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY,MAODY," +
                             "MABPD,MANES,WDIAR,ODOAL,LVMLV,LVELB,LTCAL,PLPAL,RPMain,RPBRA,RPBRC,RPCDR,RPCAT,RPCASAE,RPDOB,RPHOF,RPHOA,RPEMP,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA" +
                             "RPKOF,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPSPL,RPLTC";
                    if (approve > 0)
                    {
                        if (lcontrolling != null && lsanction == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,Payslip";
                        }
                        else if (lsanction != null && lcontrolling == null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDSHY,Payslip";
                        }
                        else if (lsanction != null && lcontrolling != null)
                        {
                            retStr += "LVLAP,LVMLA,ODOAV,WDAPL,WDSHY,Payslip";
                        }

                    }
                }

                //else if(roleid==1 && EmpId== "123456")
                //{
                //    retStr = "MemoSuper,LFPMain,DBMain,Payslip,MPMain,TsheetMain,LEMain,ODMain,LTMain,PLMain,POMain,ADMain,MAMain,WDMain,RPMain,LVMLV,LVELB,ODOAL,LTCAL," +
                //        "PLPAL,ADERG,ADTPO,ADCAU,ADCPR,ADUSRO,ADCBRMG,ADPLA,ADWDA,ADLTCA,MABAN,MABRA,MADEP,MADES,MAROL,MALTY,MAHLI,MACDL,MABCY," +
                //        "MAODY,MABPD,MANES,WDIAR,RPBRA,RPBRC,RPCDR,RPCAT,RPDOB,RPEMP,RPHOF,RPHOA," +
                //        "RPKOF,RPLTC,RPLEV,RPLLE,RPLAL,RPLCD,RPODD,RPPTS,RPPRO," +
                //        "RPSPL,RPRET,RPSEN,RPSMA,RPTXH,RPTTS,RPTMT,RPWDY,RPYLB,RPCASAE,COVID,insertleave";
                //    //retStr = "DBMain,insertleave";
                //}
                //return retStr;
            }
            return retStr;
        }
        public string getEmpImage(string empid)
        {
            string Image = "";
            string qry = "Select UploadPhoto from Employees where EmpId=" + empid;
            DataTable dt = sh.Get_Table_FromQry(qry);
            if (dt.Rows.Count > 0)
            {
                Image = (string)dt.Rows[0]["UploadPhoto"].ToString();
            }
            return Image;
        }

        public DateTime GetCurrentTime(DateTime ldate)
        {
            DateTime serverTime = DateTime.Now;
            DateTime utcTime = serverTime.ToUniversalTime();
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }


        public string Encrypt(string clearText)
        {
            string EncryptionKey = "TSCAB_HRMS@2018";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "TSCAB_HRMS@2018";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string Empleavebalance(string empId, string lempid, string currentyears)
        {
            string curentdate = DateTime.Now.Date.ToShortDateString();
            string curentdates = Convert.ToDateTime(curentdate).ToString("yyyy-MM-dd");
            string qryIns1 = "INSERT INTO EmpLeaveBalance([LeaveTypeId],[EmpId],[LeaveBalance],[UpdatedBy],Year,Credits,Debits,CarryForward,UpdatedDate) "
                                      + "VALUES(" + 1 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 2 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 3 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 4 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 5 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 6 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 7 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 12 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE()),(" + 13 + "," + empId + "," + 0 + "," + lempid + ",Year(GETDATE())," + 0 + "," + 0 + "," + 0 + ",GETDATE())";
            qryIns1 += "SELECT CAST(SCOPE_IDENTITY() as int);";

            string qryIns2 = "INSERT INTO Leaves_CarryForward([EmpId],[LeaveTypeId],[CarryForward],[LeaveCredit],[LeaveDebit],[LeaveBalance],[UpdatedDate],[Year],[PreviousYearCF]) "
                                      + "VALUES( " + empId + "," + 1 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 2 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 3 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 4 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 5 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 6 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 7 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 12 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ",getdate(),Year(GETDATE())," + 0 + "),( " + empId + "," + 13 + "," + 0 + "," + 0 + "," + 0 + "," + 0 + ", getdate(),Year(GETDATE())," + 0 + ")";
            qryIns2 += "SELECT CAST(SCOPE_IDENTITY() as int);";
            sh.Run_INS_ExecuteScalar(qryIns1 + qryIns2);
            return "";
        }
    }
}
