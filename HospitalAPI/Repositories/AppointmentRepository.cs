using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HospitalAPI.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentDate)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate);
        }


        public async Task<bool> IsDoctorAvailableForUpdateAsync(
    int appointmentId,
    int doctorId,
    DateTime appointmentDate)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.Id != appointmentId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate);
        }
    }
}