using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class PatientDto
    {

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        
        //Optional : Allow role
        public string? Role {get; set;}
        public string PhoneNumber {get; set;} = string.Empty;
        public DateTime DateOfBirth {get; set;}
        public string Gender {get; set;} = string.Empty;
        public string Address { get; set;} = string.Empty;
        public string ProfileImage {get; set;} = string.Empty;
        public bool Status { get; set; }
    }

}
