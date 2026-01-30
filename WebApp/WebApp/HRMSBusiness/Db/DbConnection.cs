
using System.Configuration;

namespace HRMSBusiness.Db
{
    /*
     *Get different db connections for Sql server, Oracle , mySql...etc 
     */
    public class DbConnection
    {
        //todo - 1. (RK) get connection string using factory pattern.
        public string GetDbConnString()
        {

            string serverName = "";
            string databaseName = "";
            string userId = "";
            string password = "";

            // Server = MSOFT - 506\MSSQLSERVER14; Database = hrms7sep; user id = sa; password = Mavensoft;

            var connstr = ConfigurationManager.AppSettings["DefaultConnection"];
            
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

            string connString = "Data Source=" + serverName +
                                "; Initial Catalog=" + databaseName +
                                ";User id=" + userId +
                                ";Password=" + password + ";";
            //string connString = "Data Source=" + serverName +
            //                  "; Initial Catalog=" + databaseName +
            //                  "; Integrated Security=True;";
            //string connString = "Data Source=" + serverName +
            //                    ";Initial Catalog=" + databaseName +
            //                    "user id=sa; password=Mavensoft;";
            //                    //";Trusted_Connection=True";
            return connString;
        }
        public string GetDbConnString1()
        {

            string serverName = "";
            string databaseName = "";
            string userId = "";
            string password = "";

            // Server = MSOFT - 506\MSSQLSERVER14; Database = hrms7sep; user id = sa; password = Mavensoft;

            var connstr = ConfigurationManager.AppSettings["SecondConnection"];

            var arrcon = connstr.Split(';');

            if (connstr == ConfigurationManager.AppSettings["SecondConnection"])
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

            string connString = "Data Source=" + serverName +
                                "; Initial Catalog=" + databaseName +
                                ";User id=" + userId +
                                ";Password=" + password + ";";
            //string connString = "Data Source=" + serverName +
            //                    "; Initial Catalog=" + databaseName +
            //                    "; Integrated Security=True;";
            //string connString = "Data Source=" + serverName +
            //                    ";Initial Catalog=" + databaseName +
            //                    "user id=sa; password=Mavensoft;";
            //                    //";Trusted_Connection=True";
            return connString;
        }
    
    }
}
