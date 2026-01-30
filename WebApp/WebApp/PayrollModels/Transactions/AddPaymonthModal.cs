using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
  
   public class AddPaymonthModal
    {
            public int Id { get; set; }
            public int FY { get; set; }
            public DateTime FM { get; set; }
            public float Weekholidays { get; set; }
            public float Paidholidays { get; set; }
            public DateTime Paymentdate { get; set; }
            public float DAslabs { get; set; }
            public int Active { get; set; }
            public float DApoints { get; set; }
            public float DApercent { get; set; }
            public int Trans_id { get; set; }
            public int month_days { get; set; }
     
    }
}
