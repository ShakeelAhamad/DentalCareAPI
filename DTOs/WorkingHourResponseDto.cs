using DentalCareAPI.Models;

namespace DentalCareAPI.DTOs
{
    public class WorkingHourResponseDto
    {
        public int Id { get; set; }
        public int DentistId { get; set; }
        // Navigation Property
        public string DentistName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        //public TimeOnly StartTime { get; set; }
        //public TimeOnly EndTime { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
