namespace DentalCareAPI.DTOs
{
    public class TreatmentViewDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public int AppointmentId { get; set; }
        public string? PatientName { get; set; }
        public string? PatientEmail { get; set; }
        public string? PatientPhone { get; set; }
        public DateTime? PatientDob { get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddress { get; set; }
        public string? PatientProfileImage { get; set; }
        public string? AppointmentStatus { get; set; }
        public string? TreatmentDesc { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime? TreatmentDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
