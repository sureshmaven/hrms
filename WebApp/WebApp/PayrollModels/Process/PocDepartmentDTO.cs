using System.Collections.Generic;

namespace PayrollModels.Process
{
    public class PocDepartmentDTO
    {
        public int EmpId { get; set; }
        public char ActionType { get; set; }
        public IList<PocDepartment> DeptList { get; set; }
    }
}
