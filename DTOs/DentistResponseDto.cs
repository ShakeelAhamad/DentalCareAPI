namespace DentalCareAPI.DTOs
{
    public class DentistResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username {get; set;} = string.Empty;
        public int userId { get; set; }
        public string Email { get; set;} = string.Empty;
        public string PhoneNumber { get; set;} = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Specialization {  get; set; } = string.Empty;
        public string Experience {  get; set; } = string.Empty;
        public string ProfileImage {  get; set; } = string.Empty;
        public string Bio {  get; set; } = string.Empty;
        public bool Status { get; set; } = true;
        public string Role {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
