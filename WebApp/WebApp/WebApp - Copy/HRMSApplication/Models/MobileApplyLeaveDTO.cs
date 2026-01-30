namespace HRMSApplication.Models
{
    public class MobileApplyLeaveDTO
    {
        public int EmpId { get; set; }

        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DeliveryDate { get; set; }
        public string Reason { get; set; }
        public string MatrenityType { get; set; }

        public string ControllingAuthority { get; set; }
        public string SanctioningAuthority { get; set; }
    }    
}