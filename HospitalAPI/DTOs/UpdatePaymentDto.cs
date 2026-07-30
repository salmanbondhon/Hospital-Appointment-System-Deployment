using HospitalAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.DTOs
{
    public class UpdatePaymentDto
    {
        [Required]
        [Range(1, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }
    }
}