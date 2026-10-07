using DentalCareAPI.DTOs;

namespace DentalCareAPI.Services
{
    public interface IDentistService
    {
        Task<WorkingHourResponseDto> CreateWorkingHoursAsync(WorkingHourDto workingHourDto, int userId);
        Task<bool> DeleteAnyWorkingHourAsync(int Id);//Delete any Working Hour
        Task<PaginatedResponseDto<WorkingHourResponseDto>> GetAllWorkingHourAsync(int userId, int pageNumber = 1, int? pageSize = null);
        Task<TreatmentResponseDto> CreateTreatmentAsync(TreatmentDto treatmentDto, int userId);
        Task<PaginatedResponseDto<TreatmentResponseDto>> GetAllTreatmentsAsync(int userId, int pageNumber = 1, int? pageSize = null);
        Task<TreatmentResponseDto?> GetTreatmentByIdAsync(int treatmentId);
    }
}
   