using HospitalAPI.Data;
using HospitalAPI.Interfaces;
using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Doctor)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByAppointmentIdAsync(int appointmentId)
        {
            return await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Patient)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Doctor)
                .FirstOrDefaultAsync(p => p.AppointmentId == appointmentId);
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}