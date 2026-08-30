using HospitalAPI.Enums;

namespace HospitalAPI.Models
{
    public class Appointment
    {
        // =========================
        // Appointment ID
        // =========================

        public int Id { get; set; }


        // =========================
        // Doctor
        // =========================

        public int DoctorId { get; set; }

        public Doctor? Doctor { get; set; }


        // =========================
        // Patient
        // =========================

        public int PatientId { get; set; }

        public Patient? Patient { get; set; }


        // =========================
        // Prescription
        // =========================

        public Prescription? Prescription { get; set; }


        // =========================
        // Medical Record
        // =========================

        public MedicalRecord? MedicalRecord { get; set; }


        // =========================
        // Appointment Information
        // =========================

        public DateTime AppointmentDate { get; set; }

        public string ProblemDescription { get; set; } = string.Empty;


        // =========================
        // Payment
        // =========================

        public Payment? Payment { get; set; }


        // =========================
        // Status
        // =========================

        public AppointmentStatus Status { get; set; }
    }
}