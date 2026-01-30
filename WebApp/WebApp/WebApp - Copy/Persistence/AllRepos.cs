using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
namespace Persistence
{
    public class Payroll_EWRepository : CrudGenericRepository<ContextBase, Payroll_EW, int>
    {

    }
    public class EmployeesRepository:CrudGenericRepository<ContextBase,Employees,int>
    {

    }
    public class BanksRepository:CrudGenericRepository<ContextBase,Banks,int>
    {

    }
    public class BranchesRepository : CrudGenericRepository<ContextBase, Branches, int>
    {

    }
    public class DepartmentsRepository : CrudGenericRepository<ContextBase, Departments, int>
    {

    }
    public class BranchStaffCountRepository : CrudGenericRepository<ContextBase, BranchStaffCount, int>
    {

    }
    public class DesignationsRepository : CrudGenericRepository<ContextBase, Designations, int>
    {

    }
    public class LeavesRepository : CrudGenericRepository<ContextBase, Leaves, int>
    {

    }

    public class LeavesDebitorcreditRepository : CrudGenericRepository<ContextBase, leaves_CreditDebit, int>
    {

    }
    public class Shift_MasterRepository : CrudGenericRepository<ContextBase, Shift_Master, int>
    {

    }
    public class LeaveTypesRepository : CrudGenericRepository<ContextBase, LeaveTypes, int>
    {

    }
    public class RolesRepository : CrudGenericRepository<ContextBase, Roles, int>
    {

    }
    public class Branch_Designation_MappingRepository : CrudGenericRepository<ContextBase, Branch_Designation_Mapping, int>
    {

    }
    public class Employee_TransferRepository : CrudGenericRepository<ContextBase, Employee_Transfer, int>
    {

    }
    public class OtherDutyRepository : CrudGenericRepository<ContextBase,OD_OtherDuty, int>
    {

    }
    public class OD_MasterRepository : CrudGenericRepository<ContextBase, OD_Master, int>
    {

    }
    public class Leaves_LTCRepository : CrudGenericRepository<ContextBase, Leaves_LTC, int>
    {

    }
    public class BlockPeriodRepository : CrudGenericRepository<ContextBase, BlockPeriod, int>
    {

    }
    public class NewsRepository : CrudGenericRepository<ContextBase, News, int>
    {

    }
    public class PLE_TypeRepository : CrudGenericRepository<ContextBase, PLE_Type, int>
    {

    }
    public class WorkDiaryRepository : CrudGenericRepository<ContextBase, WorkDiary, int>
    {

    }

    public class V_EmpLeaveHistoryRepository : CrudGenericRepository<ContextBase, V_EmpLeaveHistory, int>
    {


    }
    public class All_MastersRepository : CrudGenericRepository<ContextBase, All_Masters, int>
    {

    }

    public class PR_EmppayfieldsRepository : CrudGenericRepository<ContextBase, PR_Emppayfields, int>
    {

    }
    public class PR_PayfieldmasterRepository : CrudGenericRepository<ContextBase, PR_Payfieldmaster, int>
    {

    }
}
