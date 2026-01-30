using Mavensoft.Common;
using Mavensoft.DAL.Db;
using PayrollModels;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Mavensoft.DAL.Business
{
    public class BusinessBase
    {
        public LoginCredential _LoginCredential = null;
        public SqlHelperAsync _sha = null;
        bool _isUpdate = true;
        public BusinessBase(LoginCredential _loginCredential)
        {
            _LoginCredential = _loginCredential;
            _sha = new SqlHelperAsync();
        }

        public string GetNewNumString(string tableName)
        {
            return "declare @idnew int; "
                + "exec get_new_num '" + tableName + "'; "
                + "select @idnew = last_num from new_num where table_name = '" + tableName + "' ; ";
        }

        public string GenNewTransactionString()
        {
            return "declare @transidnew int; "
                + "exec gen_new_transaction " + _LoginCredential.EmpCode + ", '" + _LoginCredential.EmpShortName + "', '" + _LoginCredential.AppName + "', '" + _LoginCredential.AppVersion + "', 'N/A'; "
                + "select @transidnew = last_num from new_num where table_name = 'transaction_tbl'; ";
        }

        public string GenNewTransactionString(int userId, string userInit, Application_Type apptype, string appVersion, string machinename)
        {
            return "declare @transidnew int; "
                + "exec gen_new_transaction " + userId + ", '" + userInit + "', '" + apptype + "', '" + appVersion + "', '" + machinename + "'; "
                + "select @transidnew = last_num from new_num where table_name = 'transaction_tbl'; ";
        }

        public string GetTransactionTouchString(Transaction_Touch_Type touchType, string EntityName, int Oid)
        {
            return "insert into transaction_touch([operation],[entity_name],[entity_oid],[trans_id]) "
                + "values('" + touchType + "', '" + EntityName + "'," + (Oid > 0 ? Oid.ToString() : "@idnew") + ", @transidnew); ";
        }

        public async Task<string> InsertRecord(string inQry)
        {
            string sRet = "I#";
            string entity = "";
            try
            {
                entity = inQry.Split(' ')[2];
                StringBuilder qry = new StringBuilder();

                //1. Generate new num string for entity
                qry.Append(GetNewNumString(entity)); //@idnew

                //2. Generate new transaction string
                qry.Append(GenNewTransactionString()); //@transidnew

                inQry = inQry.Replace("#id", "@idnew1").Replace("#tid", " @transidnew");

                //3. Insert statement
                qry.Append(inQry + (inQry.Trim().EndsWith(";") ? "" : ";"));

                //4. Generate transaction touch string
                qry.Append(GetTransactionTouchString(Transaction_Touch_Type.I, entity, 0));

                //5.return entity new id
                qry.Append("Select @idnew ;");

                //SqlHelper sh = new SqlHelper();
                //var newID = sh.Run_INS_ExecuteScalar(qry.ToString());

                var newID = await _sha.Run_INS_ExecuteScalar(qry.ToString());

                sRet += "New " + entity + "( " + newID + " ) is created#"
                      + "New " + entity + "( " + newID + " ) is created";
            }
            catch (Exception ex)
            {
                sRet = "E#" + new ExceptionNew(entity, Transaction_Touch_Type.I, ex).ToString();
            }

            return sRet;
        }

        public async Task<string> UpdateRecord(string updQry, int entityOid)
        {
            string sRet = "I#";
            string entity = "";

            try
            {
                StringBuilder qry = new StringBuilder();
                entity = updQry.Split(' ')[1];

                //1. Generate new transaction string
                qry.Append(GenNewTransactionString()); //@transidnew
                updQry = updQry.Replace("where", ", trans_id = @transidnew WHERE");

                //2. update statement
                qry.Append(updQry + (updQry.Trim().EndsWith(";") ? "" : ";"));

                //3. Generate transaction touch string
                qry.Append(GetTransactionTouchString(_isUpdate ? Transaction_Touch_Type.U : Transaction_Touch_Type.D, entity, entityOid));

                bool isupdate = await _sha.Run_UPDDEL_ExecuteNonQuery(qry.ToString());
                if (isupdate)
                    sRet += entity + "( " + entityOid + " ) is Updated#"
                          + entity + "( " + entityOid + " ) is Updated";
            }
            catch (Exception ex)
            {
                sRet = "E#" + new ExceptionNew(entity + "( " + entityOid + " )", Transaction_Touch_Type.U, ex).ToString();
            }

            return sRet;
        }

        public async Task<string> InactivateRecord(string inactiveQry, int entityOid)
        {
            _isUpdate = false;
            var ret = await UpdateRecord(inactiveQry, entityOid);
            _isUpdate = true;
            return ret.Replace("is Updated", "is Deleted(Inactivated)");
        }

        public string ReportColHeader(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": <b>" + value + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColHead(int spaceCount, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColHeaderValueOnly(int spaceCount, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span><b>" + value + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColFooter(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + value + "</span>";
            //sRet += "<span>" + lable + ": "+ "</span>" + "<span style='float:right'>"+ value + "<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
            //    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>" + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColFooterThreestrings(int spaceCount, string lable, string value,string value1)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + lable + " " + value + " " + value1 + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColFooterValueOnly(int spaceCount, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp";
            sRet += "</span>";

            sRet += "<span>" + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }
        #region multi inserts/updates/deletes

        public string GetNewNumStringArr(string tableName, int index)
        {
            return "declare @idnew"+ index + " int; "
                + "exec get_new_num '" + tableName + "'; "
                + "select @idnew"+ index + " = last_num from new_num where table_name = '" + tableName + "' ; ";
        }
        public string GetTransactionTouchStringArr(Transaction_Touch_Type touchType, string EntityName, string EntityPkId, string oldValue)
        {
            return "insert into transaction_touch([operation],[entity_name],[entity_oid],[key1],[trans_id]) "
                + "values('" + touchType + "', '" + EntityName + "'," + EntityPkId + ",'"+ oldValue +"', @transidnew); ";
        }

        #endregion


        public string ReportColFooterlesscol(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_FOOTER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp; &nbsp; &nbsp; ";
            sRet += "</span>";

            sRet += "<span>" + lable + ": " + value + "</span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        public string ReportColHeaderlesscol(int spaceCount, string lable, string value)
        {
            string sRet = "<span style='color:" + PrConstants.PDF_REPORT_HEADER_COLOUR + "'>";
            for (int i = 1; i <= spaceCount; i++)
                sRet += "&nbsp; &nbsp;";
            sRet += "</span>";

            sRet += "<span>" + lable + ": <b>" + value + "</b></span>";

            // <span style='color:#C8EAFB'>_________________</span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            return sRet;
        }

        //public string DeleteRecord(string delQry, int entityOid)
        //{
        //    string sRet = "I#";
        //    string entity = "";
        //    try
        //    {
        //        StringBuilder qry = new StringBuilder();

        //        entity = delQry.Split(' ')[2];

        //        //1. Generate new transaction string
        //        qry.Append(GenNewTransactionString("KRJ", Application_Type.HRPAYBill_UI, "1.0", "PC 10", Transaction_Type.U)); //@transidnew

        //        //2. delete statement
        //        qry.Append(delQry);

        //        //3. Generate transaction touch string
        //        qry.Append(GetTransactionTouchString(Transaction_Touch_Type.DEL, entity, entityOid));


        //        SqlHelper sh = new SqlHelper();
        //        sh.Run_UPDDEL_ExecuteNonQuery(qry.ToString());

        //        sRet += entity + "( " + entityOid + " ) is Deleted#"
        //              + entity + "( " + entityOid + " ) is Deleted";

        //    }
        //    catch (Exception ex)
        //    {
        //        sRet = "E#" + new DBException(entity + "( " + entityOid + " )", Transaction_Touch_Type.DEL, ex).ToString();
        //    }

        //    return sRet;

        //}

    }
}
