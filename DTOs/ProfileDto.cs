namespace DentalCareAPI.DTOs
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public int DentistId { get; set; }
        public int PatientId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool Status { get; set; }
        public string joinedDate { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DentistProfileDto? Dentist { get; set; }
        public PatientProfileDto? Patient { get; set; }

    }
}
