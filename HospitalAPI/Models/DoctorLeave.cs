namespace HospitalAPI.Models
{
    public class DoctorLeave
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public Doctor Doctor { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = true;
    }
}
