using HospitalAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace HospitalAPI.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}