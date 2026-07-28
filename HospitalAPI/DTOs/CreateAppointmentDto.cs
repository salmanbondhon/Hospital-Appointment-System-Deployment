using HospitalAPI.Enums;

namespace HospitalAPI.DTOs
{
    public class CreateAppointmentDto
    {
        public int DoctorId { get; set; }


        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; }
    }
}