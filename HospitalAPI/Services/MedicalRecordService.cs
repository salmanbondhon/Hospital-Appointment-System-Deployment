using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;

namespace HospitalAPI.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPatientRepository _patientRepository;

        public MedicalRecordService(
            IPrescriptionRepository prescriptionRepository,
            IPatientRepository patientRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetPatientHistoryAsync(
            int patientId,
            int userId,
            string role)
        {
            // Patient can only view their own history
            if (role == "Patient")
            {
                var patient = await _patientRepository.GetByUserIdAsync(userId);

                if (patient == null)
                    throw new BusinessException("Patient profile not found.");

                if (patient.Id != patientId)
                    throw new BusinessException("You are not authorized.");
            }

            // Admin and Doctor can view any patient's history

            var prescriptions = await _prescriptionRepository
                .GetPatientHistoryAsync(patientId);

            return prescriptions.Select(p => new MedicalRecordDto
            {
                AppointmentDate = p.Appointment!.AppointmentDate,
                DoctorName = p.Appointment.Doctor!.FullName,
                Diagnosis = p.Diagnosis,
                Medicines = p.Medicines,
                Notes = p.Notes
            });
        }
    }
}