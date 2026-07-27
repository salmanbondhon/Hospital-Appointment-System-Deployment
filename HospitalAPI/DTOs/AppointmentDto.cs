using HospitalAPI.Enums;

namespace HospitalAPI.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public int PatientId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; }
    }
}