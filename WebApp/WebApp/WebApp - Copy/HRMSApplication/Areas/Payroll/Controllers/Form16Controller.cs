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

namespace HRMSApplication.Areas.Payroll.Controllers
{
    public class Form16Controller : Controller
    {
        Form16Model form16Data;
        CommonBusiness CommonBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());

        CommonBusiness commBus = new CommonBusiness(LoginHelper.GetCurrentUserForPR());
        Form16Business form16B = new Form16Business(LoginHelper.GetCurrentUserForPR());


        [HttpGet]
        public async Task<string> SearchEmployee(string EmpCode)
        {
            return await CommonBus.SearchEmployee(EmpCode);
        }

        // get financial period
        public async Task<string> getFy()
        {
            string qryfy = "select fy as fm_fy from pr_month_details where active=1;";
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

        //Create Payslip PDF
        public async Task<FileResult> CreatePdf(string EmpCode, string fy)
        {

            string[] arrEmpId = EmpCode.Split(',');
            Mavensoft.DAL.Db.SqlHelper _sha = new Mavensoft.DAL.Db.SqlHelper();
            string str_option = "Select [Option] from pr_tax_option_emp_wise where EmpId=" + EmpCode + ";";
            DataTable dt_option = _sha.Get_Table_FromQry(str_option);
            int i_opt = Convert.ToInt32(dt_option.Rows[0]["Option"]);
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
                FileName = string.Format("All-" + fYear + "-Form16.pdf");
            }

            else
            {
                foreach (var empId in arrEmpId)
                {
                    FileName = string.Format(empId + "(" + fYear + ") year-Form16.pdf");
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
            PdfPTable tableLayout2 = new PdfPTable(1);// for rules
            PdfPTable tableLayout3 = new PdfPTable(3);// PAN
            PdfPTable tableLayout = new PdfPTable(2);
            PdfPTable tableLayout1 = new PdfPTable(4);
            PdfPTable tableLayout4 = new PdfPTable(2);
            PdfPTable tableLayout5 = new PdfPTable(5);
            PdfPTable tableLayout13 = new PdfPTable(3);
            // PdfPTable tableLayout2 = new PdfPTable(4);
            //PdfPTable tableLayout3 = new PdfPTable(1);
            //PdfPTable tableLayout4 = new PdfPTable(4);
            //PdfPTable tableLayout5 = new PdfPTable(1);

            //PdfPTable tableLayout3 = new PdfPTable(2);
            //PdfPTable tableLayout4 = new PdfPTable(4);

            PdfPTable outer = new PdfPTable(2);

            string strAttachment = Server.MapPath("~/Downloadss/");


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();






            foreach (string empId in arrEmpId)
            {

                string fYear1 = fYear;
                Form16Model form16Data = await form16B.GetForm16Data(empId, fYear1);




                doc.Add(jpg);
                doc.Add(Heading(table, form16Data));
                table.DeleteBodyRows();

                doc.Add(Rules(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(setEmpDetails(tableLayout, form16Data));
                tableLayout.DeleteBodyRows();

                doc.Add(PanDetails(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(Address(tableLayout4, form16Data));
                tableLayout4.DeleteBodyRows();

                doc.Add(city(tableLayout4, form16Data));
                tableLayout4.DeleteBodyRows();

                doc.Add(Summary(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(quaterheading(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(Quarter1(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(Quarter2(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(Quarter3(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(Quarter4(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(totalQT(tableLayout1, form16Data));
                tableLayout1.DeleteBodyRows();

                doc.Add(detailsTax(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(DetalsofTax(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(BSRCodeheader(tableLayout5, form16Data));
                tableLayout5.DeleteBodyRows();

                doc.Add(detailsBody(tableLayout5, form16Data.deductions, form16Data));
                tableLayout5.DeleteBodyRows();

                doc.Add(detailTotal(tableLayout, form16Data));
                tableLayout.DeleteBodyRows();


                doc.Add(space(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(verifi(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(declar(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(sign(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(dateANDplace(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(dateANDplace1(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(Rules1(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(partBdetails(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(RSandPS(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(gros1(tableLayout13, form16Data.sect1, form16Data, i_opt));
                tableLayout13.DeleteBodyRows();

                doc.Add(Rules_(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(verifi(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(declar(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(sign(tableLayout2, form16Data));
                tableLayout2.DeleteBodyRows();

                doc.Add(dateANDplace(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();

                doc.Add(dateANDplace1(tableLayout3, form16Data));
                tableLayout3.DeleteBodyRows();


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


            return File(workStream, "application/pdf", FileName);
        }
        public PdfPTable Heading(PdfPTable table, Form16Model data)
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
            AddCellToBody1(table, "Form No  - 16  ");
            table.FooterRows = 1;

            return table;
        }
        public PdfPTable Rules(PdfPTable tableLayout2, Form16Model data)
        {
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
            AddCellToBody1(tableLayout2, "(See Rule 31(1)(a))");
            AddCellToBody1(tableLayout2, "Certificate Under Section 203 of the Income - tax Act, 1961 for tax deducted at source from Income chargeble under the head 'Salaries' ");
            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");
            AddCellToBody1(tableLayout2, "PART A");
            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        public PdfPTable Label(PdfPTable tableLayout5, Form16Model data)
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
        public PdfPTable setEmpDetails(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            tableLayout.SpacingAfter = 5;


            tableLayout.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout.FooterRows = 1;

            AddCellToBody(tableLayout, "Name and Address of the Employer:                                  " +
                "TELANGANA STATE CO-OP APEX BANK LTD.");

            AddCellToBody(tableLayout, "Name and Designation of the Employee :                              " + data.EmpName + "(" + data.EmpCode + ")       " + data.Designation
);



            return tableLayout;
        }
        //PanDetails
        public PdfPTable PanDetails(PdfPTable tableLayout3, Form16Model data)
        {
            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 5;


            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;

            AddCellToBody(tableLayout3, "PAN of the Deductor:      " + data.sancPAN);
            AddCellToBody(tableLayout3, "TAN of the Deductor:    " + " HYDT06401D ");

            AddCellToBody(tableLayout3, "PAN of the Employee :    " + data.PAN);



            return tableLayout3;
        }
        //Address
        public PdfPTable Address(PdfPTable tableLayout4, Form16Model data)
        {
            float[] headers = { 60, 60 }; //Header Widths  
            tableLayout4.SetWidths(headers); //Set the pdf headers  
            tableLayout4.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout4.HeaderRows = 1;
            tableLayout4.SpacingAfter = 3;


            tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout4.FooterRows = 1;


            AddCellToBody(tableLayout4, "CIT(TDS)                                                                                    Address:THE COMMISSIONER OF INCOME TAX(TDS)" +
                "ROOM NO.411,IT TOWERS,10-2-3,A.C.GUARD .");

            AddCellToBody(tableLayout4, " Assessment Year                                Period      From     To ");

            return tableLayout4;
        }
        //city
        public PdfPTable city(PdfPTable tableLayout4, Form16Model data)
        {
            float[] headers = { 60, 60 }; //Header Widths  
            tableLayout4.SetWidths(headers); //Set the pdf headers  
            tableLayout4.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout4.HeaderRows = 1;
            tableLayout4.SpacingAfter = 3;


            tableLayout4.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout4.FooterRows = 1;
            string finan = data.fy;
            //string fyear = finan.Substring(0, 4);
            //string eyear = finan.Substring(5, 4);
            string fyear = (Convert.ToInt32(finan.Substring(0, 4))-1).ToString();
            string eyear = (Convert.ToInt32(finan.Substring(5, 4))-1).ToString();

            AddCellToBody(tableLayout4, " City  :" + data.city + "           Pin Code:	");

            AddCellToBody(tableLayout4, data.fy + "                                        01-04-" + fyear + "    31-03-" + eyear);

            return tableLayout4;
        }
        //city
        //Summary
        public PdfPTable Summary(PdfPTable tableLayout2, Form16Model data)
        {
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
            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");
            AddCellToBody1(tableLayout2, " Summary of tax deducted at source");
            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //quater heading
        public PdfPTable quaterheading(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToHeader(tableLayout, "Quarter");

            AddCellToHeader(tableLayout, "Receipt number of original statement of TDS under sub-section(3) of section 200");

            AddCellToHeader(tableLayout, "Amount Of tax deducted in respect of the employee");

            AddCellToHeader(tableLayout, "Amount of tax deposite/remitted in respect Of the employee");

            return tableLayout;
        }
        //Quarter1,2,3 4
        public PdfPTable Quarter1(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToBody(tableLayout, "Quarter1");

            AddCellToBody(tableLayout, "           ");

            AddCellToBody(tableLayout, data.Quarter1);

            AddCellToBody(tableLayout, data.Quarter1);

            return tableLayout;
        }

        public PdfPTable Quarter2(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToBody(tableLayout, "Quarter2");

            AddCellToBody(tableLayout, "        ");

            AddCellToBody(tableLayout, data.Quarter2);

            AddCellToBody(tableLayout, data.Quarter2);

            return tableLayout;
        }
        public PdfPTable Quarter3(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToBody(tableLayout, "Quarter3");

            AddCellToBody(tableLayout, "         ");

            AddCellToBody(tableLayout, data.Quarter3);

            AddCellToBody(tableLayout, data.Quarter3);

            return tableLayout;
        }
        public PdfPTable Quarter4(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToBody(tableLayout, "Quarter4");

            AddCellToBody(tableLayout, "        ");

            AddCellToBody(tableLayout, data.Quarter4);

            AddCellToBody(tableLayout, data.Quarter4);

            return tableLayout;
        }
        public PdfPTable totalQT(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 60, 60, 60, 60 }; //Header Widths  
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

            AddCellToBody(tableLayout, "Total");

            AddCellToBody(tableLayout, "        ");

            double Total = (Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4));
            AddCellToBody(tableLayout, Total.ToString());

            AddCellToBody(tableLayout, (Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4)).ToString());

            return tableLayout;
        }
        // details of tax deducted
        public PdfPTable detailsTax(PdfPTable tableLayout2, Form16Model data)
        {
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

            AddCellToBody1(tableLayout2, "-------------------------------------------------------------------------------------------------------------------------------");
            AddCellToBody1(tableLayout2, "Details of Tax Deducted and Deposited into Central Government Account Through Challan");
            AddCellToBody1(tableLayout2, "-------------------------------------------------------------------------------------------------------------------------------");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //Details Tax header
        public PdfPTable DetalsofTax(PdfPTable tableLayout3, Form16Model data)
        {
            float[] headers = { 40, 60, 100 }; //Header Widths  
            tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 3;


            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;

            AddCellToHeader(tableLayout3, "SlNo");
            AddCellToHeader(tableLayout3, "Tax Deposited in respect of the employee(Rs.)");

            AddCellToHeader(tableLayout3, " Challan Identification Number(CIN)");

            return tableLayout3;
        }

        public PdfPTable BSRCodeheader(PdfPTable tableLayout5, Form16Model data)
        {
            float[] headers = { 100, 0, 35, 35, 30 }; //Header Widths  
            tableLayout5.SetWidths(headers); //Set the pdf headers  
            tableLayout5.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout5.HeaderRows = 1;
            tableLayout5.SpacingAfter = 3;


            tableLayout5.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout5.FooterRows = 1;

            AddCellToHeader(tableLayout5, "  ");
            AddCellToHeader(tableLayout5, "   ");

            AddCellToHeader(tableLayout5, " BSRCode of the bank branch");
            AddCellToHeader(tableLayout5, " Date on which tax deposited ");
            AddCellToHeader(tableLayout5, "  Challan serialNo ");

            return tableLayout5;
        }
        //details body
        public PdfPTable detailsBody(PdfPTable tableLayout5, IList<monthwiseDeductions> lstDeductions, Form16Model data)
        {
            int dedLen = lstDeductions.Count;
            string pdate1;
            float[] headers = { 40, 60, 35, 35, 30 }; //Header Widths  
            tableLayout5.SetWidths(headers); //Set the pdf headers  
            tableLayout5.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout5.HeaderRows = 1;
            tableLayout5.SpacingAfter = 3;


            tableLayout5.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 5,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout5.FooterRows = 1;
            for (int i = 0; i < dedLen; i++)
            {
                if (lstDeductions.Count > 0)
                {


                    DateTime pdate = Convert.ToDateTime(data.paymentdate);
                    if (data.paymentdate != null)
                    {
                        pdate1 = pdate.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        pdate1 = null;
                    }
                    AddCellToBody(tableLayout5, "" + (i + 1));
                    AddCellToBody(tableLayout5, lstDeductions[i].dedu_amount);
                    AddCellToBody(tableLayout5, data.bsrcode);
                    AddCellToBody(tableLayout5, pdate1);
                    AddCellToBody(tableLayout5, data.challanno);
                }
            }


            //AddCellToBody(tableLayout5, " 1 ");
            //AddCellToBody(tableLayout5,data.Quarter1+data.Quarter2+data.Quarter3+data.Quarter4);

            //AddCellToBody(tableLayout5, " 0");
            //AddCellToBody(tableLayout5, " 01-01-1900 ");
            //AddCellToBody(tableLayout5, "0 ");

            return tableLayout5;
        }
        // detailTotal
        public PdfPTable detailTotal(PdfPTable tableLayout, Form16Model data)
        {
            float[] headers = { 35, 140 }; //Header Widths  
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

            AddCellToBody(tableLayout, " Total ");


            //need to asl client regaring this
            AddCellToBody(tableLayout, (Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4)).ToString());


            return tableLayout;

        }
        //space
        public PdfPTable space(PdfPTable tableLayout2, Form16Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 90;


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

        // verification
        public PdfPTable verifi(PdfPTable tableLayout2, Form16Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            //tableLayout2.SpacingAfter = 30;
            tableLayout2.SpacingAfter = 12.5f;


            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "");
            AddCellToBody1(tableLayout2, "VERIFICATION");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //delcaration
        public PdfPTable declar(PdfPTable tableLayout2, Form16Model data)
        {
            float[] headers = { 60 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 50;
            double total = Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4);

            int total1 = Convert.ToInt32(total);

            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1declaration(tableLayout2, " I , " + data.sanctName + ", Son/daughter of " + data.sanctFantherName + " Working in the Capacity of " + data.sanctDesignation + "(designation) do hereby certify that a sum of" +
                " Rs." + total + "  ,[" + Mavensoft.Common.Helper.NumberToWords(total1) + " Rupees only] has been" +
                " deducted at source and  deposited to the credit of the Central Government.I " +
                "further certify that the  information given above is true, complete and correct and is based on the books of accounts, documents, TDS statement, TDS deposited and" +
                " other available records.");


            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }

        //signature
        public PdfPTable sign(PdfPTable tableLayout2, Form16Model data)
        {
            float[] headers = { 40 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 40;
            double total = Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4);

            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1NB(tableLayout2, "Signature of the Person Responsible for deduction of Tax ");


            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //date and palce
        public PdfPTable dateANDplace(PdfPTable tableLayout3, Form16Model data)
        {
            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 3;


            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;

            AddCellToBody1sig(tableLayout3, "Place :" + data.sanctpalce);
            AddCellToBody1sig(tableLayout3, "             ");

            AddCellToBody1sig(tableLayout3, " Full Name : " + data.sanctName);

            return tableLayout3;
        }
        public PdfPTable dateANDplace1(PdfPTable tableLayout3, Form16Model data)
        {
            float[] headers = { 60, 60, 60 }; //Header Widths  
            tableLayout3.SetWidths(headers); //Set the pdf headers  
            tableLayout3.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout3.HeaderRows = 1;
            tableLayout3.SpacingAfter = 100;


            tableLayout3.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout3.FooterRows = 1;

            AddCellToBody1sig(tableLayout3, "Date  :" + data.sanctDate);
            AddCellToBody1sig(tableLayout3, "             ");

            AddCellToBody1sig(tableLayout3, " Designation : " + data.sanctDesignation);

            return tableLayout3;
        }
        //PART B
        public PdfPTable Rules1(PdfPTable tableLayout2, Form16Model data)
        {
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

            AddCellToBody1(tableLayout2, "PART B");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        //part b details
        public PdfPTable partBdetails(PdfPTable tableLayout2, Form16Model data)
        {
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

            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");
            AddCellToBody1(tableLayout2, " Details of Salary Paid, Any Other Income and Tax Deducted");
            AddCellToBody1(tableLayout2, "----------------------------------------------------------------------------------------------------------------------------");

            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }
        // Rs. Ps.   Rs. Ps.   Rs. Ps.   Rs. Ps.
        public PdfPTable RSandPS(PdfPTable tableLayout2, Form16Model data)
        {
            float[] headers = { 40 }; //Header Widths  
            tableLayout2.SetWidths(headers); //Set the pdf headers  
            tableLayout2.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout2.HeaderRows = 1;
            tableLayout2.SpacingAfter = 40;
            double total = Convert.ToDouble(data.Quarter1) + Convert.ToDouble(data.Quarter2) + Convert.ToDouble(data.Quarter3) + Convert.ToDouble(data.Quarter4);

            tableLayout2.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,
                //Border = 0,
                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.NO_BORDER,

            });
            AddCellToBody1NB(tableLayout2, "     Rs. Ps.      Rs. Ps.      Rs. Ps.  ");


            tableLayout2.FooterRows = 1;

            return tableLayout2;
        }

        public PdfPTable gros1(PdfPTable tableLayout13, IList<section80Cform16> lstsection80C, Form16Model data, int i_opt)
        {
            int len = lstsection80C.Count;
            decimal TotSalary = 0;
            decimal sum_section80c = 0;
            decimal interest_on_housing = 0;
            interest_on_housing = Convert.ToDecimal(data.Income_from_House_Property_Interest_on_Housing_Loan);
            TotSalary = Convert.ToDecimal(data.gross_Salaryaspercontainedsec17) + Convert.ToDecimal(data.Valueofperquisitiesu17) + Convert.ToDecimal(data.Profitslieuofsalary17);
            float[] headers = { 90, 30, 60 }; //Header Widths  
            tableLayout13.SetWidths(headers); //Set the pdf headers  
            tableLayout13.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout13.HeaderRows = 1;

            tableLayout13.SpacingAfter = 3;


            tableLayout13.AddCell(new PdfPCell(new Phrase("", new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
            {
                Colspan = 20,

                PaddingBottom = 0,

                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.NO_BORDER,

            });
            tableLayout13.FooterRows = 1;

            if (i_opt == 1)
            {
                AddCellToBody1declaration(tableLayout13, "1. Gross Salary");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, " ");

                AddCellToBody1declaration(tableLayout13, "    a. Salary as per provisions contained in sec.17(1)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.gross_Salaryaspercontainedsec17);

                AddCellToBody1declaration(tableLayout13, "    b. Value of perquisities u/s 17(2)(as per Form.12BA)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Valueofperquisitiesu17);

                AddCellToBody1declaration(tableLayout13, "    c. Profits in lieu of salary u/s 17(3)(as per Form.12BA))");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Profitslieuofsalary17);

                AddCellToBody1declaration(tableLayout13, "    d. Total Salary ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Profitslieuofsalary17);
                AddCellToBody1declaration(tableLayout13, TotSalary.ToString());

                AddCellToBody1declaration(tableLayout13, "2. Less : Allowance to the Extent exempt under Section 10 & 17");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, " ");

                AddCellToBody1declaration(tableLayout13, "    a.  House Rent Allowance ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.HouseRentAllowance);

                AddCellToBody1declaration(tableLayout13, "    b. Total of Section 10 & 17 ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.TotalSection1017);
                AddCellToBody1declaration(tableLayout13, data.HouseRentAllowance);

                AddCellToBody1declaration(tableLayout13, "3. Balance(1-2)");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Balance1_2);
                AddCellToBody1declaration(tableLayout13, (TotSalary - Convert.ToDecimal(data.HouseRentAllowance)).ToString());

                AddCellToBody1declaration(tableLayout13, "4. Deductions :");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, " ");

                AddCellToBody1declaration(tableLayout13, "    a. Standard Deduction  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.StandardDeduction);

                AddCellToBody1declaration(tableLayout13, "    b. Tax on Employment ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_on_Employment);

                AddCellToBody1declaration(tableLayout13, "5. Aggregate of 4(a) to (b) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Aggregate);

                AddCellToBody1declaration(tableLayout13, "6. Income Chargeable Under the Head Salaries (3-5) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Income_Charg_Under_Salaries);
                AddCellToBody1declaration(tableLayout13, ((TotSalary - Convert.ToDecimal(data.HouseRentAllowance)) - Convert.ToDecimal(data.Aggregate)).ToString());

                AddCellToBody1declaration(tableLayout13, "7. Add : Any Other Income ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "    Reported by the Employee  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Reported_by_Employee);
                AddCellToBody1declaration(tableLayout13, "");

                if (interest_on_housing > 0)
                {
                    AddCellToBody1declaration(tableLayout13, "    Income from House Property (Interest on Housing Loan)  ");
                    AddCellToBody1declaration(tableLayout13, "             ");
                    AddCellToBody1declaration(tableLayout13, "-" + interest_on_housing);
                }


                AddCellToBody1declaration(tableLayout13, "8. Gross Total Income ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Gross_Total_Income);
                AddCellToBody1declaration(tableLayout13, (((TotSalary - Convert.ToDecimal(data.HouseRentAllowance)) - Convert.ToDecimal(data.Aggregate)) - interest_on_housing).ToString());

                AddCellToBody1declaration(tableLayout13, "9. Deductions Under Chapter VI-A");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "Gross.Amt  Qual.Amt   Ded.Amt ");

                AddCellToBody1declaration(tableLayout13, "    (A) Sections 80C,80CCC and 80CCD");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "        (a) Section 80C  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");
                for (int i = 0; i < len; i++)
                {
                    if (lstsection80C[i].type == "Housing Loan")
                    {
                        AddCellToBody1declaration(tableLayout13, "            Housing Loan ");
                        AddCellToBody1declaration(tableLayout13, "             ");
                        AddCellToBody1declaration(tableLayout13, lstsection80C[i].sect1_grss + " " + lstsection80C[i].sect1_qual + " " + lstsection80C[i].sect1_dedu);
                        sum_section80c += Convert.ToDecimal(lstsection80C[i].sect1_grss);
                    }
                    if (lstsection80C[i].type == "Provident Fund")
                    {
                        AddCellToBody1declaration(tableLayout13, "            Provident Fund ");
                        AddCellToBody1declaration(tableLayout13, "             ");
                        AddCellToBody1declaration(tableLayout13, lstsection80C[i].sect1_grss + " " + lstsection80C[i].sect1_qual + " " + lstsection80C[i].sect1_dedu);
                        sum_section80c += Convert.ToDecimal(lstsection80C[i].sect1_grss);
                    }
                    if (lstsection80C[i].type == "VPF")
                    {
                        AddCellToBody1declaration(tableLayout13, "            VPF ");
                        AddCellToBody1declaration(tableLayout13, "             ");
                        AddCellToBody1declaration(tableLayout13, lstsection80C[i].sect1_grss + " " + lstsection80C[i].sect1_qual + " " + lstsection80C[i].sect1_dedu);
                        sum_section80c += Convert.ToDecimal(lstsection80C[i].sect1_grss);
                    }
                    if (lstsection80C[i].type == "LIC")
                    {
                        AddCellToBody1declaration(tableLayout13, "            LIC ");
                        AddCellToBody1declaration(tableLayout13, "             ");
                        AddCellToBody1declaration(tableLayout13, lstsection80C[i].sect1_grss + " " + lstsection80C[i].sect1_qual + " " + lstsection80C[i].sect1_dedu);
                        sum_section80c += Convert.ToDecimal(lstsection80C[i].sect1_grss);
                    }
                }

                //AddCellToBody1declaration(tableLayout13, "            TUTUION FEE ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.sect2_grss + " " + data.sect2_qual + " " + data.sect2_dedu);

                AddCellToBody1declaration(tableLayout13, "        (b) Section 80CCC  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "        (c) Section 80CCD");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "    (B) Other Sections Under Chaper VI-A");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "10. Aggregate of deductible amount Under Chapter VIA ");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Aggregate_amount_Under_ChapterVIA);
                if (sum_section80c > 150000)
                {
                    sum_section80c = 150000;
                    AddCellToBody1declaration(tableLayout13, sum_section80c.ToString()+".00");
                }
                else
                {
                    AddCellToBody1declaration(tableLayout13, sum_section80c.ToString());
                }


                AddCellToBody1declaration(tableLayout13, "11. Total Income (8-10)");
                AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.TotalIncome8_10);
                AddCellToBody1declaration(tableLayout13, ((((TotSalary - Convert.ToDecimal(data.HouseRentAllowance)) - Convert.ToDecimal(data.Aggregate)) - sum_section80c) - interest_on_housing).ToString());

                AddCellToBody1declaration(tableLayout13, "12. Tax on Total Income");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_on_Total_Income);

                AddCellToBody1declaration(tableLayout13, "     (b). Section 87A");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Section87A);

                AddCellToBody1declaration(tableLayout13, "13. Education CESS");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.EducationCESS);

                AddCellToBody1declaration(tableLayout13, "14. Tax payable (12+13)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_payable); //(Convert.ToDouble(data.Tax_on_Total_Income)+Convert.ToDouble(data.EducationCESS)).ToString());
                //AddCellToBody1declaration(tableLayout13, (Convert.ToDecimal(data.Tax_on_Total_Income)+Convert.ToDecimal(data.Section87A)+Convert.ToDecimal(data.EducationCESS)).ToString());

                AddCellToBody1declaration(tableLayout13, "15. Less Relief Under Section 89 (Attach Details) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "0.00");

                AddCellToBody1declaration(tableLayout13, "16. Tax payable (14-15)");
                AddCellToBody1declaration(tableLayout13, "             ");
                if (data.Tax_payable != "")
                {
                    AddCellToBody1declaration(tableLayout13, (Convert.ToDouble(data.Tax_payable) + 0.00).ToString()+".00");
                    //AddCellToBody1declaration(tableLayout13,((Convert.ToDecimal(data.Tax_on_Total_Income) + Convert.ToDecimal(data.Section87A) + Convert.ToDecimal(data.EducationCESS))-Convert.ToDecimal(0.00)).ToString());
                }
                else
                {
                    AddCellToBody1declaration(tableLayout13, (0.00 + 0.00).ToString());
                }

                AddCellToBody1declaration(tableLayout13, "17. Less : a. Tax deducted at Source u/s 192(1)  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_deducted_Source);
                //AddCellToBody1declaration(tableLayout13, data.Tax_payable);

                AddCellToBody1declaration(tableLayout13, "        Less : b. Tax paid by the employer u/s 192(1A)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_paid_employer);

                AddCellToBody1declaration(tableLayout13, "18. Tax Payable/(Refundable) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_Payable_Refundable);
                //AddCellToBody1declaration(tableLayout13, (((Convert.ToDecimal(data.Tax_on_Total_Income) + Convert.ToDecimal(data.Section87A) + Convert.ToDecimal(data.EducationCESS)) - Convert.ToDecimal(0.00))-(Convert.ToDecimal(data.Tax_payable)+Convert.ToDecimal(data.Tax_paid_employer))).ToString());


                return tableLayout13;
            }
            else if (i_opt == 2)
            {
                AddCellToBody1declaration(tableLayout13, "1. Gross Salary");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, " ");

                AddCellToBody1declaration(tableLayout13, "    a. Salary as per provisions contained in sec.17(1)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.gross_Salaryaspercontainedsec17);

                AddCellToBody1declaration(tableLayout13, "    b. Value of perquisities u/s 17(2)(as per Form.12BA)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Valueofperquisitiesu17);

                AddCellToBody1declaration(tableLayout13, "    c. Profits in lieu of salary u/s 17(3)(as per Form.12BA))");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Profitslieuofsalary17);

                AddCellToBody1declaration(tableLayout13, "    d. Total Salary ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Profitslieuofsalary17);

                //AddCellToBody1declaration(tableLayout13, "2. Less : Allowance to the Extent exempt under Section 10 & 17");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, " ");

                //AddCellToBody1declaration(tableLayout13, "    a.  House Rent Allowance ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.HouseRentAllowance);

                //AddCellToBody1declaration(tableLayout13, "    b. Total of Section 10 & 17 ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.TotalSection1017);

                //AddCellToBody1declaration(tableLayout13, "3. Balance(1-2)");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Balance1_2);

                //AddCellToBody1declaration(tableLayout13, "4. Deductions :");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, " ");

                //AddCellToBody1declaration(tableLayout13, "    a. Standard Deduction  ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.StandardDeduction);

                //AddCellToBody1declaration(tableLayout13, "    b. Tax on Employment ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Tax_on_Employment);

                //AddCellToBody1declaration(tableLayout13, "5. Aggregate of 4(a) to (b) ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Aggregate);

                //AddCellToBody1declaration(tableLayout13, "6. Income Chargeable Under the Head Salaries (3-5) ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Income_Charg_Under_Salaries);

                AddCellToBody1declaration(tableLayout13, "2. Add : Any Other Income ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "");

                AddCellToBody1declaration(tableLayout13, "    Reported by the Employee  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Reported_by_Employee);

                AddCellToBody1declaration(tableLayout13, "3. Gross Total Income ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Gross_Total_Income);

                //AddCellToBody1declaration(tableLayout13, "9. Deductions Under Chapter VI-A");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "Gross.Amt  Qual.Amt   Ded.Amt ");

                //AddCellToBody1declaration(tableLayout13, "    (A) Sections 80C,80CCC and 80CCD");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "");

                //AddCellToBody1declaration(tableLayout13, "        (a) Section 80C  ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "");
                //for (int i = 0; i < len; i++)
                //{
                //    if (lstsection80C[i].type == "Provident Fund")
                //    {
                //        AddCellToBody1declaration(tableLayout13, "            Provident Fund ");
                //        AddCellToBody1declaration(tableLayout13, "             ");
                //        AddCellToBody1declaration(tableLayout13, lstsection80C[i].sect1_grss + " " + lstsection80C[i].sect1_qual + " " + lstsection80C[i].sect1_dedu);
                //    }
                //}

                //AddCellToBody1declaration(tableLayout13, "            TUTUION FEE ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.sect2_grss + " " + data.sect2_qual + " " + data.sect2_dedu);

                //AddCellToBody1declaration(tableLayout13, "        (b) Section 80CCC  ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "");

                //AddCellToBody1declaration(tableLayout13, "        (c) Section 80CCD");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "");

                //AddCellToBody1declaration(tableLayout13, "    (B) Other Sections Under Chaper VI-A");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, "");

                //AddCellToBody1declaration(tableLayout13, "10. Aggregate of deductible amount Under Chapter VIA ");
                //AddCellToBody1declaration(tableLayout13, "             ");
                //AddCellToBody1declaration(tableLayout13, data.Aggregate_amount_Under_ChapterVIA);

                AddCellToBody1declaration(tableLayout13, "4. Total Income ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.TotalIncome8_10);

                AddCellToBody1declaration(tableLayout13, "5. Tax on Total Income");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_on_Total_Income);

                AddCellToBody1declaration(tableLayout13, "     (b). Section 87A");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Section87A);

                AddCellToBody1declaration(tableLayout13, "6. Education CESS");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.EducationCESS);

                AddCellToBody1declaration(tableLayout13, "7. Tax payable ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_payable); //(Convert.ToDouble(data.Tax_on_Total_Income)+Convert.ToDouble(data.EducationCESS)).ToString());

                AddCellToBody1declaration(tableLayout13, "8. Less Relief Under Section 89 (Attach Details) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, "0.00");

                AddCellToBody1declaration(tableLayout13, "9. Tax payable ");
                AddCellToBody1declaration(tableLayout13, "             ");
                if (data.Tax_payable != "")
                {
                    AddCellToBody1declaration(tableLayout13, (Convert.ToDouble(data.Tax_payable) + 0.00).ToString());
                }
                else
                {
                    AddCellToBody1declaration(tableLayout13, (0.00 + 0.00).ToString());
                }

                AddCellToBody1declaration(tableLayout13, "10. Less : a. Tax deducted at Source u/s 192(1)  ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_deducted_Source);

                AddCellToBody1declaration(tableLayout13, "        Less : b. Tax paid by the employer u/s 192(1A)");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_paid_employer);

                AddCellToBody1declaration(tableLayout13, "11. Tax Payable/(Refundable) ");
                AddCellToBody1declaration(tableLayout13, "             ");
                AddCellToBody1declaration(tableLayout13, data.Tax_Payable_Refundable);


                return tableLayout13;
            }
            return tableLayout13;

        }

        public PdfPTable Rules_(PdfPTable tableLayout2, Form16Model data)
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