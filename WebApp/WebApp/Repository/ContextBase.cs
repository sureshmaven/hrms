using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Entities;

namespace Repository
{
    public class ContextBase:DbContext
    {
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        public string DbName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBase"/> class.
        /// </summary>
        public ContextBase()
            : base("DefaultConnection")
        {
            //Database.SetInitializer(new ContextBaseDbInitializer());
            Database.SetInitializer(new NullDatabaseInitializer<ContextBase>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextBase"/> class.
        /// </summary>
        /// <param name="sDBName">Name of the s database.</param>
        public ContextBase(string sDBName)
            : base(sDBName)
        {
            DbName = sDBName;
            //Database.SetInitializer(new ContextBaseDbInitializer());
            //Database.SetInitializer<ContextBase>(null);
            //Database.SetInitializer(new NullDatabaseInitializer<ContextBase>());
        }

        #region "Overridable/Virtual Messages"



        #endregion "Overridable/Virtual Messages"

        #region "Overridden Methods"

        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database.
        /// </summary>
        /// <returns>
        /// The number of objects written to the underlying database.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        #endregion "Overridden Methods"

        #region "Objects into Context"


        #endregion "Objects into Context"

        #region "Overridden Model Creation"

        /// <summary>
        /// Overrides the model. Use This Method to specify any Overridden Foreign Keys
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        private void OverrideModel(DbModelBuilder modelBuilder)
        {

        }

        #endregion "Overridden Model Creation"

        public DbSet<Entities.Employees> Employes { get; set; }
        public DbSet<Entities.Banks> Banks { get; set; }
        public DbSet<Entities.Branches> Branches { get; set; }
        public DbSet<Entities.Departments> Departments { get; set; }
        public DbSet<Entities.Designations> Designations { get; set; }
        public DbSet<Entities.Leaves> Leaves { get; set; }
        public DbSet<Entities.LeaveTypes> LeaveTypes { get; set; }
        public DbSet<Entities.Roles> Roles { get; set; }
        public DbSet<Entities.LeaveInfo> LeaveInfo { get; set; }
        public DbSet<Entities.V_LeaveHistory> V_LeaveHistory { get; set; }
        public DbSet<Entities.EmpLeaveBalance> EmpLeaveBalance { get; set; }
        public DbSet<Entities.V_LeaveBalance> V_LeaveBalance { get; set; }
        public DbSet<Entities.V_EmpLeaveBalance> V_EmpLeaveBalance { get; set; }
        public DbSet<Entities.HolidayList> HolidayList { get; set; }
        public DbSet<Entities.DeliveryDate_PTL> DeliveryDate_PTL { get; set; }
        public DbSet<Entities.WorkingDays> WorkingDays { get; set; }
        public DbSet<Entities.Branch_Designation_Mapping> Branch_Designation_Mapping { get; set; }
        public DbSet<Entities.Tx_History> Tx_History { get; set; }
        public DbSet<Entities.V_Tx_History>V_Tx_History { get; set; }

        public DbSet<Entities.Employee_Transfer> Employee_Transfer { get; set; }

        public DbSet<Entities.BranchStaffCount> BranchStaffCount { get; set; }
        public DbSet<Entities.view_employee> view_employee { get; set; }
       
        public DbSet<Entities.OD_OtherDuty> OD_OtherDuty { get; set; }

        public DbSet<Entities.OD_Unlock> OD_Unlock { get; set; }
        public DbSet<Entities.view_employee_dept> view_employee_dept { get; set; }
        public DbSet<Entities.view_employee_category> view_employee_category { get; set; }
        public DbSet<Entities.OD_Master> OD_Master { get; set; }
        public DbSet<Entities.view_employee_DOB_RetirementDateMonthWise> view_employee_DOB_RetirementDateMonthWise { get; set; }
        public DbSet<Entities.view_employee_senioritylist> view_employee_senioritylist { get; set; }
        public DbSet<Entities.view_employee_transfer> view_employee_transfer { get; set; }
        public DbSet<Entities.v_BranchContactList> v_BranchContactList { get; set; }
        public DbSet<Entities.Leaves_LTC> Leaves_LTC { get; set; }
        public DbSet<Entities.leaves_CreditDebit> leaves_CreditDebit { get; set; }
        public DbSet<Entities.Leaves_CarryForward> Leaves_CarryForward { get; set; }
        public DbSet<Entities.V_LeaveForward> V_LeaveForward { get; set; }
        public DbSet<Entities.V_EmpLeavesCarryForward> V_EmpLeavesCarryForward { get; set; }
        public DbSet<Entities.FamilyRelations> FamilyRelations { get; set; }
        public DbSet<Entities.BlockPeriod> BlockPeriod { get; set; }
        public DbSet<Entities.News> News { get; set; }
        public DbSet<Entities.View_ChangingAuthority> View_ChangingAuthority { get; set; }
        public DbSet<Entities.PLE_Type> PLE_Type { get; set; }
        public DbSet<Entities.V_EmpLeaveHistory> V_EmpLeaveHistory { get; set; }
        public DbSet<Entities.WorkDiary_Det> WorkDiary_Det { get; set; }
        public DbSet<Entities.WorkDiary> WorkDiary { get; set; }

        public DbSet<Entities.All_Masters> All_Masters { get; set; }
        public DbSet<Entities.Timesheet_Request_Form> Timesheet_Request_Form { get; set; }
        public DbSet<Entities.Shift_Master> Shift_Master { get; set; }
        public DbSet<Entities.Payroll_FW> Payroll_FW { get; set; }

        public virtual DbSet<Payroll_EW> Payroll_EW { get; set; }
        public virtual DbSet<PR_Monthattendance> PR_Monthattendance { get; set; }
        public virtual DbSet<AddPayMonth> PR_Monthdetails { get; set; }

        public virtual DbSet<PR_Emppayfields> PR_Emppayfields { get; set; }
        public virtual DbSet<PR_Payfieldmaster> PR_Payfieldmaster { get; set; }

        public virtual DbSet<PR_Empmaster_General_Bio> PR_Empmaster_General_Bio { get; set; }
        public DbSet<Entities.Latememo> Latememo { get; set; }
        // public DbSet<Entities.Payroll_EW> Payroll_EW { get; set; }


    }

}
