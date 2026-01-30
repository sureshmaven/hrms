using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public class DashBoard
    {
        public List<string[]>Birthdays { get; set; }

        public List<string[]> Retirements { get; set; }

        public List<string[]> News { get; set; }

        public List<string[]> Leaves { get; set; }

        [NotMapped]
        public string LoginMode { get; set; }
    }
}