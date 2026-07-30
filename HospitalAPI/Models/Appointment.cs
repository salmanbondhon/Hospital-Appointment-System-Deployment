using HospitalAPI.Enums;

namespace HospitalAPI.Models
{
    public class Appointment
    {



        public int Id { get; set; }

        public int DoctorId { get; set; }

        public Doctor? Doctor { get; set; }

        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        public Prescription? Prescription { get; set; }
        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;

        public Payment? Payment { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
