using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Process
{
    public class TDSProcesAllowanceModel
    {
        public IList<OldAllowance> OldAllw { get; set; }
        public IList<ActiveAllowance> ActiveAllw { get; set; }
    }

    public class OldAllowance
    {
        public int old_m_id { get; set; }
        public string old_alw_type { get; set; }
        public string old_Name { get; set; }
        public float old_Amount { get; set; }
    }

    public class ActiveAllowance
    {
        public int Active_m_id { get; set; }
        public string Active_alw_type { get; set; }
        public string Active_Name { get; set; }
        public float Active_Amount { get; set; }
    }
}
