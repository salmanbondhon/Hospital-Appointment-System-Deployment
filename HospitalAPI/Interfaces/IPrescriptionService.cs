using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionDto>> GetAllAsync(
            int userId,
            string role);

        Task<PrescriptionDto?> GetByIdAsync(
            int id,
            int userId,
            string role);

        Task<PrescriptionDto> CreateAsync(
            CreatePrescriptionDto dto,
            int userId);

        Task<PrescriptionDto> UpdateAsync(
            int id,
            UpdatePrescriptionDto dto,
            int userId,
            string role);

        Task DeleteAsync(
            int id,
            int userId,
            string role);
    }
}