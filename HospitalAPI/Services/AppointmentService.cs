using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;

namespace HospitalAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                return null;

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> AddAsync(CreateAppointmentDto dto)
        {
            // Validate Doctor
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);

            if (doctor == null)
                throw new BusinessException("Doctor not found.");

            // Validate Patient
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);

            if (patient == null)
                throw new BusinessException("Patient not found.");

            // Check Doctor Availability
            var isAvailable = await _appointmentRepository
                .IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDate);

            if (!isAvailable)
            {
                throw new BusinessException("Doctor already has an appointment at this time.");
            }




            var appointment = _mapper.Map<Appointment>(dto);

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            appointment = await _appointmentRepository.GetByIdAsync(appointment.Id);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<bool> UpdateAsync(int id, UpdateAppointmentDto dto)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                return false;

            _mapper.Map(dto, appointment);

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                return false;

            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return true;
        }
    }
}