using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }
        public int UserId {  get; set; }
        // Navigation Property
        public User? User { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth {  get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ProfileImage {  get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
