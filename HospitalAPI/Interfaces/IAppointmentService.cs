using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync(int userId, string role); 

        Task<AppointmentDto?> GetByIdAsync(int id);

        Task<AppointmentDto> AddAsync(CreateAppointmentDto dto, int userId);

        Task UpdateAsync(int id, UpdateAppointmentDto dto);

        Task DeleteAsync(int id);
    }
}