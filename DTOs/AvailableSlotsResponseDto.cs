namespace DentalCareAPI.DTOs
{
    public class AvailableSlotsResponseDto
    {
        public List<AvailableSlotDto> Slots { get; set; } = new List<AvailableSlotDto>();
        public string? Message { get; set; }
    }
}
