using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);

        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);

        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);

        Task AddAsync(Appointment appointment);

        Task UpdateAsync(Appointment appointment);

        Task DeleteAsync(Appointment appointment);

        Task SaveChangesAsync();

        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDate);

        Task<bool> IsDoctorAvailableForUpdateAsync(
            int appointmentId,
            int doctorId,
            DateTime appointmentDate);
    }
}