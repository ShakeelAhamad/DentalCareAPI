using DentalCareAPI.Constants;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using DentalCareAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDentistRepository _dentistRepository;
        private readonly IServicesRepository _servicesRepository;
        private readonly IConfiguration _configuration;
        public AdminService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IDentistRepository dentistRepository,
            IServicesRepository servicesRepository,
            IConfiguration configuration
        )
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _dentistRepository = dentistRepository;
            _servicesRepository = servicesRepository;
            _configuration = configuration;
        }

        public async Task<DentistResponseDto> CreateDentistAsync(DentistDto dentistDto)
        {
            var email = dentistDto.Email.Trim().ToLower();
            var username = dentistDto.Username.Trim().ToLower();
            //Check user name
            if(await _userRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException("User with this username already exists");
            }

            //Check email
            if(await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var roleName = !string.IsNullOrEmpty(dentistDto.Role) ? dentistDto.Role : RoleConstants.Dentist;
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Default user role not found. Please ensure roles are seeded");
            }
            //Has the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dentistDto.Password);
            //Create New User
            var user = new User
            {
                Username = dentistDto.Username,
                Email = dentistDto.Email,
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

            //Create Dentist
            var dentist = new Dentist
            {
                UserId = user.Id,
                FullName = dentistDto.FullName,
                PhoneNumber = dentistDto.PhoneNumber,
                Qualification = dentistDto.Qualification,
                Specialization = dentistDto.Specialization,
                Experience = dentistDto.Experience,
                ProfileImage = dentistDto.ProfileImage,
                Bio = dentistDto.Bio,
                Status = dentistDto.Status,
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _dentistRepository.AddAsync(dentist);
                await _dentistRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Dentist creation failed: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            //Return response dto
            return new DentistResponseDto
            {
                Id = userWithRole?.Dentist?.Id ?? 0,
                FullName = userWithRole?.Dentist?.FullName ?? "",
                Username = userWithRole?.Username ?? "",
                Email = userWithRole?.Email ?? "",
                PhoneNumber = userWithRole?.Dentist?.PhoneNumber ?? "",
                Qualification = userWithRole?.Dentist?.Qualification ?? "",
                Specialization = userWithRole?.Dentist?.Specialization ?? "",
                Experience = userWithRole?.Dentist?.Experience ?? "",
                ProfileImage = userWithRole?.Dentist?.ProfileImage ?? "",
                Bio = userWithRole?.Dentist?.Bio ?? "",
                Status = userWithRole?.Dentist?.Status ?? true,
                Role = userWithRole?.Role?.Name ?? "",
                CreatedAt = userWithRole?.Dentist?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = userWithRole?.Dentist?.UpdatedAt ?? DateTime.UtcNow 
            };
        }

        public async Task<DentistResponseDto?> UpdateDentistAsync(int userId, UpdateDentistDto updateDentist)
        {
            //Find the dentist by userId
            var dentist  = await _dentistRepository.GetDentistByUserIdAsync(userId);
            var user     = await _userRepository.GetUserByIdAsync(userId);
            if(user == null || dentist == null)
            {
                return null; // Dentist not found 
            }

            var roleName = !string.IsNullOrEmpty(updateDentist.Role) ? updateDentist.Role : RoleConstants.Dentist;
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Default user role not found. Please ensure roles are seeded");
            }
            //User Table Update Data 
            if (updateDentist.Username != null)
            {
                user.Username = updateDentist.Username;
            }
            if (updateDentist.Email != null)
            {
                user.Email = updateDentist.Email;
            }
            if (updateDentist.Status.HasValue)
            {
                user.Status = updateDentist.Status.Value;
            }
            if (updateDentist.Role!=null)
            {
                user.RoleId = role.Id;
            }
            user.UpdatedAt = DateTime.UtcNow;
            //Use base generic Update form Repository<User>
            _userRepository.Update(user);
            //Save changes
            //Use base generic SaveChangesAsync() from Repository<User>
            await _userRepository.SaveChangesAsync();

            //Dentist Table update data
            if (updateDentist.FullName != null)
            {
                dentist.FullName = updateDentist.FullName;
            }
            if (updateDentist.PhoneNumber != null)
            {
                dentist.PhoneNumber = updateDentist.PhoneNumber;
            }

            if (updateDentist.Qualification != null)
            {
                dentist.Qualification = updateDentist.Qualification;
            }
            if (updateDentist.Specialization != null)
            {
                dentist.Specialization = updateDentist.Specialization;
            }
            if (updateDentist.Experience != null)
            {
                dentist.Experience = updateDentist.Experience;
            }
            if (updateDentist.ProfileImage != null)
            {
                dentist.ProfileImage = updateDentist.ProfileImage;
            }
            if (updateDentist.Bio != null)
            {
                dentist.Bio = updateDentist.Bio;
            }
            if (updateDentist.Status.HasValue)
            {
                dentist.Status = updateDentist.Status.Value;
            }

            //Use base generic Update form Repository<Dentist>
            _dentistRepository.Update(dentist);
            //Save changes
            //Use base generic SaveChangesAsync() from Repository<Dentist>
            await _dentistRepository.SaveChangesAsync();

            //Return response dto
            return new DentistResponseDto
            {
                Id = dentist.Id,
                FullName = dentist.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = dentist.PhoneNumber,
                Qualification = dentist.Qualification,
                Specialization = dentist.Specialization,
                Experience = dentist.Experience,
                ProfileImage = dentist.ProfileImage,
                Bio = dentist.Bio,
                Status = dentist.Status,
                Role = roleName,
                CreatedAt = dentist.CreatedAt,
                UpdatedAt = dentist.UpdatedAt
            };
        }

        public async Task<PaginatedResponseDto<DentistResponseDto>> GetAllDentistsAsync(int pageNumber = 1,int? pageSize = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var query = _dentistRepository.GetAllDentistsAsync();
            // Total records
            var totalRecords = await query.CountAsync();
            List<Dentist> dentists;
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                dentists = await query.ToListAsync();

                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                dentists = await query
                    .Skip((pageNumber - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }

            // Entity → DTO
            var dentistDtos = dentists.Select(d => new DentistResponseDto
            {
                Id = d.Id,
                userId = d.UserId,
                FullName = d.FullName,
                Username = d.User?.Username ?? "",
                Email = d.User?.Email ?? "",
                PhoneNumber = d.PhoneNumber,
                Qualification = d.Qualification,
                Specialization = d.Specialization,
                Experience = d.Experience,
                ProfileImage = d.ProfileImage,
                Bio = d.Bio,
                Status = d.Status,
                Role = d.User?.Role?.Name ?? "",
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            }).ToList();

            var totalPages =
                pageSize > 0
                    ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                    : 0;

            return new PaginatedResponseDto<DentistResponseDto>
            {
                Data = dentistDtos,
                PageNumber = pageNumber,
                PageSize = pageSize ?? 0,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<bool> DeleteAnyDentistAsync(int userId)
        {
            //Find the dentist by userId
            var dentist = await _dentistRepository.GetDentistByUserIdAsync(userId);
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null || dentist == null)
            {
                return false; // Dentist not found 
            }
            //Remove The dentist
            //Uses base generic Remove() from Repository<Dentist>
            _dentistRepository.Remove(dentist);
            _userRepository.Remove(user);
            //Uses base generic SaveChangesAsync() from Repository<Dentist>
            await _dentistRepository.SaveChangesAsync();
            await _userRepository.SaveChangesAsync();
            return true;
        }

        //Create New Service
        public async Task<ServiceResponseDto> CreateServiceAsync(ServiceDto serviceDto)
        {
            //Create New Service
            var service = new Service
            {
                Name = serviceDto.Name,
                Description = serviceDto.Description,
                Duration = serviceDto.Duration,
                Price = serviceDto.Price,
                Status = serviceDto.Status,
                CreatedAt = DateTime.UtcNow
            };
            await _servicesRepository.AddAsync(service);
            await _servicesRepository.SaveChangesAsync();
            // Check Service ID
            if (service.Id <= 0)
            {
                throw new InvalidOperationException("Service was created but Service ID was not generated.");
            }

            return new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Duration = service.Duration,
                Price = service.Price,
                Status = service.Status,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
            };

        }

        //Update Service 
        public async Task<ServiceResponseDto?> UpdateServiceAsync(int Id, ServiceDto serviceDto)
        {
            var service = await _servicesRepository.GetServicesByIdAsync(Id);
            if (service == null)
            {
                return null;
            }
            if (serviceDto.Name != null)
            {
                service.Name = serviceDto.Name;
            }
            if (serviceDto.Description != null) { service.Description = serviceDto.Description; }
            if (serviceDto.Duration.HasValue) { service.Duration = serviceDto.Duration.Value; }
            if (serviceDto.Price.HasValue) { service.Price = serviceDto.Price.Value; }
            if (serviceDto.Status.HasValue) { service.Status = serviceDto.Status.Value; }
            service.UpdatedAt = DateTime.UtcNow;
            _servicesRepository.Update(service);
            await _servicesRepository.SaveChangesAsync();
            return new ServiceResponseDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Duration = service.Duration,
                Price = service.Price,
                Status = service.Status,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
            };
        }

        //Delete Service
        public async Task<bool> DeleteAnyServiceAsync(int Id)
        {
            //Find the Service by Id

            var service = await _servicesRepository.GetServicesByIdAsync(Id);
            if (service == null)
            {
                return false; // Service not found 
            }
            //Remove The Service
            //Uses base generic Remove() from Repository<Service>
            _servicesRepository.Remove(service);
            //Uses base generic SaveChangesAsync() from Repository<Service>
            await _servicesRepository.SaveChangesAsync();
            return true;
        }

        //Get All Service 
        public async Task<PaginatedResponseDto<ServiceResponseDto>> GetAllServiceAsync(int pageNumber = 1, int? pageSize = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            var query = _servicesRepository.GetAllServiceAsync();
            // Total records
            var totalRecords = await query.CountAsync();
            List<Service> services;
            // No pageSize = return all
            if (pageSize == null || pageSize <= 0)
            {
                services = await query.ToListAsync();

                pageNumber = 1;
                pageSize = totalRecords;
            }
            else
            {
                services = await query
                    .Skip((pageNumber - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }

            // Entity → DTO
            var servicesDtos = services.Select(s => new ServiceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Duration = s.Duration,
                Price = s.Price,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            var totalPages =
                pageSize > 0
                    ? (int)Math.Ceiling(totalRecords / (double)pageSize.Value)
                    : 0;

            return new PaginatedResponseDto<ServiceResponseDto>
            {
                Data = servicesDtos,
                PageNumber = pageNumber,
                PageSize = pageSize ?? 0,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }
    }
}
