using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mavensoft.DAL.Business;
using PayrollModels;
using PayrollModels.Masters;
using Newtonsoft.Json;
using System.Data;

namespace PayRollBusiness.Reports
{
    public class PocReportBusiness : BusinessBase
    {
        public PocReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {
        }

        public IList<Poc1Model> GetPoc1Data()
        {
            IList<Poc1Model> lst = new List<Poc1Model>();

            //1
            Poc1Model pm = new Poc1Model
            {
                RowId=1,
                HRF="H",
                SlNo = "<span style='color:#C8EAFB'>~</span>Branch: <span style='margin-right:30px;color:red;'><b>Malakpet</b></span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 2,
                HRF = "R",
                SlNo = "S.No",
                EmpCode = "Code",
                EmpName = "Emp. Name",
                Designation = "Designation",
                GrossSalary = "Gross Salary",
                Deductions = "Deductions",
                NetSalary = "Net Salary"
            };
            lst.Add(pm);

            //2
            pm = new Poc1Model
            {
                RowId = 3,
                HRF = "R",
                SlNo = "1",
                EmpCode = "476",
                EmpName = "CH Vijay Kumar",
                Designation = "Manager SC1",
                GrossSalary = "110018.41",
                Deductions = "48866.00",
                NetSalary = "61152.41"
            };
            lst.Add(pm);

            //3
            pm = new Poc1Model
            {
                RowId = 4,
                HRF = "R",
                SlNo = "2",
                EmpCode = "963",
                EmpName = "P Rajini",
                Designation = "Manager SC1",
                GrossSalary = "70000.00",
                Deductions = "20000.00",
                NetSalary = "50000.00"
            };
            lst.Add(pm);

            //branch footer data
            pm = new Poc1Model
            {
                RowId = 5,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>An amount of rupees 418199.57 be debited to the establishment account of your branch as detailed hereunder"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 6,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>a) Total Gross Salary",
                NetSalary = "394626.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 7,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>b) Bank contribution towards"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 8,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>(i) Provident Fund",
                NetSalary = "23573.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 9,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span><span style='padding-left: 30px;'>(ii) Family Pension<span>",
                NetSalary = "0.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 10,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>Total",
                NetSalary = "123456.00"
            };
            lst.Add(pm);

            //1
            pm = new Poc1Model
            {
                RowId = 11,
                HRF = "H",
                SlNo = "<span style='color:#C8EAFB'>~</span>Branch: <span style='margin-right:30px;color:red;'><b>Ameerpet</b></span><span style='margin-left: 30px;'>Payment Dt: <b>25-06-2019</b></span>"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 12,
                HRF = "R",
                SlNo = "S.No",
                EmpCode = "Code",
                EmpName = "Emp. Name",
                Designation = "Designation",
                GrossSalary = "Gross Salary",
                Deductions = "Deductions",
                NetSalary = "Net Salary"
            };
            lst.Add(pm);
                                  

            //2
            pm = new Poc1Model
            {
                RowId = 13,
                HRF = "R",
                SlNo = "1",
                EmpCode = "476",
                EmpName = "CH Reddy",
                Designation = "Manager SC1",
                GrossSalary = "110018.41",
                Deductions = "48866.00",
                NetSalary = "61152.41"
            };
            lst.Add(pm);

            //3
            pm = new Poc1Model
            {
                RowId = 14,
                HRF = "R",
                SlNo = "2",
                EmpCode = "963",
                EmpName = "P Rajini",
                Designation = "Manager SC1",
                GrossSalary = "70000.00",
                Deductions = "20000.00",
                NetSalary = "50000.00"
            };
            lst.Add(pm);

            //branch footer data
            pm = new Poc1Model
            {
                RowId = 15,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>An amount of rupees 418199.57 be debited to the establishment account of your branch as detailed hereunder abcd ef ghf dklfjd "
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 16,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>a) Total Gross Salary",
                NetSalary = "394626.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 17,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>b) Bank contribution towards"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 18,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>(i) Provident Fund",
                NetSalary = "23573.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 19,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span><span style='padding-left: 30px;'>(ii) Family Pension<span>",
                NetSalary = "0.00"
            };
            lst.Add(pm);

            pm = new Poc1Model
            {
                RowId = 20,
                HRF = "F",
                SlNo = "<span style='color:#eef8fd'>^</span>Total",
                NetSalary = "123456.00"
            };
            lst.Add(pm);


            return lst;
        }
    }

    public class Poc1Model
    {
        public int RowId { get; set; }
        public string HRF { get; set; }
        public string SlNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string GrossSalary { get; set; }
        public string Deductions { get; set; }
        public string NetSalary { get; set; }
    }
}
