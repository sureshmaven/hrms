using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace HRMSBusiness.Db
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
        private string _connectionString;

       
        public bool Run_UPDDEL_ExecuteNonQuery(string qry)
        {
            bool flg = true;
            DbConnection con = new DbConnection();

            using (SqlConnection conn = new SqlConnection(con.GetDbConnString()))
            {
                SqlCommand cmd = new SqlCommand(qry, conn);
                cmd.CommandTimeout = 2000;
                try
                {
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

        public int Run_INS_ExecuteScalar(string qry)
        {
            DbConnection con = new DbConnection();
            Int32 newID = 0;
            
            using (SqlConnection conn = new SqlConnection(con.GetDbConnString()))
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
            DbConnection conn = new DbConnection();
            DataSet ds = null;

            SqlConnection con = new SqlConnection(conn.GetDbConnString());
            try
            {
                ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.CommandTimeout = 30000;
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

        public DataSet Get_MultiTables_FromQry(string semicolonseparatedQuery)
        {
            DbConnection conn = new DbConnection();
            DataSet ds = null;

            SqlConnection con = new SqlConnection(conn.GetDbConnString());
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

        public DataTable ExecuteSp(List<SqlParameter> parameters, string spName)
        {
            DataSet ds = new DataSet();
            DbConnection conn = new DbConnection();

            using (SqlDataAdapter da = new SqlDataAdapter())
            {
                using (SqlConnection con = new SqlConnection(conn.GetDbConnString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = spName;
                        cmd.Parameters.AddRange(parameters.ToArray());
                        cmd.CommandTimeout = 300;
                        cmd.CommandType = CommandType.StoredProcedure;
                        da.SelectCommand = cmd;
                        _ = da.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            return ds.Tables[0];
        }

        public DataTable ExecuteQuery(string sqlQuery)
        {
            DbConnection conn = new DbConnection();
            DataSet ds = null;

            SqlConnection connection = new SqlConnection(conn.GetDbConnString1());
           try{
                        ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(sqlQuery, connection);
                        da.SelectCommand.CommandTimeout = 180;
                        da.Fill(ds);

                        return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }

            return null;
        }

        public object Run_SEL_ExecuteScalar(string query)
        {
            DbConnection con = new DbConnection();  // Assuming you have a class for DB connection management
            using (SqlConnection conn = new SqlConnection(con.GetDbConnString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result;
                }
                catch (Exception ex)
                {
                    // You can log the exception if necessary
                    throw new Exception("An error occurred while executing the scalar query.", ex);
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
        }



    }

}
