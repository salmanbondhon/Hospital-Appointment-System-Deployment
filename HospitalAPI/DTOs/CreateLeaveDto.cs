namespace HospitalAPI.DTOs
{
    public class CreateLeaveDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
