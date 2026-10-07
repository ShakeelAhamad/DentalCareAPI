using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.Models
{
    public class Treatment
    {
        [Key]
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public int AppointmentId {  get; set; }
        public string TreatmentDesc{  get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Notes {  get; set; } = string.Empty;
        public DateTime TreatmentDate {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Appointment? Appointment { get; set; }
        public Patient? Patient { get; set; }
        public Dentist? Dentist { get; set; }

    }
}
