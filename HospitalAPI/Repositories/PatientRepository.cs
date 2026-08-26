using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // GET ALL
        // =========================

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.User)
                .ToListAsync();
        }


        // =========================
        // GET BY ID
        // =========================

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        // =========================
        // GET BY USER ID
        // =========================

        public async Task<Patient?> GetByUserIdAsync(int userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(
                    p => p.UserId == userId);
        }


        // =========================
        // ADD
        // =========================

        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
        }


        // =========================
        // UPDATE
        // =========================

        public Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);

            return Task.CompletedTask;
        }


        // =========================
        // DELETE
        // =========================

        public Task DeleteAsync(Patient patient)
        {
            _context.Patients.Remove(patient);

            return Task.CompletedTask;
        }


        // =========================
        // SAVE
        // =========================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}