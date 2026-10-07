using System.ComponentModel.DataAnnotations;
using System.Data;

namespace DentalCareAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username  { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        //Role relationship
        public int RoleId {  get; set; } //Forgen Key
        public Role Role { get; set; } = null!;
        public bool Status { get; set; } = false;
        public DateTime CreatedAt {  get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }   = DateTime.UtcNow;

        //Navigation property
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        // Dentist relationship
        public Dentist? Dentist { get; set; }
        // Patient relationship
        public Patient? Patient { get; set; }
    }
}
