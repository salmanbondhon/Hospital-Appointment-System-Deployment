using HospitalAPI.Models;

namespace HospitalAPI.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<IEnumerable<Prescription>> GetPatientHistoryAsync(int patientId);
    }
}