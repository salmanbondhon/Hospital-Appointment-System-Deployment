namespace HospitalAPI.DTOs
{
    public class CreateAppointmentDto
    {
        public int DoctorId { get; set; }

        // Required when Admin creates appointment
        // Patient does not need to send this
        public int? PatientId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;
    }
}