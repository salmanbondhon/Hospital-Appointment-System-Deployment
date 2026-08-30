namespace HospitalAPI.DTOs
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string PatientName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Symptoms { get; set; } = string.Empty;

        public string Treatment { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}