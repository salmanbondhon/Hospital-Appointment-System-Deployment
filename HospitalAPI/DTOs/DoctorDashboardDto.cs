namespace HospitalAPI.DTOs
{
    public class DoctorDashboardDto
    {
        public int TodayAppointments { get; set; }

        public int UpcomingAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int TotalPrescriptions { get; set; }

        public int TotalLeaves { get; set; }

        public int ApprovedLeaves { get; set; }

        public int PendingLeaves { get; set; }
    }
}