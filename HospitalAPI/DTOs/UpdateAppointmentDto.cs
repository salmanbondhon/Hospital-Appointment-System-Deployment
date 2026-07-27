using HospitalAPI.Enums;

namespace HospitalAPI.DTOs
{
    public class UpdateAppointmentDto
    {
        public int DoctorId { get; set; }

        public int PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; }
    }
}