using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Masters
{
    public class HouseRentDetailsModel
    {
        public int EmpId { get; set; }

        public IList<MonthlyRents> MonthRents { get; set; }
    }

    public class MonthlyRents
    {
        public int Mid { get; set; }
        public int MAmount { get; set; }
    }
}
