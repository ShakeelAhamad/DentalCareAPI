using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Duration {  get; set; }
        public double? Price { get; set; }
        public bool? Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt {  get; set; } = DateTime.UtcNow;
    }
}
