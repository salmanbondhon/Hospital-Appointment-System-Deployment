using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Qualification { get; set; } = string.Empty;

        public int Experience { get; set; }

        public decimal ConsultationFee { get; set; }

        public string AvailableFrom { get; set; } = string.Empty;
        public string AvailableTo { get; set; } = string.Empty;

        // Foreign Key
        public int DepartmentId { get; set; }

        // Navigation Property
        public Department? Department { get; set; }


        // Foreign Key to User
        public int UserId { get; set; }

        // Navigation Property
        public User User { get; set; } = null!;
    }
}