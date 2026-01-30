using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Transactions
{
    public class DeputationEntryModel
    {
        public int EmpId { get; set; }
        public IList<DeductionEntryFields> DeductionFields { get; set; }
        public IList<ContributionEntryFields> ContributionFields { get; set; }
    }
    public class DeductionEntryFields
    {
        public int DId { get; set; }
        public int DAmount { get; set; }
    }
    public class ContributionEntryFields
    {
        public int CId { get; set; }
        public int CAmount { get; set; }
    }

    
}
