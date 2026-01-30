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
   public class Form12BReportBusiness: BusinessBase
    {
        public Form12BReportBusiness(LoginCredential loginCredential) : base(loginCredential)
        {

        }
        int RowCnt = 0;
        int SNo = 1;
        string codes = "";
        string oldempid = "";
        string empid = "";
        string fy1 = "";
        string fy = "";
        string oldempid1 = "";
        IList<CommonReportModel> lst = new List<CommonReportModel>();
        CommonReportModel crm = new CommonReportModel();
        public async Task<IList<CommonReportModel>> GetForm12BGroupingReports(string empCode)
        {
            string qry = "";
            //var empid = "0";
            string empcodes = empCode;
            //string[] empids = empcodes.Split(',');
            //foreach (var id in empids)
            //{
            //empid = id;

            if (empCode.Contains("^"))
            {
                empcodes = "0";

            }
            qry = "select distinct ie.fy,year(ie.fm) as fy1, ie.emp_code,im.nature_of_perq as Natureofperquisite,ie.perq_amt as valueofperquisite," +
                "ie.rec_amt as Recoveredamount,ie.tax_amt as TaxAmount ,eg.pan_no,eg.designation,e.ShortName,e.FatherName " +
                "from pr_emp_incometax_12ba_master im inner join pr_emp_incometax_12ba ie on im.id = ie.m_id " +
                "join pr_emp_general eg on eg.emp_code = ie.emp_code " +
                "join Employees e on e.empid = ie.emp_code ";

            if (empCode != "All")
            {
                qry += " where ie.emp_code in (" + empcodes + ") order by ie.emp_code ";

            }

            else if (empCode == "All")
            {
                qry += " order by ie.emp_code ";
            }

            DataTable dt = await _sha.Get_Table_FromQry(qry);
            int count = 0;
         
            foreach (DataRow dr in dt.Rows)
            {
                count++;
                empid = dr["emp_code"].ToString();
                fy1 = dr["fy1"].ToString();
                fy = dr["fy"].ToString();
                int count1 = dt.Select().Where(s => s["emp_code"].ToString() == empid).Count();
                if (oldempid != empid)
                {
                  //header
                    var grpdata = dr["emp_code"].ToString();
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "D",
                        grpclmn = "<span style='color:#d5e6c2'>%</span>"
                        + ReportColHeader(0, "Emp Code", dr["emp_code"].ToString()),
                        //column2="",
                        //column3="",
                        //column4=""

                    };
                    lst.Add(crm);

                   
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                      + ReportColHeader(0, "Statement showing particulars of perquisities,other fringe benefits or amenities and profits in lieu of salary with value thereof", "")

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                       + ReportColHeader(0, "1.Name and address of the employer ", "TGCAB,Hyderabad")

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                      + ReportColHeader(0, "2. TAN                                  ", "HYDT06401D")

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                    + ReportColHeader(0, "3. TDS Assessment Range of the employer ", "")

                    };
                    lst.Add(crm);


                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                    + ReportColHeader(0, "4. Name,designation and PAN of employee ", dr["ShortName"].ToString())
                    + ReportColHeader(0, " Designation: ", dr["designation"].ToString())
                    + ReportColHeader(0, " PAN No : ", dr["pan_no"].ToString())
                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                    + ReportColHeader(0, "5. Is the employee a director or a person <br> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; with substantial interest in the company <br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(where the employer is a company) ", " NO")

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                + ReportColHeader(0, "6. Income under the head 'Salaries' of <br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; the employee: (other than perquisites) ", "815792.12")

                    };

                    lst.Add(crm);
                    string fyear= fy1 +"-"+ fy;
                     crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                + ReportColHeader(0, "7. Financial Year   ", fyear)

                    };
                    lst.Add(crm);


                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                      + ReportColHeader(0, "8. Valuation of Perquisites ", "")

                    };
                    lst.Add(crm);

                    
                }

                oldempid = dr["emp_code"].ToString();

                //rows
                crm = new CommonReportModel
                {
                    RowId = RowCnt++,
                    HRF = "R",
                    //grpclmn = SNo++.ToString(),
                    grpclmn = dr["Natureofperquisite"].ToString(),
                    column2 = ReportColConvertToDecimal(dr["valueofperquisite"].ToString()),
                    column3 =ReportColConvertToDecimal( dr["Recoveredamount"].ToString()),
                    column4 = ReportColConvertToDecimal( dr["TaxAmount"].ToString()),
                   

                };
                lst.Add(crm);

                //footer
                    
                    if (count == count1)
                    {
                       count = 0;
                       oldempid ="";
                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>^</span>"
                 + ReportColFooter(0, "9.Details of Tax. ", "")

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>^</span>"
                            + ReportColFooter(0, "   a. Tax deducted from salary of the employee u/s 192(1)  ", " 10000.00")

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>^</span>"
                            + ReportColHeader(0, "   b. Tax paid by employer on behalf of the employee u/s 192(1A)   ", "  0.00")

                        };
                        lst.Add(crm);

                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>^</span>"
                            + ReportColFooter(0, "  c. Total Tax Paid    ", "10000.00")

                        };
                        lst.Add(crm);


                        crm = new CommonReportModel
                        {
                            RowId = RowCnt++,
                            HRF = "H",
                            grpclmn = "<span style='color:#C8EAFB'>^</span>"
                            + ReportColFooter(0, "    d. Date of Payment into Government treasury   ", "")

                        };

                        lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                        + ReportColFooter(0, " DECLARATION BY EMPLOYER  ", "")

                    };

                    lst.Add(crm);

                    string ename = dr["ShortName"].ToString();
                    string fname = dr["FatherName"].ToString();
                    string disg = dr["designation"].ToString();
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                       + ReportColFooter(0, "    I AM  "+ ename + " son of LATE "+ fname + " working as  "+disg+" <br> do hereby declare on behalf of TEST that the information given above is <br> based on the books of account, documents and other relevant records or information <br> available with us and the details of value of each such perquisite are in <br> accordance with section 17 and rules framed there under and that such information <br> is true and correct. ", "")

                    };

                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                     + ReportColFooterValueOnly(0, "")

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                       + ReportColFooter(0, " <span>Signature of the Person Responsible <br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; for deduction of Tax</span>  ", "")

                    };

                    lst.Add(crm);


                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                      + ReportColFooter(0, "Place     ", "HYDERABAD")

                    };
                     lst.Add(crm);
                    string date =Convert.ToString(DateTime.Now.ToString("dd/MM/yyyy"));
                    
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                      + ReportColFooter(0, "Date       ", date)

                    };
                    lst.Add(crm);

                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                    + ReportColFooter(0, "  Full Name          ", dr["ShortName"].ToString())

                    };
                    lst.Add(crm);
                    crm = new CommonReportModel
                    {
                        RowId = RowCnt++,
                        HRF = "H",
                        grpclmn = "<span style='color:#C8EAFB'>^</span>"
                     + ReportColFooter(0, "  Designation        ", dr["designation"].ToString())

                    };
                    lst.Add(crm);

                    //crm = new CommonReportModel
                    //{
                    //    RowId = RowCnt++,
                    //    HRF = "D",
                    //    grpclmn = "<span style='color:#9B870C'>%</span>"
                    // + ReportColFooterValueOnly(0,"")
                    
                    //};
                    //lst.Add(crm);
                }


            }
            return lst;

        }

        public string ReportColConvertToDecimal(string value)
        {

            if (value == "")
            {
                value = "0";
            }
            decimal Drvalue = Convert.ToDecimal(value.ToString()) + 0.00M;
            decimal DPT = Convert.ToDecimal(String.Format("{0:0.00}", Drvalue));
            string NwDPT = String.Format("{0:n}", DPT);


            return NwDPT;
        }


    }
}
