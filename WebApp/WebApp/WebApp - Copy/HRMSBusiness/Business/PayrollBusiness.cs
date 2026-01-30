
using Entities;
using HRMSBusiness.Db;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HRMSBusiness.Models;
using PayrollModels;
using System.Dynamic;
using System.Threading.Tasks;
using Mavensoft.DAL.Db;
using Mavensoft.Common;

namespace HRMSBusiness.Business
{
    public class PayrollBusiness
    {
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        Mavensoft.DAL.Business.BusinessBase bs = new Mavensoft.DAL.Business.BusinessBase(null);
        SqlHelperAsync _sh = new SqlHelperAsync();
        public string GetLeaveBalance(string empid)
        {
            string retStr = "";
            var qry = " select EmpId,CasualLeave,MedicalSickLeave,PrivilegeLeave,MaternityLeave,PaternityLeave,ExtraordinaryLeave,SpecialCasualLeave,CompensatoryOff,LOP from V_EmpLeaveBalance where empid= " + empid + "";
            DataTable dt = sh.Get_Table_FromQry(qry);
            foreach (DataRow dr in dt.Rows)
            {
                retStr = retStr + "CL" + "#" + dr["CasualLeave"].ToString() + ',' + "ML" + "#" + dr["MedicalSickLeave"].ToString() + ',' + "PL" + "#" + dr["PrivilegeLeave"].ToString() + ',' + "MTL" + "#" + dr["MaternityLeave"].ToString() + ',' + "PTL" + "#" + dr["PaternityLeave"].ToString() + ',' + "EOL" + "#" + dr["ExtraordinaryLeave"].ToString() + ',' + "SCL" + "#" + dr["SpecialCasualLeave"].ToString() + ',' + "C-OFF" + "#" + dr["CompensatoryOff"].ToString() + ',' + "LOP" + "#" + dr["LOP"].ToString() + ",";
            }
            return retStr;



        }
        public DataTable EmployeeTable(string empid)
        {
            string qry =
                " Select * from Employees " +
                " where empid =" + empid + ";";
            return sh.Get_Table_FromQry(qry);
        }


        public string InserMonthDetails(DateTime FM, int FY, int WH, int PH, DateTime PD, int DAslabs, float DApoints, float DApercent, string monthyear)
        {
            string AlertMessage = "";
            try
            {
                string qry = "Insert into PR_Monthdetails(FY,FM,Weekholidays,Paidholidays,Paymentdate,DAslabs,DApoints,DApercent)" +
                    "values('" + FY + "','" + FM + "','" + WH + "','" + PH + "','" + PD + "','" + DAslabs + "','" + DApoints + "','" + DApercent + "')";
                string query = "Select * from PR_Monthdetails where FM=convert(date,'" + monthyear + "')";
                DataTable dt = sh.Get_Table_FromQry(query);
                if (dt.Rows.Count > 0)
                {
                    string query1 = "update PR_Monthdetails set FY=" + FY + ",FM=Convert(date,'" + FM + "'), Weekholidays=" + WH + ",Paidholidays=" + PH + ",Paymentdate=Convert(date,'" + PD + "')," +
                      "DAslabs= " + DAslabs + ",DApoints=" + DApoints + ",DApercent=" + DApercent + " where FM='" + FM + "'";
                    sh.Run_UPDDEL_ExecuteNonQuery(query1);
                    AlertMessage = "I#Required#Month Details Updated Sucessfully";
                }


                else
                {
                    sh.Run_UPDDEL_ExecuteNonQuery(qry);
                    AlertMessage = "S#Success#Month Details Saved Sucessfully";
                }
            }
            catch (Exception ex)
            {

                return "Error:" + ex.Message;
            }
            return AlertMessage;
        }
    
        public string InserMonthaAttendence(int FY, int EmpId, string Status, DateTime Statusdate, float LOPdays, float Workingdays, float Absentdays, DateTime FM, string Leavesavailable)
        {
            string AlertMessage = "";
            try
            { 
            string qry = "Insert into PR_Monthattendance(FY,FM,EmpId,Status,StatusDate,Leavesavailable,LOPdays,Absentdays,Workingdays,Active,Trans_id)" +
                "values(" + FY + ", getdate() ," + EmpId + ",'" + Status + "','" + Statusdate + "','" + Leavesavailable + "'," + LOPdays + "," + Workingdays + "," + Absentdays + "," + 1 + "," + 0 + ")";
            string query = "Select * from PR_Monthattendance where EmpId=" + EmpId + " and Active= 1";
            DataTable dt = sh.Get_Table_FromQry(query);
            if (dt.Rows.Count > 0)
            {

                string query1 = "update PR_Monthattendance set FY=" + FY + ",FM= getdate(), Status='" + Status + "',Statusdate='" + Statusdate + "',Absentdays=" + Absentdays + "," +
                  "Leavesavailable= '" + Leavesavailable + "',LOPdays=" + LOPdays + ",Workingdays=" + Workingdays + " where EmpId='" + EmpId + "' and Active=1";
                string qry1 = "Update PR_Monthattendance set Active = 0 where EmpId = " + EmpId + "";
                sh.Run_UPDDEL_ExecuteNonQuery(query1);
                    AlertMessage= "Monthly Attendence Updated Sucessfully";
                }


            else
            {
                sh.Run_UPDDEL_ExecuteNonQuery(qry);
                    AlertMessage = "Monthly Attendence saved Sucessfully";
            }
        }
        catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }
            return AlertMessage;
        }

        public DataTable getmonthdetals(string monthyear)
        {
            string query = "Select * from PR_Monthdetails where FM=" + monthyear;
            return sh.Get_Table_FromQry(query);
        }
        public DataTable getmonthattendence(string empid)
        {
            string query = "Select * from PR_Monthattendance where EmpId='" + empid +"' and Active= 1" ;
            return sh.Get_Table_FromQry(query);
        }

        public IList<EmployeeMaster> GetEmployeDeatilsById(string empid)
        {
            //string query = "Select Id,FirstName, CurrentDesignation, Department from Employees where EmpId=" + int.Parse(empid) + ";";

            string query = "select e.Id,e.FirstName, d.name as CurrentDesignation, case when b.Name = 'OtherBranch' then de.Name when b.Name = 'HeadOffice' then d.Name else b.Name END as Department from Employees e join Designations d on e.CurrentDesignation = d.Id join branches b on e.Branch = b.id  join Departments de on e.Department = de.id WHERE e.EmpId=" + int.Parse(empid) + ";";
            //return sh.Get_Table_FromQry(query);
            DataTable dt = sh.Get_Table_FromQry(query);
            IList<EmployeeMaster> lstEmpData = new List<EmployeeMaster>();
            if (dt.Rows.Count > 0)
            {
                //id  FirstName, CurrentDesignation, Department
                lstEmpData.Add(new EmployeeMaster
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    Name = "Id",
                    Value = dt.Rows[0]["Id"].ToString()
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    Name = "FirstName",
                    Value = dt.Rows[0]["FirstName"].ToString()
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    Name = "CurrentDesignation",
                    Value = dt.Rows[0]["CurrentDesignation"].ToString()
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    Name = "BR/Dept",
                    Value = dt.Rows[0]["Department"].ToString()
                });
            } else
            {
                lstEmpData.Add(new EmployeeMaster
                {
                    id = 0,
                    Name = "Id",
                    Value = ""
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = 0,
                    Name = "FirstName",
                    Value = ""
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = 0,
                    Name = "CurrentDesignation",
                    Value = ""
                });

                lstEmpData.Add(new EmployeeMaster
                {
                    id = 0,
                    Name = "BR/Dept",
                    Value = ""
                });
            }
            return lstEmpData;
        }
        public DataTable GetEFPayFields()
        {
            string qry = "select * from PR_Payfieldmaster where Payfieldtype='EF';";
            return sh.Get_Table_FromQry(qry);
        }

        //public DataTable getAllowanceDetails()
        //{
        //    string qry = "select * from Allowance where Active=1;";
        //    return sh.Get_Table_FromQry(qry);
        //}

        //public async Task<DataTable> getAllowanceDetails()
        //{
        //    string qry = "select * from Allowance where Active=1;";
        //    string res = await _sh.Get_Table_FromQry(qry);
        //    return res;
        //    //string res = await UpdateRecord(qryUpd, 4);
        //    //return res;
        //    //    string qry = "select * from Allowance where Active=1;";
        //    //    return sh.Get_Table_FromQry(qry);
        //}

        public async Task<IList<Allowance>> getAllowanceDetails()
        {
            IList<Allowance> lstAllowance = new List<Allowance>();
            string qrySel = "select id,name,description,case when amount is null then 'N' else 'U' end as row_type,active from allowance_master where active=1;";
            DataTable dt = await _sh.Get_Table_FromQry(qrySel);
            foreach (DataRow dr in dt.Rows)
            {
                lstAllowance.Add(new Allowance
                {
                    Id = TypeFormat.DbIntToInteger(dr["id"]),
                    Name = dr["Name"].ToString(),
                    Description = dr["Description"].ToString(),
                    Amount = dr["Amount"].ToString(),
                });
            }
            return lstAllowance;
        }

        public DataTable GetDFPayFields()
        {
            string qry = "select * from PR_Payfieldmaster where Payfieldtype='DF';";
            return sh.Get_Table_FromQry(qry);
        }

       
        public DataSet EmpInEmppayfieldsTable(string EmpId)
        {
            //string qry1 = "select p.payfieldname from PR_Payfieldmaster p join PR_Emppayfields e on e.PFMId = p.Id where e.Active = 1 and e.PFMType = 'EF' and e.EmpId =" + EmpId + ";";
            //string qry2 = "select p.payfieldname from PR_Payfieldmaster p join PR_Emppayfields e on e.PFMId = p.Id where e.Active = 1 and e.PFMType = 'DF' and e.EmpId =" + EmpId + ";";

            string qryEFamount = "select Amount From PR_Emppayfields where EmpId=" + EmpId + " and Active=1 and PFMType='EF';";
            string qryDFamount = "select Amount From PR_Emppayfields where EmpId=" + EmpId + " and Active=1 and PFMType='DF';";
            DataSet dsEmp = sh.Get_MultiTables_FromQry(qryEFamount + qryDFamount);
             return dsEmp;
        }


        public async Task<string> UpdateAllowanceMaster(List<Allowance> values)
        {
            string AlertMessage = "";
            try {
               
                string allownsData = getAllowanceData(values);

            string empids = allownsData.TrimEnd(',');

            if (empids != "")
            {
               

                    var arrEmps = empids.Split(',');

                string updQry = "";
                string insertQry = "";
                    int allowanceId = 0;
                foreach (var emp in arrEmps)
                {
                    var arremp = emp.Split('#');
                    var Id = arremp[0];
                    var Name = arremp[1];
                    var Description = arremp[2];
                    foreach (var item in values)
                    {
                        if(item.Id == int.Parse(Id))
                        {
                                allowanceId = int.Parse(Id);

                                updQry += " update Allowance set Active=0 where Id=" + Id + ";";
                                insertQry += "INSERT INTO Allowance VALUES('" + Name + "','" + Description + "','" + item.Amount + "',1,@trans_id);";
                        }
                        
                    }
                    
                }
                   // AlertMessage = await bs.UpdateRecord(updQry, allowanceId);


                    // await UpdateRecord(qryUpd, 4);
                    //sh.Run_UPDDEL_ExecuteNonQuery(updQry + insertQry);
                    //AlertMessage = "I#Allowance Master#Allowance Data Updated Successfully."; 
                }

            }
            catch(Exception e)
            {
                return e.Message;
            }
            return AlertMessage;
        }

        private string getAllowanceData(List<Allowance> values)
        {
            string qryDetails = "";
            string retStr = "";
           
            qryDetails = "select Id,Name, Description from Allowance where Active=1";

            DataTable dtEmpIds = sh.Get_Table_FromQry(qryDetails);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Id"].ToString() + "#" + dr["Name"].ToString() + "#" + dr["Description"].ToString() + ",";

            }
            return retStr;
        }

        public string AddPayfield(PayfieldModel paymodel,int EmpId, int FY, string FM)
        {
            
            DataTable dtEmpInPR_Emppayfields = sh.Get_Table_FromQry("Select * from PR_Emppayfields where EmpId = "+ EmpId + ";");
            if (dtEmpInPR_Emppayfields.Rows.Count>0)
            {
                sh.Run_UPDDEL_ExecuteNonQuery("update PR_Emppayfields set Active =0 where EmpId="+ EmpId + ";");
            }
            string qryEFId = "";
            //for (int i = 0; i < paymodel.Payfieldname.Length; i++)
            //{

                qryEFId= "select * from PR_Payfieldmaster where Payfieldtype='EF';";
               


            //}
            DataTable payfieldEFId = sh.Get_Table_FromQry(qryEFId);
            string[] EFIdarrray = payfieldEFId.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();

            string qryInsertEFPay = "";
            for (int i = 0; i < paymodel.Amount.Length; i++)
            {
                if (paymodel.Amount.GetValue(i).ToString()!="") 

                    {
                        if (paymodel.Payfieldname.GetValue(i).ToString() != "")
                        {
                            qryInsertEFPay += "INSERT INTO PR_Emppayfields([FY],[FM],[PFMId],[PFMType],[Empid],[Amount]) "
                                            + "VALUES(" + FY + ",'" + FM + "'," + int.Parse(EFIdarrray[i]) + ",'EF'," + EmpId + ",'" + paymodel.Amount.GetValue(i).ToString() + "');";
                        }
                    }
            }

           // string qryDFId = "";
            //for (int i = 0; i < paymodel.Payfieldname.Length; i++)
            //{

            string qryDFId = "select * from PR_Payfieldmaster where Payfieldtype='DF';";



            //}
            DataTable payfieldDFId = sh.Get_Table_FromQry(qryDFId);
            string[] DFIdarrray = payfieldDFId.Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();

            string qryInsertDFPay = "";
        
            for (int i = 0; i < paymodel.DFAmount.Length; i++)
            {
                if (paymodel.DFAmount.GetValue(i).ToString() != "")
                {
                    if (paymodel.DFPayfieldname.GetValue(i).ToString() != "")
                    {
                        qryInsertDFPay += "INSERT INTO PR_Emppayfields([FY],[FM],[PFMId],[PFMType],[Empid],[Amount]) "
                                        + "VALUES(" + FY + ",'" + FM + "'," + int.Parse(DFIdarrray[i]) + ",'DF'," + EmpId + ",'" + paymodel.DFAmount.GetValue(i).ToString() + "');";
                    }
                }
            }
            string qry = qryInsertEFPay + qryInsertDFPay;
            sh.Run_UPDDEL_ExecuteNonQuery(qry);
            return "Empoyee Pay field Created Sucessfully";

        }
        //Search emp for loans and advances

        public IList<Loans_Advances> GetEmployeDeatilsForLoans_Advances(string empid)
        {
            string query = "select Id,FY,FM,EmpId,EmpCode,loantype ,Totalamt ,Noofinstall ,Intinstallment,Sanctiondate,Method,Intrate,Instalamount,Recoveramount,Completedinstall,Loanstartfrom,Loanvendorname,Active,Trans_id,Designation from PR_Emp_AdvLoans where Active=1 and EmpId=" + int.Parse(empid) + ";";
            //return sh.Get_Table_FromQry(query);
            DataTable dt = sh.Get_Table_FromQry(query);
            IList<Loans_Advances> lstEmpLoanData = new List<Loans_Advances>();
            if (dt.Rows.Count > 0)
            {
                //id  FirstName, CurrentDesignation, Department
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "Id",
                    display= "Id",
                    value = dt.Rows[0]["Id"].ToString()
                });

                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                //    dbcolumn = "FY",
                //    display = "Financial Year",
                //    value = dt.Rows[0]["FY"].ToString()
                //});

                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                //    dbcolumn = "FM",
                //    display = "Financial Month",
                //    value = dt.Rows[0]["FM"].ToString()
                //});

                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "EmpId",
                    display = "Employee Id",
                    value = dt.Rows[0]["EmpId"].ToString()
                });


                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "EmpCode",
                    display = "Employee Code",
                    value = dt.Rows[0]["EmpCode"].ToString()
                });



                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "loantype",
                    display = "Loan Type",
                    value = dt.Rows[0]["loantype"].ToString()
                });

                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "Totalamt",
                    display = "Total Amount",
                    value = dt.Rows[0]["Totalamt"].ToString()
                });
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["Id"]),
                    dbcolumn = "Noofinstall",
                    display = "No.Of Installments",
                    value = dt.Rows[0]["Noofinstall"].ToString()
                });


                //Intinstallment
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Intinstallment",
                    display = "Interest Installments",
                    value = dt.Rows[0]["Intinstallment"].ToString()
                });

                //Sanctiondate
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Sanctiondate",
                    display = "Sanction Date",
                    value = dt.Rows[0]["Sanctiondate"].ToString()
                });

                //Method
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Method",
                    display = "Method",
                    value = dt.Rows[0]["Method"].ToString()
                });

                //Intrate
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Intrate",
                    display = "Interest Rate",
                    value = dt.Rows[0]["Intrate"].ToString()
                });


                //Instalamount
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Instalamount",
                    display = "Installment Amount",
                    value = dt.Rows[0]["Instalamount"].ToString()
                });

                //Recoveramount
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Recoveramount",
                    display = "Recover Amount",
                    value = dt.Rows[0]["Recoveramount"].ToString()
                });

                //Completedinstall
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Completedinstall",
                    display = "CompletedInstallments",
                    value = dt.Rows[0]["Completedinstall"].ToString()
                });

                //Loanstartfrom
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Loanstartfrom",
                    display = "Loan Start From",
                    value = dt.Rows[0]["Loanstartfrom"].ToString()
                });


                //Loanvendorname
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Loanvendorname",
                    display = "Loan Vendor Name",
                    value = dt.Rows[0]["Loanvendorname"].ToString()
                });

                //Active
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Active",
                    display = "Active",
                    value = dt.Rows[0]["Active"].ToString()
                });

                //Active
                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    dbcolumn = "Trans_id",
                //    display = "Transaction id",
                //    value = dt.Rows[0]["Trans_id"].ToString()
                //});
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Designation",
                    display = "Designation",
                    value = dt.Rows[0]["Designation"].ToString()
                });
            }
            else
            {
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Id",
                    display = "Id",
                    value = ""
                });

                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = 0,
                //    dbcolumn = "FY",
                //    display = "Financial Year",
                //    value = ""
                //});

                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = 0,
                //    dbcolumn = "FM",
                //    display = "Financial Month",
                //    value = ""
                //});

                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "loantype",
                    display = "Loan Type",
                    value = ""
                });

                lstEmpLoanData.Add(new Loans_Advances
                {
                    id =0,
                    dbcolumn = "Totalamt",
                    display = "Total Amount",
                    value =""
                });
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Noofinstall",
                    display = "No.Of Installments",
                    value = ""
                });


                //Intinstallment
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Intinstallment",
                    display = "Interest Installments",
                    value = ""
                });

                //Sanctiondate
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Sanctiondate",
                    display = "Sanction Date",
                    value = ""
                });

                //Method
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Method",
                    display = "Method",
                    value = ""
                });

                //Intrate
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Intrate",
                    display = "Interest Rate",
                    value = ""
                });


                //Instalamount
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Instalamount",
                    display = "Installment Amount",
                    value = ""
                });

                //Recoveramount
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Recoveramount",
                    display = "Recover Amount",
                    value = ""
                });

                //Completedinstall
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Completedinstall",
                    display = "CompletedInstallments",
                    value = ""
                });

                //Loanstartfrom
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Loanstartfrom",
                    display = "Loan Start From",
                    value = ""
                });


                //Loanvendorname
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Loanvendorname",
                    display = "Loan Vendor Name",
                    value = ""
                });

                //Active
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Active",
                    display = "Active",
                    value = ""
                });

                //Trans_id
                //lstEmpLoanData.Add(new Loans_Advances
                //{
                //    id = 0,
                //    dbcolumn = "Trans_id",
                //    display = "Transaction id",
                //    value = ""
                //});
                lstEmpLoanData.Add(new Loans_Advances
                {
                    id = 0,
                    dbcolumn = "Designation",
                    display = "Designation",
                    value = ""
                });
            }
            return lstEmpLoanData;
        }
        public string insertloans_advances(List<Loans_Advances1> Values)
        {
            string AlertMessage = "";
            try
            {
                string updQry = "";
                string insertQry = "";
                int Id = 0;
                int fy = 0;
                string fm = "";
                int EmpId = 0;
                int EmpCode = 0;
                int loantype = 0;
                int Totalamt = 0;
                int Noofinstall = 0;
                float Intinstallment = 0;
                string Sanctiondate = "";
                string Method = "";
                int Intrate = 0;
                int Instalamount = 0;
                int Recoveramount = 0;
                int Completedinstall = 0;
                string Loanstartfrom = "";
                string Loanvendorname = "";
                int Active = 0;
                int Trans_id = 0;
                int Designation = 0;
                foreach (var item in Values)
                {

                    if (item.dbcolumn == "Id")
                    {
                        Id = int.Parse(item.value);
                        updQry += " update PR_Emp_AdvLoans set Active=0 where Id=" + Id + ";";
                    }
                    else if (item.dbcolumn == "FY")
                    {
                        fy = int.Parse(item.value);
                        //insertQry = "(" + int.Parse(item.value) + ","
                        //var name = item.dbcolumn;
                        //obj.name = name;
                        //obj.value = item.value;
                    }

                    else if (item.dbcolumn == "FM")
                    {
                        fm = item.value;

                    }

                    else if (item.dbcolumn == "EmpId")
                    {
                        EmpId = int.Parse(item.value);


                    }
                    else if (item.dbcolumn == "EmpCode")
                    {
                        EmpCode = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "loantype")
                    {
                        loantype = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Totalamt")
                    {
                        Totalamt = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Noofinstall")
                    {
                        Noofinstall = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Intinstallment")
                    {
                        Intinstallment = float.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Sanctiondate")
                    {
                        Sanctiondate = item.value;

                    }
                    else if (item.dbcolumn == "Method")
                    {
                        Method = item.value;

                    }
                    else if (item.dbcolumn == "Intrate")
                    {
                        Intrate = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Instalamount")
                    {
                        Instalamount = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Recoveramount")
                    {
                        Recoveramount = int.Parse(item.value);
                    }

                    else if (item.dbcolumn == "Completedinstall")
                    {
                        Completedinstall = Convert.ToInt32(item.value);

                    }
                    else if (item.dbcolumn == "Loanstartfrom")
                    {
                        Loanstartfrom = item.value;

                    }
                    else if (item.dbcolumn == "Loanvendorname")
                    {
                        Loanvendorname = item.value;

                    }
                    else if (item.dbcolumn == "Active")
                    {
                        Active = 1;
                    }
                    else if (item.dbcolumn == "Trans_id")
                    {
                        Trans_id = int.Parse(item.value);

                    }
                    else if (item.dbcolumn == "Designation")
                    {
                        Designation = int.Parse(item.value);

                    }

                }

                insertQry += "INSERT INTO PR_Emp_AdvLoans VALUES(" + fy + ",'" + fm + "'," + EmpId + "," + EmpCode + "," + loantype + "," + Totalamt + "," + Noofinstall + "," + Intinstallment + ",'" + Sanctiondate + "'," + Method + "," + Intrate + "," + Instalamount + "," + Recoveramount + "," + Completedinstall + ",'" + Loanstartfrom + "','" + Loanvendorname + "'," + Active + "," + Trans_id + ","+Designation+");";

                sh.Run_UPDDEL_ExecuteNonQuery(updQry + insertQry);

                AlertMessage = "I#Required#Loans And Advances Data Updated Successfully.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return AlertMessage;
        }
        


        private string getLoansdata(List<Loans_Advances1> values)
        {
            string qryDetails = "";
            string retStr = "";

            qryDetails = "select id,FY,FM,EmpId,EmpCode,loantype,Totalamt,Noofinstall,Intinstallment,Sanctiondate,Method,Intrate,Instalamount,Recoveramount,Completedinstall,Loanstartfrom,Loanvendorname,Active,Trans_id from PR_Emp_AdvLoans where Active=1";

            DataTable dtEmpIds = sh.Get_Table_FromQry(qryDetails);

            foreach (DataRow dr in dtEmpIds.Rows)
            {
                retStr = retStr + dr["Id"].ToString() + "#" + dr["FY"].ToString()+ "#" + dr["FM"].ToString()+"#"+dr["EmpId"].ToString()+"#"+dr["EmpCode"].ToString() +"#" + dr["loantype"].ToString() + "#" + dr["Totalamt"].ToString() +"#"+dr["Noofinstall"].ToString()+"#"+dr["Intinstallment"].ToString()+
                    "#"+dr["Sanctiondate"].ToString()+"#"+dr["Method"].ToString()+"#"+dr["Intrate"].ToString()+"#"+dr["Instalamount"].ToString()+"#"+dr["Recoveramount"].ToString()+"#"+dr["Completedinstall"].ToString()+"#"+dr["Loanstartfrom"].ToString()+"#"+dr["Loanvendorname"].ToString()+"#"+dr["Active"].ToString()+"#"+dr["Trans_id"].ToString()+",";

            }
            return retStr;
        }

        public IList<Loans_Advances> getloans_advancesColumns()
        {
            //String query = "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'PR_Emp_AdvLoans'";
            //            String query = "select Pidloantype as LoanType,Totalamt as TotalAmount,Noofinstall as NoOfInstallMents,Intinstallment as InterestInstallments,Sanctiondate,+"
            //+"Method,Intrate as InterestRate,Instalamount as InstallmentAmount,Recoveramount as RecoveredAmount,Completedinstall as CompletedInstallments,+"
            //+"Loanstartfrom as LoanStartFrom,Loanvendorname as LoanVendorName  from PR_Emp_AdvLoans";
            string query = "select Id,FY,FM,EmpId,EmpCode,loantype ,Totalamt ,Noofinstall ,Intinstallment,Sanctiondate,Method,Intrate,Instalamount,Recoveramount,Completedinstall,Loanstartfrom,Loanvendorname,Active,Trans_id,Designation from PR_Emp_AdvLoans where Active=1";
            DataTable dt = sh.Get_Table_FromQry(query);

            IList<Loans_Advances> lstLonsAdv = new List<Loans_Advances>();
            if (dt.Rows.Count > 0)
            {
                //id
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Id",
                    display = "Id",
                    value = dt.Rows[0]["Id"].ToString()
                });

                ////FY
                //lstLonsAdv.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    dbcolumn = "FY",
                //    display = "FY",
                //    value = dt.Rows[0]["FY"].ToString()
                //});

                ////FM
                //lstLonsAdv.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    dbcolumn = "FM",
                //    display = "FM",
                //    value = dt.Rows[0]["FM"].ToString()
                //});


                //EmpId
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "EmpId",
                    display = "EmpId",
                    value = dt.Rows[0]["EmpId"].ToString()
                });

                //EmpCode
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "EmpCode",
                    display = "EmpCode",
                    value = dt.Rows[0]["EmpCode"].ToString()
                });



                //loantype
                lstLonsAdv.Add(new Loans_Advances
                {
                    id =Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "loantype",
                    display = "Loan Type",
                    value = dt.Rows[0]["loantype"].ToString()
                });
                //Totalamt
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Totalamt",
                    display = "Total Amount",
                    value = dt.Rows[0]["Totalamt"].ToString()
                });
                //Noofinstall
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Noofinstall",
                    display = "Number of installments",
                    value = dt.Rows[0]["Noofinstall"].ToString()
                });
              

                //Intinstallment
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Intinstallment",
                    display = "Interest Installments",
                    value = dt.Rows[0]["Intinstallment"].ToString()
                });

                //Sanctiondate
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Sanctiondate",
                    display = "Sanction Date",
                    value = dt.Rows[0]["Sanctiondate"].ToString()
                });

                //Method
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Method",
                    display = "Method",
                    value = dt.Rows[0]["Method"].ToString()
                });

                //Intrate
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Intrate",
                    display = "Interest Rate",
                    value = dt.Rows[0]["Intrate"].ToString()
                });


                //Instalamount
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Instalamount",
                    display = "Installment Amount",
                    value = dt.Rows[0]["Instalamount"].ToString()
                });

                //Recoveramount
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Recoveramount",
                    display = "Recover Amount",
                    value = dt.Rows[0]["Recoveramount"].ToString()
                });

                //Completedinstall
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Completedinstall",
                    display = "CompletedInstallments",
                    value = dt.Rows[0]["Completedinstall"].ToString()
                });

                //Loanstartfrom
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Loanstartfrom",
                    display = "Loan Start From",
                    value = dt.Rows[0]["Loanstartfrom"].ToString()
                });


                //Loanvendorname
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Loanvendorname",
                    display = "Loan Vendor Name",
                    value = dt.Rows[0]["Loanvendorname"].ToString()
                });

                //Active
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Active",
                    display = "Active",
                    value = dt.Rows[0]["Active"].ToString()
                });

                ////Trans_id
                //lstLonsAdv.Add(new Loans_Advances
                //{
                //    id = Convert.ToInt32(dt.Rows[0]["id"]),
                //    dbcolumn = "Trans_id",
                //    display = "Transaction id",
                //    value = dt.Rows[0]["Trans_id"].ToString()
                //});
                //Designation
                lstLonsAdv.Add(new Loans_Advances
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    dbcolumn = "Designation",
                    display = "Designation",
                    value = dt.Rows[0]["Designation"].ToString()
                });
            }
            return lstLonsAdv;
        }
    }

    public class Loans_Advances
    {
        public int id { get; set; }
        public string dbcolumn { get; set; }
        public string display { get; set; }
        public string value { get; set; }
    }

        public class EmployeeMaster
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
        }

    }
