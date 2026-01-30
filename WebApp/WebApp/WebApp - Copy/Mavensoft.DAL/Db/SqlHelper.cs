using System;
using System.Data;
using System.Data.SqlClient;

namespace Mavensoft.DAL.Db
{
    /*
     * This class will be used to get connection with SQL server
     */
    public class SqlHelper
    {
        //todo - 1. (RK) get connection string using factory pattern.
        //todo - 2. (RK) use connection pool to not to exceed 'n' number of connecions.
        //todo - 3. (RK) Proper exception handling required.
        //todo - 4. (RK) write methods for stored proc executions.
        //todo - 5. (RK) write methods for execute scaler, execute query 
        //todo - 5. please add any thoughts...

        public bool Run_UPDDEL_ExecuteNonQuery(string qry)
        {
            bool flg = true;
            //DbConnection con = new DbConnection();

            using (SqlConnection conn = new SqlConnection(DbConnection.GetDbConnString()))
            {
                SqlCommand cmd = new SqlCommand(qry, conn);
                try
                {
                    conn.Open();
                    cmd.CommandTimeout = 240;
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

        public int Run_INS_ExecuteScalar(string qry)
        {
            //DbConnection con = new DbConnection();
            Int32 newID = 0;
            
            using (SqlConnection conn = new SqlConnection(DbConnection.GetDbConnString()))
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

        public DataTable Get_Table_FromQry(string query)
        {
            DataSet ds = null;

            SqlConnection con = new SqlConnection(DbConnection.GetDbConnString());
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

        }

        public DataSet Get_MultiTables_FromQry(string semicolonseparatedQuery)
        {
            DataSet ds = null;

            SqlConnection con = new SqlConnection(DbConnection.GetDbConnString());
            try
            {
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
