using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        // =========================
        // Appointment
        // =========================

        [Required]
        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; } = null!;


        // =========================
        // Medical Information
        // =========================

        [Required]
        public string Diagnosis { get; set; } = string.Empty;

        public string Symptoms { get; set; } = string.Empty;

        public string Treatment { get; set; } = string.Empty;

        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;


        // =========================
        // Dates
        // =========================

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}