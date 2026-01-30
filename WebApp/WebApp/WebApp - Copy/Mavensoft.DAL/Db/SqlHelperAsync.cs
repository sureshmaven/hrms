using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mavensoft.DAL.Db
{
    public class SqlHelperAsync
    {
        public async Task<DataTable> Get_Table_FromQry(string qry)
        {
            using (var conn = new SqlConnection(DbConnection.GetDbConnString()))
            {
                var adapter = new SqlDataAdapter(qry, conn);
                DataSet ds = new DataSet();
                await Task.Run(() => adapter.Fill(ds));
                return ds.Tables[0];
            }
        }

        public async Task<DataSet> Get_MultiTables_FromQry(string semicolonseparatedQuery)
        {
            using (var conn = new SqlConnection(DbConnection.GetDbConnString()))
            {
                var adapter = new SqlDataAdapter(semicolonseparatedQuery, conn);
                DataSet ds = new DataSet();
                await Task.Run(() => adapter.Fill(ds));
                return ds;
            }
        }

        public async Task<int> Run_INS_ExecuteScalar(string qry)
        {
            using (var connection = new SqlConnection(DbConnection.GetDbConnString()))
            {
                await connection.OpenAsync();
                using (var tran = connection.BeginTransaction())
                using (var command = new SqlCommand(qry, connection, tran))
                {
                    object ret;
                    try
                    {
                        ret = await command.ExecuteScalarAsync();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                    tran.Commit();
                    return int.Parse(ret.ToString());
                }
            }
        }

        public async Task<bool> Run_UPDDEL_ExecuteNonQuery(string qry)
        {
            using (var connection = new SqlConnection(DbConnection.GetDbConnString()))
            {
                await connection.OpenAsync();
                using (var tran = connection.BeginTransaction())
                using (var command = new SqlCommand(qry, connection, tran))
                {
                    bool ret = false;
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        ret = true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                    tran.Commit();
                    return ret;
                }
            }
        }

    }
}
