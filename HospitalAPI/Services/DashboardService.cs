using HospitalAPI.Data;
using HospitalAPI.DTOs;
using HospitalAPI.Enums;
using HospitalAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var today = DateTime.Today;

            return new AdminDashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),

                TotalDepartments = await _context.Departments.CountAsync(),

                TotalDoctors = await _context.Doctors.CountAsync(),

                TotalPatients = await _context.Patients.CountAsync(),

                TotalAppointments = await _context.Appointments.CountAsync(),

                TodayAppointments = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate.Date == today),

                PendingAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == Enums.AppointmentStatus.Pending),

                ApprovedAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == Enums.AppointmentStatus.Approved),

                CompletedAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == Enums.AppointmentStatus.Completed),

                CancelledAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == Enums.AppointmentStatus.Cancelled),

                DoctorsOnLeaveToday = await _context.DoctorLeaves
    .Where(l =>
        l.IsApproved &&
        l.StartDate.Date <= today &&
        l.EndDate.Date >= today)
    .Select(l => l.DoctorId)
    .Distinct()
    .CountAsync(),

                TotalPrescriptions = await _context.Prescriptions.CountAsync()
            };
        }

        public async Task<DoctorDashboardDto> GetDoctorDashboardAsync(int userId)
        {
            var today = DateTime.Today;

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor == null)
                throw new Exception("Doctor profile not found.");

            return new DoctorDashboardDto
            {
                TodayAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.AppointmentDate.Date == today),

                UpcomingAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.AppointmentDate.Date > today),

                PendingAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.Status == AppointmentStatus.Pending),

                CompletedAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.Status == AppointmentStatus.Completed),

                CancelledAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.DoctorId == doctor.Id &&
                        a.Status == AppointmentStatus.Cancelled),

                TotalPrescriptions = await _context.Prescriptions
                    .CountAsync(p =>
                        p.Appointment.DoctorId == doctor.Id),

                TotalLeaves = await _context.DoctorLeaves
                    .CountAsync(l =>
                        l.DoctorId == doctor.Id),

                ApprovedLeaves = await _context.DoctorLeaves
                    .CountAsync(l =>
                        l.DoctorId == doctor.Id &&
                        l.IsApproved),

                PendingLeaves = await _context.DoctorLeaves
                    .CountAsync(l =>
                        l.DoctorId == doctor.Id &&
                        !l.IsApproved)
            };
        }

        public async Task<PatientDashboardDto> GetPatientDashboardAsync(int userId)
        {
            var today = DateTime.Today;

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
                throw new Exception("Patient profile not found.");

            return new PatientDashboardDto
            {
                UpcomingAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.PatientId == patient.Id &&
                        a.AppointmentDate.Date > today),

                PendingAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.PatientId == patient.Id &&
                        a.Status == AppointmentStatus.Pending),

                CompletedAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.PatientId == patient.Id &&
                        a.Status == AppointmentStatus.Completed),

                CancelledAppointments = await _context.Appointments
                    .CountAsync(a =>
                        a.PatientId == patient.Id &&
                        a.Status == AppointmentStatus.Cancelled),

                TotalPrescriptions = await _context.Prescriptions
                    .CountAsync(p =>
                        p.Appointment.PatientId == patient.Id),

                LastVisit = await _context.Appointments
                    .Where(a =>
                        a.PatientId == patient.Id &&
                        a.Status == AppointmentStatus.Completed)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => (DateTime?)a.AppointmentDate)
                    .FirstOrDefaultAsync(),

                NextAppointment = await _context.Appointments
                    .Where(a =>
                        a.PatientId == patient.Id &&
                        a.AppointmentDate > DateTime.Now)
                    .OrderBy(a => a.AppointmentDate)
                    .Select(a => (DateTime?)a.AppointmentDate)
                    .FirstOrDefaultAsync()
            };
        }
    }
}