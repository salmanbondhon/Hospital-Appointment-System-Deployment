using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IPatientService
    {
        // =================================================
        // GET ALL
        // =================================================

        Task<IEnumerable<PatientDto>> GetAllAsync();


        // =================================================
        // GET BY ID
        // =================================================

        Task<PatientDto?> GetByIdAsync(int id);


        // =================================================
        // GET CURRENT LOGGED-IN PATIENT
        // =================================================

        Task<PatientDto?> GetMyProfileAsync(
            int userId);


        // =================================================
        // CREATE
        // =================================================

        Task<PatientDto> AddAsync(
            CreatePatientDto dto,
            int currentUserId,
            string currentUserRole);


        // =================================================
        // UPDATE
        // =================================================

        Task UpdateAsync(
            int id,
            UpdatePatientDto dto,
            int currentUserId,
            string currentUserRole);


        // =================================================
        // DELETE
        // =================================================

        Task DeleteAsync(int id);
    }
}