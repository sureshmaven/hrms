using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Table(name: "Employees")]
    public class Employees : IDbEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShortName { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string MartialStatus { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string PersonalEmailId { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Graduation { get; set; }
        public string PostGraduation { get; set; }
        public string OtherQualification { get; set; }
        public string EmergencyName { get; set; }
        public string EmergencyContactNo { get; set; }
        public string Category { get; set; }
        public string EmpId { get; set; }
        public int Branch { get; set; }
        public int JoinedDesignation { get; set; }
        public int CurrentDesignation { get; set; }
        public int Department { get; set; }
        public int Role { get; set; }
        public string OfficalEmailId { get; set; }
        public string TotalExperience { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public Nullable<System.DateTime> RelievingDate { get; set; }
        public Nullable<System.DateTime> RetirementDate { get; set; }
        public string ControllingAuthority { get; set; }
        public string SanctioningAuthority { get; set; }
        public string UploadPhoto { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string RelievingReason { get; set; }
        public string SpouseName { get; set; }
        public string AadharCardNo { get; set; }
        public string PanCardNo { get; set; }
        public string BloodGroup { get; set; }
        public string ProfessionalQualifications { get; set; }
        public Nullable<int> ControllingDepartment { get; set; }
        public Nullable<int> ControllingBranch { get; set; }
        public Nullable<int> ControllingDesignation { get; set; }
        public Nullable<int> SanctioningDepartment { get; set; }
        public Nullable<int> SanctioningBranch { get; set; }
        public Nullable<int> SanctioningDesignation { get; set; }
        public string LoginMode { get; set; }
        public string Branch_Value1 { get; set; }
        public Nullable<int> Branch_Value_2 { get; set; }
        public Nullable<int> Shift_Id { get; set; }
        [NotMapped]
        //public string Success { get; set; }
        public string branchhide { get; set; }
        public int? PerBranch { get; set; }
        public int? PerDepartment { get; set; }        
        public string Exit_type { get; set; }
        public string photo { get; set; }
        //public int FailedLoginAttempts { get; set; }
        public Nullable<int> FailedLoginAttempts { get; set; }
        public DateTime? AccountLockedUntil { get; set; }
    }
    [Table(name: "Roles")]
    public class Roles : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }
    }
    [Table(name: "Shift_Master")]
    public class Shift_Master : IDbEntity<int>
    {
        public int Id { get; set; }
        public string ShiftType { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public int BranchId { get; set; }
        public string GroupName { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

    }
    [Table(name: "LeaveTypes")]
    public class LeaveTypes : IDbEntity<int>
    {
        public int Id { get; set; }

        public string Type { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public string Code { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }



    }
    [Table(name: "Designations")]
    public class Designations : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }

    }
    [Table(name: "Departments")]
    public class Departments : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public int Active { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }
    }
    [Table(name: "Branches")]
    public class Branches : IDbEntity<int>
    {
        public int Id { get; set; }
        public int BankName { get; set; }
        public string Name { get; set; }
        public string BranchCode { get; set; }
        public string IFSCCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string PhoneNo3 { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        [NotMapped]
        public string LoginMode { get; set; }

    }
    [Table(name: "Banks")]
    public class Banks : IDbEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string City { get; set; }

        public string PhoneNo1 { get; set; }

        public string PhoneNo2 { get; set; }

        public string PhoneNo3 { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }
    }
    [Table(name: "BranchStaffCount")]
    public class BranchStaffCount : IDbEntity<int>
    {
        public int Id { get; set; }
        public int Branchid { get; set; }

        public string Category { get; set; }

        public string Staffcount { get; set; }

        public decimal AmountRange { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


    }


    [Table(name: "Leaves")]
    public class Leaves : IDbEntity<int>
    {
        public int Id { get; set; }
        public Nullable<int> EmpId { get; set; }
        public int ControllingAuthority { get; set; }
        public int SanctioningAuthority { get; set; }
        public int LeaveType { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Status { get; set; }
        public string CancelReason { get; set; }
        public string Stage { get; set; }
        public int LeaveDays { get; set; }
        public int TotalDays { get; set; }
        public int leave_balance { get; set; }
        public string MaternityType { get; set; }
        //public string DocumentPath { get; set; }
        public DateTime LeaveTimeStamp { get; set; }
        [NotMapped]
        public DateTime DateofDelivery { get; set; }
        public int? LeavesYear { get; set; }
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }



    }
    [Table(name: "LeaveInfo")]
    public class LeaveInfo : IDbEntity<int>
    {
        public int Id { get; set; }
        public Nullable<int> EmpId { get; set; }
        public int LeaveId { get; set; }
        public int LeaveType { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }


    }

    [Table(name: "EmpLeaveBalance")]
    public class EmpLeaveBalance : IDbEntity<int>
    {
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }
        public int EmpId { get; set; }
        public int Year { get; set; }
        public int CarryForward { get; set; }
        public int Credits { get; set; }
        public int Debits { get; set; }
        public int LeaveBalance { get; set; }
        public string UpdatedBy { get; set; }

    }

    [Table(name: "V_LeaveHistory")]
    public class V_LeaveHistory : IDbEntity<int>
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmpId { get; set; }
        public int LeaveType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CurrentDesignation { get; set; }
        [Required]
        [Display(Name = "Branch")]
        public int Branch { get; set; }
        public int Department { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public int LeaveDays { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public string LoginMode { get; set; }

    }

    [Table(name: "V_LeaveBalance")]
    public class V_LeaveBalance : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int CasualLeave { get; set; }
        public int MedicalSickLeave { get; set; }
        public int PrivilegeLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int ExtraOrdinaryLeave { get; set; }
        public int SpecialCasualLeave { get; set; }
       public int woff { get; set; }
        public int CWOFF{ get; set; }

        public int SpecialMedicalLeave { get; set; }
       }
    [Table(name: "V_EmpLeaveBalance")]
    public class V_EmpLeaveBalance : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int CasualLeave { get; set; }
        public int MedicalSickLeave { get; set; }
        public int PrivilegeLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int ExtraOrdinaryLeave { get; set; }
        public int SpecialCasualLeave { get; set; }
        public int CompensatoryOff { get; set; }
        public int woff { get; set; }
        public int LOP { get; set; }
      public int CWOFF { get; set; }
        public Int64 RowId { get; set; }
        public int SpecialMedicalLeave { get; set; }
        [NotMapped]
        public List<V_EmpLeaveBalance> GetAllLeavesTypes { get; set; }
    }
    [Table(name: "DeliveryDate_PTL")]
    public class DeliveryDate_PTL : IDbEntity<int>
    {
        public int Id { get; set; }
        public int LeaveId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

    }
    [Table(name: "HolidayList")]
    public class HolidayList : IDbEntity<int>
    {

        public int Id { get; set; }
        public string Occasion { get; set; }
        public DateTime Date { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Nullable<System.DateTime> DeleteAt { get; set; }

    }
    [Table(name: "WorkingDays")]
    public class WorkingDays : IDbEntity<int>
    {

        public int Id { get; set; }
        public int EmpId { get; set; }
        public DateTime LastCountDate { get; set; }
        public decimal CL { get; set; }
        public int PL { get; set; }
        public int UpdateDBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }

    [Table(name: "Branch_Designation_Mapping")]
    public class Branch_Designation_Mapping : IDbEntity<int>
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int DesignationId { get; set; }

    }
    [Table(name: "Tx_History")]
    public class Tx_History : IDbEntity<int>
    {

        public int Id { get; set; }
        //public int Tx_id { get; set; }
        public string Tx_type { get; set; }
        public string Tx_subtype { get; set; }
        public string Tx_by { get; set; }
        public string Tx_on { get; set; }
        public string Tx_Name { get; set; }
        public Nullable<System.DateTime> Tx_date { get; set; }
        public string Notes { get; set; }
        public string Comments { get; set; }

    }
    [Table(name: "V_Tx_History")]
    public class V_Tx_History : IDbEntity<int>
    {

        public int Id { get; set; }
        // public int Tx_id { get; set; }
        public string Tx_type { get; set; }
        public string Tx_subtype { get; set; }
        public string Tx_by { get; set; }
        public string Tx_on { get; set; }
        public Nullable<System.DateTime> Tx_date { get; set; }
        public string Notes { get; set; }
        public string Comments { get; set; }

    }
    public class WDViewModel
    {
        public int WDId { get; set; }
        public Array workname { get; set; }
        public Array workdes { get; set; }
        public Array wnametype { get; set; }
        public Array others { get; set; }
        public WorkDiary_Det lworkdetails { get; set; }
        public string ActionType { get; set; }
        public string draft { get; set; }
        public string wddelete { get; set; }
    }
    public class ViewModel
    {
        public Banks lbanks { get; set; }

        public Employees lEmployess { get; set; }

        public Branches lbranches { get; set; }

        public Departments ldepartments { get; set; }

        public Designations ldesignations { get; set; }

        public Roles lroles { get; set; }
        public V_EmpLeaveBalance lEmpLeaveBal { get; set; }
        public LeaveTypes lleavetypes { get; set; }
        // public Employee_Transfer lemployeetransfer { get; set; }
        public Leaves_LTC lleavetravel { get; set; }
        public PLE_Type lpl { get; set; }
        public FamilyRelations lrelation { get; set; }
        //  public List<FamilyRelations> lfamilyrelations { get; set; }
        public string ControllingAuthority { get; set; }
        public string TotalExperience { get; set; }
        public string SanctioningAuthority { get; set; }
        public string Loginmode { get; set; }
        public string Block_Period { get; set; }
        public string ModeOfTransport { get; set; }
        public Array relation { get; set; }
        public Array name { get; set; }
        public Array RelationAge { get; set; }
        public Array Occupation { get; set; }

        public string DeclarationType1 { get; set; }
        public string Form1CheckBox { get; set; }
        public string Form2CheckBox { get; set; }
        public string DeclarationType2 { get; set; }
        public string Form1Date { get; set; }
        public string Form2Date { get; set; }
        public string Form1Place { get; set; }
        public string Form2Place { get; set; }
        public string EmpId { get; set; }
        public string designation { get; set; }

    }
    public class CovidViewModel
    {
        public Banks lbanks { get; set; }
        public Employees lEmployess { get; set; }
        public Branches lbranches { get; set; }
        public Departments ldepartments { get; set; }
        public Designations ldesignations { get; set; }
        public Roles lroles { get; set; }
        public V_EmpLeaveBalance lEmpLeaveBal { get; set; }
        public LeaveTypes lleavetypes { get; set; }
        public Leaves_LTC lleavetravel { get; set; }
        public PLE_Type lpl { get; set; }
        public FamilyRelations lrelation { get; set; }
        public Array relation { get; set; }
        public Array name { get; set; }
        public Array RelationAge { get; set; }
        public Array Occupation { get; set; }
        public Array Gender { get; set; }
        public Array address { get; set; }
        public Array diabetes { get; set; }
        public Array hbp { get; set; }
        public Array quarhistory { get; set; }
        public Array complaints { get; set; }
  

    }
    public class creditdebitLeaves
    {
        public Employees lemployee { get; set; }
        public EmpLeaveBalance lempbalance { get; set; }
        public string NoofLeaveDays { get; set; }
        public LeaveTypes lleavetypes { get; set; }

        public string LeaveCredit { get; set; }
        public string credit_Value1 { get; set; }


    }
    public class LeaveViewModel
    {
        public Leaves lleaves { get; set; }

        public V_EmpLeaveBalance lEmpLeaveBal { get; set; }

        public DeliveryDate_PTL lDelivery { get; set; }

        public string ControllingAuthority { get; set; }

        public string SanctioningAuthority { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }

        public string designation { get; set; }

        public string EmpId { get; set; }

    }

    public class LeaveViewModel_admin
    {
        public Leaves lleaves { get; set; }

        public V_EmpLeaveBalance lEmpLeaveBal { get; set; }

        public DeliveryDate_PTL lDelivery { get; set; }

        public string ControllingAuthority { get; set; }

        public string SanctioningAuthority { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }

        public string designation { get; set; }

        public string empId { get; set; }

    }

    [Table(name: "Employee_Transfer")]
    public class Employee_Transfer : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Transfer_Type { get; set; }
        public int EmpId { get; set; }
        public int NewDesignation { get; set; }
        public int NewBranch { get; set; }
        public int NewDepartment { get; set; }
        public int OldDesignation { get; set; }
        public int OldBranch { get; set; }
        public int OldDepartment { get; set; }
        public int fy { get; set; }
        public string fm { get; set; }
        public string category { get; set; }
        public string new_basic { get; set; }
        public string old_basic { get; set; }
        public int senoirity_order { get; set; }
        public int authorisation { get; set; }
        public int active { get; set; }
        public Nullable<System.DateTime> incre_due_date { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }
        public string UpdatedBy { get; set; }

    }
    [Table(name: "view_employee")]
    public class view_employee : IDbEntity<int>
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public int BranchId { get; set; }
        public string EmpId { get; set; }
        public string Name { get; set; }
        public string EmpName { get; set; }
        public string Code { get; set; }
        public int designations { get; set; }


    }
    [Table(name: "OD_OtherDuty")]
    public class OD_OtherDuty : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int ControllingAuthority { get; set; }
        public int SanctioningAuthority { get; set; }
        public int VistorFrom { get; set; }
        public string VistorTo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Purpose { get; set; }
        public string Description { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Status { get; set; }
        public string CancelReason { get; set; }
        public string Stage { get; set; }

        [NotMapped]
        public string Loginmode { get; set; }
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
    }

    [Table(name: "view_employee_dept")]
    public class view_employee_dept : IDbEntity<int>
    {

        public int Id { get; set; }
        public string EmpId { get; set; }
        public string DeptName { get; set; }
        public int DepartmentId { get; set; }
        public string EmpName { get; set; }
        public string Code { get; set; }
        public int designations { get; set; }


    }

    [Table(name: "view_employee_category")]
    public class view_employee_category : IDbEntity<int>
    {

        public int Id { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Code { get; set; }
        public string category { get; set; }
        public int designations { get; set; }
        public string gender { get; set; }

    }

    [Table(name: "view_employee_DOB_RetirementDateMonthWise")]
    public class view_employee_DOB_RetirementDateMonthWise : IDbEntity<int>
    {

        public int Id { get; set; }
        public string EmpId { get; set; }
        public DateTime DOB_MonthWise { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public Nullable<System.DateTime> RetirementDate_MonthWise { get; set; }
        public string EmpName { get; set; }
        public string Code { get; set; }
        public int designations { get; set; }
        public string BranchName { get; set; }
        public string DeptName { get; set; }

    }
    [Table(name: "OD_Master")]
    public class OD_Master : IDbEntity<int>
    {
        public int Id { get; set; }
        public string ODType { get; set; }
        public string UpdatedBy { get; set; }
        public bool Status { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
    [Table(name: "view_employee_senioritylist")]
    public class view_employee_senioritylist : IDbEntity<int>
    {

        public int Id { get; set; }
        public string EmpName { get; set; }
        public string EmpId { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public Nullable<System.DateTime> RetirementDate { get; set; }
        public string name { get; set; }
        public string Code { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string BranchName { get; set; }
        public string DeptName { get; set; }
        public int designations { get; set; }


    }

    [Table(name: "view_employee_transfer")]
    public class view_employee_transfer : IDbEntity<int>
    {

        public int Id { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Designation { get; set; }
        public string BranchName { get; set; }
        public string DeptName { get; set; }
        public string PresentAddress { get; set; }
        public string ProfessionalQualifications { get; set; }
        public string MobileNumber { get; set; }
        public string Category { get; set; }
        public int? OldDesignation { get; set; }
        public int? NewDesignation { get; set; }
        public int? OldBranch { get; set; }
        public int? NewBranch { get; set; }
        public int? OldDepartment { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public Nullable<System.DateTime> RetirementDate { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public int? NewDepartment { get; set; }
        public string Graduation { get; set; }
        public string PostGraduation { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }


    }
    [Table(name: "v_BranchContactList")]
    public class v_BranchContactList : IDbEntity<int>
    {

        public int Id { get; set; }
        public string BranchName { get; set; }
        public string name { get; set; }
        public string EmpName { get; set; }
        public string EmpId { get; set; }
        public string PhoneNo1 { get; set; }
        public string PhoneNo2 { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string code { get; set; }

    }
    [Table(name: "Leaves_LTC")]
    public class Leaves_LTC : IDbEntity<int>
    {

        public int Id { get; set; }
        public int EmpId { get; set; }
        public int ControllingAuthority { get; set; }
        public int SanctioningAuthority { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Status { get; set; }
        public string LtcType { get; set; }
        public string LeaveType { get; set; }
        public string Block_Period { get; set; }
        public string TotalExperience { get; set; }
        public string PlaceOfVisits { get; set; }
        public string ModeOfTransport { get; set; }
        public string TravelAdvance { get; set; }
        public int TotalDays { get; set; }
        public int Year { get; set; }
        public int leave_balance { get; set; }
        //public string Relation { get; set; }
        //public string RelationName { get; set; }
        //public string RelationAge { get; set; }
        //public string Occupation { get; set; }

        [NotMapped]
        public string Loginmode { get; set; }
        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
    }
    [Table(name: "FamilyRelations")]
    public class FamilyRelations : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Relation { get; set; }
        public string RelationName { get; set; }
        public string RelationAge { get; set; }
        public string Occupation { get; set; }
        public string DeclarationType { get; set; }
        public string EmpWorking { get; set; }
        public string EmpAddress { get; set; }
        public string IsLiable { get; set; }
        public int LTCId { get; set; }
        public string Place { get; set; }
        public DateTime LTCDate { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }
    }


    [Table(name: "leaves_CreditDebit")]
    public class leaves_CreditDebit : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int LeaveTypeId { get; set; }
        public int CreditLeave { get; set; }
        public int DebitLeave { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int LeaveBalance { get; set; }
        public string Comments { get; set; }
        public string EmpName { get; set; }
        public int Department { get; set; }
        public int CurrentDesignation { get; set; }
        public int Branch { get; set; }
        public int year { get; set; }
        public string type { get; set; }
        public int Head_Branch_Value { get; set; }
        public int TotalBalance { get; set; }
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }
        public DateTime? LCDTimeStamp { get; set; }
    }

    [Table(name: "Leaves_CarryForward")]
    public class Leaves_CarryForward : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public int CarryForward { get; set; }
        public int LeaveCredit { get; set; }
        public int PreviousYearCF { get; set; }
        public int LeaveDebit { get; set; }
        public int LeaveBalance { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }
    }

    [Table(name: "V_LeaveForward")]
    public class V_LeaveForward : IDbEntity<int>
    {
        public int Id { get; set; }
        public int Empid { get; set; }
        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public int leavetypeid { get; set; }
        public int LeaveDebit { get; set; }
        public int LeaveCredit { get; set; }
        public int Year { get; set; }
        public int LeaveBalance { get; set; }
        public int CarryForward { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }
    }
    [Table(name: "V_EmpLeavesCarryForward")]
    public class V_EmpLeavesCarryForward : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public int leavecredit { get; set; }
        public int leavedebit { get; set; }
        public int CarryForward { get; set; }
        public int Year { get; set; }
        public int LeaveBalance { get; set; }
        public int LeaveTypeId { get; set; }
        public int CasualLeave { get; set; }
        public int MedicalSickLeave { get; set; }
        public int PrivilegeLeave { get; set; }
        public int MaternityLeave { get; set; }
        public int PaternityLeave { get; set; }
        public int ExtraOrdinaryLeave { get; set; }
        public int SpecialCasualLeave { get; set; }
        public int CompensatoryOff { get; set; }
        public int LOP { get; set; }
        public long RowId { get; set; }
    }
    [Table(name: "BlockPeriod")]
    public class BlockPeriod : IDbEntity<int>
    {
        public int Id { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string Block_Period { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    [Table(name: "News")]
    public class News : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    [Table(name: "View_ChangingAuthority")]
    public class View_ChangingAuthority : IDbEntity<int>
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string BranchName { get; set; }
        public string Designation { get; set; }
        public string DeptName { get; set; }
        public string ControllingAuthority { get; set; }
        public string ControllingEmpId { get; set; }
        public string SanctioningAuthority { get; set; }
        public string SanctioningEmpId { get; set; }

    }
    [Table(name: "PLE_Type")]
    public class PLE_Type : IDbEntity<int>
    {

        public int Id { get; set; }
        public int EmpId { get; set; }
        public int ControllingAuthority { get; set; }
        public int SanctioningAuthority { get; set; }
        public int leave_balance { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public int UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Status { get; set; }
        public string PLType { get; set; }
        public string LeaveType { get; set; }
        public int authorisation { get; set; }
        public int process { get; set; }
        public string TotalExperience { get; set; }
        public string CurrentYear { get; set; }
        public string PLEncash { get; set; }
        public string TotalPL { get; set; }

        public int? BranchId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public Int32? fy { get; set; }
        public DateTime? fm { get; set; }
        [NotMapped]
        public string Loginmode { get; set; }

    }
    [Table(name: "WorkDiary")]
    public class WorkDiary : IDbEntity<int>
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string Status { get; set; }
        public int CA { get; set; }
        public int SA { get; set; }
        public DateTime? WDDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public int? RefId { get; set; }
        public int? Br { get; set; }
        public int? Org { get; set; }
        public int? CurBr { get; set; }
        public int? CurDept { get; set; }
        public int? CurDesig { get; set; }


        [NotMapped]
        public string Loginmode { get; set; }
    }

    [Table(name: "WorkDiary_Det")]
    public class WorkDiary_Det : IDbEntity<int>
    {
        public int Id { get; set; }
        public int WDId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string WorkType { get; set; }
    }
    [Table(name: "All_Masters")]
    public class All_Masters : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Br { get; set; }
        public int Org { get; set; }
        public int Active { get; set; }
    }
    [Table(name: "V_EmpLeaveHistory")]
    public class V_EmpLeaveHistory : IDbEntity<int>
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public int EmpPkId { get; set; }
        public string EmpName { get; set; }
        public string DesigCode { get; set; }
        public string BranchName { get; set; }
        public string DeptName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveDays { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime LeaveTimeStamp { get; set; }
        public Nullable<System.DateTime> AppliedDate { get; set; }
    }

    [Table(name: "Timesheet_Request_Form")]
    public class Timesheet_Request_Form : IDbEntity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int Shift_Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime ReqFromDate { get; set; }
        public DateTime ReqToDate { get; set; }
        public string Reason_Type { get; set; }

        public string Reason_Desc { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int CA { get; set; }
        public int SA { get; set; }
        public string Status { get; set; }
        public string UpdatedBy { get; set; }
        public int Processed { get; set; }
        public string entrytime { get; set; }
        public string exittime { get; set; }
    }
 



     [Table(name: "Branch_Device")]
    public class Branch_Device : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Device_Id { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
      
    }
    [Table(name: "Government_Staff")]
    public class Government_Staff : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Empid { get; set; }
        public string Name { get; set; }
        public int Designation { get; set; }
        public int Branch { get; set; }
        public int UpdatedBy { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedDate { get; set; }
   

    }

    [Table(name: "PR_Emppayfields")]
    public class PR_Emppayfields : IDbEntity<int>
    {
        public int Id { get; set; }
        public int FY { get; set; }
        public DateTime FM { get; set; }
        public int EmpId { get; set; }
        public int PFMId { get; set; }
        public string PFMType { get; set; }
        public float Amount { get; set; }
        public bool Active { get; set; }
        public int Trans_id { get; set; }

    }
    [Table(name: "PR_Payfieldmaster")]
    public class PR_Payfieldmaster : IDbEntity<int>
    {
        public int Id { get; set; }
        public string Payfieldtype { get; set; }
        public string Payfieldname { get; set; }
        public bool Active { get; set; }
        public int Trans_id { get; set; }

    }

    [Table(name: "Payroll_FW")]
    public class Payroll_FW : IDbEntity<int>
    {
        public int Id { get; set; }
        public int FixedGross { get; set; }
        public float FixedBasic { get; set; }
        public float FixedHRA { get; set; }
        public float FixedConveyance { get; set; }
        public float FixedMedical { get; set; }
        public float FixedSA { get; set; }
        public int EmpId { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public float PFEmployer { get; set; }
        public float ESIEmployer { get; set; }
        public float CTC { get; set; }
        public string EmpFirstname { get; set; }
        public string EmpLastname { get; set; }
        public string EmpBranch { get; set; }
        public string EmpDesignation { get; set; }
        public DateTime EmpDOJ { get; set; }
        public string EmpEmailId { get; set; }
        public int EmpPANno { get; set; }
        public int EmpBankAccountno { get; set; }

        public int EmpUAN_PFno { get; set; }
        public int EmpESIno { get; set; }

    }
  


    public class PayfieldModel
    {
        public Array Payfieldname { get; set; }
        public Array Amount { get; set; }
        public Array DFPayfieldname { get; set; }
        public Array DFAmount { get; set; }
        public PR_Emppayfields payfield { get; set; }

    }
   



    [Table(name: "Payroll_EW")]
    public class Payroll_EW : IDbEntity<int>
    {

        public int Id { get; set; }
        public int EmpId { get; set; }

        public int EmpCode { get; set; }
        public DateTime Date { get; set; }
        public int WorkingDays { get; set; }
        public float WorkDays { get; set; }
        public float EarnedBasic { get; set; }
        public float EarnedHRA { get; set; }
        public float EarnedConveyance { get; set; }
        public float EarnedMedical { get; set; }
        public float EarnedSA { get; set; }
        public float EarnedGross { get; set; }
        public float LOPDays { get; set; }


        public float LOPamount { get; set; }
        public float PF { get; set; }
        public float ESI { get; set; }

        public float PT { get; set; }

        public float TDS { get; set; }
        public float Loan { get; set; }

        public float Others { get; set; }
        public float Totaldeductions { get; set; }
        public float NetSalary { get; set; }
        public bool Active { get; set; }

    }
    [Table(name: "PR_Monthdetails")]
    public class AddPayMonth : IDbEntity<int>
    {

        public int Id { get; set; }
        public int FY { get; set; }
        public DateTime FM { get; set; }
        public int Weekholidays { get; set; }
        public int Paidholidays { get; set; }
        public DateTime Paymentdate { get; set; }
        public int DAslabs { get; set; }
        public int Active { get; set; }
        public float DApoints { get; set; }
        public float DApercent { get; set; }
        public int Trans_id { get; set; }

    }

    [Table(name: "OD_Unlock")]
    public class OD_Unlock 
    {
        [Key]
        public int OD_UlId { get; set; }
        public string EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Note { get; set; }

    }
    [Table(name: "PR_Monthattendance")]
    public class PR_Monthattendance : IDbEntity<int>
    {
        public int Id { get; set; }
        public int FY { get; set; }
        public int EmpId { get; set; }
        public string Status { get; set; }
        public DateTime Statusdate { get; set; }
        public DateTime FM { get; set; }
        public string Leavesavailable { get; set; }
        public float LOPdays { get; set; }
        public float Absentdays { get; set; }
        public float Workingdays { get; set; }
        public Leaves lleaves { get; set; }

        public V_EmpLeaveBalance lEmpLeaveBal { get; set; }

        public DeliveryDate_PTL lDelivery { get; set; }
        public int Trans_id { get; set; }

    }

    [Table(name: "PR_Empmaster_General_Bio")]
    public class PR_Empmaster_General_Bio : IDbEntity<int>
    {
        public int Id { get; set; }
        public int FY { get; set; }
        public DateTime MY { get; set; }
        public int EmpId { get; set; }
        public int EmpCode { get; set; }
        public DateTime FM { get; set; }
        public string Zone { get; set; }
        public string Regionforptax { get; set; }
        public string ptaxRegion { get; set; }
        public string Address1 { get; set; }
        public string PerAddress1 { get; set; }
        public string Nativeplace { get; set; }
        public string Division { get; set; }
        public DateTime Dateofconfirm { get; set; }
        public int PFno { get; set; }
        public int UANno { get; set; }
        public DateTime DOJPF { get; set; }
        public string Identifymark1 { get; set; }
        public string Identifymark2 { get; set; }
        public string Religion { get; set; }
        public string CurReservation { get; set; }
        public string JoinReservation { get; set; }
        public string PANno { get; set; }
        public int RegOrder { get; set; }
        public string Paybank { get; set; }
        public int Accountcode { get; set; }
        public int BankAccno { get; set; }
        public int CustomerId { get; set; }
        public int AccwithDCCB { get; set; }
        public bool PHYhandicapped { get; set; }
        public bool Houseprovided { get; set; }
        public string STLTemp { get; set; }
        public int FESTadv { get; set; }
        public string ARTRemp { get; set; }
        public DateTime FatherDOB { get; set; }
        public string ARTRFHrelationemp { get; set; }
        public bool Active { get; set; }
        public int Trans_id { get; set; }


    }
    public class DeductionFact
    {
        public int id { get; set; }
        public string value { get; set; }
    }

    [Table(name: "Latememo")]
    public class Latememo : IDbEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public String Empid { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime Duedate { get; set; }
        //public DateTime DueDate { get; set; }
        public string Memodetails { get; set; }
        public int Noofdays { get; set; }
        public string Clarification { get; set; }
        public Nullable<System.DateTime> Responsedate { get; set; }
        //public DateTime Responsedate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime IssueDate { get; set; }
        //public DateTime UpdatedDate { get; set; }
        public int Issueby { get; set; }
        public string Status { get; set; }
       // public string Action { get; set; }
        public string MemoType { get; set; }

        public int  Priornoticegivendays { get; set; }
        public string ReasonForLeave { get; set; }
        public string Leaveapplieddate { get; set; }
        public string leavetype { get; set; }
        public string controllingauthority { get; set; }


    }
 




}

