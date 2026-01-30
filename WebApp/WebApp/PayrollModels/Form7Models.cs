using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels
{
    public class Form7Model
    {
        public string Month { get; set; }
        public string father { get; set; }
        public string Spouse { get; set; }
        public string EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string fy { get; set; }
        public string city { get; set; }
        public string PAN { get; set; }
        public string pf_no { get; set; }
        public int totalamount {get;set;}
        public int totalpffund { get; set; }
        public string finmonth { get; set; }
        public IList<contributionmodel> sect1 { get; set; }
    }
    

    public class contributionmodel
    { 
    
        public string month { get; set; }
    public string amount { get; set; }
    public string con_pensoinfund { get; set; }
   
}

  
}
