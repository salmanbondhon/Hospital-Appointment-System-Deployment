using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;

namespace HospitalAPI.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetPatientHistoryAsync(
            int patientId,
            int userId,
            string role)
        {
            // =========================================
            // PATIENT AUTHORIZATION
            // =========================================

            if (role == "Patient")
            {
                var patient =
                    await _patientRepository.GetByUserIdAsync(userId);

                if (patient == null)
                    throw new BusinessException(
                        "Patient profile not found.");

                if (patient.Id != patientId)
                    throw new BusinessException(
                        "You are not authorized.");
            }

            // =========================================
            // GET MEDICAL HISTORY
            // =========================================

            var prescriptions =
                await _medicalRecordRepository
                    .GetPatientHistoryAsync(patientId);

            // =========================================
            // CONVERT TO DTO
            // =========================================

            return prescriptions.Select(p => new MedicalRecordDto
            {
                AppointmentDate =
                    p.Appointment!.AppointmentDate,

                DoctorName =
                    p.Appointment.Doctor!.FullName,

                Diagnosis =
                    p.Diagnosis,

                Medicines =
                    p.Medicines,

                Notes =
                    p.Notes
            });
        }
    }
}