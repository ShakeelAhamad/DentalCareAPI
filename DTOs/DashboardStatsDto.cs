namespace DentalCareAPI.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalDentists { get; set; }
        public int TotalPatients { get; set; }
        public int TotalServices { get; set; }
        public int TotalAppointments { get; set; }

        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
    }
}
