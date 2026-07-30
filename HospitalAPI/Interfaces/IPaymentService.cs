using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync(int userId, string role);

        Task<PaymentDto?> GetByIdAsync(int id, int userId, string role);

        Task<PaymentDto> CreateAsync(CreatePaymentDto dto, int userId);

        Task UpdateAsync(int id, UpdatePaymentDto dto);

        Task DeleteAsync(int id);
    }
}