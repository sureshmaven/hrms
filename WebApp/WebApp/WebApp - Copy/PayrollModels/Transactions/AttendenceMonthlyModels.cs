using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
   public class AttendenceMonthlyModels
    {
        public int Id { get; set; }
        public int DOJ { get; set; }
        public int fy { get; set; }
        public int emp_id { get; set; }
        public string status { get; set; }
        public DateTime status_date { get; set; }
        public DateTime fm { get; set; }
        public string leaves_available { get; set; }
        public float lop_days { get; set; }
        public float absent_days { get; set; }
        public float working_days { get; set; }
        public int Active { get; set; }
        //public Leaves lleaves { get; set; }
        //public V_EmpLeaveBalance lEmpLeaveBal { get; set; }
        //public DeliveryDate_PTL lDelivery { get; set; }
        public int trans_id { get; set; }
        

    }
    public class UpdateDetails
    {
        public int id { get; set; }
        public string dbcolumn { get; set; }
        public string display { get; set; }
        public string value { get; set; }
        public string Name { get; set; }
        public string EmpId { get; set; }
        public string status { get; set; }
        public string status_date { get; set; }
        public float lop_days { get; set; }
        public float absent_days { get; set; }
        public float working_days { get; set; }
        public List<EmpD> object1 { get; set; }
        public List<EmpD> object2 { get; set; }
        public float suspend_percent { get; set; }

    }
    public class EmpD
    {
        
        public string value { get; set; }
        public string Name { get; set; }
    }
}
