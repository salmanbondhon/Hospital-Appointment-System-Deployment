using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class DoctorLeaveRepository : IDoctorLeaveRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorLeaveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorLeave>> GetAllAsync()
        {
            return await _context.DoctorLeaves
                .Include(d => d.Doctor)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorLeave>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorLeaves
                .Include(d => d.Doctor)
                .Where(d => d.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<DoctorLeave?> GetByIdAsync(int id)
        {
            return await _context.DoctorLeaves
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(DoctorLeave leave)
        {
            await _context.DoctorLeaves.AddAsync(leave);
        }

        public Task UpdateAsync(DoctorLeave leave)
        {
            _context.DoctorLeaves.Update(leave);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(DoctorLeave leave)
        {
            _context.DoctorLeaves.Remove(leave);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsDoctorOnLeaveAsync(int doctorId, DateTime appointmentDate)
        {
            return await _context.DoctorLeaves.AnyAsync(l =>
                l.DoctorId == doctorId &&
                l.IsApproved &&
                appointmentDate.Date >= l.StartDate.Date &&
                appointmentDate.Date <= l.EndDate.Date);
        }

        public async Task<bool> HasOverlappingLeaveAsync(
    int doctorId,
    DateTime startDate,
    DateTime endDate)
        {
            return await _context.DoctorLeaves.AnyAsync(l =>
                l.DoctorId == doctorId &&
                startDate <= l.EndDate &&
                endDate >= l.StartDate);
        }
    }
}
