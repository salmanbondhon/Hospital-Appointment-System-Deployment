using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Prescription>> GetPatientHistoryAsync(
            int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .Where(p => p.Appointment.PatientId == patientId)
                .OrderByDescending(
                    p => p.Appointment.AppointmentDate)
                .ToListAsync();
        }
    }
}