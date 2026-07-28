namespace HospitalAPI.DTOs
{
    public class DoctorLeaveDto
    {

        public int Id { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
    }
}
