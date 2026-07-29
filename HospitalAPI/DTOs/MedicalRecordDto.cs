namespace HospitalAPI.DTOs
{
    public class MedicalRecordDto
    {
        public DateTime AppointmentDate { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}