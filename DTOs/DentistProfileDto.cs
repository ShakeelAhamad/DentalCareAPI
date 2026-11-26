namespace DentalCareAPI.DTOs
{
    public class DentistProfileDto
    {
        public int DentistId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }
}
