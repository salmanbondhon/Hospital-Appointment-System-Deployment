using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<DoctorLeave> DoctorLeaves { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Doctor>()
     .HasOne(d => d.User)
     .WithOne(u => u.Doctor)
     .HasForeignKey<Doctor>(d => d.UserId)
     .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DoctorLeave>()
    .HasOne(dl => dl.Doctor)
    .WithMany(d => d.DoctorLeaves)
    .HasForeignKey(dl => dl.DoctorId)
    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Prescription>()
    .HasOne(p => p.Appointment)
    .WithOne(a => a.Prescription)
    .HasForeignKey<Prescription>(p => p.AppointmentId)
    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
