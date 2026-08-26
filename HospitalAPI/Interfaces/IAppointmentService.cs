using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync(
            int userId,
            string role);

        Task<AppointmentDto?> GetByIdAsync(
            int id,
            int userId,
            string role);

        Task<AppointmentDto> AddAsync(
            CreateAppointmentDto dto,
            int userId,
            string role);

        Task UpdateAsync(
            int id,
            UpdateAppointmentDto dto,
            int userId,
            string role);

        Task DeleteAsync(
            int id,
            int userId,
            string role);

        Task ApproveAppointmentAsync(
            int appointmentId,
            int userId,
            string role);

        Task CompleteAppointmentAsync(
            int appointmentId,
            int userId,
            string role);

        Task CancelAppointmentAsync(
            int appointmentId,
            int userId,
            string role);
    }
}