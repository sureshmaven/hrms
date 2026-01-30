using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollModels.Masters
{
   
        public class form12b
    {
            public int EntityId { get; set; }
            public List<form12b> form12bs { get; set; }
        }

        public class form12bs
        {
            public string id { get; set; }
            public string tanno { get; set; }
            public string panno { get; set; }
            public string salary { get; set; }
            public string hraallo { get; set; }
            public string preqamt { get; set; }
            public string grossal { get; set; }
            public string licpf { get; set; }
            public string remarks { get; set; }
            public string action { get; set; }
    }
    
}
