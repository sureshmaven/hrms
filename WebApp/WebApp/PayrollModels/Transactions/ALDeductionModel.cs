using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
    public class ALDeductionModel
    {
        public int id { get; set; }
        public string dbcolumn { get; set; }
        public string display { get; set; }
        public string value { get; set; }
    }
}
