using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.DTOs
{
    public class CreatePrescriptionDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}