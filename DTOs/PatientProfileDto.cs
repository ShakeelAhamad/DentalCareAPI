namespace DentalCareAPI.DTOs
{
    public class PatientProfileDto
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
    }
}
