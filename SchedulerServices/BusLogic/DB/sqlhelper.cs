using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


/// <summary>
/// Summary description for sqlhelper
/// </summary>
namespace BusLogic.DB
{
    public class SqlHelper
    {
        private string _dbconnstr;

        public SqlHelper(string dbConnStr)
        {
            _dbconnstr = dbConnStr;
        }
        public DataTable Get_Table_FromQry(string query)
        {
            DataSet ds = null;

            SqlConnection con = new SqlConnection(_dbconnstr);
            try
            {
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(ds);

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            return null;
        }

        public int Run_INS_ExecuteScalar(string qry)
        {
            //DbConnection con = new DbConnection();
            Int32 newID = 0;

            using (SqlConnection conn = new SqlConnection(_dbconnstr))
            {
                SqlCommand cmd = new SqlCommand(qry, conn);
                try
                {
                    conn.Open();
                    newID = (Int32)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }

            return (int)newID;
        }
        public bool Run_UPDDEL_ExecuteNonQuery(string qry)
        {
            bool flg = true;
            //DbConnection con = new DbConnection();

            using (SqlConnection conn = new SqlConnection(_dbconnstr))
            {
                SqlCommand cmd = new SqlCommand(qry, conn);
                try
                {
                    cmd.CommandTimeout = 3000;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    flg = false;
                    throw ex;
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }

            return flg;
        }
        public DataSet Get_MultiTables_FromQry(string semicolonseparatedQuery)
        {
            DbConnection conn = new DbConnection();
            DataSet ds = null;

            SqlConnection con = new SqlConnection(_dbconnstr);
            try
            {
               // string query = semicolonseparatedQuery.ToString();
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(semicolonseparatedQuery, con);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            return ds;
        }
    
}
}

