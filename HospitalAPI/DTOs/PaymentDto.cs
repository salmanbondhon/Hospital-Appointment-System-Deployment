using HospitalAPI.Enums;

namespace HospitalAPI.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus Status { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }
    }
}