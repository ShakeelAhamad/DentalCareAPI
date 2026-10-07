namespace DentalCareAPI.DTOs
{
    public class PatientDetailsResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public string DateOfBirth { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Address { get; set; } = "";
        public string ProfileImage { get; set; } = "";
        public bool Status { get; set; }

        public int AppointmentCount { get; set; }
        public List<AppointmentViewDto> Appointments { get; set; } = new();

        public int TreatmentCount { get; set; }
        public List<TreatmentResponseDto> Treatments { get; set; } = new();
    }
}
