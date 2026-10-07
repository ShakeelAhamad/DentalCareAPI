using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Dentist
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        // Navigation Property
        public User? User { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Qualification {  get; set; } = string.Empty;
        public string Specialization {  get; set; } = string.Empty;
        public string Experience {  get; set; } = string.Empty;
        public string ProfileImage {  get; set; } = string.Empty;
        public string Bio {  get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // 1 Dentist -> Many WorkingHours
        public ICollection<WorkingHour> WorkingHours { get; set; }
            = new List<WorkingHour>();

    }
}
