namespace DentalCareAPI.DTOs
{
    public class PatientResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender {  get; set; } = string.Empty;
        public string Address {  get; set; } = string.Empty;
        public string ProfileImage {  get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
