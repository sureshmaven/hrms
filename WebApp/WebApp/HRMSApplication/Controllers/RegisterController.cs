using Entities;
using HRMSApplication.Filters;
using HRMSApplication.Helpers;
using HRMSApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using HRMSBusiness.Comm;
using HRMSBusiness.Business;
using HRMSBusiness.Timesheet;

namespace HRMSApplication.Controllers
{
    [Authorize]
    public class RegisterController : Controller
    {
        private ContextBase db = new ContextBase();
        TimesheetBusiness TBUS = new TimesheetBusiness();
        LoginCredential lCredentials = LoginHelper.GetCurrentUser();

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(RegisterController));

        PswdEncryptDecrypt lencrptypassword = new PswdEncryptDecrypt();


        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
        public ActionResult ViewProfile(string username, string loginmode)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                LoginBus lgbus = new LoginBus();
                Session.SetDataToSession<string>("ActiveImage", lgbus.getEmpImage(lCredentials.EmpId));
                int id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();

                LogInformation.Info("RegisterController.id " + id);
                var data = new Persistence.EmployeesRepository().GetIt(id);
                //for updating encrypted passwords of all users
                // string lresult = lencrptypassword.ChangePasswordEncrepty();
                var selectedshift = (from sub in db.Employes
                                     where sub.Id == id
                                     select sub.Shift_Id).FirstOrDefault();
                var shiftselected = (from sub in db.Employes
                                     where sub.Id == id
                                     select sub.Shift_Id).FirstOrDefault();
                //var shift = Facade.EntitiesFacade.GetAllShifts().Where(a => a.BranchId == Convert.ToInt32(lCredentials.Branch)).Select(x => new Shift_Master
                //{
                //    Id = x.Id,
                //    ShiftType = x.GroupName + " - " + x.InTime + " " + x.OutTime
                //});
                var shift = Facade.EntitiesFacade.GetAllShifts().Select(x => new Shift_Master
                {
                    Id = x.Id,
                    ShiftType = x.GroupName + " - " + x.InTime + " " + x.OutTime
                });
                //ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType", "");
                ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType", shiftselected);
                var selected = (from sub in db.Employes
                                where sub.Id == id
                                select sub.JoinedDesignation).FirstOrDefault();
                var items4 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                LogInformation.Info("RegisterController.selected " + selected);
                ViewBag.JoinedDesignation = new SelectList(items4, "Id", "Name", selected);

                var selected2 = (from sub in db.Employes
                                 join dep in db.Departments on sub.Department equals dep.Id
                                 where sub.Id == id && dep.Active == 1

                                 select sub.Department).FirstOrDefault();

                var employeelist = db.Employes.ToList();
                var selectedc = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.CurrentDesignation).FirstOrDefault();
                LogInformation.Info("RegisterController.selectedc " + selectedc);
                var items4c = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.CurrentDesig = new SelectList(items4c, "Id", "Name", selectedc);
                //for (int i = 0; i < employeelist.Count; i++)
                //{

                //    string empidss = lCredentials.EmpId;
                //    int empdss1 = db.Employes.Where(a => a.EmpId == empidss).Select(a => a.Id).FirstOrDefault();

                //    string control = db.Employes.Where(a => a.EmpId == empidss).Select(a => a.ControllingAuthority).FirstOrDefault();
                //    string sanction = db.Employes.Where(a => a.EmpId == empidss).Select(a => a.SanctioningAuthority).FirstOrDefault();
                //    int control1 = Convert.ToInt32(control);
                //    int sanction1 = Convert.ToInt32(sanction);
                //    var controlid = db.Employes.Where(a => a.Id == control1).Select(a => a.Id).FirstOrDefault();
                //    var sanctionid = db.Employes.Where(a => a.Id == sanction1).Select(a => a.Id).FirstOrDefault();
                //    var controlEcode = db.Employes.Where(a => a.EmpId == empidss).Select(a => a.ControllingAuthority).FirstOrDefault();
                //    var sanctionlEcode = db.Employes.Where(a => a.EmpId == empidss).Select(a => a.SanctioningAuthority).FirstOrDefault();


                //    if (empdss1 == sanctionid)
                //    {
                //       // TempData["AlertMessage"] = "This employee is  assigned as Sanctioning Authority";

                var items2 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments

                {
                    Id = x.Id,
                    Name = x.Name
                });
                //var datatemp = items2.ToList();

                //    for (int i = 0; i < datatemp.Count(); i++)
                //    {
                //       if (datatemp[i].Name.Contains(";"))
                //       {
                //          string[] multiArray = datatemp[i].Name.Split(new Char[] { ';' });
                //          datatemp[i].Name =  multiArray[0] + "\r\n" + multiArray[1] + "\n" + multiArray[2];
                //       }
                //    }
                ViewBag.Department = new SelectList(items2, "Id", "Name", selected2);

                var selected1 = (from sub1 in db.Employes
                                 where sub1.Id == id
                                 select sub1.CurrentDesignation).FirstOrDefault();
                LogInformation.Info("RegisterController.selected1 " + selected1);

                var items3 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.CurrentDesignation = new SelectList(items3, "Id", "Name", selected1);

                var selected3 = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.Role).FirstOrDefault();
                LogInformation.Info("RegisterController.selected3 " + selected3);
                var items1 = Facade.EntitiesFacade.GetAllRoles().Select(x => new Roles
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.CurrentDesignationId = selected1;
                ViewBag.Role = new SelectList(items1, "Id", "Name", selected3);

                var selected4 = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.Branch).FirstOrDefault();
                LogInformation.Info("RegisterController.selected4 " + selected4);
                var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name.ToString(),
                });
                ViewBag.Branch = new SelectList(items, "Id", "Name", selected4);

                //Controlling
                int lcontrolling = Convert.ToInt32(data.ControllingAuthority);
                int lSancationing = Convert.ToInt32(data.SanctioningAuthority);

                Session["Oldlcontrols"] = lcontrolling;
                Session["OldlSancation"] = lSancationing;
                //Conttrolling Branch Id
                var selected4b = (from sub in db.Employes
                                  where sub.Id == lcontrolling
                                  select sub.Branch).FirstOrDefault();
                LogInformation.Info("RegisterController.selected4b " + selected4b);
                var selected2d = (from sub in db.Employes
                                  join dep in db.Departments on sub.Department equals dep.Id
                                  where sub.Id == lcontrolling && dep.Active == 1
                                  select sub.Department).FirstOrDefault();
                LogInformation.Info("RegisterController.selected2d " + selected2d);
                var items4b = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name,
                });
                ViewBag.Branch4b = new SelectList(items4b, "Id", "Name", selected4b);

                ViewBag.ControllingBranchId = selected4b;

                ViewBag.ControllingDepartmentId = selected2d;

                var selected1c = (from sub1 in db.Employes
                                  where sub1.Id == lcontrolling
                                  select sub1.CurrentDesignation).FirstOrDefault();
                LogInformation.Info("RegisterController.selected1c " + selected1c);


                var lControllingSelectValue = (from sub1 in db.Employes
                                               where sub1.Id == lcontrolling
                                               select sub1.Branch_Value1).FirstOrDefault();
                int lvalue = Convert.ToInt32(lControllingSelectValue);
                string lHeadValueName = db.Branches.Where(a => a.Id == lvalue).Select(a => a.Name).FirstOrDefault();
                LogInformation.Info("RegisterController.lHeadValueName " + lHeadValueName);
                ViewBag.myVar5 = lHeadValueName;

                var lbranchdesig = db.Branch_Designation_Mapping.ToList();
                var ldesignation = db.Designations.ToList();
                //int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
                int lHeadofficeValue = (from sub1 in db.Branches
                                        where sub1.Name == "HeadOffice"
                                        select sub1.Id).FirstOrDefault();
                LogInformation.Info("RegisterController.lHeadofficeValue " + lHeadofficeValue);
                var items3c = (from emplist in lbranchdesig
                               join desig in ldesignation on emplist.DesignationId equals desig.Id
                               where emplist.BranchId == lHeadofficeValue
                               select new
                               {
                                   desig.Id,
                                   desig.Name
                               });
                LogInformation.Info("RegisterController.items3c " + items3c);
                ViewBag.CurrentDesignation3c = new SelectList(items3c, "Id", "Name");
                ViewBag.ControllingDesignationid = selected1c;


                var items2d = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });
                LogInformation.Info("RegisterController.items2d " + items2d);
                ViewBag.Department2d = new SelectList(items2d, "Id", "Name", selected2d);



                var selected5 = (from sub in db.Employes
                                 where sub.Id == id && sub.RetirementDate >= DateTime.Now
                                 select sub.ControllingAuthority).FirstOrDefault();
                LogInformation.Info("RegisterController.selected5 " + selected5);
                var items7 = Facade.EntitiesFacade.GetAll().Where(a => a.RetirementDate >= DateTime.Now).Select(x => new Employees
                {
                    Id = x.Id,
                    ControllingAuthority = x.FirstName + " " + x.LastName,
                });
                LogInformation.Info("RegisterController.items7 " + items7);
                ViewBag.ControllingAuthority = new SelectList(items7, "Id", "ControllingAuthority", selected5);

                ViewBag.ControllingAuthorityId = selected5;

                //Sancationing

                var selected2sd = (from sub in db.Employes
                                   join dep in db.Departments on sub.Department equals dep.Id
                                   where sub.Id == lSancationing && dep.Active == 1
                                   select sub.Department).FirstOrDefault();
                var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Department2sd = new SelectList(items2sd, "Id", "Name", selected2sd);
                //var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment").Select(x => new Departments
                //{
                //    Id = x.Id,
                //    Name = x.Name
                //});
                //ViewBag.Department2sd = new SelectList(items2sd, "Id", "Name", selected2sd);
                ViewBag.SancationingDepartmentId = selected2sd;

                var selected4sb = (from sub in db.Employes
                                   where sub.Id == id
                                   select sub.Branch).FirstOrDefault();
                LogInformation.Info("RegisterController.selected4sb " + selected4sb);
                var items4sb = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Id == selected2sd).Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name,
                });
                ViewBag.Branch4sb = new SelectList(items4b, "Id", "Name", selected4sb);

                string rolename = db.Roles.Where(a => a.Id == id).Select(a => a.Name).FirstOrDefault();


                var selected1sc = (from sub1 in db.Employes
                                   where sub1.Id == lSancationing
                                   select sub1.CurrentDesignation).FirstOrDefault();
                ViewBag.SancationingDesignationId = selected1sc;
                LogInformation.Info("RegisterController.selected1sc " + selected1sc);

                var items3sc = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.CurrentDesignation3sc = new SelectList(items3sc, "Id", "Name", selected1sc);


                var selected6 = (from sub in db.Employes
                                 where sub.Id == id && sub.RetirementDate >= DateTime.Now
                                 select sub.SanctioningAuthority).FirstOrDefault();
                LogInformation.Info("RegisterController.selected6 " + selected6);
                var items8 = Facade.EntitiesFacade.GetAll().Where(a => a.CurrentDesignation == selected1sc && a.RetirementDate >= DateTime.Now).Select(x => new Employees
                {
                    Id = x.Id,
                    SanctioningAuthority = x.FirstName + " " + x.LastName,
                });
                ViewBag.SanctioningAuthority = new SelectList(items8, "Id", "SanctioningAuthority", selected6);

                ViewBag.SanctioningAuthorityId = selected6;

                Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
                var lPassword = lencrptypassword.Decrypt(edata.Password);
                data.Password = lPassword;
                TempData["Loginmode"] = lCredentials.LoginMode;
                ViewBag.Message = lCredentials.LoginMode;
                ViewData["photo"] = data.UploadPhoto;
                int lofficevalue = Convert.ToInt32(data.Branch_Value1);
                string lValuess = db.Branches.Where(a => a.Id == lofficevalue).Select(a => a.Name).FirstOrDefault();
                ViewBag.myVar3 = lValuess;
                ViewBag.myVar4 = data.Branch_Value_2;
                Session["Data"] = null;
                LogInformation.Info("RegisterController.data " + data);
                //System.Threading.Thread.Sleep(4000);
                return View(data);
            }

            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.Index() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.Index() - Stack trace: " + ex.StackTrace);
                return View();
            }
        }

        public void SetActiveImage(string image)
        {
            try
            {
                string value = Session.GetDataFromSession<string>("ActiveImage");
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.SetActiveImage() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.SetActiveImage() - Stack trace: " + ex.StackTrace);
            }
        }


        public DateTime getFormatDate(DateTime dt)
        {
            try
            {
                DateTime theDate = dt;
                string myTime = theDate.ToString("MM/dd/yyyy");
                return Convert.ToDateTime(myTime);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.getFormatDate() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.getFormatDate() - Stack trace: " + ex.StackTrace);
            }
            return DateTime.Now;
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult InsertEmployees(Employees lmodel1)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                var shift = Facade.EntitiesFacade.GetAllShifts().Select(x => new Shift_Master
                {
                    Id = x.Id,
                    ShiftType = x.GroupName + " - " + x.InTime + " " + x.OutTime
                });
                ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType");
                var items = Facade.EntitiesFacade.GetAll().Select(x => new Employees
                {
                    Id = x.Id,
                    Category = x.Category
                }).Distinct();
                ViewBag.Category = new SelectList(items, "Id", "Category");

                var items1 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.JoinedDesignation = new SelectList(items1, "Id", "Name");

                var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.CurrentDesignation = new SelectList(items2, "Id", "Name");

                var lbranchdesig = db.Branch_Designation_Mapping.ToList();
                var ldesignation = db.Designations.ToList();
                int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
                //var itemss = (from emplist in lbranchdesig
                //              join desig in ldesignation on emplist.DesignationId equals desig.Id
                //              where emplist.BranchId == lHeadofficeValue
                //              select new
                //              {
                //                  desig.Id,
                //                  desig.Name
                //              });
                var itemss = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.CurrentDesignationhead = new SelectList(itemss, "Id", "Name");

                Session["Data"] = null;



                var items3 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();

                ViewBag.Department = new SelectList(items3, "Id", "Name");
                var items4 = Facade.EntitiesFacade.GetAllRoles().Select(x => new Roles
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.Role = new SelectList(items4, "Id", "Name");
                var items5 = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,

                    Name = x.Name.ToString()
                }).Distinct();
                ViewBag.Branch = new SelectList(items5, "Id", "Name");
                //controllings
                var items2c = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.CurrentDesignation2c = new SelectList(items2c, "Id", "Name");
                var items3d = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();

                ViewBag.Department3d = new SelectList(items3d, "Id", "Name");
                var items5b = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name.ToString()
                }).Distinct();
                ViewBag.Branch5b = new SelectList(items5b, "Id", "Name");

                //Sancationing
                var items3sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();

                ViewBag.Department3sd = new SelectList(items3sd, "Id", "Name");
                var items2sc = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.CurrentDesignation2sc = new SelectList(items2sc, "Id", "Name");
                var items5sb = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name.ToString()
                }).Distinct();
                ViewBag.Branch5sb = new SelectList(items5sb, "Id", "Name");



                var items6 = Facade.EntitiesFacade.GetAll().Select(x => new Employees
                {
                    Id = x.Id,
                    ControllingAuthority = x.FirstName.ToString() + " " + x.LastName.ToString()

                }).Distinct();

                var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Sanctioningddepartment = new SelectList(items2sd, "Id", "Name");

                //var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment").Select(x => new Departments
                //{
                //    Id = x.Id,
                //    Name = x.Name
                //});
                //ViewBag.Sanctioningddepartment = new SelectList(items2sd, "Id", "Name");

                ViewBag.ControllingAuthority = new SelectList(items6, "ControllingAuthority", "ControllingAuthority");
                var items7 = Facade.EntitiesFacade.GetAll().Where(a => a.RetirementDate >= DateTime.Now).Select(x => new Employees
                {
                    Id = x.Id,
                    SanctioningAuthority = x.FirstName.ToString() + " " + x.LastName.ToString()
                }).Distinct();
                ViewBag.SanctioningAuthority = new SelectList(items7, "SanctioningAuthority", "SanctioningAuthority");
                if (TempData["SuccessStatus"] != null)
                {
                    ViewBag.Success = TempData["SuccessStatus"].ToString();
                }
                TempData["Loginmode"] = lCredentials.LoginMode;
                Employees lmodel = new Employees();
                lmodel.LoginMode = lCredentials.LoginMode;
                return View("~/Views/Register/EmployeeRegistration.cshtml", lmodel);
            }

            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.InsertEmployees() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.InsertEmployees() - Stack trace: " + ex.StackTrace);

            }
            return View("~/Views/Register/EmployeeRegistration.cshtml");
        }


        [HttpPost]
        public ActionResult InsertEmployeesadmin(Employees Employee, HttpPostedFileBase file, FormCollection form)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                var passwordValidation = ValidatePasswordStrength(Employee.Password);
                if (!passwordValidation.IsValid)
                {
                    TempData["AlertMessage"] = passwordValidation.Message;
                    return RedirectToAction("InsertEmployees");
                }
                Employee.UpdatedDate = GetCurrentTime(DateTime.Today);
                Employee.Password = lencrptypassword.Encrypt(Employee.Password);

                // if (ModelState.IsValid)
                //  {

                if (file != null)
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/uploads"), Employee.EmpId + " " + pic);
                    file.SaveAs(path);

                    Employee.UploadPhoto = Employee.EmpId + " " + file.FileName.ToString();

                    string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                    if (IsEmployeeExists(Employee.EmpId))
                    {
                        TempData["AlertMessage"] = "Employee with EmpCode is already exists";
                        return View("~/Views/Register/EmployeeRegistration.cshtml");
                    }
                    Employee.LoginMode = rolename;
                    if (Employee.Branch_Value1 == "HeadOffice")
                    {
                        string lbranch = Employee.Branch_Value1;
                        string lvalue1 = "HeadOffice";
                        int Id = db.Branches.Where(a => a.Name == lvalue1).Select(a => a.Id).FirstOrDefault();
                        string lvalue2 = "OtherBranch";
                        int Id1 = db.Branches.Where(a => a.Name == lvalue2).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch = Id1;
                        Employee.Branch_Value1 = Convert.ToString(Id1);

                    }
                    else
                    {
                        int lbranch = Employee.Branch;
                        int Id = db.Branches.Where(a => a.Id == lbranch).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch = Id;
                        string lvalue1 = "OtherDepartment";
                        int lId1 = db.Departments.Where(a => a.Code == lvalue1).Select(a => a.Id).FirstOrDefault();
                        Employee.Department = lId1;
                        string lvalue2 = "OtherBranch";
                        int lId2 = db.Branches.Where(a => a.Name == lvalue2).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch_Value1 = Convert.ToString(lId2);
                    }
                    Employee.PerBranch = Employee.Branch;
                    Employee.Role = Employee.Role;

                    Employee.PerDepartment = Employee.Department;
                    db.Employes.Add(Employee);
                    Employee.UpdatedBy = lCredentials.EmpId;
                    Employee.UpdatedDate = GetCurrentTime(DateTime.Now);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Employee Created Successfully";
                    //string empid = Employee.EmpId;
                    //int lid = db.Employes.Where(a => a.EmpId == Employee.EmpId).Select(a => a.Id).FirstOrDefault();
                    //int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    //LoginBus lbus = new LoginBus();
                    //lbus.Empleavebalance(empid,LoginHelper.GetCurrentUser().EmpId);
                    int lid = db.Employes.Where(a => a.EmpId == Employee.EmpId).Select(a => a.Id).FirstOrDefault();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    LoginBus lbus = new LoginBus();
                    string currentyears = DateTime.Now.Year.ToString();
                    lbus.Empleavebalance(lid.ToString(), lCredentials.EmpPkId, currentyears);
                    return RedirectToAction("InsertEmployees");
                }
                else
                {
                    if (IsEmployeeExists(Employee.EmpId))
                    {
                        TempData["AlertMessage"] = "Employee Code is already exists";
                        return View("~/Views/Register/EmployeeRegistration.cshtml");
                    }
                    string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                    if (Employee.Branch_Value1 == "HeadOffice")
                    {
                        string lbranch = Employee.Branch_Value1;
                        string lvalue1 = "HeadOffice";
                        int Id = db.Branches.Where(a => a.Name == lvalue1).Select(a => a.Id).FirstOrDefault();
                        string lvalue2 = "OtherBranch";
                        int Id1 = db.Branches.Where(a => a.Name == lvalue2).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch = Id1;
                        Employee.Branch_Value1 = Convert.ToString(Id);
                        var shift = db.Shift_Master.Where(a => a.BranchId == Id1).Select(x => new SelectListItem()
                        {
                            Value = x.Id.ToString(),
                            Text = x.GroupName + " - " + x.InTime + " " + x.OutTime
                        });
                        var stiftimes = shift.Where(a => a.Value == Id1.ToString()).Select(x => x.Text);
                        var stiftimesval = shift.Where(a => a.Value == Id1.ToString()).Select(x => x.Value).FirstOrDefault();
                        Employee.Shift_Id = Convert.ToInt32(stiftimesval);
                    }
                    else
                    {
                        int lbranch = Employee.Branch;
                        int Id = db.Branches.Where(a => a.Id == lbranch).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch = Id;
                        string lvalue1 = "OtherDepartment";
                        int lId1 = db.Departments.Where(a => a.Code == lvalue1).Select(a => a.Id).FirstOrDefault();
                        Employee.Department = lId1;
                        string lvalue2 = "OtherBranch";
                        int lId2 = db.Branches.Where(a => a.Name == lvalue2).Select(a => a.Id).FirstOrDefault();
                        Employee.Branch_Value1 = Convert.ToString(lId2);
                        var shift = db.Shift_Master.Where(a => a.BranchId == lbranch).Select(x => new SelectListItem()
                        {
                            Value = x.Id.ToString(),
                            Text = x.GroupName + " - " + x.InTime + " " + x.OutTime
                        });
                        var stiftimes = shift.Where(a => a.Value == lbranch.ToString()).Select(x => x.Text);
                        var stiftimesval = shift.Where(a => a.Value == lbranch.ToString()).Select(x => x.Value).FirstOrDefault();
                        Employee.Shift_Id = Convert.ToInt32(stiftimesval);
                    }

                    Employee.UploadPhoto = Employee.EmpId + " " + "m.png";
                    Employee.LoginMode = rolename;
                    Employee.PerBranch = Employee.Branch;
                    Employee.PerDepartment = Employee.Department;
                    db.Employes.Add(Employee);
                    Employee.UpdatedBy = lCredentials.EmpId;
                    Employee.UpdatedDate = GetCurrentTime(DateTime.Now);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Employee Created Successfully";
                    int lid = db.Employes.Where(a => a.EmpId == Employee.EmpId).Select(a => a.Id).FirstOrDefault();
                    int lEmpId = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    string currentyears = DateTime.Now.Year.ToString();
                    LoginBus lbus = new LoginBus();
                    lbus.Empleavebalance(lid.ToString(), lCredentials.EmpPkId, currentyears);

                    return RedirectToAction("InsertEmployees");
                }

            }


            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.InsertEmployeesadmin() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.InsertEmployeesadmin() - Stack trace: " + ex.StackTrace);
            }
            return RedirectToAction("InsertEmployees");
        }

        public bool IsEmployeeExists(string EmpId)
        {
            try
            {
                bool flag = false;

                int count = db.Employes.Where(a => a.EmpId == EmpId).Count();
                if (count != 0)
                {
                    flag = true;
                }
                return flag;
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.IsEmployeeExists() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.IsEmployeeExists() - Stack trace: " + ex.StackTrace);
            }
            return false;
        }


        public JsonResult checkEmailExistOrNot(string empid)
        {
            try
            {
                var empids = from u in db.Employes where u.EmpId.Equals(empid) select u;
                int count = empids.Count();
                if (empid.StartsWith("0"))
                {
                    return Json(new { message = "zero" }, JsonRequestBehavior.AllowGet);
                }
                else
                if (count == 0)
                {


                    return Json(new { message = "use" }, JsonRequestBehavior.AllowGet);


                }
                else
                {

                    return Json(new { message = "used" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.checkEmailExistOrNot() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.checkEmailExistOrNot() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }

        [HttpPost]
        public ActionResult UpdateViewProfile(Employees Employee, HttpPostedFileBase file, string EmployeeImage)
        {
            LogInformation.Info("Code started ");
            try
            {
                if (Employee.ControllingDepartment == 0)
                {
                    TempData["AlertMessage"] = "Controlling Department cannot be null";
                }
                else if (Employee.ControllingBranch == 0)
                {
                    TempData["AlertMessage"] = "Controlling Branch cannot be null";
                }
                else if (Employee.SanctioningDepartment == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning Department cannot be null";
                }
                else if (Employee.CurrentDesignation == 0)
                {
                    TempData["AlertMessage"] = "current Designation cannot be null";
                }
                else if (Employee.ControllingDesignation == 0)
                {
                    TempData["AlertMessage"] = "Controlling Designation cannot be null";
                }
                else if (Employee.SanctioningDesignation == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning Designation cannot be null";
                }
                else if (Convert.ToInt32(Employee.SanctioningAuthority) == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning Authority cannot be null";
                }
                else if (Convert.ToInt32(Employee.ControllingAuthority) == 0)
                {
                    TempData["AlertMessage"] = "Controlling Authority cannot be null";
                }
                else if (Convert.ToInt32(Employee.CurrentDesignation) == 0)
                {
                    TempData["AlertMessage"] = "Current Designation cannot be null";
                }
                else
                {
                    // Validate password strength before processing
                    var passwordValidation = ValidatePasswordStrength(Employee.Password);
                    if (!passwordValidation.IsValid)
                    {
                        TempData["AlertMessage"] = passwordValidation.Message;
                        return RedirectToAction("ViewProfile");
                    }
                    int id = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    var data = new Persistence.EmployeesRepository().GetIt(id);

                    //if (ModelState.IsValid)
                    //{
                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/uploads"), lCredentials.EmpId + " " + pic);
                        file.SaveAs(path);
                        LogInformation.Info("Register, Images. Info: " + path);
                        Int32 wempid = Convert.ToInt32(Employee.EmpId);
                        //WorkDiary workdata = (from w in db.WorkDiary where w.EmpId == wempid orderby w.Id descending select w).First();
                        WorkDiaryBus Wbus = new WorkDiaryBus();
                        var dt = Wbus.UpdateWorkdiaryControlling(wempid, Employee.ControllingAuthority, Employee.SanctioningAuthority);

                        Int32 Oempid = Convert.ToInt32(Employee.Id);
                        OD_OtherDuty Odata = (from o in db.OD_OtherDuty where o.EmpId == Oempid orderby o.Id descending select o).First();

                        Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
                        edata.FirstName = Employee.FirstName;
                        edata.LastName = Employee.LastName;
                        edata.ShortName = Employee.ShortName;
                        edata.Password = lencrptypassword.Encrypt(Employee.Password);
                        edata.Gender = Employee.Gender;
                        edata.MartialStatus = Employee.MartialStatus;
                        edata.SpouseName = Employee.SpouseName;
                        edata.DOB = Employee.DOB;
                        edata.FatherName = Employee.FatherName;
                        edata.MotherName = Employee.MotherName;
                        edata.MobileNumber = Employee.MobileNumber;
                        edata.HomeNumber = Employee.HomeNumber;
                        edata.PersonalEmailId = Employee.PersonalEmailId;
                        edata.PresentAddress = Employee.PresentAddress;
                        edata.PermanentAddress = Employee.PermanentAddress;
                        edata.Graduation = Employee.Graduation;
                        edata.PostGraduation = Employee.PostGraduation;
                        edata.Exit_type = Employee.Exit_type;
                        edata.OtherQualification = Employee.OtherQualification;
                        edata.EmergencyName = Employee.EmergencyName;
                        edata.EmergencyContactNo = Employee.EmergencyContactNo;
                        edata.Category = Employee.Category;
                        edata.EmpId = Employee.EmpId;
                        edata.Branch = Employee.Branch;
                        edata.Shift_Id = Employee.Shift_Id;
                        edata.PerBranch = Employee.Branch;
                        edata.PerDepartment = Employee.Department;
                        edata.JoinedDesignation = Employee.JoinedDesignation;
                        edata.CurrentDesignation = Employee.CurrentDesignation;
                        edata.Department = Employee.Department;
                        edata.Role = Employee.Role;
                        edata.OfficalEmailId = Employee.OfficalEmailId;
                        edata.TotalExperience = Employee.TotalExperience;
                        edata.DOJ = Employee.DOJ;
                        edata.RelievingDate = Employee.RelievingDate;
                        edata.RetirementDate = Employee.RetirementDate;
                        edata.photo = Employee.photo;
                        if (lCredentials.LoginMode == Constants.SuperAdmin || lCredentials.LoginMode == Constants.AdminHRDPayments || lCredentials.LoginMode == Constants.AdminHRDPolicy || lCredentials.LoginMode == Constants.Employee)
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }
                        else
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }
                        if (EmployeeImage != "" && file != null)
                        {
                            string photo = file.FileName.ToString();
                            edata.UploadPhoto = lCredentials.EmpId + " " + photo;
                        }
                        else if (EmployeeImage == "")
                        {
                            string photo = file.FileName.ToString();
                            edata.UploadPhoto = lCredentials.EmpId + " " + photo;
                        }

                        edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                        edata.RelievingReason = Employee.RelievingReason;
                        edata.BloodGroup = Employee.BloodGroup;
                        edata.AadharCardNo = Employee.AadharCardNo;
                        edata.PanCardNo = Employee.PanCardNo;
                        edata.ProfessionalQualifications = Employee.ProfessionalQualifications;
                        edata.ControllingAuthority = Employee.ControllingAuthority;
                        //workdata.CA =Convert.ToInt32(Employee.ControllingAuthority);
                        //workdata.SA =Convert.ToInt32(Employee.SanctioningAuthority);

                        Odata.ControllingAuthority = Convert.ToInt32(Employee.ControllingAuthority);
                        Odata.SanctioningAuthority = Convert.ToInt32(Employee.SanctioningAuthority);

                        edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                        edata.LoginMode = rolename;
                        int roleid = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Id).FirstOrDefault();
                        edata.Role = roleid;
                        if (Employee.Branch_Value1 == "42")
                        {
                            edata.Department = Employee.Department;

                            string lBvalue = "OtherBranch";

                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Branch = lBranch;
                        }
                        else
                        {
                            edata.Branch = Employee.Branch;

                            string lDvalue = "OtherDepartment";

                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Department = ldept;
                        }
                        int lOldControlling = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                        int lOldSacantioning = Convert.ToInt32(Session["OldlSancation"].ToString());
                        int NewControlling = Convert.ToInt32(edata.ControllingAuthority);
                        int NewSancationing = Convert.ToInt32(edata.SanctioningAuthority);
                        if (lOldControlling == NewControlling && NewSancationing == lOldSacantioning)
                        {
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            //db.Entry(workdata).State = EntityState.Modified;
                            //db.SaveChanges();

                            db.Entry(Odata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";

                        }
                        else
                        {
                            var ldbresult = db.Leaves.ToList();
                            var lupdate = (from l in ldbresult where l.ControllingAuthority == lOldControlling && l.SanctioningAuthority == lOldSacantioning && l.EmpId == id select l);
                            foreach (var item in lupdate)
                            {

                                item.ControllingAuthority = NewControlling;
                                item.SanctioningAuthority = NewSancationing;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                    }
                    else
                    {
                        Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
                        edata.FirstName = Employee.FirstName;
                        edata.LastName = Employee.LastName;
                        edata.ShortName = Employee.ShortName;
                        edata.Password = lencrptypassword.Encrypt(Employee.Password);
                        edata.Gender = Employee.Gender;
                        edata.MartialStatus = Employee.MartialStatus;
                        edata.SpouseName = Employee.SpouseName;
                        edata.DOB = Employee.DOB;
                        edata.FatherName = Employee.FatherName;
                        edata.MotherName = Employee.MotherName;
                        edata.Exit_type = Employee.Exit_type;
                        edata.MobileNumber = Employee.MobileNumber;
                        edata.HomeNumber = Employee.HomeNumber;
                        edata.PersonalEmailId = Employee.PersonalEmailId;
                        edata.PresentAddress = Employee.PresentAddress;
                        edata.PermanentAddress = Employee.PermanentAddress;
                        edata.Graduation = Employee.Graduation;
                        edata.PostGraduation = Employee.PostGraduation;
                        edata.OtherQualification = Employee.OtherQualification;
                        edata.EmergencyName = Employee.EmergencyName;
                        edata.EmergencyContactNo = Employee.EmergencyContactNo;
                        edata.Category = Employee.Category;
                        edata.EmpId = Employee.EmpId;
                        edata.Branch = Employee.Branch;
                        edata.PerBranch = Employee.Branch;
                        edata.Shift_Id = Employee.Shift_Id;
                        edata.PerDepartment = Employee.Department;
                        edata.JoinedDesignation = Employee.JoinedDesignation;
                        edata.CurrentDesignation = Employee.CurrentDesignation;
                        edata.Department = Employee.Department;
                        edata.Role = Employee.Role;
                        edata.OfficalEmailId = Employee.OfficalEmailId;
                        edata.TotalExperience = Employee.TotalExperience;
                        edata.DOJ = Employee.DOJ;
                        edata.RelievingDate = Employee.RelievingDate;
                        edata.RetirementDate = Employee.RetirementDate;
                        edata.photo = Employee.photo;
                        if (lCredentials.LoginMode == Constants.SuperAdmin || lCredentials.LoginMode == Constants.AdminHRDPayments || lCredentials.LoginMode == Constants.AdminHRDPolicy || lCredentials.LoginMode == Constants.Employee)
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }

                        else
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }
                        if (EmployeeImage != "")
                        {
                            edata.UploadPhoto = EmployeeImage;
                        }
                        else
                        {
                            edata.UploadPhoto = file.FileName.ToString();
                        }

                        edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                        edata.RelievingReason = Employee.RelievingReason;
                        edata.BloodGroup = Employee.BloodGroup;
                        edata.AadharCardNo = Employee.AadharCardNo;
                        edata.PanCardNo = Employee.PanCardNo;
                        edata.ProfessionalQualifications = Employee.ProfessionalQualifications;
                        string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                        edata.LoginMode = rolename;
                        int roleid = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Id).FirstOrDefault();
                        edata.Role = roleid;
                        if (Employee.Branch_Value1 == "42")
                        {
                            edata.Department = Employee.Department;

                            string lBvalue = "OtherBranch";

                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Branch = lBranch;
                        }
                        else
                        {
                            edata.Branch = Employee.Branch;

                            string lDvalue = "OtherDepartment";

                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Department = ldept;
                        }
                        int lOldControlling = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                        int lOldSacantioning = Convert.ToInt32(Session["OldlSancation"].ToString());
                        int NewControlling = Convert.ToInt32(edata.ControllingAuthority);
                        int NewSancationing = Convert.ToInt32(edata.SanctioningAuthority);
                        if (lOldControlling == NewControlling && NewSancationing == lOldSacantioning)
                        {
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                        else
                        {
                            var ldbresult = db.Leaves.ToList();
                            var lupdate = (from l in ldbresult where l.ControllingAuthority == lOldControlling && l.SanctioningAuthority == lOldSacantioning && l.EmpId == id select l);
                            foreach (var item in lupdate)
                            {

                                item.ControllingAuthority = NewControlling;
                                item.SanctioningAuthority = NewSancationing;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            var lduty = db.OD_OtherDuty.ToList();
                            var lodupdate = (from l in lduty where l.ControllingAuthority == lOldControlling && l.SanctioningAuthority == lOldSacantioning && l.EmpId == id select l);
                            foreach (var item1 in lodupdate)
                            {

                                item1.ControllingAuthority = NewControlling;
                                item1.SanctioningAuthority = NewSancationing;
                                db.Entry(item1).State = EntityState.Modified;
                            }
                            var lwork = db.WorkDiary.ToList();
                            var lworkupdate = (from l in lwork where l.CA == lOldControlling && l.SA == lOldSacantioning && l.EmpId == id select l);
                            foreach (var item2 in lodupdate)
                            {

                                item2.ControllingAuthority = NewControlling;
                                item2.SanctioningAuthority = NewSancationing;
                                db.Entry(item2).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                    }
                    int roleid1 = db.Roles.Where(a => a.Id.ToString() == lCredentials.Role).Select(a => a.Id).FirstOrDefault();
                    if (roleid1 != Employee.Role)
                    {
                        TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                        return RedirectToAction("Index", "Home");

                    }
                    int Branchid = db.Branches.Where(a => a.Id.ToString() == lCredentials.Branch).Select(a => a.Id).FirstOrDefault();
                    int Deptid = db.Departments.Where(a => a.Id.ToString() == lCredentials.Department).Select(a => a.Id).FirstOrDefault();
                    int currentdesignationid = db.Designations.Where(a => a.Id.ToString() == lCredentials.Designation).Select(a => a.Id).FirstOrDefault();
                    int lOldControlling1 = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                    int lOldSacantioning1 = Convert.ToInt32(Session["OldlSancation"].ToString());
                    int NewControlling1 = Convert.ToInt32(Employee.ControllingAuthority);
                    int NewSancationing1 = Convert.ToInt32(Employee.SanctioningAuthority);
                    if (lOldControlling1 != NewControlling1)
                    {
                        TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                        return RedirectToAction("Index", "Home");
                    }
                    if (lOldSacantioning1 != NewSancationing1)
                    {
                        TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                        return RedirectToAction("Index", "Home");
                    }
                    if (currentdesignationid != Employee.CurrentDesignation)
                    {
                        TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                        return RedirectToAction("Index", "Home");
                    }
                    if (Employee.Branch_Value1 == "42")
                    {
                        if (Employee.Department != Deptid)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {

                        if (Employee.Branch != Branchid)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    LogInformation.Info("Code ended ");
                    return RedirectToAction("ViewProfile", "Register");
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.UpdateProfile() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.UpdateProfile() - Stack trace: " + ex.StackTrace);
            }
            return RedirectToAction("ViewProfile", "Register");
        }
        [HttpPost]

        public ActionResult UpdateProfile(Employees Employee, HttpPostedFileBase file, string EmployeeImage)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            LogInformation.Info("Code started ");
            try
            {
                if (Employee.ControllingDepartment == 0)
                {
                    TempData["AlertMessage"] = "Controlling Department cannot be null";
                }
                else if (Employee.ControllingBranch == 0)
                {
                    TempData["AlertMessage"] = "Controlling Branch cannot be null";
                }
                else if (Employee.SanctioningDepartment == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning Department cannot be null";
                }
                else if (Employee.ControllingDesignation == 0)
                {
                    TempData["AlertMessage"] = "Controlling designation cannot be null";
                }
                else if (Employee.SanctioningDesignation == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning designation cannot be null";
                }
                else if (Convert.ToInt32(Employee.SanctioningAuthority) == 0)
                {
                    TempData["AlertMessage"] = "Sanctioning authority cannot be null";
                }
                else if (Convert.ToInt32(Employee.ControllingAuthority) == 0)
                {
                    TempData["AlertMessage"] = "controlling authority cannot be null";
                }
                else if (Convert.ToInt32(Employee.CurrentDesignation) == 0)
                {
                    TempData["AlertMessage"] = "Current Designation cannot be null";
                }
                else
                {
                 
        // Validate password strength before processing
                    var passwordValidation = ValidatePasswordStrength(Employee.Password);
                    if (!passwordValidation.IsValid)
                    {
                        TempData["AlertMessage"] = passwordValidation.Message;
                        return RedirectToAction("EmployeesRept", "AllReports");
                    }

                    int id1 = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                    int empid = 0;
                    int editgridEmpId = Employee.Id;
                    if (empid == editgridEmpId)
                    {
                        empid = Convert.ToInt32(Session["id"]);
                    }
                    else
                    {
                        empid = Employee.Id;
                    }


                    var data = new Persistence.EmployeesRepository().GetIt(empid);


                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/uploads"), data.EmpId + " " + pic);
                        file.SaveAs(path);
                        LogInformation.Info("Register, Images. Info: " + path);
                        Employees edata = (from u in db.Employes where u.Id == Employee.Id select u).FirstOrDefault();
                        edata.FirstName = Employee.FirstName;
                        edata.LastName = Employee.LastName;
                        edata.ShortName = Employee.ShortName;
                        edata.Password = lencrptypassword.Encrypt(Employee.Password);
                        edata.Gender = Employee.Gender;
                        edata.MartialStatus = Employee.MartialStatus;
                        edata.SpouseName = Employee.SpouseName;
                        edata.DOB = Employee.DOB;
                        edata.FatherName = Employee.FatherName;
                        edata.MotherName = Employee.MotherName;
                        edata.MobileNumber = Employee.MobileNumber;
                        edata.HomeNumber = Employee.HomeNumber;
                        edata.PersonalEmailId = Employee.PersonalEmailId;
                        edata.PresentAddress = Employee.PresentAddress;
                        edata.PermanentAddress = Employee.PermanentAddress;
                        edata.Exit_type = Employee.Exit_type;
                        edata.Graduation = Employee.Graduation;
                        edata.PostGraduation = Employee.PostGraduation;
                        edata.OtherQualification = Employee.OtherQualification;
                        edata.EmergencyName = Employee.EmergencyName;
                        edata.EmergencyContactNo = Employee.EmergencyContactNo;
                        edata.Category = Employee.Category;
                        edata.EmpId = Employee.EmpId;
                        edata.Branch = Employee.Branch;
                        edata.PerBranch = Employee.Branch;
                        edata.PerDepartment = Employee.Department;
                        edata.JoinedDesignation = Employee.JoinedDesignation;
                        edata.CurrentDesignation = Employee.CurrentDesignation;
                        edata.Department = Employee.Department;
                        edata.Role = Employee.Role;
                        edata.OfficalEmailId = Employee.OfficalEmailId;
                        edata.TotalExperience = Employee.TotalExperience;
                        edata.DOJ = Employee.DOJ;
                        edata.Shift_Id = Employee.Shift_Id;
                        edata.RelievingDate = Employee.RelievingDate;
                        edata.RetirementDate = Employee.RetirementDate;
                        edata.photo = Employee.photo;
                        if (lCredentials.LoginMode == Constants.SuperAdmin)
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;

                        }
                        else
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }
                        if (EmployeeImage != "" && file != null)
                        {
                            string photo = file.FileName.ToString();
                            edata.UploadPhoto = data.EmpId + " " + photo;
                        }
                        else if (EmployeeImage == "")
                        {
                            string photo = file.FileName.ToString();
                            edata.UploadPhoto = data.EmpId + " " + photo;
                        }

                        edata.UpdatedBy = lCredentials.EmpId;
                        edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                        edata.RelievingReason = Employee.RelievingReason;
                        edata.BloodGroup = Employee.BloodGroup;
                        edata.AadharCardNo = Employee.AadharCardNo;
                        edata.PanCardNo = Employee.PanCardNo;
                        edata.ProfessionalQualifications = Employee.ProfessionalQualifications;

                        string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                        edata.LoginMode = rolename;
                        int roleid = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Id).FirstOrDefault();
                        edata.Role = roleid;

                        if (Employee.Branch_Value1 == "42")
                        {
                            edata.Department = Employee.Department;

                            string lBvalue = "OtherBranch";

                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Branch = lBranch;
                        }
                        else
                        {
                            edata.Branch = Employee.Branch;

                            string lDvalue = "OtherDepartment";

                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Department = ldept;
                        }

                        int lOldControlling = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                        int lOldSacantioning = Convert.ToInt32(Session["OldlSancation"].ToString());
                        int NewControlling = Convert.ToInt32(edata.ControllingAuthority);
                        int NewSancationing = Convert.ToInt32(edata.SanctioningAuthority);
                        if (lOldControlling == NewControlling && NewSancationing == lOldSacantioning)
                        {
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                        else
                        {
                            var ldbresult = db.Leaves.ToList();
                            var lupdates = (from l in ldbresult where l.ControllingAuthority == lOldControlling && l.SanctioningAuthority == lOldSacantioning && l.EmpId == id1 select l);
                            foreach (var item in lupdates)
                            {

                                item.ControllingAuthority = NewControlling;
                                item.SanctioningAuthority = NewSancationing;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                    }
                    else
                    {
                        Int32 wempid = Convert.ToInt32(Employee.EmpId);
                        Int32 Oempid = Convert.ToInt32(Employee.Id);
                        WorkDiaryBus Wbus = new WorkDiaryBus();
                        var dt = Wbus.UpdateWorkdiaryControlling(wempid, Employee.ControllingAuthority, Employee.SanctioningAuthority);
                        //var workdata = (from w in db.WorkDiary where w.EmpId == wempid && w.Status != "Approved" orderby w.Id descending  select w).ToList();
                        try
                        {
                            OD_OtherDuty Odata = (from o in db.OD_OtherDuty where o.EmpId == Oempid orderby o.Id descending select o).First();
                        }
                        catch (Exception e)
                        { }

                        Employees edata = (from u in db.Employes where u.Id == Employee.Id select u).FirstOrDefault();
                        edata.FirstName = Employee.FirstName;
                        edata.LastName = Employee.LastName;
                        edata.ShortName = Employee.ShortName;
                        edata.Password = lencrptypassword.Encrypt(Employee.Password);
                        edata.Gender = Employee.Gender;
                        edata.MartialStatus = Employee.MartialStatus;
                        edata.SpouseName = Employee.SpouseName;
                        edata.DOB = Employee.DOB;
                        edata.FatherName = Employee.FatherName;
                        edata.MotherName = Employee.MotherName;
                        edata.MobileNumber = Employee.MobileNumber;
                        edata.HomeNumber = Employee.HomeNumber;
                        edata.PersonalEmailId = Employee.PersonalEmailId;
                        edata.PresentAddress = Employee.PresentAddress;
                        edata.PermanentAddress = Employee.PermanentAddress;
                        edata.Exit_type = Employee.Exit_type;
                        edata.Graduation = Employee.Graduation;
                        edata.PostGraduation = Employee.PostGraduation;
                        edata.OtherQualification = Employee.OtherQualification;
                        edata.EmergencyName = Employee.EmergencyName;
                        edata.EmergencyContactNo = Employee.EmergencyContactNo;
                        edata.Category = Employee.Category;
                        edata.EmpId = Employee.EmpId;
                        edata.Branch = Employee.Branch;
                        edata.PerBranch = Employee.Branch;
                        edata.PerDepartment = Employee.Department;
                        edata.JoinedDesignation = Employee.JoinedDesignation;
                        edata.CurrentDesignation = Employee.CurrentDesignation;
                        edata.Department = Employee.Department;
                        edata.Role = Employee.Role;
                        edata.Shift_Id = Employee.Shift_Id;
                        edata.OfficalEmailId = Employee.OfficalEmailId;
                        edata.TotalExperience = Employee.TotalExperience;
                        edata.DOJ = Employee.DOJ;
                        edata.RelievingDate = Employee.RelievingDate;
                        edata.RetirementDate = Employee.RetirementDate;
                        edata.photo = Employee.photo;
                        if (lCredentials.LoginMode == Constants.SuperAdmin)
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            //workdata.CA =Convert.ToInt32(Employee.ControllingAuthority);
                            //workdata.SA =Convert.ToInt32(Employee.SanctioningAuthority);

                            //Odata.ControllingAuthority = Convert.ToInt32(Employee.ControllingAuthority);
                            //Odata.SanctioningAuthority = Convert.ToInt32(Employee.SanctioningAuthority);

                            edata.SanctioningAuthority = Employee.SanctioningAuthority;


                        }
                        else
                        {
                            edata.ControllingDepartment = Employee.ControllingDepartment;
                            edata.ControllingDesignation = Employee.ControllingDesignation;
                            edata.ControllingBranch = Employee.ControllingBranch;
                            edata.SanctioningBranch = Employee.SanctioningBranch;
                            edata.SanctioningDesignation = Employee.SanctioningDesignation;
                            edata.SanctioningDepartment = Employee.SanctioningDepartment;
                            edata.ControllingAuthority = Employee.ControllingAuthority;
                            //workdata.CA = Convert.ToInt32(Employee.ControllingAuthority);
                            //workdata.SA = Convert.ToInt32(Employee.SanctioningAuthority);

                            //Odata.ControllingAuthority = Convert.ToInt32(Employee.ControllingAuthority);
                            //Odata.SanctioningAuthority = Convert.ToInt32(Employee.SanctioningAuthority);

                            edata.SanctioningAuthority = Employee.SanctioningAuthority;
                        }
                        if (EmployeeImage != "")
                        {

                            edata.UploadPhoto = EmployeeImage;
                        }
                        else
                        {
                            edata.UploadPhoto = EmployeeImage;
                        }

                        edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                        edata.RelievingReason = Employee.RelievingReason;
                        edata.BloodGroup = Employee.BloodGroup;
                        edata.AadharCardNo = Employee.AadharCardNo;
                        edata.PanCardNo = Employee.PanCardNo;
                        edata.ProfessionalQualifications = Employee.ProfessionalQualifications;
                        string rolename = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Name).FirstOrDefault();
                        edata.LoginMode = rolename;
                        int roleid = db.Roles.Where(a => a.Id == Employee.Role).Select(a => a.Id).FirstOrDefault();
                        edata.Role = roleid;
                        if (Employee.Branch_Value1 == "42")
                        {
                            edata.Department = Employee.Department;

                            string lBvalue = "OtherBranch";

                            int lBranch = db.Branches.Where(a => a.Name == lBvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Branch = lBranch;
                        }
                        else
                        {
                            edata.Branch = Employee.Branch;

                            string lDvalue = "OtherDepartment";

                            int ldept = db.Departments.Where(a => a.Name == lDvalue).Select(a => a.Id).FirstOrDefault();

                            edata.Department = ldept;
                        }
                        int lOldControlling = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                        int lOldSacantioning = Convert.ToInt32(Session["OldlSancation"].ToString());
                        int NewControlling = Convert.ToInt32(edata.ControllingAuthority);
                        int NewSancationing = Convert.ToInt32(edata.SanctioningAuthority);
                        if (lOldControlling == NewControlling && NewSancationing == lOldSacantioning)
                        {
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                        else
                        {
                            var ldbresult = db.Leaves.ToList();
                            var lupdates = (from l in ldbresult where l.ControllingAuthority == lOldControlling && l.SanctioningAuthority == lOldSacantioning && l.EmpId == id1 select l);
                            foreach (var item in lupdates)
                            {

                                item.ControllingAuthority = NewControlling;
                                item.SanctioningAuthority = NewSancationing;
                                db.Entry(item).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                            edata.UpdatedBy = lCredentials.EmpId;
                            edata.UpdatedDate = GetCurrentTime(DateTime.Now);
                            edata.Branch_Value1 = Employee.Branch_Value1;
                            edata.Branch_Value_2 = Employee.Branch_Value_2;
                            db.Entry(edata).State = EntityState.Modified;
                            db.SaveChanges();
                            //db.Entry(workdata).State = EntityState.Modified;
                            //db.SaveChanges();

                            //db.Entry(Odata).State = EntityState.Modified;
                            db.SaveChanges();
                            TempData["AlertMessage"] = "Profile Updated Successfully";
                        }
                    }

                    int Branchid = db.Branches.Where(a => a.Id.ToString() == lCredentials.Branch).Select(a => a.Id).FirstOrDefault();
                    int Deptid = db.Departments.Where(a => a.Id.ToString() == lCredentials.Department).Select(a => a.Id).FirstOrDefault();
                    int currentdesignationid = db.Designations.Where(a => a.Id.ToString() == lCredentials.Designation).Select(a => a.Id).FirstOrDefault();
                    int lOldControlling1 = Convert.ToInt32(Session["Oldlcontrols"].ToString());
                    int lOldSacantioning1 = Convert.ToInt32(Session["OldlSancation"].ToString());
                    int NewControlling1 = Convert.ToInt32(Employee.ControllingAuthority);
                    int NewSancationing1 = Convert.ToInt32(Employee.SanctioningAuthority);
                    if (Employee.EmpId == lCredentials.EmpId)
                    {
                        if (Employee.Branch_Value1 == "42")
                        {
                            if (Employee.Department != Deptid)
                            {
                                TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {

                            if (Employee.Branch != Branchid)
                            {
                                TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        if (lOldControlling1 != NewControlling1)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");
                        }
                        if (lOldSacantioning1 != NewSancationing1)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");
                        }
                        if (currentdesignationid != Employee.CurrentDesignation)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");
                        }


                        int roleid1 = db.Roles.Where(a => a.Id.ToString() == lCredentials.Role).Select(a => a.Id).FirstOrDefault();
                        if (roleid1 != Employee.Role)
                        {
                            TempData["AlertMessage"] = "Profile updated Successfully Please ReLogin";
                            return RedirectToAction("Index", "Home");

                        }
                    }
                }
                LogInformation.Info("Code ended ");



            }

            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.Employeelistview() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.Employeelistview() - Stack trace: " + ex.StackTrace);
            }
            return RedirectToAction("EmployeesRept", "AllReports");
        }


        [NoDirectAccess]
        [HttpGet]
        public ActionResult Employeelist()
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            return View("~/Views/AllReports/Employeelist.cshtml");
        }
        public string GetBranchDepartmentConcat(string branch, string Department)
        {
            try
            {
                string requireformate = "";
                if (branch != "OtherBranch" && branch != "TBD")
                {
                    requireformate = branch;
                }
                if (Department != "OtherDepartment" && Department != "TBD")
                {
                    requireformate = Department;
                }

                if (branch != null && Department != null)
                {

                }
                else
                {
                    requireformate = requireformate.Replace('/', ' ');
                }
                return requireformate;
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.GetBranchDepartmentConcat() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.GetBranchDepartmentConcat() - Stack trace: " + ex.StackTrace);
            }
            return "";
        }

        [NoDirectAccess]
        [HttpGet]
        public JsonResult Employeelistview(string EmpId)

        {
            Session["lempid"] = EmpId;
            try
            {
                var dbResult = db.Employes.ToList();
                var Banks = db.Banks.ToList();
                var Branches = db.Branches.ToList();
                var Departments = db.Departments.ToList();
                var Designations = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                join branchlist in Branches on emplist.Branch equals branchlist.Id
                                join desig in Designations on emplist.CurrentDesignation equals desig.Id
                                join dept in Departments on emplist.Department equals dept.Id
                                where emplist.RetirementDate >= lStartDate && dept.Active == 1
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = GetFirstandLastName(emplist.FirstName, emplist.LastName),
                                    emplist.ShortName,
                                    Employeename = emplist.FirstName + " " + emplist.LastName,
                                    //emplist.CurrentDesignation,
                                    //emplist.Department,
                                    //deptb=dept.Name,
                                    desib = desig.Name,
                                    //branchb=branchlist.Name,
                                    Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),

                                    //  branchlist.Name,
                                    //emplist.Branch,
                                    emplist.Category,
                                    emplist.JoinedDesignation,
                                    emplist.Graduation,
                                    emplist.DOB,
                                    emplist.PersonalEmailId,
                                    emplist.PermanentAddress,
                                    emplist.PostGraduation,
                                    emplist.OtherQualification,
                                    emplist.EmergencyName,
                                    emplist.EmergencyContactNo,
                                    emplist.Role,
                                    emplist.OfficalEmailId,
                                    emplist.TotalExperience,
                                    emplist.DOJ,
                                    emplist.RelievingDate,
                                    emplist.RetirementDate,
                                    emplist.ControllingAuthority,
                                    emplist.SanctioningAuthority,
                                    emplist.UploadPhoto,
                                    emplist.UpdatedBy,
                                    emplist.UpdatedDate,
                                    emplist.RelievingReason,
                                    emplist.BloodGroup,
                                    emplist.AadharCardNo,
                                    emplist.PanCardNo

                                }).OrderByDescending(A => A.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var data = (from emplist in dbResult
                                join branchlist in Branches on emplist.Branch equals branchlist.Id
                                join desig in Designations on emplist.CurrentDesignation equals desig.Id
                                join dept in Departments on emplist.Department equals dept.Id
                                where (emplist.EmpId.ToLower().Contains(EmpId.ToLower())) || (emplist.FirstName.ToLower().Contains(EmpId.ToLower())) || (emplist.LastName.ToLower().Contains(EmpId.ToLower())) || (branchlist.Name.ToString().ToLower().Contains(EmpId.ToLower()) || (dept.Name.ToString().ToLower().Contains(EmpId.ToLower()) || (desig.Name.ToString().ToLower().Contains(EmpId.ToLower()))))
                                where emplist.RetirementDate >= lStartDate && dept.Active == 1
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    Name = GetFirstandLastName(emplist.FirstName, emplist.LastName),
                                    emplist.ShortName,
                                    Employeename = emplist.FirstName + " " + emplist.LastName,
                                    //emplist.CurrentDesignation,
                                    //emplist.Department,
                                    //deptb=dept.Name,
                                    desib = desig.Name,
                                    //branchb=branchlist.Name,
                                    Deptbranch = GetBranchDepartmentConcat(branchlist.Name, dept.Name),

                                    //  branchlist.Name,
                                    //emplist.Branch,
                                    emplist.Category,
                                    emplist.JoinedDesignation,
                                    emplist.Graduation,
                                    emplist.DOB,
                                    emplist.PersonalEmailId,
                                    emplist.PermanentAddress,
                                    emplist.PostGraduation,
                                    emplist.OtherQualification,
                                    emplist.EmergencyName,
                                    emplist.EmergencyContactNo,
                                    emplist.Role,
                                    emplist.OfficalEmailId,
                                    emplist.TotalExperience,
                                    emplist.DOJ,
                                    emplist.RelievingDate,
                                    emplist.RetirementDate,
                                    emplist.ControllingAuthority,
                                    emplist.SanctioningAuthority,
                                    emplist.UploadPhoto,
                                    emplist.UpdatedBy,
                                    emplist.UpdatedDate,
                                    emplist.RelievingReason,
                                    emplist.BloodGroup,
                                    emplist.AadharCardNo,
                                    emplist.PanCardNo

                                }).OrderByDescending(A => A.UpdatedDate);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.Edit() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.Edit() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }

        public string GetFirstandLastName(string FirstName, string LastName)
        {
            string lfirstname = "";
            lfirstname = FirstName.Length.ToString();
            int lfirst = Convert.ToInt32(lfirstname);
            if (lfirst >= 3)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            if (lfirst == 4)
            {
                lfirstname = FirstName.Substring(0, 2);
                if (lfirstname == "DR" || lfirstname == "Dr")
                {
                    lfirstname = lfirstname + "." + FirstName.Substring(2, 2);
                }
                else
                {
                    lfirstname = FirstName.Substring(0, 1);
                }
            }
            if (lfirst == 1)
            {
                lfirstname = FirstName.Substring(0, 1);
            }
            if (lfirst == 2)
            {
                lfirstname = FirstName.Substring(0, 2);
            }
            lfirstname = lfirstname + " " + LastName;
            return lfirstname;
        }

        [NoDirectAccess]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {
                DateTime dtt = DateTime.Now;

                int empid = (int)id;
                var data = new Persistence.EmployeesRepository().GetIt(empid);
                var shiftselected = TBUS.GetShiftids(empid);

                //var shiftselected = (from sub in db.Employes

                // where sub.Id == id
                //select sub.Shift_Id).FirstOrDefault();

                var shift = Facade.EntitiesFacade.GetAllShifts().Where(a => a.BranchId == data.Branch && dtt > a.ValidFrom && dtt < a.ValidTo).
                    Select(x => new Shift_Master
                    {
                        Id = x.Id,
                        ShiftType = x.GroupName + " - " + x.InTime + " " + x.OutTime
                    });
                int shiftselectid = Convert.ToInt32(shiftselected);
                ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType", shiftselectid);

                var selected = (from sub in db.Employes
                                where sub.Id == id
                                select sub.JoinedDesignation).FirstOrDefault();
                var items4 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.JoinedDesignation = new SelectList(items4, "Id", "Name", selected);

                var selected2 = (from sub in db.Employes
                                 join dep in db.Departments on sub.Department equals dep.Id
                                 where sub.Id == id && dep.Active == 1
                                 select sub.Department).FirstOrDefault();
                var employeelist = db.Employes.ToList();
                var selectedc = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.CurrentDesignation).FirstOrDefault();
                var items4c = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.CurrentDesig = new SelectList(items4c, "Id", "Name", selectedc);
                //for (int i = 0; i < employeelist.Count; i++)
                //{


                //    int empdss1 = db.Employes.Where(a => a.Id == id).Select(a => a.Id).FirstOrDefault();

                //    string control = db.Employes.Where(a => a.Id == id).Select(a => a.ControllingAuthority).FirstOrDefault();
                //    string sanction = db.Employes.Where(a => a.Id == id).Select(a => a.SanctioningAuthority).FirstOrDefault();
                //    int control1 = Convert.ToInt32(control);
                //    int sanction1 = Convert.ToInt32(sanction);
                //    var controlid = db.Employes.Where(a => a.Id == control1).Select(a => a.Id).FirstOrDefault();
                //    var sanctionid = db.Employes.Where(a => a.Id == sanction1).Select(a => a.Id).FirstOrDefault();
                //    var controlEcode = db.Employes.Where(a => a.Id == id).Select(a => a.ControllingAuthority).FirstOrDefault();
                //    var sanctionlEcode = db.Employes.Where(a => a.Id == id).Select(a => a.SanctioningAuthority).FirstOrDefault();


                //    if (empdss1 == sanctionid)
                //    {
                //        // TempData["AlertMessage"] = "This employee is  assigned as Sanctioning Authority";

                var items2 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments

                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Department = new SelectList(items2, "Id", "Name", selected2);




                //    }
                //    else
                //    {
                //        var items2 = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment").Select(x => new Departments
                //        {
                //            Id = x.Id,
                //            Name = x.Name
                //        });
                //        ViewBag.Department = new SelectList(items2, "Id", "Name", selected2);
                //    }
                //}

                var selected1 = (from sub1 in db.Employes
                                 where sub1.Id == id
                                 select sub1.CurrentDesignation).FirstOrDefault();
                var items3 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.CurrentDesignation = new SelectList(items3, "Id", "Name", selected1);
                var selected3 = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.Role).FirstOrDefault();
                var items1 = Facade.EntitiesFacade.GetAllRoles().Select(x => new Roles
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.CurrentDesignationId = selected1;
                ViewBag.Role = new SelectList(items1, "Id", "Name", selected3);

                //var itemssssss = shift.Where(a => a.ShiftType.Contains(items3.Where(b => b.Id == selected1).Select(x=> x.Name).First().ToString())).FirstOrDefault();
                //ViewBag.Shifts = new SelectList("", "Id", "ShiftType", itemssssss);
                var selected4 = (from sub in db.Employes
                                 where sub.Id == id
                                 select sub.Branch).FirstOrDefault();
                var items = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name.ToString(),
                });
                ViewBag.Branch = new SelectList(items, "Id", "Name", selected4);

                //Controlling
                int lcontrolling = Convert.ToInt32(data.ControllingAuthority);
                int lSancationing = Convert.ToInt32(data.SanctioningAuthority);

                Session["Oldlcontrols"] = lcontrolling;
                Session["OldlSancation"] = lSancationing;
                //Conttrolling Branch Id
                var selected4b = (from sub in db.Employes
                                  where sub.Id == lcontrolling
                                  select sub.Branch).FirstOrDefault();

                var selected2d = (from sub in db.Employes
                                  join dep in db.Departments on sub.Department equals dep.Id
                                  where sub.Id == lcontrolling && dep.Active == 1
                                  select sub.Department).FirstOrDefault();

                var items4b = Facade.EntitiesFacade.GetAllBranches().Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name,
                });
                ViewBag.Branch4b = new SelectList(items4b, "Id", "Name", selected4b);

                ViewBag.ControllingBranchId = selected4b;

                ViewBag.ControllingDepartmentId = selected2d;

                var selected1c = (from sub1 in db.Employes
                                  where sub1.Id == lcontrolling
                                  select sub1.CurrentDesignation).FirstOrDefault();



                var lControllingSelectValue = (from sub1 in db.Employes
                                               where sub1.Id == lcontrolling
                                               select sub1.Branch_Value1).FirstOrDefault();
                int lvalue = Convert.ToInt32(lControllingSelectValue);
                string lHeadValueName = db.Branches.Where(a => a.Id == lvalue).Select(a => a.Name).FirstOrDefault();

                ViewBag.myVar5 = lHeadValueName;

                var lbranchdesig = db.Branch_Designation_Mapping.ToList();
                var ldesignation = db.Designations.ToList();
                //int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
                int lHeadofficeValue = (from sub1 in db.Branches
                                        where sub1.Name == "HeadOffice"
                                        select sub1.Id).FirstOrDefault();
                //var items3c = (from emplist in lbranchdesig
                //               join desig in ldesignation on emplist.DesignationId equals desig.Id
                //               where emplist.BranchId == lHeadofficeValue
                //               select new
                //               {
                //                   desig.Id,
                //                   desig.Name
                //               });
                var items3c = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct();
                ViewBag.CurrentDesignation3c = new SelectList(items3c, "Id", "Name");
                ViewBag.ControllingDesignationid = selected1c;


                var items2d = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });

                ViewBag.Department2d = new SelectList(items2d, "Id", "Name", selected2d);



                var selected5 = (from sub in db.Employes
                                 where sub.Id == id && sub.RetirementDate >= DateTime.Now
                                 select sub.ControllingAuthority).FirstOrDefault();
                var items7 = Facade.EntitiesFacade.GetAll().Where(a => a.RetirementDate >= DateTime.Now).Select(x => new Employees
                {
                    Id = x.Id,
                    ControllingAuthority = x.FirstName + " " + x.LastName,
                });
                ViewBag.ControllingAuthority = new SelectList(items7, "Id", "ControllingAuthority", selected5);

                ViewBag.ControllingAuthorityId = selected5;

                //Sancationing

                var selected2sd = (from sub in db.Employes
                                   join dep in db.Departments on sub.Department equals dep.Id
                                   where sub.Id == lSancationing && dep.Active == 1
                                   select sub.Department).FirstOrDefault();
                var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment" && a.Active == 1).Select(x => new Departments
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.Department2sd = new SelectList(items2sd, "Id", "Name", selected2sd);
                //var items2sd = Facade.EntitiesFacade.GetAllDepartments().Where(a => a.Code != "OtherDepartment").Select(x => new Departments
                //{
                //    Id = x.Id,
                //    Name = x.Name
                //});
                //ViewBag.Department2sd = new SelectList(items2sd, "Id", "Name", selected2sd);
                ViewBag.SancationingDepartmentId = selected2sd;

                var selected4sb = (from sub in db.Employes
                                   where sub.Id == id
                                   select sub.Branch).FirstOrDefault();
                var items4sb = Facade.EntitiesFacade.GetAllBranches().Where(a => a.Id == selected2sd).Where(a => a.IFSCCode != "TSHEADOFF" && a.Name != "TGCAB-CTI").Select(x => new Branches
                {
                    Id = x.Id,
                    Name = x.Name,
                });
                ViewBag.Branch4sb = new SelectList(items4b, "Id", "Name", selected4sb);

                string rolename = db.Roles.Where(a => a.Id == id).Select(a => a.Name).FirstOrDefault();


                var selected1sc = (from sub1 in db.Employes
                                   where sub1.Id == lSancationing
                                   select sub1.CurrentDesignation).FirstOrDefault();
                ViewBag.SancationingDesignationId = selected1sc;

                var items3sc = Facade.EntitiesFacade.GetAllDesignations().Where(a => a.Code != "StaffAssistant").Where(a => a.Code != "SubordinateStaff").Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.CurrentDesignation3sc = new SelectList(items3sc, "Id", "Name", selected1sc);


                var selected6 = (from sub in db.Employes
                                 where sub.Id == id && sub.RetirementDate >= DateTime.Now
                                 select sub.SanctioningAuthority).FirstOrDefault();
                var items8 = Facade.EntitiesFacade.GetAll().Where(a => a.CurrentDesignation == selected1sc && a.RetirementDate >= DateTime.Now).Select(x => new Employees
                {
                    Id = x.Id,
                    SanctioningAuthority = x.FirstName + " " + x.LastName,
                });
                ViewBag.SanctioningAuthority = new SelectList(items8, "Id", "SanctioningAuthority", selected6);

                ViewBag.SanctioningAuthorityId = selected6;

                Employees edata = (from u in db.Employes where u.Id == id select u).FirstOrDefault();
                var lPassword = lencrptypassword.Decrypt(edata.Password);
                data.Password = lPassword;
                ViewData["photo"] = data.UploadPhoto;
                int lofficevalue = Convert.ToInt32(data.Branch_Value1);
                string lValuess = db.Branches.Where(a => a.Id == lofficevalue).Select(a => a.Name).FirstOrDefault();
                ViewBag.myVar3 = lValuess;
                ViewBag.myVar4 = data.Branch_Value_2;
                Session["Data"] = null;

                return View(data);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.Delete() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.Delete() - Stack trace: " + ex.StackTrace);
                return View();
            }
        }

        // Delete
        [NoDirectAccess]
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
            try
            {

                Employees employee = db.Employes.Find(id);
                string rolename = db.Roles.Where(a => a.Id == employee.Role).Select(a => a.Name).FirstOrDefault();
                int lEmpid = db.Employes.Where(a => a.EmpId == lCredentials.EmpId).Select(a => a.Id).FirstOrDefault();
                int roleid = db.Roles.Where(a => a.Id == employee.Role).Select(a => a.Id).FirstOrDefault();
                employee.Role = roleid;
                employee.LoginMode = rolename;
                int lCount = db.Leaves.Where(a => a.EmpId == id).Count();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                if (rolename == "SuperAdmin")
                {
                    TempData["AlertMessage"] = "Admin cannot be deleted.";
                    return RedirectToAction("Employeelist");
                }

                if (lCount != 0)
                {
                    TempData["AlertMessage"] = "Employee cannot be deleted";
                    return RedirectToAction("Employeelist");


                }
                else
                {
                    db.Employes.Remove(employee);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Employee Details deleted Successfully";
                    return RedirectToAction("Employeelist");
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.deleted() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.deleted() - Stack trace: " + ex.StackTrace);
            }
            return RedirectToAction("Employeelist");
        }

        /// <summary>
        /// Loading Branch Dropdown for Selecting Current Designation
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadByBranches(string State, string branch, string Desig)
        {
            try
            {
                DateTime dtt = DateTime.Now;
                string stateid = State;
                int branchid = Convert.ToInt32(branch);
                int desigId = Convert.ToInt32(Desig);
                var lbranchdesig = db.Branch_Designation_Mapping.ToList();
                var ldesignation = db.Designations.ToList();
                int lid = db.Branches.Where(a => a.Name == stateid).Select(a => a.Id).FirstOrDefault();
                int lOtherBranchValue = db.Branches.Where(a => a.Name == "OtherBranch").Select(a => a.Id).FirstOrDefault();

                var items3 = db.Designations.Where(a => a.Id == desigId).Select(x => x.Name).FirstOrDefault();
                var query = (from emplist in lbranchdesig
                             join desig in ldesignation on emplist.DesignationId equals desig.Id
                             where emplist.BranchId == lOtherBranchValue


                             select new
                             {
                                 desig.Name,
                                 desig.Id
                             }).Distinct();
                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.Name.ToString(),
                    Value = m.Id.ToString(),
                });
                //var shiftselected = (from sub in db.Employes
                //                     where sub.Id == id
                //                     select sub.Shift_Id).FirstOrDefault();
                var shift = db.Shift_Master.Where(a => a.BranchId == branchid && dtt > a.ValidFrom && dtt < a.ValidTo).Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.GroupName + " - " + x.InTime + " " + x.OutTime
                });
                ////var itemssssss = shift.Where(a => a.Text.Contains(items3)).Select(x => x.Text).First();
                ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType", shift);
                //var stiftimes = shift.Where(a => a.Text.Contains(items3)).Select(x => x.Text).First();
                //var stiftimesval = shift.Where(a => a.Text.Contains(items3)).Select(x => x.Value).First();
                //ViewBag.Shifts = stiftimesval;
                //ViewBag.shifttime = stiftimes;

                ViewBag.DataStatus = stateData;

                string Names = "";
                foreach (var item in stateData)
                {
                    Names = Names + "," + item.Text;

                }
                Session["Data"] = Names;
                return Json(new
                {
                    stateData,
                    //stiftimes,
                    //stiftimesval
                    shift,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.LoadByBranches() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.LoadByBranches() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }

        /// <summary>
        /// Loading Head Office DropdownList Current Designation
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadByBranches1(string State, string Dept, string Desig)
        {
            try
            {
                DateTime dtt = DateTime.Now;
                string stateid = State;
                int Deptid = 43;
                var lbranchdesig = db.Branch_Designation_Mapping.ToList();
                int desigId = Convert.ToInt32(Desig);
                var ldesignation = db.Designations.ToList();
                int lHeadofficeValue = db.Branches.Where(a => a.Name == "HeadOffice").Select(a => a.Id).FirstOrDefault();
                int lid = db.Branches.Where(a => a.Name == stateid).Select(a => a.Id).FirstOrDefault();
                var items3 = db.Designations.Where(a => a.Id == desigId).Select(x => x.Name).FirstOrDefault();
                var query = (from emplist in lbranchdesig
                             join desig in ldesignation on emplist.DesignationId equals desig.Id
                             where emplist.BranchId == lHeadofficeValue

                             select new
                             {
                                 desig.Name,
                                 desig.Id
                             }).Distinct();
                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.Name.ToString(),
                    Value = m.Id.ToString(),
                });

                var shift = db.Shift_Master.Where(a => a.BranchId == Deptid && dtt > a.ValidFrom && dtt < a.ValidTo).Select(x => new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.GroupName + " - " + x.InTime + " " + x.OutTime
                });
                //var stiftimes = shift.Where(a => a.Text.Contains(items3)).Select(x => x.Text).First();
                //var stiftimesval = shift.Where(a => a.Text.Contains(items3)).Select(x => x.Value).First();
                //ViewBag.Shifts = stiftimesval;
                //ViewBag.shifttime = stiftimes;
                ViewBag.Shifts = new SelectList(shift, "Id", "ShiftType", shift);

                ViewBag.DataStatus = stateData;

                string Names = "";
                foreach (var item in stateData)
                {
                    Names = Names + "," + item.Text;

                }
                //  Session["Data"] = Names;
                return Json(new
                {
                    stateData,
                    //stiftimes,
                    //stiftimesval
                    shift
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.LoadByBranches1() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.LoadByBranches1() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }


        //Controlling Authority Dropdown Selection


        //Head office dropdown start
        public JsonResult HLoadByBranchId(string State)
        {
            try
            {
                int stateid = Convert.ToInt32(State);
                var query = (from b in db.Employes
                             join m in db.Designations on b.CurrentDesignation equals m.Id
                             join dep in db.Departments on b.Department equals dep.Id
                             where b.Department.Equals(stateid)
                             where dep.Active == 1
                             select new
                             {
                                 m.Name,
                                 m.Id
                             }).Distinct();



                //  List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.Name.ToString(),
                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.HLoadByBranchId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.HLoadByBranchId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }

        public JsonResult HLoadByDesignationId(string dept, string branc)
        {
            try
            {

                int stateid1 = 0;
                int stateid2 = 0;

                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                stateid2 = Convert.ToInt32(branc);
                stateid1 = Convert.ToInt32(dept);
                var query1 = (from b in db.Employes
                              where b.Branch == stateid2 && b.CurrentDesignation == stateid1
                              where b.RetirementDate >= lStartDate
                              where b.Role != 4
                              select new
                              {
                                  b.FirstName,
                                  b.LastName,
                                  b.Id
                              }).Distinct();
                //List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query1.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.HLoadByDesignationId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.HLoadByDesignationId() - Stack trace: " + ex.StackTrace);
            }
    
            return null;

        }


        public JsonResult HeadOfficeId(string State, string dept)
        {
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                int stateid = Convert.ToInt32(State);
                int stateid1 = Convert.ToInt32(dept);
                var query = (from b in db.Employes
                             join dep in db.Departments on b.Department equals dep.Id
                             where b.Department.Equals(stateid) && b.CurrentDesignation.Equals(stateid1)
                             where b.RetirementDate >= lStartDate && dep.Active == 1
                             where b.Role != 4
                             select new
                             {
                                 b.FirstName,
                                 b.LastName,
                                 b.Id
                             }).Distinct();

                //List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.HeadOfficeId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.HeadOfficeId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }



        //headoffice end
        //Branch dropdowns
        public JsonResult BLoadByBranchId(string State)
        {
            try
            {

                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                int stateid = Convert.ToInt32(State);
                var query = (from b in db.Employes
                             join m in db.Designations on b.CurrentDesignation equals m.Id
                             where b.RetirementDate >= lStartDate
                             where b.Branch == stateid

                             select new
                             {
                                 m.Name,
                                 m.Id
                             }).Distinct();



                //List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.Name.ToString(),
                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.BLoadByBranchId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.BLoadByBranchId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        public JsonResult BLoadByDesignationId(string State, string dept)
        {
            try
            {

                int stateid1 = Convert.ToInt32(dept);
                int stateid = Convert.ToInt32(State);
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                var query = (from b in db.Employes
                             where b.Branch.Equals(stateid) && b.CurrentDesignation.Equals(stateid1)
                             where b.RetirementDate >= lStartDate
                             where b.Role != 4
                             select new
                             {
                                 b.FirstName,
                                 b.LastName,
                                 b.Id
                             }).Distinct();

                List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.BLoadByDesignationId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.BLoadByDesignationId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }

        //Branches







        //Sancationing Authority Dropdown Selection

        public JsonResult HSLoadByBranchId(string State)
        {
            try
            {
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                int stateid = Convert.ToInt32(State);
                var query = (from b in db.Employes
                             join m in db.Designations on b.CurrentDesignation equals m.Id
                             join dep in db.Departments on b.Department equals dep.Id
                             where b.RetirementDate >= lStartDate
                             where b.Department.Equals(stateid)
                             where dep.Active == 1
                             select new
                             {
                                 m.Name,
                                 m.Id
                             }).Distinct();



                List<Employees> lBranchs = db.Employes.Where(a => a.Department.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.Name.ToString(),
                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.HSLoadByBranchId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.HSLoadByBranchId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        public JsonResult HSLoadByDesignationId(string State, string dept)
        {
            try
            {

                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                int stateid1 = Convert.ToInt32(dept);
                int stateid = Convert.ToInt32(State);
                var query = (from b in db.Employes
                             join dep in db.Departments on b.Department equals dep.Id
                             where b.Department.Equals(stateid) && dep.Active == 1 && b.CurrentDesignation.Equals(stateid1)
                             where b.RetirementDate >= lStartDate

                             where b.Role != 4
                             select new
                             {
                                 b.FirstName,
                                 b.LastName,
                                 b.Id
                             }).Distinct();

                List<Employees> lDesigination = db.Employes.Where(a => a.CurrentDesignation.ToString() == State).ToList();

                var stateData = query.Select(m => new SelectListItem()
                {
                    Text = m.FirstName.ToString() + " " + m.LastName.ToString(),

                    Value = m.Id.ToString(),
                });
                return Json(stateData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.HSLoadByDesignationId() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.HSLoadByDesignationId() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }


        public static DateTime GetCurrentTime(DateTime ldate)
        {
            try
            {

                DateTime serverTime = DateTime.Now;
                DateTime utcTime = serverTime.ToUniversalTime();
                // convert it to Utc using timezone setting of server computer
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                return localTime;
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.GetCurrentTime() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.GetCurrentTime() - Stack trace: " + ex.StackTrace);
            }
            return DateTime.Now;
        }
        // Create PDF File For Employees List
        public FileResult CreatePdfEmployee()
        {
            try
            {

                string lempid = Convert.ToString(Session["lempid"]);
                MemoryStream workStream = new MemoryStream();
                StringBuilder status = new StringBuilder("");
                DateTime dTime = DateTime.Now;
                string strPDFFileName = string.Format("EmployeeList" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
                Document doc = new Document();
                doc.SetMargins(20f, 20f, 20f, 20f);
                //Create PDF Table with 5 columns  
                PdfPTable tableLayout1 = new PdfPTable(5);
                //file will created in this path  
                string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);
                PdfWriter.GetInstance(doc, workStream).CloseStream = false;
                doc.Open();
                //Add Content to PDF
                doc.Add(Add_Content_To_PDF17(tableLayout1, lempid));
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
                            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase("Page:" + i.ToString() + "/" + pages.ToString(), blackFont), 568f, 15f, 0);
                            tableLayout1.FooterRows = 1;
                            tableLayout1.AddCell(new PdfPCell(new Phrase(i.ToString(), new Font(Font.FontFamily.HELVETICA, 2, 1, new BaseColor(0, 0, 0))))
                            {
                                Colspan = 20,
                                Border = 0,
                                PaddingBottom = 5,
                                HorizontalAlignment = Element.ALIGN_LEFT,
                            });
                        }
                    }
                    byteInfo = stream.ToArray();
                }
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;
                Session.Remove("lempid");
                return File(workStream, "application/pdf", strPDFFileName);
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.CreatePdfEmployee() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.CreatePdfEmployee() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        protected PdfPTable Add_Content_To_PDF17(PdfPTable tableLayout1, string empid)
        {
            try
            {

                float[] headers1 = { 30, 45, 35, 50, 60 }; //Header Widths  
                tableLayout1.SetWidths(headers1); //Set the pdf headers  
                tableLayout1.WidthPercentage = 100; //Set the PDF File witdh percentage  
                tableLayout1.HeaderRows = 1;
                DateTime printdate = DateTime.Now;
                tableLayout1.AddCell(new PdfPCell(new Phrase(printdate.Date.ToShortDateString() + " " + printdate.ToShortTimeString(), new Font(Font.FontFamily.HELVETICA, 10, 1, new BaseColor(0, 0, 0))))
                {
                    Colspan = 20,
                    Border = 0,
                    PaddingBottom = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                });
                tableLayout1.FooterRows = 1;
                tableLayout1.AddCell(new PdfPCell(new Phrase("EmployeeList", new Font(Font.FontFamily.HELVETICA, 10, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
                {
                    Colspan = 20,
                    Border = 0,
                    PaddingBottom = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                });
                var lemployee = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                DateTime lstartdate = GetCurrentTime(DateTime.Now).Date;
                if (empid == "")
                {
                    var data = (from emp in lemployee
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                where emp.RetirementDate >= lstartdate && dept.Active == 1
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    Designation = desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    EmpName = emp.FirstName + "  " + emp.LastName,
                                    desigId = desig.Id,
                                }).OrderBy(a => a.desigId);
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "EmpFullName");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.ShortName);
                        AddCellToBody(tableLayout1, lemp.Designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.EmpName);

                    }
                    return tableLayout1;
                }
                else
                {
                    var data = (from emp in lemployee
                                join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                join branch in lbranch on emp.Branch equals branch.Id
                                join dept in ldepartments on emp.Department equals dept.Id
                                where emp.RetirementDate >= lstartdate && dept.Active == 1
                                where emp.EmpId.Contains(empid) || emp.ShortName.Contains(empid) || desig.Name.Contains(empid) || branch.Name.Contains(empid) || dept.Name.Contains(empid)
                                select new
                                {
                                    emp.EmpId,
                                    emp.ShortName,
                                    Designation = desig.Code,
                                    Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                    EmpName = emp.FirstName + "  " + emp.LastName,
                                    desigId = desig.Id,
                                }).OrderBy(a => a.desigId);
                    //Adding headers  
                    AddCellToHeader(tableLayout1, "EmpId");
                    AddCellToHeader(tableLayout1, "Name");
                    AddCellToHeader(tableLayout1, "Designation");
                    AddCellToHeader(tableLayout1, "Branch");
                    AddCellToHeader(tableLayout1, "EmpFullName");
                    //Adding body  
                    foreach (var lemp in data)
                    {
                        AddCellToBody(tableLayout1, lemp.EmpId);
                        AddCellToBody(tableLayout1, lemp.ShortName);
                        AddCellToBody(tableLayout1, lemp.Designation);
                        AddCellToBody(tableLayout1, lemp.Deptbranch);
                        AddCellToBody(tableLayout1, lemp.EmpName);

                    }
                    return tableLayout1;
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.Add_Content_To_PDF17() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.Add_Content_To_PDF17() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 10, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(60, 141, 188)
            });
        }
        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 9, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 8,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

        //Code for Export to Excel for Employee List

        public void ExportGridToExcel(string empid)
        {

            try
            {
                string lempid = Convert.ToString(Session["lempid"]);
                var lemployee = db.Employes.ToList();
                var lbranch = db.Branches.ToList();
                var ldepartments = db.Departments.ToList();
                var ldesignation = db.Designations.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (lempid == "")
                {
                    var employeeList = (from emp in lemployee
                                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                        join branch in lbranch on emp.Branch equals branch.Id
                                        join dept in ldepartments on emp.Department equals dept.Id
                                        where emp.RetirementDate >= lStartDate && dept.Active == 1
                                        select new
                                        {
                                            emp.EmpId,
                                            emp.ShortName,
                                            Designation = desig.Code,
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            EmpFullName = emp.FirstName + " " + emp.LastName,
                                        }).ToList();

                    var gv = new GridView();
                    gv.DataSource = employeeList;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=EmployeeReport.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "GB2312";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                    gv.Width = 5;
                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Response.Flush();
                    Session.Remove("lempid");
                    Response.End();
                }

                else
                {
                    var employeeList = (from emp in lemployee
                                        join desig in ldesignation on emp.CurrentDesignation equals desig.Id
                                        join branch in lbranch on emp.Branch equals branch.Id
                                        join dept in ldepartments on emp.Department equals dept.Id
                                        where emp.RetirementDate >= lStartDate && dept.Active == 1
                                        where emp.EmpId.Contains(lempid) || emp.ShortName.Contains(lempid) || desig.Name.Contains(lempid) || branch.Name.Contains(lempid) || dept.Name.Contains(lempid)
                                        select new
                                        {
                                            emp.EmpId,
                                            emp.ShortName,
                                            Designation = desig.Code,
                                            Deptbranch = GetBranchDepartmentConcat(branch.Name, dept.Name),
                                            EmpFullName = emp.FirstName + " " + emp.LastName,
                                        }).ToList();

                    var gv = new GridView();
                    gv.DataSource = employeeList;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=EmployeeReport.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "GB2312";
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    StringWriter objStringWriter = new StringWriter();
                    HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                    gv.HeaderStyle.BackColor = System.Drawing.Color.LightSkyBlue;
                    gv.Width = 5;
                    gv.RenderControl(objHtmlTextWriter);
                    Response.Output.Write(objStringWriter.ToString());
                    Response.Flush();
                    Session.Remove("lempid");
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.ExportGridToExcel - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.ExportGridToExcel() - Stack trace: " + ex.StackTrace);
            }
        }
        [NoDirectAccess]
        public ActionResult Senioritylist()

        {
            try
            {

                TempData["RolePages"] = LoginHelper.GetCurrentUserPages();
                ViewBag.Message = lCredentials.LoginMode;
                var items2 = Facade.EntitiesFacade.GetAllDesignations().Select(x => new Designations
                {
                    Id = x.Id,
                    Name = x.Name
                });
                ViewBag.CurrentDesignation = new SelectList(items2, "Name", "Name");
                TempData["Loginmode"] = lCredentials.LoginMode;
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.SenioritylistViews() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.SenioritylistViews() - Stack trace: " + ex.StackTrace);
            }
            return View("~/Views/Register/_SeniorityPartialView.cshtml");
        }
        [HttpGet]
        public JsonResult SenioritylistView(string EmpId)
        {

            try
            {
                Session["lseniorempid"] = EmpId;
                var dbResult = db.view_employee_senioritylist.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (string.IsNullOrEmpty(EmpId))
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.name,
                                    emplist.DOB,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = (from emplist in dbResult
                                where emplist.RetirementDate >= lStartDate
                                where (emplist.EmpId.ToLower().Contains(EmpId.ToLower()))
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.name,
                                    emplist.DOB,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.SenioritylistView() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.SenioritylistView() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        [HttpPost]
        public JsonResult SenioritylistViews(string EmpId)
        {
            Session["lempid"] = EmpId;
            try
            {
                var dbResult = db.view_employee_senioritylist.ToList();
                DateTime lStartDate = GetCurrentTime(DateTime.Now).Date;
                if (EmpId != "")
                {
                    //int lbranch = Convert.ToInt32(branch);
                    var data = (from emplist in dbResult
                                where emplist.name == EmpId
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.DOB,

                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
                                });

                    return Json(data, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    var data = (from emplist in dbResult
                                where (emplist.EmpId.ToLower().Contains(EmpId.ToLower()))
                                where emplist.RetirementDate >= lStartDate
                                select new
                                {
                                    emplist.Id,
                                    emplist.EmpId,
                                    emplist.EmpName,
                                    emplist.DOB,
                                    emplist.name,
                                    BranchName = GetBranchDepartmentConcat(emplist.BranchName, emplist.DeptName),
                                    emplist.DOJ,
                                    emplist.RetirementDate
                                });
                    return Json(data, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                LogInformation.Info("RegisterController.SenioritylistViews() - Error occurred: " + ex.Message);
                LogInformation.Info("RegisterController.SenioritylistViews() - Stack trace: " + ex.StackTrace);
            }
            return null;
        }
        /// <summary>
        /// Validates password strength according to security requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>Validation result with message</returns>
        private (bool IsValid, string Message) ValidatePasswordStrength(string password)
        {
            string msg = "Your password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character (e.g., !@#$%^&*).";
            if (string.IsNullOrEmpty(password))
            {
                return (false, "Password cannot be empty.");
            }

            // Check minimum length
            if (password.Length < 8)
            {
                return (false, msg);
            }

            // Check for upper-case letters
            if (!password.Any(char.IsUpper))
            {
                return (false, msg);
            }

            // Check for lower-case letters
            if (!password.Any(char.IsLower))
            {
                return (false, msg);
            }

            // Check for numbers
            if (!password.Any(char.IsDigit))
            {
                return (false, msg);
            }

            // Check for symbols (special characters)
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return (false, msg);
            }

            return (true, "Password meets strength requirements.");
        }

        /// <summary>
        /// Generates a strong password suggestion that meets all requirements
        /// </summary>
        /// <returns>Strong password suggestion</returns>
        private string GenerateStrongPasswordSuggestion()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var random = new Random();
            var password = new char[12]; // 12 characters for extra security

            // Ensure at least one character from each category
            password[0] = upperCase[random.Next(upperCase.Length)];
            password[1] = lowerCase[random.Next(lowerCase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = symbols[random.Next(symbols.Length)];

            // Fill remaining positions with random characters from all categories
            var allChars = upperCase + lowerCase + numbers + symbols;
            for (int i = 4; i < password.Length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the password to avoid predictable patterns
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            return new string(password);
        }

        /// <summary>
        /// AJAX endpoint to validate password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>JSON result with validation details</returns>
        [HttpPost]
        public JsonResult ValidatePassword(string password)
        {
            var validation = ValidatePasswordStrength(password);
            var suggestion = validation.IsValid ? "" : GenerateStrongPasswordSuggestion();

            return Json(new
            {
                isValid = validation.IsValid,
                message = validation.Message,
                suggestion = suggestion,
                strength = GetPasswordStrengthScore(password)
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a password strength score (0-100)
        /// </summary>
        /// <param name="password">Password to score</param>
        /// <returns>Strength score from 0 to 100</returns>
        private int GetPasswordStrengthScore(string password)
        {
            if (string.IsNullOrEmpty(password)) return 0;

            int score = 0;

            // Length contribution (up to 25 points)
            if (password.Length >= 8) score += 10;
            if (password.Length >= 10) score += 10;
            if (password.Length >= 12) score += 5;

            // Character variety contribution (up to 40 points)
            if (password.Any(char.IsUpper)) score += 10;
            if (password.Any(char.IsLower)) score += 10;
            if (password.Any(char.IsDigit)) score += 10;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score += 10;

            // Complexity contribution (up to 35 points)
            var uniqueChars = password.Distinct().Count();
            if (uniqueChars >= 8) score += 10;
            if (uniqueChars >= 10) score += 10;
            if (uniqueChars >= 12) score += 15;

            return Math.Min(score, 100);
        }

        /// <summary>
        /// AJAX endpoint to generate a strong password suggestion
        /// </summary>
        /// <returns>JSON result with password suggestion</returns>
        [HttpPost]
        public JsonResult GeneratePassword()
        {
            var password = GenerateStrongPasswordSuggestion();
            return Json(new { password = password }, JsonRequestBehavior.AllowGet);
        }

    }
}