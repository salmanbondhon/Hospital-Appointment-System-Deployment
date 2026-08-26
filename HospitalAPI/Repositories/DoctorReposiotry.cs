using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors

                .Include(d => d.Department)

                .Include(d => d.User)

                .ToListAsync();
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _context.Doctors

                .Include(d => d.Department)

                .Include(d => d.User)

                .FirstOrDefaultAsync(
                    d => d.Id == id);
        }


        // =====================================================
        // GET BY USER ID
        // =====================================================

        public async Task<Doctor?> GetByUserIdAsync(
            int userId)
        {
            return await _context.Doctors

                .Include(d => d.Department)

                .Include(d => d.User)

                .FirstOrDefaultAsync(
                    d => d.UserId == userId);
        }


        // =====================================================
        // ADD
        // =====================================================

        public async Task AddAsync(
            Doctor doctor)
        {
            await _context.Doctors
                .AddAsync(doctor);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        public Task UpdateAsync(
            Doctor doctor)
        {
            // Entity is already tracked
            // by Entity Framework Core.

            return Task.CompletedTask;
        }


        // =====================================================
        // DELETE
        // =====================================================

        public Task DeleteAsync(
            Doctor doctor)
        {
            _context.Doctors.Remove(doctor);

            return Task.CompletedTask;
        }


        // =====================================================
        // SAVE
        // =====================================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}