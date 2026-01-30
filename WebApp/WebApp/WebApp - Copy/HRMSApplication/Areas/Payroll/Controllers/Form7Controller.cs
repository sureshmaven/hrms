using HRMSApplication.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using Org.BouncyCastle.OpenSsl;
using PayRollBusiness;
using PayRollBusiness.Masters;
using PayRollBusiness.Reports;
using PayrollModels;
using PayrollModels.Masters;
using Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
//using System.Web.UI;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    public class Form7Controller : Controller
    {
        Form7Model form7Data;
        CommonBusiness CommonBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());

        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        Form7Business form7B = new Form7Business(LoginHelper.GetCurrentUserForPR());


        [HttpGet]
        public async Task<string> SearchEmployee(string EmpCode)
        {
            return await CommonBus.SearchEmployee(EmpCode);
        }

        // get financial period
        public async Task<string> getFy()
        {
            string qryfy = "select fm as fm_fy from pr_month_details where active=1;";
            int fm_fy = Convert.ToInt32(qryfy);
            string fyear = "";

            int fy = fm_fy - 1;



            fyear = (fy + "-" + fm_fy).ToString();



            return fyear;

        }

        public FileResult generatePdf(string EmpCode)
        {
            string[] arrEmpId = EmpCode.Split(',');
            foreach (string empId in arrEmpId)
            {
                //return await CreatePdf(empId);
            }
            return File("", "application/pdf", "");
        }

        public char[] ConvertNames(string name)
        {
            char[] array = name.ToCharArray();
            // Handle the first letter in the string.  
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.  
            // ... Uppercase the lowercase letters following spaces.  
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
                else if (char.IsUpper(array[i - 1]) || array[i - 1] != ' ')
                {
                    array[i] = char.ToLower(array[i]);
                }
            }

            return array;
        }
        //Create Excel
        public async Task <ActionResult> CreateExcel(string EmpCode, string fy, string btntype)
        {

            string[] arrEmpId = EmpCode.Split(',');
            Mavensoft.DAL.Db.SqlHelper _sha = new Mavensoft.DAL.Db.SqlHelper();
            StringBuilder sb_status = new StringBuilder("");
            //StringBuilder status1 = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string FileName = "";
            MemoryStream workStream = new MemoryStream();
            string fYear = fy;
            int fy1 = -1;
            string[] arr = fYear.Split('-');
            // fYear = (fy1 + "-" + fm_fy).ToString();
            if (arrEmpId.Length > 1)
            {
                FileName = string.Format("All-" + fYear + "-Form7.xls");

            }
            else
            {
                foreach (var empId in arrEmpId)
                {
                    FileName = string.Format(empId + "(" + fYear + ") year-Form7.xls");

                }
            }

            sb_status.Append("<table border=1 style='font - family:HELVETICA' align='center'>");
            sb_status.Append("<tr id='trhead'><td colspan='8'><h2 align='center'>");
            sb_status.Append("Telangana State Co-Operative Apex Bank Ltd </h2></td></tr>");
            sb_status.Append("<tr id='trhead1'><td colspan='8'>");
            sb_status.Append("<h4 align ='center' > State Govt.Partnered Scheduled Bank </ h4></td></tr>");
            sb_status.Append("<tr><td colspan='8'><h3 align='center'> Form 7(PS) </h3></td></tr></table>");

            foreach (string empId in arrEmpId)
            {
                string fYear1 = fYear;
                Form7Model form7Data = await form7B.GetForm7Data(empId, fYear1);
                IList<contributionmodel> form7_CB = form7Data.sect1;
                int cb_ded = form7_CB.Count;


                sb_status.Append("<table border=1 style='font - family:HELVETICA' align='center'>");
                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'>1.  Account No </td>");
                sb_status.Append("<td colspan='4'> AP/2505/" + form7Data.pf_no + "</td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'>2.  Name /Surname </td>");
                sb_status.Append("<td colspan='4'> " + form7Data.EmpName + "(" + form7Data.EmpCode + ") </td>");
                sb_status.Append("</tr>");

                if (form7Data.father != "" || form7Data.father != null)
                {
                    sb_status.Append("<tr>");
                    sb_status.Append("<td colspan='4'>3. Father/Husband Name </td>");
                    sb_status.Append("<td colspan='4'> " + form7Data.father + "</td>");
                    sb_status.Append("</tr>");

                }
                else if (form7Data.Spouse != "" || form7Data.Spouse != null)
                {
                    sb_status.Append("<tr>");
                    sb_status.Append("<td colspan='4'>3. Father/Husband Name </td>");
                    sb_status.Append("<td colspan='4'> " + form7Data.Spouse + "</td>");
                    sb_status.Append("</tr>");
                }

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'> 4. Name and Address of The Establishment </td>");
                sb_status.Append("<td colspan='4'> TSCAB </td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td td colspan='4'> 5. Statutory rate of Contribution </td>");
                sb_status.Append("<td td colspan='4' align='left'> 8.33% </td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'> 6. Voluntary Higher rate of Contribution if any </td>");
                sb_status.Append("<td colspan='4'>  </td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'> 7. Date of Commencement of Membership of EPS </td>");
                sb_status.Append("<td colspan='4'> 16/11/1995 </td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'> A Rounding of Contribution to the nearest of One Rupee. </td>");
                sb_status.Append("<td colspan='4'>  </td>");
                sb_status.Append("</tr>");

                sb_status.Append("<tr>");
                sb_status.Append("<td colspan='4'> B Certified the total amount of contributions" +
                    "indicated in the EPS Card i.e., Toal Rs."+ form7Data.totalpffund + "/- " +
                    "has been already remitted in full in EPS A/c  No.10 <br>"+
                    "(Employees Pension Contribution Account)</td>");
                sb_status.Append("<td colspan='4'>  </td>");
                sb_status.Append("</tr>");
                // status.Append("</table> <br><br><br>");

                //status.Append("<table border=1>");
                sb_status.Append("<tr align='center' id='trhead2'><td colspan=8><h4></h4></td></tr>");
                sb_status.Append("<tr align='center' id='trhead3'><td colspan=8><h4>For Telangana State Co-Operative Apex Bank Ltd.</h4></td></tr>");
                sb_status.Append("<tr align='left' id='trhead4'>");
                sb_status.Append("<td colspan=4><h4>Official Seal</h4></td>");
                sb_status.Append("<td colspan=4 align='right'><h4>Asst General Manager (Payments)</h4></td></tr>");

                sb_status.Append("<th bgcolor='lightcyan' colspan='2' id='trhead5'>Month</th>");
                sb_status.Append("<th bgcolor='lightcyan' colspan='2' id='trhead6'>Amount of Wages(Rs) </th>");
                sb_status.Append("<th bgcolor='lightcyan' colspan='4' id='trhead7'>Contribution To The Pension Fund(Employer Share) </th>");
                //status.Append("<th>Month</th>");
                //status.Append("<th>Amount of Wages(Rs) </th>");
                //status.Append("<th>Contribution To The Pension Fund(Employer Share) </th>");

                for (int i = 0; i < cb_ded; i++)
                {
                    if (form7_CB.Count > 0)
                    {
                        sb_status.Append("<tr>");
                        sb_status.Append("<td colspan='2'>" + form7_CB[i].month+ "</td>");
                        sb_status.Append("<td colspan='2'>" + form7_CB[i].amount + "</td>");
                        sb_status.Append("<td colspan='4'>" + form7_CB[i].con_pensoinfund + "</td>");
                        sb_status.Append("</tr>");
                    }
                    
                }

                sb_status.Append("<tr>");
                sb_status.Append("<td bgcolor='lightcyan' colspan='2'><b>Total</b></td>");
                sb_status.Append("<td bgcolor='lightcyan' colspan='2'><b>" + form7Data.totalamount.ToString()+ "</b></td>");
                sb_status.Append("<td bgcolor='lightcyan' colspan='4'><b>" + form7Data.totalpffund.ToString()+ "</b></td>");
                sb_status.Append("</tr>");

                sb_status.Append("</table><br>");

            }
            
            this.Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
            this.Response.ContentType = "application/excel";
            //this.Response.ContentType = "application/vnd.ms-excel";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sb_status.ToString());
            return File(buffer, this.Response.ContentType);

        }
        //Create Payslip PDF
        public async Task<FileResult> CreatePdf(string EmpCode, string fy, string btntype)
        {

            string[] arrEmpId = EmpCode.Split(',');
            Mavensoft.DAL.Db.SqlHelper _sha = new Mavensoft.DAL.Db.SqlHelper();
            //DataTable getidsfrompaysliptable;
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            string FileName = "";

            //To get financial period


            string fYear = fy;

            int fy1 = -1;

            string[] arr = fYear.Split('-');

            // fYear = (fy1 + "-" + fm_fy).ToString();
            if (arrEmpId.Length > 1)
            {
                FileName = string.Format("All-" + fYear + "-Form7.pdf");
            }

            else
            {
                foreach (var empId in arrEmpId)
                {
                    FileName = string.Format(empId + "(" + fYear + ") year-Form7.pdf");
                }
            }

            //for adding images
            string imageURL = Server.MapPath("/Assets/images/PDFLogo.png");
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            jpg.ScaleToFit(500f, 500f);

            //Give space before image

            jpg.SpacingBefore = 3f;

            //Give some space after the image
            
            jpg.SpacingAfter = 1f;

            jpg.Alignment = Element.ALIGN_LEFT;
            Document doc = new Document();
            PdfPTable table = new PdfPTable(1);
            PdfPTable tableLayout2 = new PdfPTable(1);
            PdfPTable tableLayout3 = new PdfPTable(2);
            PdfPTable tableLayout = new PdfPTable(3);

            string strAttachment = Server.MapPath("~/Downloadss/");


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();






            foreach (string empId in arrEmpId)
            {

                string fYear1 = fYear;
                Form7Model form7Data = await form7B.GetForm7Data(empId, fYear1);

                


                doc.Add(jpg);

                doc.Add(Heading(table, form7Data));
                doc.Add(space(tableLayout2, form7Data));
                table.DeleteBodyRows();

                doc.Add(empaccountDetails(tableLayout3, form7Data));
                //doc.Add(Labelforseal(table, form7Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(space(tableLayout2, form7Data));
                tableLayout2.DeleteBodyRows();
                doc.Add(Summary(tableLayout2, form7Data));
                tableLayout2.DeleteBodyRows();
                doc.Add(space(tableLayout2, form7Data));
                tableLayout2.DeleteBodyRows();
                doc.Add(quaterheading(tableLayout, form7Data));
                tableLayout.DeleteBodyRows();
                doc.Add(detailsBody(tableLayout, form7Data.sect1, form7Data));
                tableLayout.DeleteBodyRows();

                doc.Add(total(tableLayout, form7Data));
                tableLayout.DeleteBodyRows();

                doc.Add(space(tableLayout2, form7Data));
                tableLayout2.DeleteBodyRows();

                Response.Clear();
                doc.NewPage();

                //}
            }
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(byteInfo);
                Font blackFont = FontFactory.GetFont("HELVETICA", 10, Font.BOLD, BaseColor.BLACK);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        //PdfImportedPage page = stamper.GetImportedPage(reader, i);
                        //stamper.InsertPage(3,);
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 568f, 15f, 0);
                        tableLayout.FooterRows = 1;
                    }
                }
                byteInfo = stream.ToArray();
            }
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;

            if (btntype == "Download PDF")
            {
                return File(workStream, "application/pdf", FileName);
            }
            else
            {

                Response.AppendHeader("content-disposition", "inline; filename=" + FileName);
                return new FileStreamResult(workStream, "application/pdf");
            }
        }
        public PdfPTable Heading(PdfPTable table, Form7Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            table.SetWidths(headers); //Set the pdf headers  
            table.WidthPercentage = 100; //Set the PDF File witdh percentage  
            table.HeaderRows = 1;
            table.SpacingAfter = 0;


            table.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1(table, "Form No  - 7(PS)  ");
            AddCellToBody1(table, "");
            table.FooterRows = 1;

            return table;
        }


        public PdfPTable Label(PdfPTable tableLayout5, Form7Model data)
        {
            float[] headers = { 100 }; //Header Widths  
            tableLayout5.SetWidths(headers); //Set the pdf headers  
            tableLayout5.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout5.HeaderRows = 1;
            tableLayout5.SpacingAfter = 30;

            tableLayout5.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout5.FooterRows = 1;

            AddCellToBody3(tableLayout5, " System Generated Report Doesn't Require Stamp & Signature ");
            return tableLayout5;
        }

        //empDetails
        public PdfPTable empaccountDetails(PdfPTable tableLayout3, Form7Model data)
        {
            PdfPTable tableLayout2 = new PdfPTable(1);
            //float[] headers = { 60 }; //Header Widths  
            //tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 0;

            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.COURIER, 5, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;
            AddCellToBody(tableLayout3, "1.  Account No ");
            AddCellToBody(tableLayout3, "AP/2505/" + data.pf_no + " ");
            AddCellToBody(tableLayout3, "2.  Name /Surname ");
            AddCellToBody(tableLayout3, "" + data.EmpName + " (" + data.EmpCode + ")");
            if (data.father != "" || data.father != null)
            {
                AddCellToBody(tableLayout3, "3.Father/Husband Name ");
                AddCellToBody(tableLayout3, "" + data.father + "");
            }
            else if (data.Spouse != "" || data.Spouse != null)
            {
                AddCellToBody(tableLayout3, "3.Father/Husband Name ");
                AddCellToBody(tableLayout3, " " + data.Spouse + " ");
            }
            AddCellToBody(tableLayout3, "4.Name and Address of The Establishment");
            AddCellToBody(tableLayout3, "TSCAB");
            AddCellToBody(tableLayout3, "5.Statutory rate of Contribution ");
            AddCellToBody(tableLayout3, "8.33 %");
            AddCellToBody(tableLayout3, "6.Voluntary Higher rate of Contribution if any ");
            AddCellToBody(tableLayout3, " ");
            AddCellToBody(tableLayout3, "7.Date of Commencement of Membership of EPS");
            AddCellToBody(tableLayout3, "  16/11/1995");
            AddCellToBody(tableLayout3, "  A Rounding of Contribution to the nearest of One Rupee.");
            AddCellToBody(tableLayout3, " ");
            AddCellToBody(tableLayout3, "  B Certified the total amount of contributions indicated in the EPS Card i.e., Toal Rs. " + data.totalpffund + " /- has been already remitted in full in EPS A/c  No.10(Employees Pension Contribution Account)");
            AddCellToBody(tableLayout3, " ");
            /*  AddCellToBody(tableLayout3, "   )")*/
            ;
            //AddCellToBody(tableLayout3, "");
            //AddCellToBody(tableLayout3, "  No.10(Employees Pension Contribution Account");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBodyempdetails(tableLayout3, "For Telangana State Co-Operative Apex Bank Ltd.");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBody(tableLayout3, "");
            //AddCellToBodyempdetails(tableLayout3, "Official Seal");

            return tableLayout3;
        }



        //Summary
        public PdfPTable Summary(PdfPTable tableLayout2, Form7Model data)
        {
            Document doc = new Document();
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 0;

            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });

            AddCellToBody1(tableLayout2, "");
            AddCellToBodyHead(tableLayout2, "For Telangana State Co-Operative Apex Bank Ltd.");
            AddCellToBody1(tableLayout2, " Official Seal                                                                      Asst General Manager (Payments)");
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "");
            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //table heading
        public PdfPTable quaterheading(PdfPTable tableLayout, Form7Model data)
        {
            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingAfter = 3;


            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout.FooterRows = 1;

            AddCellToHeader(tableLayout, "Month");

            AddCellToHeader(tableLayout, "Amount of Wages(Rs)");

            AddCellToHeader(tableLayout, "Contribution To The Pension Fund(Employer Share)");




            return tableLayout;
        }


        public PdfPTable detailsBody(PdfPTable tableLayout, IList<contributionmodel> lstDeductions, Form7Model data)
        {
            int dedLen = lstDeductions.Count;

            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingAfter = 3;


            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout.FooterRows = 1;
            for (int i = 0; i < dedLen; i++)
            {
                if (lstDeductions.Count > 0)
                {



                    AddCellToBody(tableLayout, lstDeductions[i].month);
                    AddCellToBody(tableLayout, lstDeductions[i].amount);
                    AddCellToBody(tableLayout, lstDeductions[i].con_pensoinfund);

                }
            }

            return tableLayout;
        }

        public PdfPTable space(PdfPTable tableLayout2, Form7Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 20;


            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1(tableLayout2, "");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }


        public PdfPTable total(PdfPTable tableLayout, Form7Model data)
        {
            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingAfter = 3;


            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout.FooterRows = 1;

            AddCellToHeader(tableLayout, "Total");

            AddCellToHeader(tableLayout, data.totalamount.ToString());

            AddCellToHeader(tableLayout, data.totalpffund.ToString());




            return tableLayout;
        }




        public PdfPTable Rules_(PdfPTable tableLayout2, Form7Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 40;


            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });

            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }








        // for tabel cell data display
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                //Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(230, 255, 230)
            });
        }
        private static void AddCellToHeader1(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                //Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(230, 255, 230)
            });
        }
        public const int BOTTOM_BORDER = 2;
        public const int LEFT_BORDER = 2;
        public const int RIGHT_BORDER = 2;
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,

                //Border = PdfPCell.BOTTOM_BORDER & LEFT_BORDER & RIGHT_BORDER,
                //Border = PdfPCell.LEFT_BORDER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBodyempdetails(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
                //Border = PdfPCell.BOTTOM_BORDER & LEFT_BORDER & RIGHT_BORDER,
                //Border = PdfPCell.LEFT_BORDER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBodyHead(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                //Border = PdfPCell.BOTTOM_BORDER & LEFT_BORDER & RIGHT_BORDER,
                //Border = PdfPCell.LEFT_BORDER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody1(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 12, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                Padding = 1,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }

        private static void AddCellToBody1declaration(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
                Padding = 3,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }

        private static void AddCellToBody1NB(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER,
                Padding = 1,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody1sig(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,
                Padding = 1,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,

                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody3(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }


    }
}