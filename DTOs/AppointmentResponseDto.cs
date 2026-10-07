namespace DentalCareAPI.DTOs
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public int ServiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Note { get; set; }
    }
}
