namespace HospitalAPI.DTOs
{
    public class PrescriptionDto
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
