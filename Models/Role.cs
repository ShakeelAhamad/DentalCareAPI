using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;//Admin,Doctor,Patient
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt {  get; set; } = DateTime.UtcNow;
        //Navigation property
        public ICollection<User> Users { get; set; } = new List <User>();
    }
}
