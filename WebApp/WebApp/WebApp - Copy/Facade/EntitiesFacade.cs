using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Persistence;
namespace Facade
{
    public static class EntitiesFacade
    {
       
        public static List<Employees> GetAll()
        {
            return new Persistence.EmployeesRepository().GetAll().ToList();
        }
        public static List<Leaves_LTC> GetLTCAll()
        {
            return new Persistence.Leaves_LTCRepository().GetAll().ToList();
        }
        public static List<All_Masters> GetworkmasterAll()
        {
            return new Persistence.All_MastersRepository().GetAll().ToList();
        }
        public static List<Departments> GetAllDepartments()
        {
            return new Persistence.DepartmentsRepository().GetAll().ToList();
        }
        public static List<Roles> GetAllRoles()
        {
            return new Persistence.RolesRepository().GetAll().ToList();
        }
       
        public static List<LeaveTypes> GetAllLeaveTypes()
        {
            return new Persistence.LeaveTypesRepository().GetAll().ToList();
        }
       
        public static List<Branches> GetAllBranches()
        {
            return new Persistence.BranchesRepository().GetAll().ToList();
        }
        public static List<BranchStaffCount> GetAllBranchStaff()
        {
            return new Persistence.BranchStaffCountRepository().GetAll().ToList();
        }
        public static List<Banks> GetAllBanks()
        {
            return new Persistence.BanksRepository().GetAll().ToList();
        }
        public static List<OD_Master> GetAllODMaster()
        {
            return new Persistence.OD_MasterRepository().GetAll().ToList();
        }
        public static List<BlockPeriod> GetAllblockperiod()
        {
            return new Persistence.BlockPeriodRepository().GetAll().ToList();
        }
        public static List<News> GetAllNews()
        {
            return new Persistence.NewsRepository().GetAll().ToList();
        }
        public static void EmpUpdate(Employees emp)
        {
             new Persistence.EmployeesRepository().AddOrUpdate(emp);
        }

        public static List<Designations> GetAllDesignations()
        {
            return new Persistence.DesignationsRepository().GetAll().ToList();
            }
        public static List<Shift_Master> GetAllShifts()
        {
            return new Persistence.Shift_MasterRepository().GetAll().ToList();
        }
        public static Entities.Leaves Insert(Entities.Leaves order)
        {
            return new Persistence.LeavesRepository().Add(order);
        }
        public static List<Payroll_EW> GetAllEW()
        {
            return new Persistence.Payroll_EWRepository().GetAll().ToList();
        }
        public static List<PR_Emppayfields> GetAllEmppayfields()
        {
            return new Persistence.PR_EmppayfieldsRepository().GetAll().ToList();
        }
        public static List<PR_Payfieldmaster> GetAllEmppayfieldsMaster()
        {
            return new Persistence.PR_PayfieldmasterRepository().GetAll().ToList();
        }
        public static void Update(Entities.Leaves pro)
        {
            new Persistence.LeavesRepository().Update(pro);
        }
        public static bool Delete(int id)
        {
            return new Persistence.LeavesRepository().Delete(id);
        }
        public static class LeavesRepositoryFacade
        {
            public static List<Entities.Leaves> GetAll()
            {
                return new Persistence.LeavesRepository().GetAll().ToList();
            }
        }

        public static class LeavesTypesRepositoryFacade
        {
            public static List<Entities.LeaveTypes> GetAll()
            {
                return new Persistence.LeaveTypesRepository().GetAll().ToList();
            }

            public static List<Entities.LeaveTypes> GetAll1()
            {
                return new Persistence.LeaveTypesRepository().GetAll().OrderBy(a=>a.Type).ToList();
            }
        }

        public static List<Employee_Transfer> GetALLTransfer()
        {
            return new Persistence.Employee_TransferRepository().GetAll().ToList();
        }
        public static class Branch_Designation_MappingFacade
        {
            public static List<Entities.Branch_Designation_Mapping> GetBranchDesAll()
            {
                return new Persistence.Branch_Designation_MappingRepository().GetAll().ToList();
            }
        }
        public static class GetEmpTabledata
        {
            public static Entities.Employees GetById(int id)
            {
                return new Persistence.EmployeesRepository().GetIt(id);
            }
        }

        public static class GetLeaveTabledata
        {
            public static Entities.Leaves GetById(int id)
            {
                return new Persistence.LeavesRepository().GetIt(id);
            }
        }

        public static class GetLeaveDebitorcreditTabledata
        {
            public static Entities.leaves_CreditDebit GetById(int id)
            {
                return new Persistence.LeavesDebitorcreditRepository().GetIt(id);
            }
        }

        public static class GetOtherdutyTabledata
        {
            public static Entities.OD_OtherDuty GetById(int id)
            {
                return new Persistence.OtherDutyRepository().GetIt(id);
            }
        }
        public static class GetWorkTabledata
        {
            public static Entities.WorkDiary GetById(int id)
            {
                return new Persistence.WorkDiaryRepository().GetIt(id);
            }
        }
        public static class GetLTCTabledata
        {
            public static Entities.Leaves_LTC GetById(int id)
            {
                return new Persistence.Leaves_LTCRepository().GetIt(id);
            }
        }
        public static class GetPLTabledata
        {
            public static Entities.PLE_Type GetById(int id)
            {
                return new Persistence.PLE_TypeRepository().GetIt(id);
            }
        }

        public static class GetV_EmpLeaveHistorydata
        {
            public static Entities.V_EmpLeaveHistory GetById(int id)
            {
                return new Persistence.V_EmpLeaveHistoryRepository().GetIt(id);
            }
        }

    }
}
