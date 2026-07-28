using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync(int userId, string role);

        Task<AppointmentDto?> GetByIdAsync(int id, int userId, string role);

        Task<AppointmentDto> AddAsync(CreateAppointmentDto dto, int userId);

        Task UpdateAsync(int id, UpdateAppointmentDto dto, int userId, string role);

        Task DeleteAsync(int id, int userId, string role);
    }
}