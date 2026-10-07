using DentalCareAPI.DTOs;

namespace DentalCareAPI.Services
{
    public interface IPatientService
    {
        Task<PatientResponseDto> CreatePatientAsync(PatientDto patientDto);
        Task<PaginatedResponseDto<PatientListResponseDto>> GetAllPatientsAsync(int pageNumber, int? pageSize = null);
        Task<PatientDetailsResponseDto> GetPatientByIdAsync(int patientId);

    }
}
