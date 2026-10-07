using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class AppointmentDto
    {
        public int PatientId { get; set; }
        [Required(ErrorMessage = "Dentist is required")]
        public int DentistId { get; set; }
        [Required(ErrorMessage = "Service is required")]
        public int ServiceId { get; set; }
        [Required(ErrorMessage = "Preferred date is required")]
        public DateTime AppointmentDate { get; set; }
        [Required(ErrorMessage = "Start time is required")]
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

    }
}
