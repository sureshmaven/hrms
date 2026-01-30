using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
  public class AllowanceModel
    {
        public int id { get; set; }
        public int emp_id { get; set; }
        public int emp_code { get; set; }
        public int fy { get; set; }
        public DateTime fm { get; set; }
        public string allowance { get; set; }
        public DateTime from_date { get; set; }
        public DateTime to_date { get; set; }
        public int active { get; set; }
        public int trans_id { get; set; }
    }
}
