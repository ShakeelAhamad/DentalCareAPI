namespace DentalCareAPI.DTOs
{
    public class ServiceResponseDto
    {
        public int Id { get; set; }
        public string Name {get; set;} = string.Empty;
        public string Description {get; set;} = string.Empty;
        public int? Duration {get; set;}
        public double? Price {get; set;}
        public bool? Status;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
