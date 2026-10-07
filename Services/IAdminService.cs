using DentalCareAPI.DTOs;
using DentalCareAPI.Models;

namespace DentalCareAPI.Services
{
    public interface IAdminService
    {
        Task<DentistResponseDto> CreateDentistAsync(DentistDto dentistDto);
        Task<DentistResponseDto?> UpdateDentistAsync(int userId , UpdateDentistDto updateDentist);
        Task<PaginatedResponseDto<DentistResponseDto>> GetAllDentistsAsync(int pageNumber = 1,int? pageSize = null);
        Task<bool> DeleteAnyDentistAsync(int userId);//Delete any Dentists
        Task<ServiceResponseDto> CreateServiceAsync(ServiceDto serviceDto);
        Task<ServiceResponseDto?> UpdateServiceAsync(int Id,ServiceDto serviceDto);
        Task<bool> DeleteAnyServiceAsync(int Id);//Delete any Service
        Task<PaginatedResponseDto<ServiceResponseDto>> GetAllServiceAsync(int pageNumber = 1, int? pageSize = null);
    }
}
