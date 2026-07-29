using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetAllAsync();

        Task<Prescription?> GetByIdAsync(int id);

        Task<Prescription?> GetByAppointmentIdAsync(int appointmentId);

        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId);

        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);

        Task<IEnumerable<Prescription>> GetPatientHistoryAsync(int patientId);
        Task AddAsync(Prescription prescription);

        Task UpdateAsync(Prescription prescription);

        Task DeleteAsync(Prescription prescription);

        Task SaveChangesAsync();

        
    }
}