using System.ComponentModel.DataAnnotations;

namespace DentalCareAPI.DTOs
{
    public class ServiceDto
    {
        [Required(ErrorMessage = "Service name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Duration is required")]
        public int? Duration { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public double? Price {  get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? Status { get; set; }
    }
}
