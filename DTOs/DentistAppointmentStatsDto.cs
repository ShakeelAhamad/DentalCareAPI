namespace DentalCareAPI.DTOs
{
    public class DentistAppointmentStatsDto
    {
        public int Today { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Completed { get; set; }
        public int Total { get; set; }
    }
}
