namespace DentalCareAPI.DTOs
{
    public class TreatmentResponseDto
    {
        public int Id { get; set; }
        public int appointmentId { get; set; }
        public string AppointmentStatus { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhoneNumber { get; set; } = string.Empty;
        public string DentistName { get; set; } = string.Empty;
        public string DentistPhoneNumber { get; set; } = string.Empty;
        public string DentistQualification { get; set; } = string.Empty;
        public string DentistSpecialization { get; set; } = string.Empty;
        public string TreatmentDate { get; set; } = string.Empty;
        public string TreatmentDesc { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
    }
}
