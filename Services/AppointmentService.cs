using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using DentalCareAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentalCareAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IWorkHourRepository _workHourRepository;
        private readonly IBlockedDateRepository _blockedDateRepository;
        private readonly IServicesRepository _servicesRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDentistRepository _dentistRepository;
        public AppointmentService(
            IWorkHourRepository workHourRepository,
            IBlockedDateRepository blockedDateRepository,
            IServicesRepository servicesRepository,
            IPatientRepository patientRepository,
            IDentistRepository dentistRepository,
            IAppointmentRepository appointmentRepository
            )
        {
            _workHourRepository = workHourRepository;
            _blockedDateRepository = blockedDateRepository;
            _servicesRepository = servicesRepository;
            _patientRepository = patientRepository;
            _dentistRepository = dentistRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<AvailableSlotsResponseDto> GetAvailableSlotsAsync(AvailableSlotsRequestDto request)
        {
            var date = DateOnly.FromDateTime(request.Date);
            // -----------------------------------
            // 1. Check blocked date
            // -----------------------------------
            var isBlocked = await _blockedDateRepository.ExistsAsync(request.DentistId, date);
            if (isBlocked)
            {
                return new AvailableSlotsResponseDto
                {
                    Slots = new List<AvailableSlotDto>(),
                    Message = "Dentist is not available on this date."
                };
            }

            // -----------------------------------
            // 2. Get day of week
            // -----------------------------------
            var dayOfWeek = date.DayOfWeek.ToString();

            // -----------------------------------
            // 3. Get working hours
            // -----------------------------------
            var workingHour = await _workHourRepository 
           .GetAllWorkHoursAsync(request.DentistId)
           .FirstOrDefaultAsync(x =>
               x.DentistId == request.DentistId &&
               x.DayOfWeek == dayOfWeek &&
               x.IsAvailable);

            if (workingHour == null)
            {
                return new AvailableSlotsResponseDto
                {
                    Slots = new List<AvailableSlotDto>(),
                    Message = $"Dentist does not work on {dayOfWeek}s."
                };
            }
            // -----------------------------------
            // 4. Get service
            // -----------------------------------

            var service = await _servicesRepository
                .GetByIdAsync(request.ServiceId);

            if (service == null)
            {
                return new AvailableSlotsResponseDto
                {
                    Slots = new List<AvailableSlotDto>(),
                    Message = "Service not found."
                };
            }

            var duration = service.Duration;
            if (!duration.HasValue || duration.Value <= 0)
            {
                return new AvailableSlotsResponseDto
                {
                    Slots = new List<AvailableSlotDto>(),
                    Message = "Invalid service duration."
                };
            }

            // -----------------------------------
            // 5. Get existing appointments
            // -----------------------------------
            var datetime = request.Date;
            var bookedAppointments =
                await _appointmentRepository
                    .GetAppointmentsByDentistAndDateAsync(request.DentistId, datetime);

            // -----------------------------------
            // 6. Create start and end DateTime
            // -----------------------------------

            var start = datetime.Add(workingHour.StartTime.ToTimeSpan());
            var end = datetime.Add(workingHour.EndTime.ToTimeSpan());
            var slots = new List<AvailableSlotDto>();

            // -----------------------------------
            // 7. Generate slots every 30 minutes
            // -----------------------------------

            while (start.AddMinutes(duration.Value).TimeOfDay <= end.TimeOfDay)
            {
                var slotEnd = start.AddMinutes(duration.Value);

                // -----------------------------------
                // 8. Skip past slots for today
                // -----------------------------------

                if (datetime == DateTime.Today && start < DateTime.Now)
                {
                    start = start.AddMinutes(30);
                    continue;
                }

                // -----------------------------------
                // 9. Check appointment collision
                // -----------------------------------

                var hasConflict = bookedAppointments.Any(appointment =>
                {
                    var appointmentStart =
                        datetime.Add(appointment.StartTime.ToTimeSpan());

                    var appointmentEnd =
                        datetime.Add(appointment.EndTime.ToTimeSpan());

                    return start < appointmentEnd &&
                           slotEnd > appointmentStart;
                });

                // -----------------------------------
                // 10. Add available slot
                // -----------------------------------

                if (!hasConflict)
                {
                    slots.Add(new AvailableSlotDto
                    {
                        Time = start.ToString("HH:mm"),

                        Label =
                            $"{start:h:mm tt} - {slotEnd:h:mm tt}"
                    });
                }

                // Move by 30 minutes
                start = start.AddMinutes(30);
            }

            return new AvailableSlotsResponseDto
            {
                Slots = slots,
                Message = slots.Count == 0
                    ? "No available slots."
                    : null
            };
        }

        public async Task<string> CancelAppointmentAsync(int appointmentId, int userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            int patientId = patient?.Id ?? 0;
            
            // Get appointment (await the task to get the Appointment instance)
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }

            // Check appointment belongs to logged-in patient
            if (appointment.PatientId != patientId)
            {
                throw new UnauthorizedAccessException("Unauthorized.");
            }

            // Check status
            if (appointment.Status == "completed" || appointment.Status == "cancelled")
            {
                throw new InvalidOperationException("This appointment cannot be cancelled.");
            }

            // Cancel appointment
            appointment.Status = "cancelled";
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return "Appointment cancelled successfully.";

        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentDto request,int userId)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient profile not found.");
            }
            int patientId = patient.Id;

            // Check dentist
            var dentist = await _dentistRepository.GetDentistByIdAsync(request.DentistId);

            if (dentist == null)
            {
                throw new InvalidOperationException("Dentist not found.");
            }

            // Check service
            var service = await _servicesRepository.GetServicesByIdAsync(request.ServiceId);

            if (service == null)
            {
                throw new InvalidOperationException("Service not found.");
            }

            // Don't allow past dates
            var appointmentDate = DateOnly.FromDateTime(request.AppointmentDate);
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (appointmentDate < today)
            {
                throw new InvalidOperationException("Appointment date must be today or a future date.");
            }

            // Calculate end time from service duration
            var startTime = request.StartTime;

            if (service.Duration == null || service.Duration <= 0)
            {
                throw new InvalidOperationException("Service duration is not configured.");
            }

            var endTime = startTime.AddMinutes(service.Duration.Value);

            // Check appointment conflict
            var conflict = await _appointmentRepository.HasConflictAsync(
                request.DentistId,
                appointmentDate,
                startTime,
                endTime
            );

            if (conflict)
            {
                throw new InvalidOperationException("The selected time slot is no longer available. Please choose another time.");
            }

            // Create appointment
            var appointment = new Appointment
            {
                PatientId = patientId,
                DentistId = request.DentistId,
                ServiceId = request.ServiceId,
                AppointmentDate = request.AppointmentDate,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Pending",
                Reason = request.Reason
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                DentistId = appointment.DentistId,
                ServiceId = appointment.ServiceId,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Reason = appointment.Reason
            };
        }

        public async Task<PaginatedResponseDto<AppointmentListDto>> GetAllAppointmentsAsync(int userId,string? status, int pageNumber = 1, int? pageSize = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            int? patientId = null;
            int? dentistId = null;
            var dentist = await _dentistRepository.GetDentistByIdAsync(userId);
            var patient = await _patientRepository.GetPatientByUserIdAsync(userId);
            //if (dentist == null || patient == null)
            //{
            //    throw new Exception("Dentist or patient not found."+ userId);
            //}
            dentistId = dentist?.Id;
            patientId = patient?.Id;

            var query = _appointmentRepository.GetAllAppointmentsAsync(patientId, dentistId, status);
            // Total records
            var totalRecords = await query.CountAsync();
            List<Appointment> appointments;
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                appointments = await query.ToListAsync();
                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                appointments = await query
                    .Skip((pageNumber - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }

            // Entity → DTO
            var AppointmentDtos = appointments.Select(s => new AppointmentListDto
            {
                Id                     = s.Id,
                appointmentDate        = s.AppointmentDate.ToString("MMM dd, yyyy") ?? "",
                startTime              = s.StartTime.ToString("h:mm tt"),
                endTime                = s.EndTime.ToString("h:mm tt"),
                patientName            = s.Patient?.FullName ?? "",
                patientPhone           = s.Patient?.PhoneNumber ?? "",
                patientDob             = s.Patient?.DateOfBirth.ToString("MMM dd, yyyy") ?? "",
                patientGender          = s.Patient?.Gender ?? "",
                patientAddress         = s.Patient?.Address ?? "",
                dentistName            = s.Dentist?.FullName ?? "",
                dentistPhone           = s.Dentist?.PhoneNumber ?? "",
                dentistSpecialization  = s.Dentist?.Specialization ?? "",
                dentistQualification   = s.Dentist?.Qualification ?? "",
                serviceName            = s.Service?.Name ?? "",
                servicePrice           = s.Service?.Price ?? 0,
                serviceDuration        = s.Service?.Duration ?? 0,
                status                 = s.Status
            }).ToList();

            var totalPages =
               pageSize > 0
                   ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                   : 0;

            return new PaginatedResponseDto<AppointmentListDto>
            {
                Data = AppointmentDtos,
                PageNumber = pageNumber,
                PageSize = pageSize ?? 0,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<AppointmentViewDto?> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return null;
            }
            return new AppointmentViewDto
            {
                Id = appointment.Id,
                appointmentDate = appointment.AppointmentDate.ToString("dddd, MMMM dd, yyyy"),
                bookedDate = appointment.CreatedAt.ToString("MMM dd, yyyy"),
                reason = appointment.Reason,
                notes = appointment.Note,
                startTime = appointment.StartTime.ToString("h:mm tt"),
                endTime = appointment.EndTime.ToString("h:mm tt"),
                patientName = appointment.Patient?.FullName ?? "",
                patientPhone = appointment.Patient?.PhoneNumber ?? "",
                patientEmail = appointment.Patient?.User?.Email ?? "",
                patientDob = appointment.Patient?.DateOfBirth.ToString("MMM dd, yyyy") ?? "",
                patientGender = appointment.Patient?.Gender ?? "",
                patientAddress = appointment.Patient?.Address ?? "",
                dentistName = appointment.Dentist?.FullName ?? "",
                dentistPhone = appointment.Dentist?.PhoneNumber ?? "",
                dentistSpecialization = appointment.Dentist?.Specialization ?? "",
                dentistQualification = appointment.Dentist?.Qualification ?? "",
                serviceName = appointment.Service?.Name ?? "",
                servicePrice = appointment.Service?.Price ?? 0,
                serviceDuration = appointment.Service?.Duration ?? 0,
                status = appointment.Status
            };
        }

        //Delete Appointment
        public async Task<bool> DeleteAnyAppointmentAsync(int Id)
        {
            //Find the Appointment by Id

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(Id);
            if (appointment == null)
            {
                return false; // Appointment not found
            }
            //Remove The Appointment
            //Uses base generic Remove() from Repository<Appointment>
            _appointmentRepository.Remove(appointment);
            //Uses base generic SaveChangesAsync() from Repository<Appointment>
            await _appointmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<string> UpdateAppointmentStatusAsync(int appointmentId, string status)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found.");
            }
            // Validate status
            var validStatuses = new List<string> { "Pending", "Confirmed", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                throw new ArgumentException("Invalid status value.");
            }
            appointment.Status = status;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return $"Appointment status updated to {status}.";
        }   
    }
}
 