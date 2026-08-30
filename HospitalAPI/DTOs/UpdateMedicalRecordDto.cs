using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.DTOs
{
    public class UpdateMedicalRecordDto
    {
        // =========================
        // Medical Information
        // =========================

        [Required]
        public string Diagnosis { get; set; } = string.Empty;

        public string Symptoms { get; set; } = string.Empty;

        public string Treatment { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}