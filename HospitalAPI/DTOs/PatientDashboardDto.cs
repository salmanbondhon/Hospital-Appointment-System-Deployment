namespace HospitalAPI.DTOs
{
    public class PatientDashboardDto
    {
        public int UpcomingAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }

        public int TotalPrescriptions { get; set; }

        public DateTime? LastVisit { get; set; }

        public DateTime? NextAppointment { get; set; }

    }


}
