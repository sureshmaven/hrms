
using System.Configuration;

namespace Mavensoft.DAL.Db
{
    /*
     *Get different db connections for Sql server, Oracle , mySql...etc 
     */
    public class DbConnection
    {
        //todo - 1. (RK) get connection string using factory pattern.
        public static string GetDbConnString()
        {

            string serverName = "";
            string databaseName = "";
            string userId = "";
            string password = "";
            string connString = "";

            // Server = MSOFT - 506\MSSQLSERVER14; Database = hrms7sep; user id = sa; password = Mavensoft;

            var connstr = ConfigurationManager.AppSettings["DefaultConnection"];

            if (connstr != null)
            {
                //web.config connection for web apps
                var arrcon = connstr.Split(';');
                if (connstr == ConfigurationManager.AppSettings["DefaultConnection"])
                {
                    foreach (var itm in arrcon)
                    {
                        if (itm.Contains("Server="))
                        {
                            serverName = itm.Split('=')[1];
                        }
                        else if (itm.Contains("Database="))
                        {
                            databaseName = itm.Split('=')[1];
                        }
                        else if (itm.Contains("user id="))
                        {
                            userId = itm.Split('=')[1];
                        }
                        else if (itm.Contains("password="))
                        {
                            password = itm.Split('=')[1];
                        }
                    }
                }
                connString = "Data Source=" + serverName +
                    "; Initial Catalog=" + databaseName +
                    ";User id=" + userId +
                    ";Password=" + password + ";";
            }
            else
            {
                //service db connection
                connString = System.Configuration.ConfigurationManager.AppSettings.Get("mavendb");
            }

           return connString;
        }

    }
}
