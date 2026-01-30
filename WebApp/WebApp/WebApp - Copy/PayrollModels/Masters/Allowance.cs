using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Masters
{
    public class Allowance
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Amount { get; set; }

        public List<Allowance> AllowanceList { get; set; }
    }
}
