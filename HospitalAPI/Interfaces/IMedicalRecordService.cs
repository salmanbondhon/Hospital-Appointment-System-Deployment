using HospitalAPI.DTOs;

namespace HospitalAPI.Interfaces
{
    public interface IMedicalRecordService
    {

        Task<IEnumerable<MedicalRecordDto>>
GetPatientHistoryAsync(
    int patientId,
    int userId,
    string role);
    }
}
