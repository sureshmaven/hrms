using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRMSApplication.Models
{
    public class EmpTransfer
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Transfer_Type { get; set; }
        public int EmpId { get; set; }
        public int NewDesignation { get; set; }
        public int NewDesignationPT { get; set; }
        public int NewBranch { get; set; }
        public int NewDepartment { get; set; }
        public int OldDesignation { get; set; }
        public int OldBranch { get; set; }
        public int OldDepartment { get; set; }
        public Nullable<System.DateTime> EffectiveFrom { get; set; }
        public Nullable<System.DateTime> EffectiveTo { get; set; }
        public Nullable<System.DateTime> EffectiveFromP { get; set; }
        public string RadioValue1 { get; set; }
        public string RadioValue2 { get; set; }
        public string radiovalue { get; set; }
        public int NewDepartmentT { get; set; }
        public int NewBranchT { get; set; }
        public Nullable<System.DateTime> EffectiveFromT { get; set; }
        public Nullable<System.DateTime> EffectiveToT { get; set; }
        public int NewDepartmentPT { get; set; }
        public int NewBranchPT { get; set; }
        public Nullable<System.DateTime> EffectiveFromPT { get; set; }
        public string Pcategory { get; set; }
        public string Poldbasic { get; set; }
        public string Pnewbasic { get; set; }
        public int Pseniority { get; set; }
        public Nullable<System.DateTime> Pincrement { get; set; }
    }
}