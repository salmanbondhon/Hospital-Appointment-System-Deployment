using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IDoctorLeaveRepository
    {
        Task<IEnumerable<DoctorLeave>> GetAllAsync();

        Task<IEnumerable<DoctorLeave>> GetByDoctorIdAsync(int doctorId);

        Task<DoctorLeave?> GetByIdAsync(int id);

        Task AddAsync(DoctorLeave leave);

        Task UpdateAsync(DoctorLeave leave);

        Task DeleteAsync(DoctorLeave leave);

        Task SaveChangesAsync();

        Task<bool> IsDoctorOnLeaveAsync(int doctorId, DateTime appointmentDate);
    }
}
