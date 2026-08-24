using HospitalAPI.Data;
using HospitalAPI.Enums;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FindAsync(id);
        }


        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        // =========================
        // AVAILABLE DOCTOR USERS
        // =========================

        public async Task<IEnumerable<User>> GetAvailableDoctorUsersAsync()
        {
            return await _context.Users

                .Include(u => u.Doctor)

                .Where(u =>
                    u.Role == UserRole.Doctor &&
                    u.Doctor == null)

                .ToListAsync();
        }


        // =========================
        // AVAILABLE PATIENT USERS
        // =========================

        public async Task<IEnumerable<User>> GetAvailablePatientUsersAsync()
        {
            return await _context.Users

                .Include(u => u.Patient)

                .Where(u =>
                    u.Role == UserRole.Patient &&
                    u.Patient == null)

                .ToListAsync();
        }
    }
}