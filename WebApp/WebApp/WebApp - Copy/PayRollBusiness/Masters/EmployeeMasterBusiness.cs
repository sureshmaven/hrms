using Mavensoft.Common;
using Mavensoft.DAL.Business;
using Newtonsoft.Json;
using PayrollModels;
using PayrollModels.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace PayRollBusiness.Masters
{
    public class EmployeeMasterBusiness : BusinessBase
    {
        public EmployeeMasterBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        Mavensoft.DAL.Db.SqlHelper sh = new Mavensoft.DAL.Db.SqlHelper();
        // Mavensoft.DAL.Business.BusinessBase bs = new Mavensoft.DAL.Business.BusinessBase();
        TypeFormat tf = new TypeFormat();

        public async Task<IList<CommonGetModel>> GetEmployeDeatilsById(string empid)
        {
            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            try {
                string empdata = "";
                string query = "select * from pr_emp_general WHERE active=1 AND emp_code=" + int.Parse(empid) + ";";
                //DataTable empDt = await _sha.Get_Table_FromQry(empQuery);

                DataTable dt = await _sha.Get_Table_FromQry(query);

                
                if (dt.Rows.Count > 0)
                {
                    empdata = "exec nulls " + int.Parse(empid) + ";";
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "action_type",
                        Value = "update"
                    });
                }
                else
                {
                    empdata = "exec exist " + int.Parse(empid) + ";";
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "action_type",
                        Value = "insert"
                    });
                }
                DataTable empdt = await _sha.Get_Table_FromQry(empdata);
                if (empdt.Rows.Count > 0)
                {
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "Gender",
                        Name = "Gender",
                        Value = empdt.Rows[0]["sex"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "martial_status",
                        Name = "Martial Status",
                        Value = empdt.Rows[0]["martial_status"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "zone",
                        Name = "Zone",
                        Value = empdt.Rows[0]["zone"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "designation",
                        Name = "Designation",
                        Value = empdt.Rows[0]["designation"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "designation_category",
                        Name = "Designation Category",
                        Value = empdt.Rows[0]["designation_category"].ToString()
                    });
                    //category and designation pending
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "region_for_p_tax",
                        Name = "Region For PTax",
                        Value = empdt.Rows[0]["region_for_p_tax"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "p_tax_region",
                        Name = "Ptax Region",
                        Value = empdt.Rows[0]["p_tax_region"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "address",
                        Name = "Address",
                        Value = empdt.Rows[0]["address"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "per_phoneno",
                        Name = "Phone No",
                        Value = empdt.Rows[0]["per_phoneno"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "per_address",
                        Name = "Permanent Address",
                        Value = empdt.Rows[0]["per_address"].ToString()
                    });
                    //personal phone number
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "native_place",
                        Name = "Native Place",
                        Value = empdt.Rows[0]["native_place"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "division",
                        Name = "Division",
                        Value = empdt.Rows[0]["division"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "dob",
                        Name = "Date of Birth",
                        Value = empdt.Rows[0]["dob"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "emp_age",
                        Name = "Employee Age",
                        Value = empdt.Rows[0]["emp_age"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "exp",
                        Name = "Experience",
                        Value = empdt.Rows[0]["exp"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "pf_no",
                        Name = "Provident Fund No",
                        Value = empdt.Rows[0]["pf_no"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "uan_no",
                        Name = "UAN",
                        Value = empdt.Rows[0]["uan_no"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "doj_pf",
                        Name = "Date Of Joining PF",
                        Value = empdt.Rows[0]["doj_pf"].ToString()
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "email_id",
                        Name = "Email Id",
                        Value = empdt.Rows[0]["email_id"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "identify_mark1",
                        Name = "Identification Mark 1",
                        Value = empdt.Rows[0]["identify_mark1"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "identify_mark2",
                        Name = "Identification Mark 2",
                        Value = empdt.Rows[0]["identify_mark2"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "blood_group",
                        Name = "Blood Group",
                        Value = empdt.Rows[0]["blood_group"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "religion",
                        Name = "Religion",
                        Value = empdt.Rows[0]["religion"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "cur_reservation",
                        Name = "Current Reservation",
                        Value = empdt.Rows[0]["cur_reservation"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "join_reservation",
                        Name = "Joining Reservation",
                        Value = empdt.Rows[0]["join_reservation"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "pan_no",
                        Name = "PAN No",
                        Value = empdt.Rows[0]["pan_no"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "reg_order",
                        Name = "REG Order",
                        Value = empdt.Rows[0]["reg_order"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "branch_code",
                        Name = "Branch Code",
                        Value = empdt.Rows[0]["branch_code"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "pay_bank",
                        Name = "Pay Bank",
                        Value = empdt.Rows[0]["pay_bank"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "account_code",
                        Name = "Account Code",
                        Value = empdt.Rows[0]["account_code"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "bank_accno",
                        Name = "Bank A/c No",
                        Value = empdt.Rows[0]["bank_accno"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "customer_id",
                        Name = "Customer ID",
                        Value = empdt.Rows[0]["customer_id"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "acc_with_dccb",
                        Name = "A.C With Br.DCCB",
                        Value = empdt.Rows[0]["acc_with_dccb"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "phy_handicapped",
                        Name = "PHY Handicapped",
                        Value = empdt.Rows[0]["phy_handicapped"].ToString()
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "house_provided",
                        Name = "House Provided",
                        Value = empdt.Rows[0]["house_provided"].ToString()
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "designation_no",
                        Name = "Designation No",
                        Value = empdt.Rows[0]["designation_no"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "stl_temp",
                        Name = "STLT EMP",
                        Value = empdt.Rows[0]["stl_temp"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "fest_adv",
                        Name = "FEST ADV",
                        Value = empdt.Rows[0]["fest_adv"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "artr_emp",
                        Name = "ARTR EMP",
                        Value = empdt.Rows[0]["artr_emp"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "aadhaar_no",
                        Name = "AADHAR NUMBER",
                        Value = empdt.Rows[0]["aadhaar_no"].ToString()
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "NPS",
                        Name = "NPS Opted",
                        Value = empdt.Rows[0]["NPS"].ToString()
                    });
                }
                else
                {
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Gender",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Martial Status",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Designation",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Designation Category",
                        Value = ""
                    });
                    
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Region For PTax",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "PTax Region ",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "address",
                        Value = ""
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Permanent address",
                        Value = ""
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Phone No",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Native Place",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Division",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "PF Number",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "UAN",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "DOJ PF",
                        Value = ""
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Email Id",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Identification Mark 1",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Identification Mark 2",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Blood Group",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Region",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Current Reservation",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Joining Reservation",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "PAN No",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Reg Order",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Branch Code",
                        Value = ""
                    });

                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Pay Bank",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Account Code",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Bank A/c No",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Customer ID",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "A.C With Br.DCCB",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Iff PHY Handicapped '1'",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "House Provided '1'",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Emp Age",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "Designation No",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "STLT EMP",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "FEST ADV",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "ARTR EMP",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "AADHAAR NUMBER",
                        Value = ""
                    });
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = "",
                        Name = "NPS Opted",
                        Value = ""
                    });
                }
                
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return lstEmpData;
        }


        public async Task<IList<CommonGetModel>> GetEmployeeBiologicalData(string empid)
        {

            string bioQuery = "select emp_id, emp_code,father_husband_name,[f/h_relation],format(father_dob,'yyyy-MM-dd') " +
                "as father_dob FROM pr_emp_biological_field WHERE emp_code =" + int.Parse(empid);
            DataTable dt = await _sha.Get_Table_FromQry(bioQuery);

            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            if (dt.Rows.Count > 0)
            {
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "action_type",
                    Value = "update"
                });

                lstEmpData.Add(new CommonGetModel
                {
                    Id = "father_husband_name",
                    Name = "Father Name",
                    Value = dt.Rows[0]["father_husband_name"].ToString()
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "father_dob",
                    Name = "Father Date Of Birth",
                    Value = dt.Rows[0]["father_dob"].ToString()
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "relation",
                    Name = "F or H Relation",
                    Value = dt.Rows[0]["f/h_relation"].ToString()
                });

            }
            else
            {
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "action_type",
                    Value = "insert"
                });

                lstEmpData.Add(new CommonGetModel
                {
                    Id = "father_husband_name",
                    Name = "Father Name",
                    Value = ""
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "father_dob",
                    Name = "Father Date Of Birth",
                    Value = ""
                });
                lstEmpData.Add(new CommonGetModel
                {
                    Id = "relation",
                    Name = "F or H Relation",
                    Value = ""
                });

            }
            return lstEmpData;
        }

        public async Task<string> GetEmployeePayFieldData(string empid)
        {

            string qryGetEFpayfields = "select ef.id as Id, ef.name as Name, epf.amount as Value, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_earn_field_master ef left outer join  pr_emp_pay_field " +
                 "epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + int.Parse(empid) +
                 " WHERE ef.type='pay_fields' and ef.active=1 and ef.name not in('Shift Duty Allowance');";

            DataTable dt = await _sha.Get_Table_FromQry(qryGetEFpayfields);
            var empPayFields = dt;

            var empjson = JsonConvert.SerializeObject(empPayFields);


            empjson = empjson.Replace("null", "''");

            return empjson;
        }

        public async Task<IList<CommonGetModel>> GetEmployeeLicData(string EmpCode)
        {

            string qryLic = "select id,account_no,amount,pay_type,pay_months,stop," +
                " stop_month " +
                "from pr_emp_lic_details where emp_code =" + int.Parse(EmpCode) + " and active=1";
            DataTable dt = await _sha.Get_Table_FromQry(qryLic);

            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = dr["id"].ToString(),
                        AccNum = dr["account_no"].ToString(),
                        Amount = dr["amount"].ToString(),
                        PayType = dr["pay_type"].ToString(),
                        PayMonths = dr["pay_months"].ToString(),
                        StartMonth = dr["stop"].ToString(),
                        StopMonth = dr["stop_month"].ToString(),
                        Action = "Add"
                    });
                }


            }
            
            return lstEmpData;
        }
        public async Task<IList<CommonGetModel>> GetEmployeeHfcData(string EmpCode)
        {

            string qryLic = "select id,account_no,amount,pay_type,pay_months,stop," +
                " stop_month " +
                "from pr_emp_hfc_details where emp_code =" + int.Parse(EmpCode) + " and active=1";
            DataTable dt = await _sha.Get_Table_FromQry(qryLic);

            IList<CommonGetModel> lstEmpData = new List<CommonGetModel>();
            if (dt.Rows.Count > 0)
            {

                foreach (DataRow dr in dt.Rows)
                {
                    lstEmpData.Add(new CommonGetModel
                    {
                        Id = dr["id"].ToString(),
                        AccNum = dr["account_no"].ToString(),
                        Amount = dr["amount"].ToString(),
                        PayType = dr["pay_type"].ToString(),
                        PayMonths = dr["pay_months"].ToString(),
                        StartMonth = dr["stop"].ToString(),
                        StopMonth = dr["stop_month"].ToString(),
                        Action = "Add"
                    });
                }


            }
            
            return lstEmpData;
        }

        public async Task<string> GetEmployeeAllowanceData(string empid)
        {

            string qryGetAllowanceData = "select distinct ef.id as Id, ef.name as Name, epf.amount as Value, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_emp_allowances_gen " +
                 "epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + int.Parse(empid) + " WHERE ef.type='EMPA' ;";

            DataTable dt = await _sha.Get_Table_FromQry(qryGetAllowanceData);
            var empAllowanceFields = dt;

            var empjson = JsonConvert.SerializeObject(empAllowanceFields);


            empjson = empjson.Replace("null", "''");

            return empjson;
        }

        public async Task<string> GetEmployeeAllowanceSpecialData(string empid)
        {

            string qryGetAllowanceData = "select ef.id as Id, ef.name as Name, epf.amount as Value, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_allowance_field_master ef left outer join  pr_emp_allowances_spl " +
                 "epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + int.Parse(empid) + " WHERE ef.type='EMPSA';";

            DataTable dt = await _sha.Get_Table_FromQry(qryGetAllowanceData);
            var empAllowanceSpecialFields = dt;

            var empjson = JsonConvert.SerializeObject(empAllowanceSpecialFields);


            empjson = empjson.Replace("null", "''");

            return empjson;
        }

        public async Task<string> GetEmployeeDeductionData(string empid)
        {

            string qryGetDeductionData = "select ef.id as Id, ef.name as Name, epf.amount as Value, " +
                "case when epf.m_id is null then 'N' else 'U' end as row_type " +
                 "from pr_deduction_field_master ef left outer join  pr_emp_deductions " +
                 "epf on ef.id = epf.m_id and epf.active = 1 and epf.emp_code = " + int.Parse(empid) + " WHERE ef.type='EPD';";

            DataTable dt = await _sha.Get_Table_FromQry(qryGetDeductionData);
            var empDeductionFields = dt;

            var empjson = JsonConvert.SerializeObject(empDeductionFields);


            empjson = empjson.Replace("null", "''");

            return empjson;
        }

        public async Task<string> InsertGeneralData(CommonPostDTO values)
        {
            var generalInfo = values.object1;
            var bioInfo = values.object2;
            var payInfo = values.object3;
            var allowanceInfo = values.object4;
            var speacialAllowanceInfo = values.object5;
            var deductionInfo = values.object6;
            var licInfo = values.multiObject;
            var hfcInfo = values.multiObject2;
            string qryLic = "";
            int emp_code = values.EntityId;
            string Gender = "";
            string martial_status = "";
            string zone = "";
            string designation_category = "";
            string designation = "";
            string region_for_p_tax = "";
            string p_tax_region = "";
            string address = "";
            string per_address = "";
            string per_phoneno = "";
            string native_place = "";
            string division = "";
            string pf_no = "";
            string uan_no = "";
            string doj_pf = "";
            string email_id = "";
            string identify_mark1 = "";
            string identify_mark2 = "";
            string blood_group = "";
            string religion = "";
            string cur_reservation = "";
            string join_reservation = "";
            string pan_no = "";
            int? reg_order = 0;
            string branch_code = "";
            string pay_bank = "";
            string account_code = "";
            string bank_accno = "";
            string acc_with_dccb = "";
            int? phy_handicapped = 0;
            int? house_provided = 0;
            int? emp_age = 0;
            int? designation_no = 0;
            string stl_temp = "";
            string fest_adv = "";
            string artr_emp = "";
            string aadhaar_no = "";
            string dob = "";
            string exp = "";
            string customer_id = "";
            string father_husband_name = "";
            string relation = "";
            int NewNumIndex = 0;
            string fdob = "";
            //int FY = DateTime.Now.Year + 1;
            //string FM = DateTime.Now.ToString("MM-dd-yyyy");

            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");

            StringBuilder sbqry = new StringBuilder();
            //1. trans_id
            sbqry.Append(GenNewTransactionString());

            if (generalInfo != null)
            {
                foreach (var emp in generalInfo)
                {
                    
                    if (emp.Id == "Gender")
                    {
                        Gender = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "martial_status")
                    {
                        martial_status = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "zone")
                    {
                        zone = Convert.ToString(emp.Value);
                    }
                    
                    if (emp.Id == "designation")
                    {
                        designation = Convert.ToString(emp.Value);
                    }
                    
                    if (emp.Id == "designation_category")
                    {
                        designation_category = Convert.ToString(emp.Value);
                    }
                    
                    if (emp.Id == "region_for_p_tax")
                    {
                        region_for_p_tax = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "p_tax_region")
                    {
                        p_tax_region = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "address")
                    {
                        address = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "per_address")
                    {
                        per_address = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "per_phoneno")
                    {
                        per_phoneno = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "native_place")
                    {
                        native_place = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "division")
                    {
                        division = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "pf_no")
                    {
                        pf_no = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "uan_no")
                    {
                        uan_no = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "doj_pf")
                    {
                        doj_pf = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "email_id")
                    {
                        email_id = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "identify_mark1")
                    {
                        identify_mark1 = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "identify_mark2")
                    {
                        identify_mark2 = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "blood_group")
                    {
                        blood_group = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "religion")
                    {
                        religion = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "cur_reservation")
                    {
                        cur_reservation = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "join_reservation")
                    {
                        join_reservation = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "pan_no")
                    {
                        pan_no = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "reg_order")
                    {
                        reg_order = Convert.ToInt32(emp.Value);
                    }
                    if (emp.Id == "branch_code")
                    {
                        branch_code = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "pay_bank")
                    {
                        pay_bank = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "account_code")
                    {
                        account_code = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "bank_accno")
                    {
                        bank_accno = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "customer_id")
                    {
                        customer_id = Convert.ToString(emp.Value);
                    }

                    if (emp.Id == "acc_with_dccb")
                    {
                        acc_with_dccb = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "phy_handicapped")
                    {
                        if (Convert.ToString(emp.Value) == "true")
                        {
                            phy_handicapped = 1;
                        }
                        else
                        {
                            phy_handicapped = 0;
                        }
                    }
                    if (emp.Id == "house_provided")
                    {
                        if (Convert.ToString(emp.Value) == "true")
                        {
                            house_provided = 1;
                        }
                        else
                        {
                            house_provided = 0;
                        }
                    }
                    if (emp.Id == "emp_age")
                    {
                        emp_age = Convert.ToInt32(emp.Value);
                    }
                    if (emp.Id == "designation_no")
                    {
                        designation_no = Convert.ToInt32(emp.Value);
                    }
                    if (emp.Id == "stl_temp")
                    {
                        stl_temp = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "fest_adv")
                    {
                        fest_adv = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "artr_emp")
                    {
                        artr_emp = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "aadhaar_no")
                    {
                        aadhaar_no = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "dob")
                    {
                        dob = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "exp")
                    {
                        exp = Convert.ToString(emp.Value);
                    }

                }
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_general", NewNumIndex));
                //3. qry
                if(designation_no == 0 && reg_order ==0)
                {
                    string qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code], " +
                    "[sex],[martial_status],[zone],[designation],[designation_category], " +
                    "[region_for_p_tax],[p_tax_region],[address],[per_address]," +
                    "[per_phoneno],[native_place] ,[division],[pf_no]," +
                    "[uan_no],[doj_pf],[email_id],[identify_mark1],[identify_mark2],[blood_group],[religion]," +
                    "[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code]," +
                    "[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped]," +
                    "[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                    "[dob],[exp],[active],[trans_id]) VALUES((select id from employees where empid = " + emp_code + ")," +
                    "" + FY + ",'" + FM + "'," +
                    "" + emp_code + ",'" + Gender + "'," +
                    "'" + martial_status + "','" + zone + "'," +
                    "'" + designation + "','" + designation_category + "'," +
                    "'" + region_for_p_tax + "'," +
                    "'" + p_tax_region + "','" + address + "'," +
                    "'" + per_address + "'," +
                    "" +
                    "'" + per_phoneno + "','" + native_place + "','" + division + "'," +
                    "" +
                    "'" + pf_no + "','" + uan_no + "',CASE WHEN '" + doj_pf + "' ='' THEN NULL ELSE '" + doj_pf + "' END," +
                    "'" + email_id + "','" + identify_mark1 + "'," +
                    "'" + identify_mark2 + "','" + blood_group + "'," +
                    "'" + religion + "','" + cur_reservation + "'," +
                    "'" + join_reservation + "','" + pan_no + "',null,'" + branch_code + "'," +
                    "" +
                    "'" + pay_bank + "','" + account_code + "'," +
                    "'" + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "'," +
                    "" + phy_handicapped + "," +
                    "" + house_provided + "," + emp_age + ",null,'" + stl_temp + "','" + fest_adv + "'," +
                    "'" + artr_emp + "'," +
                    "'" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                    sbqry.Append(qry);
                } else if(designation_no == 0)
                {
                    string qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code], " +
                    "[sex],[martial_status],[zone],[designation],[designation_category], " +
                    "[region_for_p_tax],[p_tax_region],[address],[per_address]," +
                    "[per_phoneno],[native_place] ,[division],[pf_no]," +
                    "[uan_no],[doj_pf],[email_id],[identify_mark1],[identify_mark2],[blood_group],[religion]," +
                    "[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code]," +
                    "[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped]," +
                    "[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                    "[dob],[exp],[active],[trans_id]) VALUES((select id from employees where empid = " + emp_code + ")," +
                    "" + FY + ",'" + FM + "'," +
                    "" + emp_code + ",'" + Gender + "'," +
                    "'" + martial_status + "','" + zone + "'," +
                    "'" + designation + "','" + designation_category + "'," +
                    "'" + region_for_p_tax + "'," +
                    "'" + p_tax_region + "','" + address + "'," +
                    "'" + per_address + "'," +
                    "" +
                    "'" + per_phoneno + "','" + native_place + "','" + division + "'," +
                    "" +
                    "'" + pf_no + "','" + uan_no + "',CASE WHEN '" + doj_pf + "' ='' THEN NULL ELSE '" + doj_pf + "' END," +
                    "'" + email_id + "','" + identify_mark1 + "'," +
                    "'" + identify_mark2 + "','" + blood_group + "'," +
                    "'" + religion + "','" + cur_reservation + "'," +
                    "'" + join_reservation + "','" + pan_no + "'," +
                    "" + reg_order + ",'" + branch_code + "'," +
                    "" +
                    "'" + pay_bank + "','" + account_code + "'," +
                    "'" + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "'," +
                    "" + phy_handicapped + "," +
                    "" + house_provided + "," + emp_age + ",null,'" + stl_temp + "','" + fest_adv + "'," +
                    "'" + artr_emp + "'," +
                    "'" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                    sbqry.Append(qry);

                } else if(reg_order == 0)
                {
                    string qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code], " +
                    "[sex],[martial_status],[zone],[designation],[designation_category], " +
                    "[region_for_p_tax],[p_tax_region],[address],[per_address]," +
                    "[per_phoneno],[native_place] ,[division],[pf_no]," +
                    "[uan_no],[doj_pf],[email_id],[identify_mark1],[identify_mark2],[blood_group],[religion]," +
                    "[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code]," +
                    "[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped]," +
                    "[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                    "[dob],[exp],[active],[trans_id]) VALUES((select id from employees where empid = " + emp_code + ")," +
                    "" + FY + ",'" + FM + "'," +
                    "" + emp_code + ",'" + Gender + "'," +
                    "'" + martial_status + "','" + zone + "'," +
                    "'" + designation + "','" + designation_category + "'," +
                    "'" + region_for_p_tax + "'," +
                    "'" + p_tax_region + "','" + address + "'," +
                    "'" + per_address + "'," +
                    "" +
                    "'" + per_phoneno + "','" + native_place + "','" + division + "'," +
                    "" +
                    "'" + pf_no + "','" + uan_no + "',CASE WHEN '" + doj_pf + "' ='' THEN NULL ELSE '" + doj_pf + "' END," +
                    "'" + email_id + "','" + identify_mark1 + "'," +
                    "'" + identify_mark2 + "','" + blood_group + "'," +
                    "'" + religion + "','" + cur_reservation + "'," +
                    "'" + join_reservation + "','" + pan_no + "',null,'" + branch_code + "'," +
                    "" +
                    "'" + pay_bank + "','" + account_code + "'," +
                    "'" + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "'," +
                    "" + phy_handicapped + "," +
                    "" + house_provided + "," + emp_age + "," +
                    "" + designation_no + ",'" + stl_temp + "','" + fest_adv + "'," +
                    "'" + artr_emp + "'," +
                    "'" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                    sbqry.Append(qry);
                } else
                {
                    string qry = "INSERT INTO pr_emp_general ([emp_id],[fy],[fm],[emp_code], " +
                    "[sex],[martial_status],[zone],[designation],[designation_category], " +
                    "[region_for_p_tax],[p_tax_region],[address],[per_address]," +
                    "[per_phoneno],[native_place] ,[division],[pf_no]," +
                    "[uan_no],[doj_pf],[email_id],[identify_mark1],[identify_mark2],[blood_group],[religion]," +
                    "[cur_reservation],[join_reservation],[pan_no],[reg_order],[branch_code]," +
                    "[pay_bank],[account_code],[bank_accno],[customer_id],[acc_with_dccb],[phy_handicapped]," +
                    "[house_provided],[emp_age],[designation_no],[stl_temp],[fest_adv],[artr_emp],[aadhaar_no]," +
                    "[dob],[exp],[active],[trans_id]) VALUES((select id from employees where empid = " + emp_code + ")," +
                    "" + FY + ",'" + FM + "'," +
                    "" + emp_code + ",'" + Gender + "'," +
                    "'" + martial_status + "','" + zone + "'," +
                    "'" + designation + "','" + designation_category + "'," +
                    "'" + region_for_p_tax + "'," +
                    "'" + p_tax_region + "','" + address + "'," +
                    "'" + per_address + "'," +
                    "" +
                    "'" + per_phoneno + "','" + native_place + "','" + division + "'," +
                    "" +
                    "'" + pf_no + "','" + uan_no + "',CASE WHEN '" + doj_pf + "' ='' THEN NULL ELSE '" + doj_pf + "' END," +
                    "'" + email_id + "','" + identify_mark1 + "'," +
                    "'" + identify_mark2 + "','" + blood_group + "'," +
                    "'" + religion + "','" + cur_reservation + "'," +
                    "'" + join_reservation + "','" + pan_no + "'," + reg_order +",'" + branch_code + "'," +
                    "" +
                    "'" + pay_bank + "','" + account_code + "'," +
                    "'" + bank_accno + "','" + customer_id + "','" + acc_with_dccb + "'," +
                    "" + phy_handicapped + "," +
                    "" + house_provided + "," + emp_age + "," +
                    "" + designation_no + ",'" + stl_temp + "','" + fest_adv + "'," +
                    "'" + artr_emp + "'," +
                    "'" + aadhaar_no + "','" + dob + "','" + exp + "',1,@transidnew);";
                    sbqry.Append(qry);
                }

                

               

                //4. transaction touch
                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_general", "@idnew" + NewNumIndex, emp_code.ToString()));
            }
            if (bioInfo != null)
            {
                foreach (var emp in bioInfo)
                {
                    if (emp.Id == "father_husband_name")
                    {
                        father_husband_name = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "relation")
                    {
                        relation = Convert.ToString(emp.Value);
                    }
                    if (emp.Id == "father_dob")
                    {
                        if (Convert.ToString(emp.Value) == null)
                        {
                            fdob = null;
                        }
                        else
                        {
                            fdob = Convert.ToString(emp.Value);
                        }

                    }
                }
                NewNumIndex++;
                sbqry.Append(GetNewNumStringArr("pr_emp_biological_field", NewNumIndex));

                string bioQry = "INSERT INTO pr_emp_biological_field ([emp_code],[emp_id],[fy],[fm]," +
                    "[father_husband_name],[father_dob],[f/h_relation],[active],[trans_id]) " +
                    "VALUES(" + emp_code + ",(select id from employees where empid=" + emp_code + ")," +
                 "" + FY + ",'" + FM + "','" + father_husband_name + "',CASE WHEN '" + fdob + "' ='' THEN NULL ELSE '" + fdob + "' END," +
                 "'" + relation + "',1,@transidnew);";
                sbqry.Append(bioQry);

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_biological_field", "@idnew" + NewNumIndex, emp_code.ToString()));
            }
            if (licInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in licInfo)
                {
                    if (emp.Action == "New")
                    {
                        emp_amount = Convert.ToDecimal(emp.Amount);
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_lic_details", NewNumIndex));
                        //3. qry

                        qryLic = "INSERT INTO pr_emp_lic_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no]," +
                            "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) " +
                            "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," +
                       "(select id from employees where empid=" + emp_code + ")," + emp_code + "," +
                       "'" + Convert.ToString(emp.AccNum) + "'," + emp_amount + "," +
                       "'" + Convert.ToString(emp.PayType) + "','" +
                       Convert.ToString(emp.PayMonths) + "',CASE WHEN  '" + Convert.ToString(emp.StartMonth) + "' = '' THEN NULL ELSE " + "'" + Convert.ToString(emp.StartMonth) + "'  END ," +
                       "CASE WHEN  '" + Convert.ToString(emp.StopMonth) + "' = '' THEN NULL ELSE " + "'" +
                       Convert.ToString(emp.StopMonth) + "'  END ,1,@transidnew);";

                        sbqry.Append(qryLic);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_lic_details", "@idnew" + NewNumIndex, ""));
                    }
                    else if (emp.Action == "Update")
                    {
                        //qryLic = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "'," +
                        //    "amount=" + Convert.ToInt32(emp.Amount) + "" +
                        //    ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months=" +
                        //    Convert.ToInt32(emp.PayMonths) + "," +
                        //    "start_month='" + Convert.ToString(emp.StartMonth) + "',stop_month='" +
                        //    Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        //sbqry.Append(qryLic);

                        if (Convert.ToString(emp.StartMonth) == null && Convert.ToString(emp.StopMonth) != null)
                        {
                            qryLic = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) != null)
                        {
                            qryLic = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) == null)
                        {
                            qryLic = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else
                        {
                            qryLic = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        sbqry.Append(qryLic);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_lic_details", emp.Id.ToString(), ""));
                    }
                    else if (emp.Action == "Deleted")
                    {
                        qryLic = " Update pr_emp_lic_details SET active=0 where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        sbqry.Append(qryLic);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_lic_details", emp.Id.ToString(), ""));
                    }
                }
            }
            if (hfcInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in hfcInfo)
                {
                    if (emp.Action == "New")
                    {
                        emp_amount = Convert.ToDecimal(emp.Amount);
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_hfc_details", NewNumIndex));
                        //3. qry

                        qryLic = "INSERT INTO pr_emp_hfc_details ([id],[fy],[fm],[emp_id]," +
                            "[emp_code],[account_no],[amount],[pay_type],[pay_months],[stop],[stop_month]," +
                            "[active],[trans_id]) VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," +
                       "(select id from employees where empid=" + emp_code + ")," + emp_code + "," +
                       "'" + Convert.ToString(emp.AccNum) + "'," + emp_amount + ",'" +
                       Convert.ToString(emp.PayType) + "','" +
                       Convert.ToString(emp.PayMonths) + "',CASE WHEN  '" + Convert.ToString(emp.StartMonth) + "' = '' " +
                       "THEN NULL ELSE " + "'" + Convert.ToString(emp.StartMonth) + "'  END ," +
                       "CASE WHEN  '" + Convert.ToString(emp.StopMonth) + "' = '' THEN NULL ELSE " + "'" +
                       Convert.ToString(emp.StopMonth) + "'  END ,1,@transidnew);";

                        sbqry.Append(qryLic);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_hfc_details", "@idnew" + NewNumIndex, ""));
                    }
                    else if (emp.Action == "Update")
                    {
                        //qryLic = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "'," +
                        //    "amount=" + Convert.ToInt32(emp.Amount) + "" +
                        //    ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months=" + Convert.ToInt32(emp.PayMonths) + "," +
                        //    "start_month='" + Convert.ToString(emp.StartMonth) + "',stop_month='" +
                        //    Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        //sbqry.Append(qryLic);
                        if (Convert.ToString(emp.StartMonth) == null && Convert.ToString(emp.StopMonth) != null)
                        {
                            qryLic = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) != null)
                        {
                            qryLic = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) == null)
                        {
                            qryLic = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else
                        {
                            qryLic = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToInt32(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        sbqry.Append(qryLic);
                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_hfc_details", emp.Id.ToString(), ""));
                    }
                    else if (emp.Action == "Deleted")
                    {
                        qryLic = " Update pr_emp_hfc_details SET active=0 where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        sbqry.Append(qryLic);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_hfc_details", emp.Id.ToString(), ""));
                    }
                }
            }
            if (payInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in payInfo)
                {
                    emp_amount = Convert.ToDecimal(emp.Value);
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                    //3. qry
                    string qry = "Insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                        "[fm],[m_id],[m_type],[amount],[active],[trans_id]) values "
                        + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                        "" + emp_code + "," + FY + ",'" + FM + "'," + emp.Id + ",'Pay_fields', " + emp_amount + ",1, @transidnew);";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                }


            }
            if (allowanceInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in allowanceInfo)
                {
                    emp_amount = Convert.ToDecimal(emp.Value);
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                    //3. qry
                    string qry = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                        "[m_id],[m_type],[amount],[active],[trans_id]) values "
                        + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + "," + FY + ",'" + FM + "'," +
                        "" + emp.Id + ",'EMPA', " + emp_amount + ",1, @transidnew);";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));
                }


            }
            if (speacialAllowanceInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in speacialAllowanceInfo)
                {
                    emp_amount = Convert.ToDecimal(emp.Value);
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                    //3. qry
                    string qry = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                        "[m_id],[m_type],[amount],[active],[trans_id]) values "
                        + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                        "" + emp_code + "," + FY + ",'" + FM + "'," + emp.Id + ",'EMPA', " + emp_amount + ",1, @transidnew);";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                }


            }
            if (deductionInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in deductionInfo)
                {
                    emp_amount = Convert.ToDecimal(emp.Value);
                    NewNumIndex++;
                    //2. gen new num
                    sbqry.Append(GetNewNumStringArr("pr_emp_deductions", NewNumIndex));

                    //3. qry
                    string qry = "Insert into pr_emp_deductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id]," +
                        "[m_type],[amount],[active],[trans_id]) values "
                        + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                        " where empid=" + emp_code + ")," + emp_code + "," + emp.Id + ",'EPD', " + emp_amount + ",1, @transidnew);";
                    sbqry.Append(qry);

                    //4. transaction touch
                    sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deductions", "@idnew" + NewNumIndex, ""));
                }


            }
            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Employee Master#New Employee Data Inserted Successfully..!!";
            }
            else
            {
                return "E#Employee Master#Error While Employee Data Insertion";
            }

        }
        public async Task<string> UpdateGeneralData(CommonPostDTO Values)
        {

            string uqry = "";
            string bioQry = "";
            int emp_code = Values.EntityId;
            int phy_handicapped = 0;
            int house_provided = 0;
            var generalData = Values.object1;
            var bioData = Values.object2;
            //var licInfo = Values.object3;
            var licInfo = Values.multiObject;
            var hfcInfo = Values.multiObject2;
            string qry = "";
            int NewNumIndex = 0;
            //int FY = DateTime.Now.Year + 1;
            //string FM = DateTime.Now.ToString("MM-dd-yyyy");

            int FY = _LoginCredential.FY;
            string FM = _LoginCredential.FinancialMonthDate.ToString("yyyy-MM-dd");
            StringBuilder sbqry = new StringBuilder();
            sbqry.Append(GenNewTransactionString());
            if (generalData != null)
            {
                foreach (var emp in generalData)
                {

                    if (emp.Id == "emp_code")
                    {
                        emp_code = Convert.ToInt32(emp.Value);
                    }
                    if (emp.Id == "phy_handicapped")
                    {
                        if (Convert.ToString(emp.Value) == "True")
                        {
                            phy_handicapped = 1;
                        }
                        else
                        {
                            phy_handicapped = 0;
                        }
                    }
                    if (emp.Id == "house_provided")
                    {
                        if (Convert.ToString(emp.Value) == "True")
                        {
                            house_provided = 1;
                        }
                        else
                        {
                            house_provided = 0;
                        }
                    }

                    if (emp.Id == "emp_code" || emp.Id == "reg_order" || emp.Id == "emp_age" || emp.Id == "designation_no")
                    {
                        uqry = uqry + "," + emp.Id + " =" + Convert.ToInt32(emp.Value) + "";
                    }
                    else if (emp.Id == "phy_handicapped")
                    {
                        uqry = uqry + "," + emp.Id + " =" + phy_handicapped + "";
                    }
                    else if (emp.Id == "house_provided")
                    {
                        uqry = uqry + "," + emp.Id + " =" + house_provided + "";
                    } 
                    else if(emp.Id == "doj_pf")
                    {
                        if(Convert.ToString(emp.Value) == null)
                        {
                            uqry = uqry + "," + emp.Id + " = null ";
                        } else
                        {
                            uqry = uqry + "," + emp.Id + " ='" + Convert.ToString(emp.Value) + "'";
                        }

                    }
                    else
                    {
                        uqry = uqry + "," + emp.Id + " ='" + Convert.ToString(emp.Value) + "'";
                    }
                }

                uqry = uqry.Substring(1);

                string qry1 = "Update pr_emp_general SET " + uqry + " where emp_code=" + emp_code + "  and fm='"+FM+"'";
                sbqry.Append(qry1);

                //3. transaction touch

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_general", emp_code.ToString(), "emp_master"));
            }

            if (bioData != null)
            {
                foreach (var emp in bioData)
                {
                    if (emp.Id == "father_husband_name")
                    {
                        bioQry = bioQry + "," + emp.Id + " ='" + Convert.ToString(emp.Value) + "'";
                    }
                    else if (emp.Id == "relation")
                    {
                        bioQry = bioQry + ",[f/h_relation]='" + Convert.ToString(emp.Value) + "'";
                    }
                    if (emp.Id == "emp_code" || emp.Id == "reg_order" || emp.Id == "emp_age" || emp.Id == "designation_no")
                    {
                        //uqry = uqry + "," + emp.Id + " =" + Convert.ToInt32(emp.Value) + "";
                    }
                    else if (emp.Id == "father_dob")
                    {
                        if (Convert.ToString(emp.Value) == null)
                        {
                            bioQry = bioQry + "," + emp.Id + " = null ";
                        }
                        else
                        {
                            bioQry = bioQry + "," + emp.Id + " ='" + Convert.ToString(emp.Value) + "'";
                        }

                    }
                }
                bioQry = bioQry.Substring(1);


                string updateBio = "Update pr_emp_biological_field SET " + bioQry + " where emp_code=" + emp_code + " ";
                sbqry.Append(updateBio);

                sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_biological_field", emp_code.ToString(), "emp_master"));
            }
            if (licInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in licInfo)
                {
                    if (emp.Action == "New")
                    {
                        emp_amount = Convert.ToDecimal(emp.Amount);
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_lic_details", NewNumIndex));
                        //3. qry
                        qry = "INSERT INTO pr_emp_lic_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no]," +
                            "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," +
                       "(select id from employees where empid=" + emp_code + ")," + emp_code + "," +
                       "'" + Convert.ToString(emp.AccNum) + "'," + emp_amount + ",'" +
                       Convert.ToString(emp.PayType) + "','" +
                       Convert.ToString(emp.PayMonths) + "',CASE WHEN  '" + Convert.ToString(emp.StartMonth) + "' = '' " +
                       "THEN NULL ELSE " + "'" + Convert.ToString(emp.StartMonth) + "'  END ," +
                       "CASE WHEN  '" + Convert.ToString(emp.StopMonth) + "' = '' THEN NULL ELSE " + "'" + Convert.ToString(emp.StopMonth) + "'  END ,1,@transidnew);";

                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_lic_details", "@idnew" + NewNumIndex, ""));
                    }
                    else if (emp.Action == "Update")
                    {
                        if (Convert.ToString(emp.StartMonth) == null  && Convert.ToString(emp.StopMonth) != null)
                        {
                            qry = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        } else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) != null) {
                            qry = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) == null)
                        {
                            qry = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else
                        {
                            qry = " Update pr_emp_lic_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_lic_details", emp.Id.ToString(), ""));
                    }
                    else if (emp.Action == "Deleted")
                    {
                        qry = " Update pr_emp_lic_details SET active=0 where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_lic_details", emp.Id.ToString(), ""));
                    }
                }
            }
            if (hfcInfo != null)
            {
                decimal? emp_amount = 0;
                foreach (var emp in hfcInfo)
                {
                    if (emp.Action == "New")
                    {
                        emp_amount = Convert.ToDecimal(emp.Amount);
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_hfc_details", NewNumIndex));
                        //3. qry
                        qry = "INSERT INTO pr_emp_hfc_details ([id],[fy],[fm],[emp_id],[emp_code],[account_no]," +
                            "[amount],[pay_type],[pay_months],[stop],[stop_month],[active],[trans_id]) " +
                            "VALUES(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "'," +
                       "(select id from employees where empid=" + emp_code + ")," + emp_code + "," +
                       "'" + Convert.ToString(emp.AccNum) + "'," + emp_amount + ",'" + Convert.ToString(emp.PayType) + "','" +
                       Convert.ToString(emp.PayMonths) + "',CASE WHEN  '" + Convert.ToString(emp.StartMonth) + "' = '' " +
                       "THEN NULL ELSE " + "'" + Convert.ToString(emp.StartMonth) + "'  END ," +
                       "CASE WHEN  '" + Convert.ToString(emp.StopMonth) + "' = '' THEN NULL ELSE " + "'" +
                       Convert.ToString(emp.StopMonth) + "'  END ,1,@transidnew);";

                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_hfc_details", "@idnew" + NewNumIndex, ""));
                    }
                    else if (emp.Action == "Update")
                    {
                        //qry = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "'," +
                        //    "amount=" + Convert.ToInt32(emp.Amount) + "" +
                        //    ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months=" + Convert.ToInt32(emp.PayMonths) + "," +
                        //    "start_month='" + Convert.ToString(emp.StartMonth) + "',stop_month='" +
                        //    Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        //sbqry.Append(qry);

                        if (Convert.ToString(emp.StartMonth) == null && Convert.ToString(emp.StopMonth) != null)
                        {
                            qry = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) != null)
                        {
                            qry = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else if (Convert.ToString(emp.StopMonth) == null && Convert.ToString(emp.StartMonth) == null)
                        {
                            qry = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop=null,stop_month=null where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        else
                        {
                            qry = " Update pr_emp_hfc_details SET account_no='" + Convert.ToString(emp.AccNum) + "',amount=" + Convert.ToDecimal(emp.Amount) + "" +
                            ",pay_type='" + Convert.ToString(emp.PayType) + "',pay_months='" + Convert.ToString(emp.PayMonths) + "'," +
                            "stop='" + Convert.ToString(emp.StartMonth) + "',stop_month='" + Convert.ToString(emp.StopMonth) + "'where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        }
                        sbqry.Append(qry);
                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_hfc_details", emp.Id.ToString(), ""));
                    }
                    else if (emp.Action == "Deleted")
                    {
                        qry = " Update pr_emp_hfc_details SET active=0 where id=" + emp.Id + " AND Emp_Code=" + emp_code + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.D, "pr_emp_hfc_details", emp.Id.ToString(), ""));
                    }
                }
            }

            //pay fields
            if (Values.StringData != null)
            {
                string[] EfarrRows = Values.StringData.Split('~');

                foreach (string rdata in EfarrRows)
                {
                    var arrData = rdata.Split('=');

                    var arrTypId = arrData[0].Split('^'); //N^1
                    var type = arrTypId[0];
                    var pkid = arrTypId[1];

                    var arrVals = arrData[1].Split('^'); //456^400
                    var newVal = arrVals[0];
                    var oldVal = arrVals[1];

                    //string qry = "";
                    if (type == "N")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_pay_field", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_pay_field ([id],[emp_id],[emp_code],[fy]," +
                            "[fm],[m_id],[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                            "" + emp_code + "," + FY + ",'" + FM + "'," + pkid + ",'Pay_fields', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_pay_field", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                    }
                    //qry = "Update pr_emp_pay_field SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //sbqry.Append(qry);

                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_pay_field", pkid.ToString(), oldVal));
                }
            }
            //allowance
            if (Values.StringData2 != null)
            {
                string[] EfarrRows = Values.StringData2.Split('~');

                foreach (string rdata in EfarrRows)
                {
                    var arrData = rdata.Split('=');

                    var arrTypId = arrData[0].Split('^'); //N^1
                    var type = arrTypId[0];
                    var pkid = arrTypId[1];

                    var arrVals = arrData[1].Split('^'); //456^400
                    var newVal = arrVals[0];
                    var oldVal = arrVals[1];
                    if (type == "N")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_allowances_gen", NewNumIndex));

                        //3. qry
                         qry = "Insert into pr_emp_allowances_gen ([id],[emp_id],[emp_code],[fy],[fm]," +
                            "[m_id],[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," + emp_code + "," + FY + ",'" + FM + "'," +
                            "" + pkid + ",'EMPA', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_gen", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_allowances_gen SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_allowances_gen SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", pkid.ToString(), oldVal));
                    }

                    //qry = "Update pr_emp_allowances_gen SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //sbqry.Append(qry);

                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_gen", pkid.ToString(), oldVal));
                }
            }
            //speacial allowance
            if (Values.StringData3 != null)
            {
                string[] EfarrRows = Values.StringData3.Split('~');

                foreach (string rdata in EfarrRows)
                {
                    var arrData = rdata.Split('=');

                    var arrTypId = arrData[0].Split('^'); //N^1
                    var type = arrTypId[0];
                    var pkid = arrTypId[1];

                    var arrVals = arrData[1].Split('^'); //456^400
                    var newVal = arrVals[0];
                    var oldVal = arrVals[1];

                    if (type == "N")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_allowances_spl", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_allowances_spl ([id],[emp_id],[emp_code],[fy],[fm]," +
                            "[m_id],[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + ",(select id from employees where empid=" + emp_code + ")," +
                            "" + emp_code + "," + FY + ",'" + FM + "'," + pkid + ",'EMPA', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_allowances_spl", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_allowances_spl SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_allowances_spl SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", pkid.ToString(), oldVal));
                    }

                    //qry = "Update pr_emp_allowances_spl SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //sbqry.Append(qry);

                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_allowances_spl", pkid.ToString(), oldVal));
                }
            }
            //deduction
            if (Values.StringData4 != null)
            {
                string[] EfarrRows = Values.StringData4.Split('~');

                foreach (string rdata in EfarrRows)
                {
                    var arrData = rdata.Split('=');

                    var arrTypId = arrData[0].Split('^'); //N^1
                    var type = arrTypId[0];
                    var pkid = arrTypId[1];

                    var arrVals = arrData[1].Split('^'); //456^400
                    var newVal = arrVals[0];
                    var oldVal = arrVals[1];

                    if (type == "N")
                    {
                        NewNumIndex++;
                        //2. gen new num
                        sbqry.Append(GetNewNumStringArr("pr_emp_deductions", NewNumIndex));

                        //3. qry
                        qry = "Insert into pr_emp_deductions ([id],[fy],[fm],[emp_id],[emp_code],[m_id]," +
                            "[m_type],[amount],[active],[trans_id]) values "
                            + "(@idnew" + NewNumIndex + "," + FY + ",'" + FM + "',(select id from employees" +
                            " where empid=" + emp_code + ")," + emp_code + "," + pkid + ",'EPD', " + newVal + ",1, @transidnew);";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.I, "pr_emp_deductions", "@idnew" + NewNumIndex, ""));
                    }
                    else if (type == "U" && newVal != "") //update
                    {
                        qry = "Update pr_emp_deductions SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deductions", pkid.ToString(), oldVal));
                    }
                    else if (type == "U" && newVal == "") //delete
                    {
                        qry = "Update pr_emp_deductions SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                        sbqry.Append(qry);

                        //4. transaction touch
                        sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deductions", pkid.ToString(), oldVal));
                    }

                    //qry = "Update pr_emp_deductions SET amount=" + newVal + ", trans_id=@transidnew where m_id=" + pkid + " AND emp_code=" + Values.EntityId + " ;";
                    //sbqry.Append(qry);

                    ////4. transaction touch
                    //sbqry.Append(GetTransactionTouchStringArr(Transaction_Touch_Type.U, "pr_emp_deductions", pkid.ToString(), oldVal));
                }
            }

            if (await _sha.Run_UPDDEL_ExecuteNonQuery(sbqry.ToString()))
            {
                return "I#Employee Master#Employee Data Updated Successfully..!!";
            }
            else
            {
                return "E#Employee Master#Error While Employee Data Updation";
            }

        }
    }

}
