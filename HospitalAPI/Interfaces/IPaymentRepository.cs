using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<Payment?> GetByIdAsync(int id);

        Task<Payment?> GetByAppointmentIdAsync(int appointmentId);

        Task AddAsync(Payment payment);

        Task UpdateAsync(Payment payment);

        Task DeleteAsync(Payment payment);

        Task SaveChangesAsync();
    }
}