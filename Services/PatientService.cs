using DentalCareAPI.Constants;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using DentalCareAPI.Repositories;
using Microsoft.EntityFrameworkCore;

using System.Globalization;
namespace DentalCareAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IRoleRepository _roleRepository;
        public PatientService(
            IPatientRepository patientRepository, 
            IConfiguration configuration, 
            IUserRepository userRepository,
            IAppointmentRepository appointmentRepository,
            ITreatmentRepository treatmentRepository,
            IRoleRepository roleRepository
        )
        {
            _patientRepository = patientRepository;
            _configuration = configuration;
            _userRepository = userRepository;
            _appointmentRepository = appointmentRepository;
            _treatmentRepository = treatmentRepository;
            _roleRepository = roleRepository;
        }

        public async Task<PatientResponseDto> CreatePatientAsync(PatientDto patientDto)
        {
            var email = patientDto.Email.Trim().ToLower();
            var username = patientDto.Username.Trim().ToLower();
            //Check user name
            if (await _userRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException("User with this username already exists");
            }

            //Check email
            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var roleName = !string.IsNullOrEmpty(patientDto.Role) ? patientDto.Role : RoleConstants.Patient;
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Default user role not found. Please ensure roles are seeded");
            }

            // Use DateOfBirth directly because DTO already contains a DateTime
            var dateOfBirth = patientDto.DateOfBirth;

            // Optional: validate it's not the default value
            if (dateOfBirth == default(DateTime))
            {
                throw new InvalidOperationException("Date of birth must be provided.");
            }

            //Has the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(patientDto.Password);

            //Create New User
            var user = new User
            {
                Username = patientDto.Username,
                Email = patientDto.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                Status = true,
                CreatedAt = DateTime.UtcNow,
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Check User ID
            if (user.Id <= 0)
            {
                throw new InvalidOperationException("User was created but User ID was not generated.");
            }
            //get user with role name by user id
            var userWithRole = await _userRepository.GetUserWithRoleAsync(user.Id);

            //Create New Patient
            var patient = new Patient
            {
                UserId = user.Id,
                FullName = patientDto.FullName,
                PhoneNumber = patientDto.PhoneNumber,
                DateOfBirth = dateOfBirth,
                Gender = patientDto.Gender,
                Address = patientDto.Address,
                ProfileImage = patientDto.ProfileImage,
                Status = patientDto.Status,
                CreatedAt = DateTime.UtcNow,
            };

            try
            {
                await _patientRepository.AddAsync(patient);
                await _patientRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Patient creation failed: {ex.InnerException?.Message ?? ex.Message}", ex);
            }

            //Return response dto
            return new PatientResponseDto
            {
                Id = userWithRole?.Patient?.Id ?? 0,
                FullName = userWithRole?.Patient?.FullName ?? "",
                Username = userWithRole?.Username ?? "",
                Email = userWithRole?.Email ?? "",
                PhoneNumber = userWithRole?.Patient?.PhoneNumber ?? "",
                DateOfBirth = userWithRole?.Patient?.DateOfBirth ?? DateTime.MinValue,
                Gender = userWithRole?.Patient?.Gender ?? "",
                Address = userWithRole?.Patient?.Address ?? "",
                ProfileImage = userWithRole?.Patient?.ProfileImage ?? "",
                Status = userWithRole?.Patient?.Status ?? true,
                Role = userWithRole?.Role?.Name ?? "",
                CreatedAt = userWithRole?.Patient?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = userWithRole?.Patient?.UpdatedAt ?? DateTime.UtcNow
            };

        }

        public async Task<PaginatedResponseDto<PatientListResponseDto>> GetAllPatientsAsync(int pageNumber = 1,int? pageSize = null)
        {

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = _patientRepository.GetAllPatientsAsync();
            // Total records before pagination
            var totalRecords = await query.CountAsync();
            var allPatients = await query.ToListAsync();
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                allPatients = await query.ToListAsync();
                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                 allPatients = await query
                   .Skip((pageNumber - 1) * pageSize.Value)
                   .Take(pageSize.Value)
                   .ToListAsync();
            }

            var totalPages =
              pageSize > 0
                  ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                  : 0;
            return new PaginatedResponseDto<PatientListResponseDto>
            {
                PageNumber   = pageNumber,
                PageSize     = pageSize.Value,
                TotalRecords = totalRecords,
                TotalPages   = totalPages,
                Data         = allPatients
            };


        }

        public async Task<PatientDetailsResponseDto> GetPatientByIdAsync(int patientId)
        {
            var patient = await _patientRepository.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
            }
            var Appointments = _appointmentRepository.GetAppointmentByPatientId(patientId);
            var Treatments   = _treatmentRepository.GetTreatmentByPatientIdAsync(patientId);
            // Execute queries
            var appointmentList = await Appointments.ToListAsync();
            var treatmentList = await Treatments.ToListAsync();
            return new PatientDetailsResponseDto
            {
                Id               = patient.Id,
                UserId           = patient.UserId,
                FullName         = patient.FullName,
                PhoneNumber      = patient.PhoneNumber,
                Email            = patient.User?.Email ?? "",
                DateOfBirth      = patient.DateOfBirth.ToString("MMMM dd, yyyy"),
                Gender           = patient.Gender,
                Address          = patient.Address,
                ProfileImage     = patient.ProfileImage,
                Status           = patient.Status,
                AppointmentCount = appointmentList.Count,
                Appointments     = appointmentList.Select(a => new AppointmentViewDto
                {
                    Id                     = a.Id,
                    appointmentDate        = a.AppointmentDate.ToString("dddd, MMMM dd, yyyy"),
                    bookedDate             = a.CreatedAt.ToString("MMM dd, yyyy"),
                    reason                 = a.Reason ?? "",
                    notes                  = a.Note ?? "",
                    startTime              = a.StartTime.ToString("h:mm tt"),
                    endTime                = a.EndTime.ToString("h:mm tt"),
                    patientName            = a.Patient?.FullName ?? "",
                    patientPhone           = a.Patient?.PhoneNumber ?? "",
                    patientEmail           = a.Patient?.User?.Email ?? "",
                    patientDob             = a.Patient?.DateOfBirth.ToString("MMMM dd, yyyy") ?? "",
                    patientGender          = a.Patient?.Gender ?? "",
                    patientAddress         = a.Patient?.Address ?? "",
                    dentistName            = a.Dentist?.FullName ?? "",
                    dentistPhone           = a.Dentist?.PhoneNumber ?? "",
                    dentistSpecialization  = a.Dentist?.Specialization ?? "",
                    dentistQualification   = a.Dentist?.Qualification ?? "",
                    serviceName            = a.Service?.Name ?? "",
                    servicePrice           = a.Service?.Price ?? 0,
                    serviceDuration        = a.Service?.Duration ?? 0,
                    status                 = a.Status
                    
                }).ToList() ?? new List<AppointmentViewDto>(),

                Treatments = treatmentList.Select(t => new TreatmentResponseDto
                {
                    Id                     = t.Id,
                    appointmentId          = t.AppointmentId,
                    AppointmentStatus      = t.Appointment?.Status ?? "",
                    PatientName            = t.Patient?.FullName ?? "",
                    PatientPhoneNumber     = t.Patient?.PhoneNumber ?? "",
                    DentistName            = t.Dentist?.FullName ?? "",
                    DentistPhoneNumber     = t.Dentist?.PhoneNumber ?? "",
                    DentistQualification   = t.Dentist?.Qualification ?? "",
                    DentistSpecialization  = t.Dentist?.Specialization ?? "",
                    TreatmentDate          = t.TreatmentDate.ToString("MMMM dd, yyyy"),
                    TreatmentDesc          = t.TreatmentDesc ?? "",
                    Diagnosis              = t.Diagnosis ?? "",
                }).ToList() ?? new List<TreatmentResponseDto>(),
                TreatmentCount = treatmentList.Count
            };
        }
    }
}
