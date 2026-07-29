using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _prescriptionRepository = prescriptionRepository;
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        // ==========================
        // Get All Prescriptions
        // ==========================
        public async Task<IEnumerable<PrescriptionDto>> GetAllAsync(int userId, string role)
        {
            // Admin -> All prescriptions
            if (role == "Admin")
            {
                var prescriptions = await _prescriptionRepository.GetAllAsync();

                return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            }

            // Doctor -> Only prescriptions written by this doctor
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctor.Id);

                return _mapper.Map<IEnumerable<PrescriptionDto>>(prescriptions);
            }

            // Patient -> Only own prescriptions
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            var patientPrescriptions =
                await _prescriptionRepository.GetByPatientIdAsync(patient.Id);

            return _mapper.Map<IEnumerable<PrescriptionDto>>(patientPrescriptions);
        }

        // ==========================
        // Get Prescription By Id
        // ==========================
        public async Task<PrescriptionDto?> GetByIdAsync(
            int id,
            int userId,
            string role)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
                return null;

            // Admin
            if (role == "Admin")
            {
                return _mapper.Map<PrescriptionDto>(prescription);
            }

            // Doctor
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (prescription.Appointment.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized.");

                return _mapper.Map<PrescriptionDto>(prescription);
            }

            // Patient
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            if (prescription.Appointment.PatientId != patient.Id)
                throw new BusinessException("You are not authorized.");

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        // ==========================
        // Create Prescription
        // ==========================
        public async Task<PrescriptionDto> CreateAsync(
            CreatePrescriptionDto dto,
            int userId)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(userId);

            if (doctor == null)
                throw new BusinessException("Doctor profile not found.");

            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId);

            if (appointment == null)
                throw new BusinessException("Appointment not found.");

            // Doctor can write only his own prescription
            if (appointment.DoctorId != doctor.Id)
                throw new BusinessException("You are not authorized.");

            // Appointment must be completed
            if (appointment.Status != AppointmentStatus.Completed)
                throw new BusinessException(
                    "Appointment must be completed before writing a prescription.");

            // Only one prescription per appointment
            var existingPrescription =
                await _prescriptionRepository.GetByAppointmentIdAsync(dto.AppointmentId);

            if (existingPrescription != null)
                throw new BusinessException(
                    "Prescription already exists for this appointment.");

            var prescription = _mapper.Map<Prescription>(dto);

            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();

            prescription = await _prescriptionRepository
                .GetByAppointmentIdAsync(dto.AppointmentId);

            return _mapper.Map<PrescriptionDto>(prescription!);
        }

        // ==========================
        // Delete Prescription
        // ==========================
        public async Task DeleteAsync(int id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
                throw new BusinessException("Prescription not found.");

            await _prescriptionRepository.DeleteAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();
        }
    }
}