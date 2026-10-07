using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class TreatmentDto
    {
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        [Required(ErrorMessage = "Appointment ID is required")]
        public int AppointmentId { get; set; }
        [Required(ErrorMessage = "Treatment date is required")]
        public DateTime TreatmentDate { get; set; }
        [Required(ErrorMessage = "Treatment description is required")]
        public string TreatmentDesc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Diagnosis is required")]
        public string Diagnosis { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
