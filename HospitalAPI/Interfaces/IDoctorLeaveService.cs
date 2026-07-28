using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IDoctorLeaveService
    {
        Task<IEnumerable<DoctorLeaveDto>> GetAllAsync();

        Task<IEnumerable<DoctorLeaveDto>> GetMyLeavesAsync(int userId);

        Task<DoctorLeaveDto> CreateLeaveAsync(CreateLeaveDto dto, int userId);

        Task ApproveLeaveAsync(int leaveId);

        Task DeleteLeaveAsync(int leaveId, int userId, string role);
    }
}
