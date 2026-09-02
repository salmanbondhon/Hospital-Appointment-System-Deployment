using HospitalAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        public Patient? Patient { get; set; }

        public Doctor? Doctor { get; set; }

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();

        // Password Reset
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}