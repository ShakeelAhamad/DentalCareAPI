using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using DentalCareAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Services
{
    public class DentistService : IDentistService
    {
        private readonly IWorkHourRepository _workHourRepository; 
        private readonly IDentistRepository _dentistRepository;
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IConfiguration _configuration; 

        public DentistService(IDentistRepository dentistRepository, ITreatmentRepository treatmentRepository,IWorkHourRepository workHourRepository, IConfiguration configuration)
        {
             _dentistRepository = dentistRepository;
             _workHourRepository = workHourRepository;
            _treatmentRepository = treatmentRepository;
            _configuration = configuration;
        } 

        public async Task<WorkingHourResponseDto> CreateWorkingHoursAsync(WorkingHourDto workHourDto, int userId)
        {
            var dentist = await _dentistRepository.GetDentistByIdAsync(userId);
            if(dentist == null)
            {
                throw new Exception("Dentist not found.");
            }
            int DentistId = dentist.Id;
            //Check Working Hours Allready Exit than update record or Not Exit than create new record
            var workingHourExit = await _workHourRepository.GetWorkingHourByDentistIdAndDayOfWeekAsync(DentistId, workHourDto.DayOfWeek);
            if (workingHourExit!= null)
            {
                workingHourExit.DentistId = DentistId;
                workingHourExit.DayOfWeek = workHourDto.DayOfWeek;
                workingHourExit.StartTime = workHourDto.StartTime;
                workingHourExit.EndTime   = workHourDto.EndTime;
                workingHourExit.IsAvailable = true;
                workingHourExit.UpdatedAt = DateTime.UtcNow;
                 _workHourRepository.Update(workingHourExit);
                await _workHourRepository.SaveChangesAsync();
            }
            else
            {
                var workingHour = new WorkingHour
                {
                    DentistId = DentistId,
                    DayOfWeek = workHourDto.DayOfWeek,
                    StartTime = workHourDto.StartTime,
                    EndTime = workHourDto.EndTime,
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _workHourRepository.AddAsync(workingHour);
                await _workHourRepository.SaveChangesAsync();
            }
            var working = await _workHourRepository.GetWorkingHourByDentistIdAndDayOfWeekAsync(DentistId, workHourDto.DayOfWeek);
            return new WorkingHourResponseDto
            {
                Id          = working!.Id ,
                DentistId   = working.DentistId,
                DentistName = working.Dentist?.FullName ?? "",
                Specialization = working.Dentist?.Specialization ?? "",
                DayOfWeek   = working.DayOfWeek ?? "",
                StartTime   = working.StartTime.ToString("h:mm tt"),
                EndTime     = working.EndTime.ToString("h:mm tt"),
                IsAvailable = true,
                CreatedAt   = working?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt   = working?.UpdatedAt ?? DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAnyWorkingHourAsync(int Id)
        {
            var workingHour = await _workHourRepository.GetWorkingHourByIdAsync(Id);
            if(workingHour == null)
            {
                return false;
            }

            _workHourRepository.Remove(workingHour);
            await _workHourRepository.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResponseDto<WorkingHourResponseDto>> GetAllWorkingHourAsync(int userId, int pageNumber = 1, int? pageSize = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            } 
          
            var dentist = await _dentistRepository.GetDentistByIdAsync(userId);
            if (dentist == null)
            {
                throw new Exception("Dentist not found.");
            }
            int dentistId = dentist.Id;
                

            var query =  _workHourRepository.GetAllWorkHoursAsync(dentistId);
            // Total records
            var totalRecords = await query.CountAsync();
            List<WorkingHour> workings;
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                workings = await query.ToListAsync();
                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                workings = await query
                    .Skip((pageNumber - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }

            // Entity → DTO
            var workingHourDtos = workings.Select(s => new WorkingHourResponseDto
            {
                Id             = s.Id,
                DentistId      = s.DentistId,
                DentistName    = s.Dentist?.FullName ?? "",
                Specialization = s.Dentist?.Specialization ?? "",
                DayOfWeek      = s.DayOfWeek ?? "",
                StartTime      = s.StartTime.ToString("h:mm tt"),
                EndTime        = s.EndTime.ToString("h:mm tt"),
                IsAvailable    = s.IsAvailable,
                CreatedAt      = s?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt      = s?.UpdatedAt ?? DateTime.UtcNow
            }).ToList();

            var totalPages =
               pageSize > 0
                   ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                   : 0;

            return new PaginatedResponseDto<WorkingHourResponseDto>
            {
                Data = workingHourDtos,
                PageNumber = pageNumber,
                PageSize = pageSize ?? 0,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        //Create Treatment
        public async Task<TreatmentResponseDto> CreateTreatmentAsync(TreatmentDto treatmentDto, int userId)
        {
            var dentist = await _dentistRepository.GetDentistByIdAsync(userId);
            if (dentist == null)
            {
                throw new Exception("Dentist not found.");
            }
            int DentistId = dentist.Id;
            var treatment = new Treatment
            {
                PatientId     = treatmentDto.PatientId,
                DentistId     = DentistId,
                AppointmentId = treatmentDto.AppointmentId,
                TreatmentDate = treatmentDto.TreatmentDate,
                TreatmentDesc = treatmentDto.TreatmentDesc,
                Diagnosis     = treatmentDto.Diagnosis,
                Notes         = treatmentDto.Notes,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow
            };
            await _treatmentRepository.AddAsync(treatment);
            await _treatmentRepository.SaveChangesAsync();
            var createdTreatment = await _treatmentRepository.GetTreatmentByIdAsync(treatment.Id);
            return new TreatmentResponseDto
            {
                Id                     = createdTreatment!.Id,
                appointmentId          = createdTreatment.AppointmentId,
                AppointmentStatus      = createdTreatment?.Appointment?.Status ?? "",
                PatientName            = createdTreatment?.Patient?.FullName ?? "",
                PatientPhoneNumber     = createdTreatment?.Patient?.PhoneNumber ?? "",
                DentistName            = createdTreatment?.Dentist?.FullName ?? "",
                DentistPhoneNumber     = createdTreatment?.Dentist?.PhoneNumber ?? "",
                DentistQualification   = createdTreatment?.Dentist?.Qualification ?? "",
                DentistSpecialization  = createdTreatment?.Dentist?.Specialization ?? "",
                TreatmentDate          = createdTreatment!.TreatmentDate.ToString("MMMM dd, yyyy"),
                TreatmentDesc          = createdTreatment.TreatmentDesc,
                Diagnosis              = createdTreatment.Diagnosis
            };
        }

        //Get All Treatment By Dentist Id
        public async Task<PaginatedResponseDto<TreatmentResponseDto>> GetAllTreatmentsAsync(int userId, int pageNumber = 1, int? pageSize = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var dentist = await _dentistRepository.GetDentistByIdAsync(userId);
            if (dentist == null)
            {
                throw new Exception("Dentist not found.");
            }
            int dentistId = dentist.Id;
            var query = _treatmentRepository.GetAllTreatmentsAsync(dentistId);
           
            // Total records
            var totalRecords = await query.CountAsync();
            List<Treatment> treatments;
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                treatments = await query.ToListAsync();
                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                treatments = await query
                    .Skip((pageNumber - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }
            // Entity → DTO
            var treatmentDtos = treatments.Select(s => new TreatmentResponseDto
            {
                Id                     = s.Id,
                appointmentId          = s.AppointmentId,
                AppointmentStatus      = s.Appointment?.Status ?? "",
                PatientName            = s?.Patient?.FullName ?? "",
                PatientPhoneNumber     = s?.Patient?.PhoneNumber ?? "",
                DentistName            = s?.Dentist?.FullName ?? "",
                DentistPhoneNumber     = s?.Dentist?.PhoneNumber ?? "",
                DentistQualification   = s?.Dentist?.Qualification ?? "",
                DentistSpecialization  = s?.Dentist?.Specialization ?? "",
                TreatmentDate          = s!.TreatmentDate.ToString("MMMM dd, yyyy"),
                TreatmentDesc          = s.TreatmentDesc,
                Diagnosis              = s.Diagnosis
            }).ToList();
            var totalPages =
               pageSize > 0
                   ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                   : 0;
            return new PaginatedResponseDto<TreatmentResponseDto>
            {
                Data = treatmentDtos,
                PageNumber = pageNumber,
                PageSize = pageSize ?? 0,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }
        //Get Treatment By Id
        public async Task<TreatmentResponseDto?> GetTreatmentByIdAsync(int treatmentId)
        {
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(treatmentId);
            if (treatment == null)
            {
                return null;
            }
            return new TreatmentResponseDto
            {
                Id                     = treatment.Id,
                appointmentId          = treatment.AppointmentId,
                AppointmentStatus      = treatment?.Appointment?.Status ?? "",
                PatientName            = treatment?.Patient?.FullName ?? "",
                PatientPhoneNumber     = treatment?.Patient?.PhoneNumber ?? "",
                DentistName            = treatment?.Dentist?.FullName ?? "",
                DentistPhoneNumber     = treatment?.Dentist?.PhoneNumber ?? "",
                DentistQualification   = treatment?.Dentist?.Qualification ?? "",
                DentistSpecialization  = treatment?.Dentist?.Specialization ?? "",
                TreatmentDate          = treatment!.TreatmentDate.ToString("MMMM dd, yyyy"),
                TreatmentDesc          = treatment.TreatmentDesc,
                Diagnosis              = treatment.Diagnosis
            };
        }

    }
}
