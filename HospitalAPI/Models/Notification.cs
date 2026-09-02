using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.Models
{
    public class Notification
    {
        public int Id { get; set; }

        // User who receives the notification
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}