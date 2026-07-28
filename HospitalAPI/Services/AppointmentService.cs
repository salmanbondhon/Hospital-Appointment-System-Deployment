using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Exceptions;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using System.Numerics;

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

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync(int userId, string role)
        {
            // Admin -> All appointments
            if (role == "Admin")
            {
                var appointments = await _appointmentRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            }

            // Doctor -> Only their appointments
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                var appointments = await _appointmentRepository
                    .GetByDoctorIdAsync(doctor.Id);

                return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            }

            // Patient -> Only their appointments
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            var patientAppointments = await _appointmentRepository
                .GetByPatientIdAsync(patient.Id);

            return _mapper.Map<IEnumerable<AppointmentDto>>(patientAppointments);
        }
        public async Task<AppointmentDto?> GetByIdAsync(int id, int userId, string role)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                return null;

            // Admin can access everything
            if (role == "Admin")
            {
                return _mapper.Map<AppointmentDto>(appointment);
            }

            // Doctor can access only their appointments
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized to view this appointment.");

                return _mapper.Map<AppointmentDto>(appointment);
            }

            // Patient can access only their appointments
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient profile not found.");

            if (appointment.PatientId != patient.Id)
                throw new BusinessException("You are not authorized to view this appointment.");

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> AddAsync(CreateAppointmentDto dto, int userId)
        {
            // Prevent past appointments*3e
            if (dto.AppointmentDate < DateTime.Now)
            {
                throw new BusinessException("Appointment date cannot be in the past.");
            }

            if (!IsValidAppointmentSlot(dto.AppointmentDate))
            {
                throw new BusinessException(
                    "Appointments can only be booked in 30-minute intervals.");
            }

            // Validate Doctor
            var doctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);

            if (doctor == null)
                throw new BusinessException("Doctor not found.");

            // Check doctor's working hours
            bool isWorking = IsDoctorWorking(
                dto.AppointmentDate,
                doctor.AvailableFrom,
                doctor.AvailableTo);

            if (!isWorking)
            {
                throw new BusinessException(
                    $"Doctor is available only between {doctor.AvailableFrom} and {doctor.AvailableTo}");
            }

            // Validate Patient
            var patient = await _patientRepository.GetByUserIdAsync(userId);

            if (patient == null)
                throw new BusinessException("Patient not found.");

           

            // Check Doctor Availability
            var isAvailable = await _appointmentRepository
                .IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDate);

            if (!isAvailable)
            {
                throw new BusinessException("Doctor already has an appointment at this time.");
            }




            // Create Appointment
            var appointment = _mapper.Map<Appointment>(dto);
            appointment.PatientId = patient.Id;

            appointment.Status = AppointmentStatus.Pending;

            // IMPORTANT: Link the appointment to the logged-in patient
            appointment.PatientId = patient.Id;

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            appointment = await _appointmentRepository.GetByIdAsync(appointment.Id);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task UpdateAsync(
    int id,
    UpdateAppointmentDto dto,
    int userId,
    string role)
        {
            // Prevent past appointments
            if (dto.AppointmentDate < DateTime.Now)
            {
                throw new BusinessException("Appointment date cannot be in the past.");
            }

            if (!IsValidAppointmentSlot(dto.AppointmentDate))
            {
                throw new BusinessException(
                    "Appointments can only be booked in 30-minute intervals.");
            }

            // Check if appointment exists
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                throw new BusinessException("Appointment not found.");
            }

            // Validate Doctor
            var selectedDoctor = await _doctorRepository.GetByIdAsync(dto.DoctorId);

            if (selectedDoctor == null)
            {
                throw new BusinessException("Doctor not found.");
            }

            bool isWorking = IsDoctorWorking(
    dto.AppointmentDate,
    selectedDoctor.AvailableFrom,
    selectedDoctor.AvailableTo);

            if (!isWorking)
            {
                throw new BusinessException(
                    $"Doctor is available only between {selectedDoctor.AvailableFrom} and {selectedDoctor.AvailableTo}");
            }

            // Validate Patient
            var patient = await _patientRepository.GetByIdAsync(dto.PatientId);

            if (patient == null)
            {
                throw new BusinessException("Patient not found.");
            }

            // Check doctor's availability (ignore the current appointment)
            var isAvailable = await _appointmentRepository
                .IsDoctorAvailableForUpdateAsync(
                    id,
                    dto.DoctorId,
                    dto.AppointmentDate);

            if (!isAvailable)
            {
                throw new BusinessException("Doctor already has an appointment at this time.");
            }



            // Admin can update everything
            if (role == "Doctor")
            {
                var loggedInDoctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (loggedInDoctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != loggedInDoctor.Id)
                    throw new BusinessException("You are not authorized to update this appointment.");
            }


            // Update appointment
            _mapper.Map(dto, appointment);

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            
        }

        public async Task DeleteAsync(
    int id,
    int userId,
    string role)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                throw new BusinessException("Appointment not found.");
            }

            // Doctor can delete only their own appointments
            if (role == "Doctor")
            {
                var loggedInDoctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (loggedInDoctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != loggedInDoctor.Id)
                    throw new BusinessException("You are not authorized to delete this appointment.");
            }

            // Admin reaches here automatically
            await _appointmentRepository.DeleteAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }


        public async Task ApproveAppointmentAsync(
    int appointmentId,
    int userId,
    string role)
        {
            var appointment = await _appointmentRepository
                .GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new BusinessException("Appointment not found.");

            // Only doctor assigned to this appointment can approve it
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized.");
            }

            if (appointment.Status != AppointmentStatus.Pending)
                throw new BusinessException("Only pending appointments can be approved.");

            appointment.Status = AppointmentStatus.Approved;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }



        public async Task CompleteAppointmentAsync(
    int appointmentId,
    int userId,
    string role)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new BusinessException("Appointment not found.");

            // Doctor authorization
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized.");
            }

            // Business rule
            if (appointment.Status != AppointmentStatus.Approved)
                throw new BusinessException("Only approved appointments can be completed.");

            appointment.Status = AppointmentStatus.Completed;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }


        public async Task CancelAppointmentAsync(
    int appointmentId,
    int userId,
    string role)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new BusinessException("Appointment not found.");

            // Doctor authorization
            if (role == "Doctor")
            {
                var doctor = await _doctorRepository.GetByUserIdAsync(userId);

                if (doctor == null)
                    throw new BusinessException("Doctor profile not found.");

                if (appointment.DoctorId != doctor.Id)
                    throw new BusinessException("You are not authorized.");
            }

            // Business Rule
            if (appointment.Status == AppointmentStatus.Completed)
            {
                throw new BusinessException("Completed appointments cannot be cancelled.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                throw new BusinessException("Appointment is already cancelled.");
            }

            appointment.Status = AppointmentStatus.Cancelled;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }


        private bool IsDoctorWorking(
    DateTime appointmentDate,
    string availableFrom,
    string availableTo)
        {
            TimeSpan appointmentTime = appointmentDate.TimeOfDay;

            TimeSpan from = TimeSpan.Parse(availableFrom);
            TimeSpan to = TimeSpan.Parse(availableTo);

            return appointmentTime >= from &&
                   appointmentTime <= to;
        }

        private bool IsValidAppointmentSlot(DateTime appointmentDate)
        {
            return (appointmentDate.Minute == 0 ||
                    appointmentDate.Minute == 30)
                   && appointmentDate.Second == 0;
        }
    }
}