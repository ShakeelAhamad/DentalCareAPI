using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class AvailableSlotsRequestDto
    {
        [Required(ErrorMessage = "Dentist is required")]
        public int DentistId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Service is required")]
        public int ServiceId { get; set; }
    }
}
