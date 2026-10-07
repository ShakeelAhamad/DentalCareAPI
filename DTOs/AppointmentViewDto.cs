namespace DentalCareAPI.DTOs
{
    public class AppointmentViewDto
    {
        public int Id { get; set; }
        public string appointmentDate { get; set; } = string.Empty;
        public string bookedDate { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
        public string notes { get; set; } = string.Empty;
        public string startTime { get; set; } = string.Empty;
        public string endTime { get; set; } = string.Empty;
        public string patientName { get; set; } = string.Empty;
        public string patientPhone { get; set; } = string.Empty;
        public string patientEmail { get; set; } = string.Empty;
        public string patientDob { get; set; } = string.Empty;
        public string patientGender { get; set; } = string.Empty;
        public string patientAddress { get; set; } = string.Empty;
        public string dentistName { get; set; } = string.Empty;
        public string dentistPhone { get; set; } = string.Empty;
        public string dentistSpecialization { get; set; } = string.Empty;
        public string dentistQualification { get; set; } = string.Empty;
        public string serviceName { get; set; } = string.Empty;
        public double servicePrice { get; set; }
        public int serviceDuration { get; set; }
        public string status { get; set; } = string.Empty;
    }
}
