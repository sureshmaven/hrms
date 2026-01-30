using HRMSApplication.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using Org.BouncyCastle.OpenSsl;
using PayRollBusiness;
using PayRollBusiness.Masters;
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
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using payslippdfmodel = PayrollModels.payslippdfmodel;

namespace HRMSApplication.Areas.Payroll.Controllers
{
    [Authorize]
    public class NegativeNetSalaryController : Controller
    {
        // GET: Payroll/NegativeNetSalary
        PayslipPdf pdfData;
        CommonBusiness CommonBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());

        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        NegativeNetSalaryBusiness pdf = new NegativeNetSalaryBusiness(LoginHelper.GetCurrentUserForPR());
        // GET: Payroll/PayslipPdf
        public ActionResult Mainmethod()
        {
            ViewBag.Env_Fm_Fy = commBus.Env_Fm_Fy();
            ViewBag.SectionName = "Masters";
            return View();
        }

        [HttpGet]
        public async Task<string> SearchEmployee(string EmpCode)
        {
            return await CommonBus.SearchEmployee(EmpCode);
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

        //Create Payslip PDF
        public async Task<FileResult> CreatePdf(string EmpCode, string pdftypes)
        {
            string emp_code = "";
            string dfm = "";
            string[] arrEmpId = EmpCode.Split(',');
            Mavensoft.DAL.Db.SqlHelper _sha = new Mavensoft.DAL.Db.SqlHelper();
            //DataTable getidsfrompaysliptable;
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //string Env_Fm_Fy = "todo";// commBus.Env_Fm_Fy1();
            //file name to be created 
            string strPDFFileName = "";


            DateTime fm = pdf._LoginCredential.FinancialMonthDate;
            string all_fm = fm.ToString("MMMM");
            if (pdftypes.Contains("Regular") && pdftypes.Contains("Supended") && pdftypes.Contains("stopsalary"))
            {
                pdftypes = "";
            }

            else if (pdftypes.Contains("Regular") && pdftypes.Contains("Supended"))
            {
                pdftypes = "";
            }
            else if (pdftypes.Contains("Supended") && pdftypes.Contains("stopsalary"))
            {
                pdftypes = "";
            }
            else if (pdftypes.Contains("Regular") && pdftypes.Contains("stopsalary"))
            {
                pdftypes = "";
            }
            else if (pdftypes.Contains("Regular"))
            {
                pdftypes = "";
            }
            else if (pdftypes.Contains("Supended"))
            {
                pdftypes = "";
            }
            else if (pdftypes.Contains("stopsalary"))
            {
                pdftypes = "";
            }
            if (arrEmpId.Length > 1)
            {
                strPDFFileName = string.Format("All Payslip-" + pdftypes + ".pdf");
            }
            else
            {
                DataTable GetEmpcode = _sha.Get_Table_FromQry("select emp_code,format(fm,'MMMM') as fm from  pr_emp_payslip_netSalary where id=" + EmpCode + ";");

                if (GetEmpcode.Rows.Count > 0)
                {
                    DataRow dr = GetEmpcode.Rows[0];
                    emp_code = dr["emp_code"].ToString();
                    dfm = dr["fm"].ToString();
                }
                if (dfm == "January")
                {
                    dfm = "Jan";
                }
                if (dfm == "February")
                {
                    dfm = "Feb";
                }
                if (dfm == "March")
                {
                    dfm = "Mar";
                }
                if (dfm == "April")
                {
                    dfm = "Apr";
                }
                if (dfm == "May")
                {
                    dfm = "May";
                }
                if (dfm == "June")
                {
                    dfm = "Jun";
                }
                if (dfm == "July")
                {
                    dfm = "Jul";
                }
                if (dfm == "August")
                {
                    dfm = "Aug";
                }
                if (dfm == "September")
                {
                    dfm = "Sep";
                }

                if (dfm == "October")
                {
                    dfm = "Oct";
                }
                if (dfm == "November")
                {
                    dfm = "Nov";
                }
                if (dfm == "December")
                {
                    dfm = "Dec";
                }


                strPDFFileName = string.Format(emp_code + "-" + dfm + "-Payslip" + ".pdf");
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
            PdfPTable tableLayout = new PdfPTable(4);
            PdfPTable tableLayout1 = new PdfPTable(4);
            PdfPTable tableLayout1n = new PdfPTable(4);
            PdfPTable tableLayout2 = new PdfPTable(4);
            PdfPTable tableLayout3 = new PdfPTable(1);
            PdfPTable tableLayout4 = new PdfPTable(4);
            PdfPTable tableLayout5 = new PdfPTable(1);

            //PdfPTable tableLayout3 = new PdfPTable(2);
            //PdfPTable tableLayout4 = new PdfPTable(4);

            PdfPTable outer = new PdfPTable(2);
            //outer.AddCell(new PdfPCell(tableLayout2));
            //outer.AddCell(new PdfPCell(tableLayout3));
            //PdfPTable outer = new PdfPTable(2);
            //outer.AddCell(tableLayout);
            //outer.AddCell(tableLayout1);
            //outer.AddCell(tableLayout2);
            //outer.AddCell(tableLayout3);

            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //DateTime str = Convert.ToDateTime(Months);
            //string str1 = str.ToString("MM");
            //string str2 = str.ToString("yyyy");
            string pdftype = "";
            //('stopsalary','Encashment','Regular')
            //if (pdftypes != "")
            //{
            //    string[] split_pdftype = pdftypes.Split(',');
            //    pdftype = "'";
            //    foreach (string pdt in split_pdftype)
            //    {
            //        pdftype = pdftype + pdt + "','";
            //    }
            //    pdftype = pdftype.Substring(0, pdftype.LastIndexOf(",'"));
            //}
            //if (EmpCode == "All" || EmpCode == "")
            //{
            //    EmpCode = "";
            //    getidsfrompaysliptable = _sha.Get_Table_FromQry("select emp_code from pr_emp_payslip where Month(fm)='" + str1 + "' and Year(fm)='" + str2 + "' and spl_type in (" + pdftype + ");");
            //    if (getidsfrompaysliptable.Rows.Count > 0)
            //    {
            //        foreach (DataRow dr in getidsfrompaysliptable.Rows)
            //        {
            //            //string str;
            //            EmpCode += dr["emp_code"] + ",";

            //        }
            //    }
            //    EmpCode = EmpCode.Remove(EmpCode.Length - 1, 1);
            //}


            //string[] arrEmpId = EmpCode.Split(',');




            foreach (string empId in arrEmpId)
            {
                //string[] split_pdftype = pdftypes.Split(',');
                //var pdftype = "'";

                //pdftype = pdftype.Substring(0, pdftype.LastIndexOf(",'"));
                //foreach (var Ptypes in split_pdftype)
                //{
                //DataTable findrecord_empcode_paysliptype = _sha.Get_Table_FromQry("select emp_code from pr_emp_payslip where emp_code="+ empId + " Month(fm)='" + str1 + "' and Year(fm)='" + str2 + "' and spl_type in (" + Ptypes + ");");
                //Paragraph p = new Paragraph("PAYSLIP");
                //p.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);

                //p.IndentationRight = 200;
                //p.IndentationLeft = 200;
                //p.SpacingAfter = 20;

                //doc.Add(p);



                //get data from database

                PayslipPdf pdfData = await pdf.getPdfDetails(empId);



                //Add Content to PDF   

                //Empllyee details table

                doc.Add(jpg);
                doc.Add(monthdetails(table, pdfData));
                //if (Ptypes == "Adhoc")
                //{
                //    Paragraph p = new Paragraph();
                //    Chunk c1 = new Chunk("(" + Ptypes + ")");

                //    c1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
                //    //c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);
                //    //c1.HorizontalScaling(Element.ALIGN_CENTER);
                //    p.Alignment = Element.ALIGN_CENTER;

                //    c1.SetHorizontalScaling(Element.ALIGN_CENTER);
                //    ////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
                //    ////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
                //    //c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
                //    p.Add(c1);
                //    PdfDiv div = new PdfDiv();
                //    div.AddElement(p);
                //    doc.Add(div);
                //}
                //else if (Ptypes == "Encashment")
                //{
                //    Paragraph p = new Paragraph();
                //    Chunk c1 = new Chunk("(" + Ptypes + ")");

                //    c1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
                //    //c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);
                //    //c1.HorizontalScaling(Element.ALIGN_CENTER);
                //    p.Alignment = Element.ALIGN_CENTER;

                //    c1.SetHorizontalScaling(Element.ALIGN_CENTER);
                //    ////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
                //    ////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
                //    //c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
                //    p.Add(c1);

                //    PdfDiv div = new PdfDiv();
                //    div.AddElement(p);
                //    doc.Add(div);
                //}

                table.DeleteBodyRows();

                doc.Add(setEmpDetails(tableLayout, pdfData));
                tableLayout.DeleteBodyRows();
                doc.Add(setGenralDetails(tableLayout4, pdfData));
                tableLayout4.DeleteBodyRows();
                //allwences, Deductions table
                doc.Add(setAllowanceDeductions(tableLayout2, pdfData.Allowences, pdfData.Deductions));
                tableLayout2.DeleteBodyRows();
                doc.Add(settotalDetails(tableLayout1, pdfData));
                tableLayout1.DeleteBodyRows();
                doc.Add(netded(tableLayout1n, pdfData));
                tableLayout1n.DeleteBodyRows();
                doc.Add(setNetWordsDetails(tableLayout3, pdfData));
                tableLayout3.DeleteBodyRows();
                doc.Add(Label(tableLayout5, pdfData));
                tableLayout5.DeleteBodyRows();

                //doc.Add(await getdeductionDetails(tableLayout3, EmpCode));
                //doc.Add(await getTotalAmount(tableLayout4, EmpCode));
                // Closing the document  
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


            return File(workStream, "application/pdf", strPDFFileName);
        }
        public PdfPTable monthdetails(PdfPTable table, PayslipPdf data)
        {
            float[] headers = { 60 }; //Header Widths  
            table.SetWidths(headers); //Set the pdf headers  
            table.WidthPercentage = 100; //Set the PDF File witdh percentage  
            table.HeaderRows = 1;
            table.SpacingAfter = 10;


            table.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });
            table.FooterRows = 1;

            //string Env_Fm_Fy = "todo";// commBus.Env_Fm_Fy1();

            AddCellToBody1(table, "Payslip - " + data.Fm);





            return table;
        }
        public PdfPTable Label(PdfPTable tableLayout5, PayslipPdf data)
        {
            float[] headers = { 100 }; //Header Widths  
            tableLayout5.SetWidths(headers); //Set the pdf headers  
            tableLayout5.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout5.HeaderRows = 1;
            tableLayout5.SpacingAfter = 30;
            //Document doc = new Document();
            //doc.Open();
            //Paragraph P1 = new Paragraph("EARNINGS               RS");
            ////Chunk c1 = new Chunk("Payslip");

            //P1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
            ////c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);

            //////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
            //////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
            ////c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
            //P1.IndentationRight = 200;
            //P1.IndentationLeft = 200;
            //P1.SpacingAfter = 20;

            //doc.Add(P1);

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
        public PdfPTable setEmpDetails(PdfPTable tableLayout, PayslipPdf data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingAfter = 0;
            //Document doc = new Document();
            //doc.Open();
            //Paragraph P1 = new Paragraph("EARNINGS               RS");
            ////Chunk c1 = new Chunk("Payslip");

            //P1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
            ////c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);

            //////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
            //////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
            ////c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
            //P1.IndentationRight = 200;
            //P1.IndentationLeft = 200;
            //P1.SpacingAfter = 20;

            //doc.Add(P1);

            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout.FooterRows = 1;

            AddCellToBody6(tableLayout, "Employee Code: " + data.EmpCode);

            AddCellToBody12(tableLayout, data.EmpName);

            AddCellToBody12(tableLayout, data.Designation);
            if (data.Branch == "OtherBranch")
            {
                AddCellToBody123(tableLayout, "Head Office");
            }
            else
            {
                char[] char_branch = ConvertNames(data.Branch);
                string Str_Branch = new string(char_branch);
                AddCellToBody123(tableLayout, Str_Branch);
            }

            return tableLayout;
        }
        public PdfPTable setGenralDetails(PdfPTable tableLayout4, PayslipPdf data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
            tableLayout4.SetWidths(headers); //Set the pdf headers  
            tableLayout4.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout4.HeaderRows = 1;
            tableLayout4.SpacingAfter = 10;
            //Document doc = new Document();
            //doc.Open();
            //Paragraph P1 = new Paragraph("EARNINGS               RS");
            ////Chunk c1 = new Chunk("Payslip");

            //P1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
            ////c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);

            //////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
            //////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
            ////c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
            //P1.IndentationRight = 200;
            //P1.IndentationLeft = 200;
            //P1.SpacingAfter = 20;

            //doc.Add(P1);

            tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout4.FooterRows = 1;
            if (data.DOJ != null)
            {
                AddCellToBody7(tableLayout4, "DOJ: " + data.DOJ);
            }
            if (data.RetirementDate != null)
            {
                AddCellToBody(tableLayout4, "DOR: " + data.RetirementDate);
            }
            if (data.PfNo != "")
            {
                AddCellToBody(tableLayout4, "PF No: " + data.PfNo);
            }
            if (data.PfNo == "")
            {
                AddCellToBody(tableLayout4, "PF No: ");
            }

            AddCellToBody5(tableLayout4, " ");

            return tableLayout4;
        }
        public PdfPTable setNetWordsDetails(PdfPTable tableLayout3, PayslipPdf data)
        {
            float[] headers = { 100 }; //Header Widths  
            tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 30;
            //Document doc = new Document();
            //doc.Open();
            //Paragraph P1 = new Paragraph("EARNINGS               RS");
            ////Chunk c1 = new Chunk("Payslip");

            //P1.Font = FontFactory.GetFont("TimesNewRoman", 10, BaseColor.BLACK);
            ////c1.SetUnderline(BaseColor.PINK, 1.2f, 1, 1, 1, 0);

            //////c1.setHorizontalAlignment(Element.ALIGN_CENTER);
            //////c1.setVerticalAlignment(Element.ALIGN_MIDDLE);
            ////c1.setBackgroundColor(BaseColor.LIGHT_GRAY);
            //P1.IndentationRight = 200;
            //P1.IndentationLeft = 200;
            //P1.SpacingAfter = 20;

            //doc.Add(P1);

            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                //Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;

            string netSalWords = "Net salary in words";
            foreach (var itm in data.netwrkingList)
            {
                if (itm.Perticular == "Net Amount")
                {
                    try
                    {
                        var arramt = itm.Amount.Split('.');
                        if (arramt[1] == "00")
                        {
                            int netSal = Convert.ToInt32(Math.Round(Convert.ToDouble(itm.Amount)));
                            netSalWords = Mavensoft.Common.Helper.NumbersToWords(netSal);
                            netSalWords += " Rupees Only";
                        }
                        else
                        {
                            netSalWords = Mavensoft.Common.Helper.NumbersToWords(Convert.ToInt32(arramt[0]));
                            netSalWords += " Rupees and ";
                            netSalWords += Mavensoft.Common.Helper.NumbersToWords(Convert.ToInt32(arramt[1]));
                            netSalWords += " Paise Only";
                        }
                    }
                    catch { }
                }
            }

            AddCellToBodyn(tableLayout3, netSalWords);





            return tableLayout3;
        }
        public PdfPTable setAllowanceDeductions(PdfPTable tableLayout2, IList<PayslipPdfAlwDed> lstAllowences, IList<PayslipPdfAlwDed> lstDeductions)
        {
            int allwLen = lstAllowences.Count;
            int dedLen = lstDeductions.Count;
            int maxRows = allwLen > dedLen ? allwLen : dedLen;
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.HeaderRows = 1;
            tableLayout2.DefaultCell.Border = 1;
            tableLayout2.HorizontalAlignment = 0;

            tableLayout2.SpacingBefore = 0;
            tableLayout2.WidthPercentage = 100;
            tableLayout2.SpacingAfter = 20;
            DateTime printdate = DateTime.Now;
            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = PdfPCell.NO_BORDER,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
            });
            tableLayout2.FooterRows = 1;

            AddCellToHeader(tableLayout2, "EARNINGS");
            AddCellToHeader1(tableLayout2, "Rs.    ");
            AddCellToHeader(tableLayout2, "DEDUCTIONS    ");
            AddCellToHeader2(tableLayout2, "Rs.    ");

            for (int i = 0; i < maxRows; i++)
            {
                //allowe
                if (i < lstAllowences.Count)
                {
                    AddCellToBodylight(tableLayout2, lstAllowences[i].Perticular);
                    AddCellToBody2light(tableLayout2, lstAllowences[i].Amount + "    ");
                }
                else
                {
                    AddCellToBodylight(tableLayout2, "");
                    AddCellToBody2light(tableLayout2, "");
                }

                //dedu
                if (i < lstDeductions.Count)
                {
                    AddCellToBodylightded(tableLayout2, lstDeductions[i].Perticular + "    ");
                    AddCellToBody2lightded(tableLayout2, lstDeductions[i].Amount + "    ");

                }
                else
                {
                    AddCellToBodylightded(tableLayout2, "");
                    AddCellToBody2lightded(tableLayout2, "");
                }
            }

            return tableLayout2;
        }
        public PdfPTable settotalDetails(PdfPTable tableLayout1, PayslipPdf data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths             
            tableLayout1.SetWidths(headers); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;

            tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))

            {

                Colspan = 20,
                //Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });

            tableLayout1.FooterRows = 1;

            if (data.grossamount != null)
            {
                AddCellToBody6(tableLayout1, "Gross Amount ");
            }
            if (data.grossamount != null)
            {
                AddCellToBody2light1(tableLayout1, data.grossamount);
            }


            if (data.workingdays != null)
            {
                AddCellToBody12(tableLayout1, "Deduction Amount ");
            }
            if (data.workingdays != null)
            {
                AddCellToBody2lightded1(tableLayout1, data.deductionsamount);
            }

            //AddCellToBody2lightded1(tableLayout1, " ");

            return tableLayout1;
        }
        public PdfPTable netded(PdfPTable tableLayout1, PayslipPdf data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths             
            tableLayout1.SetWidths(headers); //Set the pdf headers  
            tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout1.HeaderRows = 1;

            tableLayout1.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))

            {

                Colspan = 20,
                //Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
            });
            tableLayout1.FooterRows = 1;

            if (data.deductionsamount != null)
            {
                AddCellToBody6t(tableLayout1, "Working Days ");
            }
            if (data.deductionsamount != null)
            {
                AddCellToBody2lightt(tableLayout1, data.workingdays);
            }
            if (data.netamount != null)
            {
                AddCellToBody12t(tableLayout1, "Net Amount ");
            }
            if (data.netamount != null)
            {
                AddCellToBody2lightded1t(tableLayout1, data.netamount);
            }
            //AddCellToBody2lightded1t(tableLayout1, " ");

            return tableLayout1;
        }
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                //Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(0, 0, 0),
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
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorRight = new iTextSharp.text.BaseColor(255, 255, 255),
                BorderColorLeft = new iTextSharp.text.BaseColor(255, 255, 255),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(230, 255, 230)
            });
        }
        private static void AddCellToHeader2(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                //Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(230, 255, 230)
            });
        }
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBodyn(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorBottom = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody5(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody12(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody12t(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody1(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody123(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {

                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
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
        private static void AddCellToBodylight(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                Padding = 8,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderWidthLeft = .25f,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)

            });


        }
        private static void AddCellToBodylight1(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                Padding = 8,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderWidthLeft = .25f,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)

            });


        }
        private static void AddCellToBodylightded(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                Padding = 8,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2light(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2lightded(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBodylightded1(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                Padding = 8,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2light1(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2lightt(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
        private static void AddCellToBody2lightded1(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody2lightded1t(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                UseVariableBorders = true,
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorLeft = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody6(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(0, 0, 0),
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody6t(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorBottom = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        private static void AddCellToBody7(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                //Border = PdfPCell.NO_BORDER,
                UseVariableBorders = true,
                BorderColorTop = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorRight = new iTextSharp.text.BaseColor(224, 224, 224),
                BorderColorLeft = new iTextSharp.text.BaseColor(0, 0, 0),
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });


        }
        //sent pdf through mail
        [HttpPost]
        public async Task<string> SendePdf(List<string> data)
        {
            string emp_code = "";
            string dfm = "";
            MemoryStream workStream1 = new MemoryStream();
            Mavensoft.DAL.Db.SqlHelper _sha = new Mavensoft.DAL.Db.SqlHelper();
            string strPDFFileName1 = string.Format(data + "Payslip" + "-" + ".pdf");
            string strAttachment1 = Server.MapPath("~/Downloadss/" + strPDFFileName1);
            try
            {

                //string[] count = values.StringData.Split(',');

                //string[] arrEmpId = EmpCode.Split(',');
                foreach (string empId in data)
                {

                    MemoryStream workStream = new MemoryStream();
                    StringBuilder status = new StringBuilder("");
                    DateTime dTime = DateTime.Now;
                    //string Env_Fm_Fy = "todo"; // commBus.Env_Fm_Fy1();
                    //                           //file name to be created   
                    //string strPDFFileName = string.Format(empId + "Payslip" + Env_Fm_Fy + "-" + ".pdf");
                    DataTable GetEmpcode = _sha.Get_Table_FromQry("select emp_code,format(fm,'MMMM') as fm from  pr_emp_payslip where id=" + empId + ";");

                    if (GetEmpcode.Rows.Count > 0)
                    {
                        DataRow dr = GetEmpcode.Rows[0];
                        emp_code = dr["emp_code"].ToString();
                        dfm = dr["fm"].ToString();
                    }
                    if (dfm == "January")
                    {
                        dfm = "Jan";
                    }
                    if (dfm == "February")
                    {
                        dfm = "Feb";
                    }
                    if (dfm == "March")
                    {
                        dfm = "Mar";
                    }
                    if (dfm == "April")
                    {
                        dfm = "Apr";
                    }
                    if (dfm == "May")
                    {
                        dfm = "May";
                    }
                    if (dfm == "June")
                    {
                        dfm = "Jun";
                    }
                    if (dfm == "July")
                    {
                        dfm = "Jul";
                    }
                    if (dfm == "August")
                    {
                        dfm = "Aug";
                    }
                    if (dfm == "September")
                    {
                        dfm = "Sep";
                    }

                    if (dfm == "October")
                    {
                        dfm = "Oct";
                    }
                    if (dfm == "November")
                    {
                        dfm = "Nov";
                    }
                    if (dfm == "December")
                    {
                        dfm = "Dec";
                    }


                    string strPDFFileName = string.Format(emp_code + "-" + dfm + "-Payslip" + ".pdf");
                    Document doc = new Document();
                    //for adding images
                    string imageURL = Server.MapPath("/Assets/images/PDFLogo.png");
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                    jpg.ScaleToFit(500f, 500f);

                    //Give space before image

                    jpg.SpacingBefore = 3f;

                    //Give some space after the image

                    jpg.SpacingAfter = 1f;

                    jpg.Alignment = Element.ALIGN_CENTER;
                    PdfPTable table = new PdfPTable(1);
                    PdfPTable tableLayout = new PdfPTable(4);
                    PdfPTable tableLayout1 = new PdfPTable(4);
                    PdfPTable tableLayout1n = new PdfPTable(4);
                    PdfPTable tableLayout2 = new PdfPTable(4);
                    PdfPTable tableLayout3 = new PdfPTable(1);
                    PdfPTable tableLayout4 = new PdfPTable(4);
                    PdfPTable tableLayout5 = new PdfPTable(1);

                    PdfPTable outer = new PdfPTable(2);

                    string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


                    PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                    doc.Open();

                    doc.Add(jpg);
                    PayslipPdf pdfData1 = await pdf.getPdfDetails(empId);

                    //Add Content to PDF   

                    //Empllyee details table
                    doc.Add(monthdetails(table, pdfData1));

                    table.DeleteBodyRows();

                    doc.Add(setEmpDetails(tableLayout, pdfData1));
                    tableLayout.DeleteBodyRows();
                    doc.Add(setGenralDetails(tableLayout4, pdfData1));
                    tableLayout.DeleteBodyRows();
                    //allwences, Deductions table
                    doc.Add(setAllowanceDeductions(tableLayout2, pdfData1.Allowences, pdfData1.Deductions));
                    tableLayout2.DeleteBodyRows();
                    doc.Add(settotalDetails(tableLayout1, pdfData1));
                    tableLayout1.DeleteBodyRows();
                    doc.Add(netded(tableLayout1n, pdfData1));
                    tableLayout1n.DeleteBodyRows();
                    doc.Add(setNetWordsDetails(tableLayout3, pdfData1));
                    tableLayout.DeleteBodyRows();
                    doc.Add(Label(tableLayout5, pdfData1));
                    // Closing the document  
                    Response.Clear();
                    doc.NewPage();

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

                        workStream.Write(byteInfo, 0, byteInfo.Length);
                        workStream.Position = 0;

                        PayslipPdf pdfSend = await pdf.SendPdfDetails(empId);

                        MailMessage message = new MailMessage();
                        message.Subject = "TGCAB";

                        message.Body = "PAYSLIP";
                        message.IsBodyHtml = true;
                        SmtpClient client = new SmtpClient();
                        message.From = new MailAddress("mavensoftspl@gmail.com");
                        client.Credentials = new System.Net.NetworkCredential("mavensoftspl@gmail.com", "Reset@123");
                        message.To.Add(pdfSend.PersonalEmailId);
                        message.Attachments.Add(new Attachment(new MemoryStream(byteInfo), strPDFFileName));
                        client.Port = 587;
                        client.Host = "smtp.gmail.com";
                        client.EnableSsl = true;
                        client.Send(message);
                        File(workStream, "application/pdf", strPDFFileName);
                    }
                }




                //string BCC = WebConfigurationManager.AppSettings["BCC"];
                //string fromEmail = WebConfigurationManager.AppSettings["emailFrom"];
                //string emailpw = WebConfigurationManager.AppSettings["emailPassword"];

                //int emailresult = sm.DispatchEmail(pdfSend);

            }
            catch (Exception e)
            {
                string msg = e.Message;

            }
            return "I#MailSent #Successfully";

        }

    }
}