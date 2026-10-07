using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; } 
        public int DentistId { get; set; }
        public int ServiceId { get; set; }
        public Patient? Patient { get; set; }
        public Dentist? Dentist { get; set; }
        public Service? Service { get; set; }

        public DateTime AppointmentDate { get; set; }   
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
