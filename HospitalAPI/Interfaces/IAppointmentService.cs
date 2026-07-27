using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync();

        Task<AppointmentDto?> GetByIdAsync(int id);

        Task<AppointmentDto> AddAsync(CreateAppointmentDto dto);

        Task<bool> UpdateAsync(int id, UpdateAppointmentDto dto);

        Task<bool> DeleteAsync(int id);
    }
}