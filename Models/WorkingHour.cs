using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class WorkingHour
    {
        [Key]
        public int  Id  { get; set; }
        public int DentistId { get; set; }
        // Navigation Property
        public Dentist? Dentist { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt {  get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
