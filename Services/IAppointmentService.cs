using DentalCareAPI.DTOs;

namespace DentalCareAPI.Services
{
    public interface IAppointmentService
    {
        Task<AvailableSlotsResponseDto> GetAvailableSlotsAsync(AvailableSlotsRequestDto request);
        Task<string> CancelAppointmentAsync(int appointmentId, int userId);
        Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentDto request, int patientId);
        Task<PaginatedResponseDto<AppointmentListDto>> GetAllAppointmentsAsync(int userId,string? status, int pageNumber = 1, int? pageSize = null);
        Task<AppointmentViewDto?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> DeleteAnyAppointmentAsync(int Id);//Delete any appointment
        Task<string> UpdateAppointmentStatusAsync(int appointmentId, string status);
    }
}
