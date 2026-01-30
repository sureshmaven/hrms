using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
   
    public class AdjestPaymentsModel
    {
        //public int EFId { get; set; }
        //public int DFId { get; set; }
        //public int EmpId { get; set; }
        //public int EFAmount { get; set; }
        //public int DFAmount { get; set; }
        //public string Payfieldtype { get; set; }
        //public string Payfieldname { get; set; }
        //public bool Active { get; set; }
        //public int Trans_id { get; set; }
        public int EmpId { get; set; }
        public IList<EFPayFields> EarningFields { get; set; }
        public IList<DFPayFields> DeductionFields { get; set; }
        public IList<CFPayFields> ContributionFields { get; set; }

    }

    public class CFPayFields
    {
        public int CId { get; set; }
        public int CAmount { get; set; }
    }
    
}
