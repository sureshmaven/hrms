using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using HRMSBusiness.Db;
using System.Data;

namespace HRMSBusiness.Business
{
    public class Covid19Business
    {
        SqlHelper sh = new SqlHelper();

        public string AddCovid19(CovidViewModel LTC,string EmpCode)
        {
            int sno = 0;
            string msg = "";
            var quarhistory = "";
            var complaints = "";
            StringBuilder sb = new StringBuilder();
            //string selquery = " select * from KovidSurvey where empid = " + EmpCode + "";
            //DataTable dt = sh.Get_Table_FromQry(selquery);
            //if (dt.Rows.Count > 0)
            //{
                string delquery = "delete from KovidSurvey where empid=" + EmpCode + "";
                sb.Append(delquery);
            //}
            for (int i = 0; i < LTC.relation.Length; i++)
            {
                string qry = "";
                sno++;
                        var RelationName = LTC.name.GetValue(i).ToString();
                        var RelationAge = LTC.RelationAge.GetValue(i).ToString();
                        var Gender = LTC.Gender.GetValue(i).ToString();
                        var address = LTC.address.GetValue(i).ToString();
                        var diabetes = LTC.diabetes.GetValue(i).ToString();
                        var hbp = LTC.hbp.GetValue(i).ToString();
                         quarhistory = LTC.quarhistory.GetValue(i).ToString();
                         complaints = LTC.complaints.GetValue(i).ToString();
                        var relation = LTC.relation.GetValue(i).ToString();
                        qry = " insert into KovidSurvey(Empid,sno,Name,Gender,Age,Relationship,Address,Complaints,Diabetes,Hypertension,Quarantine,CreateDate)" +
                   "values(" + EmpCode + " ," + sno + ",'" + RelationName + "','" + Gender + "'," + RelationAge + ",'" + relation + "'," +
                   " '" + address + "','" + complaints + "','" + diabetes + "','" + hbp + "','" + quarhistory + "', getdate())";
                        sb.Append(qry);
        
                }
        
           if( sh.Run_UPDDEL_ExecuteNonQuery(sb.ToString()))
            {
                msg = "I#Required#Data Inserted Sucessfully";
            }
            return msg;
        }

        public DataTable empdata( string EmpCode)
        {
            string selquery = "Select CONCAT( FirstName, ' ', lastname) as lastname,gender from employees where empid = " + EmpCode + "";
            DataTable dt = sh.Get_Table_FromQry(selquery);
            return dt;
        }
        public DataTable empdatainservey(string EmpCode)
        {
            string selquery = "Select Empid from KovidSurvey where empid = " + EmpCode + "";
            DataTable dt = sh.Get_Table_FromQry(selquery);
            return dt;
        }
        public DataTable EditCovidFormData(string empcode)
        {
            string selquery = " select * from KovidSurvey where empid = " + empcode + "";
            DataTable dt = sh.Get_Table_FromQry(selquery);
            return dt;
        }
    }
}
