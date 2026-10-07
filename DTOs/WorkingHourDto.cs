using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class WorkingHourDto
    {
        public int DentistId { get; set; }
        [Required(ErrorMessage = "Day of week is required")]
        public string DayOfWeek { get; set; } = string.Empty;
        [Required(ErrorMessage = "Start time is required")]
        public TimeOnly StartTime { get; set; }
        [Required(ErrorMessage = "End time is required")]
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
