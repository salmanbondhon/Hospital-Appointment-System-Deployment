using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();

        Task<DoctorDashboardDto> GetDoctorDashboardAsync(int userId);

        Task<PatientDashboardDto> GetPatientDashboardAsync(int userId);
    }
}