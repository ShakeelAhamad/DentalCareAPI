using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class BlockedDate
    {
        [Key]
        public int Id { get; set; }
        public int DentistId { get; set; }
        public Dentist? Dentist { get; set; }
        public DateOnly BlockDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
