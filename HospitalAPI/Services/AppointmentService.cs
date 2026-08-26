using AutoMapper;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
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
        private readonly IDoctorLeaveRepository _doctorLeaveRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;


        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IDoctorLeaveRepository doctorLeaveRepository,
            IMapper mapper,
            IEmailService emailService)
        {
            _appointmentRepository =
                appointmentRepository;

            _doctorRepository =
                doctorRepository;

            _patientRepository =
                patientRepository;

            _doctorLeaveRepository =
                doctorLeaveRepository;

            _mapper = mapper;

            _emailService =
                emailService;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<IEnumerable<AppointmentDto>>
            GetAllAsync(
                int userId,
                string role)
        {
            // ADMIN
            if (role == "Admin")
            {
                var appointments =
                    await _appointmentRepository
                        .GetAllAsync();

                return _mapper.Map<
                    IEnumerable<AppointmentDto>>(
                        appointments);
            }


            // DOCTOR
            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                var appointments =
                    await _appointmentRepository
                        .GetByDoctorIdAsync(
                            doctor.Id);

                return _mapper.Map<
                    IEnumerable<AppointmentDto>>(
                        appointments);
            }


            // PATIENT
            var patient =
                await _patientRepository
                    .GetByUserIdAsync(userId);

            if (patient == null)
            {
                throw new BusinessException(
                    "Patient profile not found.");
            }

            var patientAppointments =
                await _appointmentRepository
                    .GetByPatientIdAsync(
                        patient.Id);

            return _mapper.Map<
                IEnumerable<AppointmentDto>>(
                    patientAppointments);
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<AppointmentDto?>
            GetByIdAsync(
                int id,
                int userId,
                string role)
        {
            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(id);

            if (appointment == null)
            {
                return null;
            }


            // ADMIN
            if (role == "Admin")
            {
                return _mapper.Map<
                    AppointmentDto>(
                        appointment);
            }


            // DOCTOR
            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId != doctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized to view this appointment.");
                }

                return _mapper.Map<
                    AppointmentDto>(
                        appointment);
            }


            // PATIENT
            var patient =
                await _patientRepository
                    .GetByUserIdAsync(userId);

            if (patient == null)
            {
                throw new BusinessException(
                    "Patient profile not found.");
            }

            if (appointment.PatientId != patient.Id)
            {
                throw new BusinessException(
                    "You are not authorized to view this appointment.");
            }

            return _mapper.Map<
                AppointmentDto>(
                    appointment);
        }


        // =====================================================
        // CREATE APPOINTMENT
        // =====================================================

        public async Task<AppointmentDto>
            AddAsync(
                CreateAppointmentDto dto,
                int userId,
                string role)
        {
            // -------------------------------------------------
            // DATE VALIDATION
            // -------------------------------------------------

            if (dto.AppointmentDate < DateTime.Now)
            {
                throw new BusinessException(
                    "Appointment date cannot be in the past.");
            }


            // -------------------------------------------------
            // 30 MINUTE SLOT
            // -------------------------------------------------

            if (!IsValidAppointmentSlot(
                dto.AppointmentDate))
            {
                throw new BusinessException(
                    "Appointments can only be booked in 30-minute intervals.");
            }


            // -------------------------------------------------
            // DOCTOR
            // -------------------------------------------------

            var doctor =
                await _doctorRepository
                    .GetByIdAsync(dto.DoctorId);

            if (doctor == null)
            {
                throw new BusinessException(
                    "Doctor not found.");
            }


            // -------------------------------------------------
            // DOCTOR LEAVE
            // -------------------------------------------------

            var onLeave =
                await _doctorLeaveRepository
                    .IsDoctorOnLeaveAsync(
                        doctor.Id,
                        dto.AppointmentDate);

            if (onLeave)
            {
                throw new BusinessException(
                    "Doctor is on approved leave.");
            }


            // -------------------------------------------------
            // DOCTOR WORKING HOURS
            // -------------------------------------------------

            bool isWorking =
                IsDoctorWorking(
                    dto.AppointmentDate,
                    doctor.AvailableFrom,
                    doctor.AvailableTo);

            if (!isWorking)
            {
                throw new BusinessException(
                    $"Doctor is available only between {doctor.AvailableFrom} and {doctor.AvailableTo}");
            }


            // -------------------------------------------------
            // PATIENT
            // -------------------------------------------------

            Patient? patient;


            // ADMIN
            if (role == "Admin")
            {
                if (!dto.PatientId.HasValue)
                {
                    throw new BusinessException(
                        "Please select a patient.");
                }

                patient =
                    await _patientRepository
                        .GetByIdAsync(
                            dto.PatientId.Value);

                if (patient == null)
                {
                    throw new BusinessException(
                        "Patient not found.");
                }
            }


            // PATIENT
            else
            {
                patient =
                    await _patientRepository
                        .GetByUserIdAsync(userId);

                if (patient == null)
                {
                    throw new BusinessException(
                        "Patient profile not found.");
                }
            }


            // -------------------------------------------------
            // DOCTOR AVAILABILITY
            // -------------------------------------------------

            var isAvailable =
                await _appointmentRepository
                    .IsDoctorAvailableAsync(
                        dto.DoctorId,
                        dto.AppointmentDate);

            if (!isAvailable)
            {
                throw new BusinessException(
                    "Doctor already has an appointment at this time.");
            }


            // -------------------------------------------------
            // CREATE
            // -------------------------------------------------

            var appointment =
                _mapper.Map<Appointment>(dto);

            appointment.PatientId =
                patient.Id;

            appointment.Status =
                AppointmentStatus.Pending;


            await _appointmentRepository
                .AddAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();


            // -------------------------------------------------
            // EMAIL
            // -------------------------------------------------

            var body =
                EmailTemplateService.AppointmentBooked(
                    patient.FullName,
                    doctor.FullName,
                    doctor.Department?.Name,
                    appointment.AppointmentDate,
                    appointment.Status.ToString());


            if (patient.User != null)
            {
                await _emailService.SendEmailAsync(
                    patient.User.Email,
                    "Appointment Booked Successfully",
                    body);
            }


            // -------------------------------------------------
            // GET CREATED APPOINTMENT
            // -------------------------------------------------

            appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointment.Id);

            return _mapper.Map<
                AppointmentDto>(
                    appointment);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        public async Task UpdateAsync(
            int id,
            UpdateAppointmentDto dto,
            int userId,
            string role)
        {
            if (dto.AppointmentDate < DateTime.Now)
            {
                throw new BusinessException(
                    "Appointment date cannot be in the past.");
            }


            if (!IsValidAppointmentSlot(
                dto.AppointmentDate))
            {
                throw new BusinessException(
                    "Appointments can only be booked in 30-minute intervals.");
            }


            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(id);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            // -------------------------------------------------
            // DOCTOR
            // -------------------------------------------------

            var selectedDoctor =
                await _doctorRepository
                    .GetByIdAsync(
                        dto.DoctorId);

            if (selectedDoctor == null)
            {
                throw new BusinessException(
                    "Doctor not found.");
            }


            bool isWorking =
                IsDoctorWorking(
                    dto.AppointmentDate,
                    selectedDoctor.AvailableFrom,
                    selectedDoctor.AvailableTo);

            if (!isWorking)
            {
                throw new BusinessException(
                    $"Doctor is available only between {selectedDoctor.AvailableFrom} and {selectedDoctor.AvailableTo}");
            }


            var onLeave =
                await _doctorLeaveRepository
                    .IsDoctorOnLeaveAsync(
                        selectedDoctor.Id,
                        dto.AppointmentDate);

            if (onLeave)
            {
                throw new BusinessException(
                    "Doctor is on approved leave.");
            }


            // -------------------------------------------------
            // PATIENT
            // -------------------------------------------------

            var patient =
                await _patientRepository
                    .GetByIdAsync(
                        dto.PatientId);

            if (patient == null)
            {
                throw new BusinessException(
                    "Patient not found.");
            }


            // -------------------------------------------------
            // DOCTOR AUTHORIZATION
            // -------------------------------------------------

            if (role == "Doctor")
            {
                var loggedInDoctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (loggedInDoctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId !=
                    loggedInDoctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized to update this appointment.");
                }
            }


            // -------------------------------------------------
            // AVAILABILITY
            // -------------------------------------------------

            var isAvailable =
                await _appointmentRepository
                    .IsDoctorAvailableForUpdateAsync(
                        id,
                        dto.DoctorId,
                        dto.AppointmentDate);

            if (!isAvailable)
            {
                throw new BusinessException(
                    "Doctor already has an appointment at this time.");
            }


            // -------------------------------------------------
            // UPDATE
            // -------------------------------------------------

            _mapper.Map(
                dto,
                appointment);

            await _appointmentRepository
                .UpdateAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();
        }


        // =====================================================
        // DELETE
        // =====================================================

        public async Task DeleteAsync(
            int id,
            int userId,
            string role)
        {
            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(id);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId != doctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized to delete this appointment.");
                }
            }


            await _appointmentRepository
                .DeleteAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();
        }


        // =====================================================
        // APPROVE
        // =====================================================

        public async Task ApproveAppointmentAsync(
            int appointmentId,
            int userId,
            string role)
        {
            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointmentId);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId != doctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized.");
                }
            }


            if (appointment.Status !=
                AppointmentStatus.Pending)
            {
                throw new BusinessException(
                    "Only pending appointments can be approved.");
            }


            appointment.Status =
                AppointmentStatus.Approved;


            await _appointmentRepository
                .UpdateAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();


            appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointment.Id);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            if (appointment.Patient?.User != null &&
                appointment.Doctor != null)
            {
                await _emailService.SendEmailAsync(
                    appointment.Patient.User.Email,
                    "Appointment Approved",
                    EmailTemplateService.AppointmentApproved(
                        appointment.Patient.FullName,
                        appointment.Doctor.FullName,
                        appointment.AppointmentDate));
            }
        }


        // =====================================================
        // COMPLETE
        // =====================================================

        public async Task CompleteAppointmentAsync(
            int appointmentId,
            int userId,
            string role)
        {
            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointmentId);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId != doctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized.");
                }
            }


            if (appointment.Status !=
                AppointmentStatus.Approved)
            {
                throw new BusinessException(
                    "Only approved appointments can be completed.");
            }


            appointment.Status =
                AppointmentStatus.Completed;


            await _appointmentRepository
                .UpdateAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();


            appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointment.Id);

            if (appointment == null ||
                appointment.Patient == null ||
                appointment.Patient.User == null ||
                appointment.Doctor == null)
            {
                throw new BusinessException(
                    "Appointment data is incomplete.");
            }


            await _emailService.SendEmailAsync(
                appointment.Patient.User.Email,
                "Appointment Completed",
                EmailTemplateService.AppointmentCompleted(
                    appointment.Patient.FullName,
                    appointment.Doctor.FullName,
                    appointment.AppointmentDate));
        }


        // =====================================================
        // CANCEL
        // =====================================================

        public async Task CancelAppointmentAsync(
            int appointmentId,
            int userId,
            string role)
        {
            var appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointmentId);

            if (appointment == null)
            {
                throw new BusinessException(
                    "Appointment not found.");
            }


            // -------------------------------------------------
            // DOCTOR
            // -------------------------------------------------

            if (role == "Doctor")
            {
                var doctor =
                    await _doctorRepository
                        .GetByUserIdAsync(userId);

                if (doctor == null)
                {
                    throw new BusinessException(
                        "Doctor profile not found.");
                }

                if (appointment.DoctorId !=
                    doctor.Id)
                {
                    throw new BusinessException(
                        "You are not authorized.");
                }
            }


            // -------------------------------------------------
            // PATIENT
            // -------------------------------------------------

            if (role == "Patient")
            {
                var patient =
                    await _patientRepository
                        .GetByUserIdAsync(userId);

                if (patient == null)
                {
                    throw new BusinessException(
                        "Patient profile not found.");
                }

                if (appointment.PatientId !=
                    patient.Id)
                {
                    throw new BusinessException(
                        "You are not authorized to cancel this appointment.");
                }
            }


            // -------------------------------------------------
            // STATUS
            // -------------------------------------------------

            if (appointment.Status ==
                AppointmentStatus.Completed)
            {
                throw new BusinessException(
                    "Completed appointments cannot be cancelled.");
            }


            if (appointment.Status ==
                AppointmentStatus.Cancelled)
            {
                throw new BusinessException(
                    "Appointment is already cancelled.");
            }


            appointment.Status =
                AppointmentStatus.Cancelled;


            await _appointmentRepository
                .UpdateAsync(appointment);

            await _appointmentRepository
                .SaveChangesAsync();


            appointment =
                await _appointmentRepository
                    .GetByIdAsync(
                        appointment.Id);

            if (appointment == null ||
                appointment.Patient == null ||
                appointment.Patient.User == null ||
                appointment.Doctor == null)
            {
                throw new BusinessException(
                    "Appointment data is incomplete.");
            }


            await _emailService.SendEmailAsync(
                appointment.Patient.User.Email,
                "Appointment Cancelled",
                EmailTemplateService.AppointmentCancelled(
                    appointment.Patient.FullName,
                    appointment.Doctor.FullName,
                    appointment.AppointmentDate));
        }


        // =====================================================
        // DOCTOR WORKING HOURS
        // =====================================================

        private bool IsDoctorWorking(
            DateTime appointmentDate,
            string availableFrom,
            string availableTo)
        {
            TimeSpan appointmentTime =
                appointmentDate.TimeOfDay;

            TimeSpan from =
                TimeSpan.Parse(
                    availableFrom);

            TimeSpan to =
                TimeSpan.Parse(
                    availableTo);

            return appointmentTime >= from &&
                   appointmentTime <= to;
        }


        // =====================================================
        // APPOINTMENT SLOT
        // =====================================================

        private bool IsValidAppointmentSlot(
            DateTime appointmentDate)
        {
            return
                (appointmentDate.Minute == 0 ||
                 appointmentDate.Minute == 30)
                &&
                appointmentDate.Second == 0
                &&
                appointmentDate.Millisecond == 0;
        }
    }
}