namespace HospitalAPI.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }

        public int TotalDepartments { get; set; }

        public int TotalDoctors { get; set; }

        public int TotalPatients { get; set; }

        public int TotalAppointments { get; set; }

        public int TodayAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int ApprovedAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int DoctorsOnLeaveToday { get; set; }

        public int TotalPrescriptions { get; set; }
    }
}