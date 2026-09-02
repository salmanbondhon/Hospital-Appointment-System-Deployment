using HospitalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        // =========================
        // DbSets
        // =========================

        public DbSet<Department> Departments { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<DoctorLeave> DoctorLeaves { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        // =========================
        // Model Configuration
        // =========================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================
            // Email Case Sensitive
            // =========================

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .UseCollation("SQL_Latin1_General_CP1_CS_AS");


            // =========================
            // Doctor Consultation Fee
            // =========================

            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(18, 2);


            // =========================
            // Patient - User
            // One-to-One
            // =========================

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // Doctor - User
            // One-to-One
            // =========================

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // =========================
            // User - Notification
            // One-to-Many
            // =========================

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Doctor Leave - Doctor
            // One-to-Many
            // =========================

            modelBuilder.Entity<DoctorLeave>()
                .HasOne(dl => dl.Doctor)
                .WithMany(d => d.DoctorLeaves)
                .HasForeignKey(dl => dl.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // Prescription - Appointment
            // One-to-One
            // =========================

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Appointment)
                .WithOne(a => a.Prescription)
                .HasForeignKey<Prescription>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // Medical Record - Appointment
            // One-to-One
            // =========================

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<MedicalRecord>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // Payment - Appointment
            // One-to-One
            // =========================

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Payment)
                .WithOne(p => p.Appointment)
                .HasForeignKey<Payment>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);


            // =========================
            // Payment Amount
            // =========================

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
        }
    }
}