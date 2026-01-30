using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Masters
{
   public class IncrementDateChangeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Branch { get; set; }
        public string Increment_Date_WEF { get; set; }
        public string IncDate { get; set; }
        public string change_incr_date { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }

       // public List<IncrementDateChangeModel> IncrementDateChange { get; set; }
        public List<IncrementDateChange> object1 { get; set; }
        public List<string> arrData { get; set; }
    }
    public class IncrementDateChange
    {
        public string Value { get; set; }
        public string Id { get; set; }
    }

}
