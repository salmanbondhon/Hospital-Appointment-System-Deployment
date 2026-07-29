using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models
{
    public class Prescription
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; } = null!;

        [Required]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        public string Medicines { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
