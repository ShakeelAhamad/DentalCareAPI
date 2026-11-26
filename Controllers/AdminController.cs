using DentalCareAPI.DTOs;
using DentalCareAPI.Extensions;
using DentalCareAPI.Models;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Policy = "AdminOnly")]//Enter controller requred only Admin role
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IAdminService _adminService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        public AdminController(IAdminService adminService, IAppointmentService appointmentService, IPatientService patientService, IWebHostEnvironment environment)
        {
            _adminService = adminService;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _environment = environment;
        }

        //POST : api/admin/dentists/create - create dentists
        [HttpPost("dentists/create")]
        public async Task<IActionResult> Create([FromBody] DentistDto dentistDto)
        {
            try
            {
                if (dentistDto.ProfileImageFile is { Length: > 0 } imageFile)
                {
                    var imagePath = await UploadProfileImageAsync(
                        imageFile,
                        "dentists"
                    );

                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        dentistDto.ProfileImage = imagePath;
                    }
                }
                var user = await _adminService.CreateDentistAsync(dentistDto);
                return CreatedAtAction(
                    nameof(Create), 
                    new { 
                        id = user.Id 
                    },
                    new {
                        success = true,
                        message = "Dentist added successfully.",
                        data = user
                     }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/dentists/Id
        
        [HttpGet("dentists/{Id}")]
        public async Task<IActionResult> GetDentistByIdAsync(int Id)
        {
            try
            {
                var dentist = await _adminService.GetDentistByIdAsync(Id);
                if (dentist == null)
                {
                    return NotFound(new { success = false, message = "Dentist not found or you don`t have permission to update it" });
                }
                return Ok(new { success = true, message = "Dentist retrieved successfully.", data = dentist });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }


        //POST : api/admin/dentists/update/{userId}
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("dentists/update/{Id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] UpdateDentistDto updateDentistDto)
        {
            try
            {
                var dentist  = await _adminService.UpdateDentistAsync(Id, updateDentistDto);
                if(dentist == null)
                {
                    return NotFound(new { success = false, message = "Dentist not found or you don`t have permission to update it" });
                }
                return Ok(new { success = true, message = "Dentist updated successfully.", data = dentist });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/dentists
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("dentists")]
        public async Task<IActionResult> GetAllDentists(int pageNumber = 1,int? pageSize = null)
        {
            try
            {
                var result = await _adminService.GetAllDentistsAsync(
                        pageNumber,
                        pageSize
                    );

                return Ok(new
                {
                    success = true,
                    message = "Dentists retrieved successfully.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //DELETE : api/admin/dentists/delete/{userId}
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("dentists/delete/{userId}")]
        public async Task<IActionResult> DeleteDentists(int userId)
        {
            try
            {
                var success = await _adminService.DeleteAnyDentistAsync(userId);
                if (!success)
                {
                    return NotFound(new {success = false,message = "Dentist not found."});
                }
                return Ok(new { success = true,message = "Dentist deleted successfully."});
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //POST : api/admin/services/create
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("services/create")]
        public async Task<IActionResult> CreateService([FromBody] ServiceDto serviceDto)
        {
            try
            {
                var service = await _adminService.CreateServiceAsync(serviceDto);
                return CreatedAtAction(
                    nameof(Create),
                    new
                    {
                        id = service.Id
                    },
                    new
                    {
                        success = true,
                        message = "Service added successfully.",
                        data = service
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }


        //PUT : api/admin/services/update/{Id}
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("services/update/{Id}")]
        public async Task<IActionResult> UpdateService(int Id, [FromBody] ServiceDto serviceDto)
        {
            try
            {
                var service = await _adminService.UpdateServiceAsync(Id, serviceDto);
                if (service == null)
                {
                    return NotFound(new { success = false, message = "Service not found or you don`t have permission to update it" });
                }
                return Ok(new { success = true, message = "Service updated successfully.", data = service });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //DELETE : api/admin/services/delete/{Id}
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("services/delete/{Id}")]
        public async Task<IActionResult> DeleteService(int Id)
        {
            try
            {
                var success = await _adminService.DeleteAnyServiceAsync(Id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Service not found." });
                }
                return Ok(new { success = true, message = "Service deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/services
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("services")]
        public async Task<IActionResult> GetAllService(int pageNumber = 1, int? pageSize = null)
        {
            try
            {
                var result = await _adminService.GetAllServiceAsync(
                        pageNumber,
                        pageSize
                    );
                return Ok(new
                {
                    success = true,
                    message = "Service retrieved successfully.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/dentists/Id

        [HttpGet("services/{Id}")]
        public async Task<IActionResult> GetServiceByIdAsync(int Id)
        {
            try
            {
                var service = await _adminService.GetServiceByIdAsync(Id);
                if (service == null)
                {
                    return NotFound(new { success = false, message = "Service not found or you don`t have permission to update it" });
                }
                return Ok(new { success = true, message = "Service retrieved successfully.", data = service });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/appointments?status=cancelled - get all patients
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAllAppointments([FromQuery] string? status, [FromQuery] int pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            try
            {
                //Get the current user`s Id from the JWT toke
                var userId = User.GetUserId();
                var appointments = await _appointmentService.GetAllAppointmentsAsync(userId, status, pageNumber, pageSize);
                return Ok(new
                {
                    success = true,
                    message = "Appointments retrieved successfully.",
                    data = appointments
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //GET : api/admin/appointments/{appointmentId} - get appointment by Id
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("appointments/{appointmentId}")]
        public async Task<IActionResult> GetAppointmentById([FromRoute] int appointmentId)
        {
            try
            {
                var userId = User.GetUserId();
                var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return NotFound(new { success = false, message = "Appointment not found." });
                }
                return Ok(new
                {
                    success = true,
                    message = "Appointment retrieved successfully.",
                    data = appointment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the appointment", details = ex.Message });
            }
        }

        //DELETE : api/admin/appointments/delete/{Id}
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("appointments/delete/{Id}")]
        public async Task<IActionResult> DeleteAppointment(int Id)
        {
            try
            {
                var success = await _appointmentService.DeleteAnyAppointmentAsync(Id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Appointment not found." });
                }
                return Ok(new { success = true, message = "Appointment deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred during registration", details = ex.Message });
            }
        }

        //PUT : api/admin/appointments/update-status/{appointmentId}?status=completed - update appointment status
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("appointments/update-status/{appointmentId}")]
        public async Task<IActionResult> UpdateAppointmentStatus([FromRoute] int appointmentId, [FromQuery] string status)
        {
            try
            {
                var message = await _appointmentService.UpdateAppointmentStatusAsync(appointmentId, status);
                return Ok(new { success = true, message = message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the appointment status", details = ex.Message });
            }
        }

        //GET : api/admin/patients
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("patients")]
        public async Task<IActionResult> GetAllPatients([FromQuery] int pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            try
            {
                var patients = await _patientService.GetAllPatientsAsync(pageNumber, pageSize);
                return Ok(new { success = true, message = "Patients retrieved successfully.", data = patients });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving patients", details = ex.Message });
            }
        }

        //GET : api/admin/patients/{patientId}
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("patients/{patientId}")]
        public async Task<IActionResult> GetPatientById([FromRoute] int patientId)
        {
            try
            {
                var patient = await _patientService.GetPatientByIdAsync(patientId);
                if (patient == null)
                {
                    return NotFound(new { success = false, message = "Patient not found." });
                }
                return Ok(new { success = true, message = "Patient retrieved successfully.", data = patient });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the patient", details = ex.Message });
            }
        }

        //GET : api/admin/dashboard-stats
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _appointmentService.GetDashboardStatsAsync();
                return Ok(new { success = true, message = "Dashboard stats retrieved successfully.", data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving dashboard stats", details = ex.Message });
            }
        }

        private async Task<string?> UploadProfileImageAsync(IFormFile? file,string folderName = "profiles")
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException(
                    "Only JPG, JPEG, PNG and WEBP images are allowed.");

            if (file.Length > 2 * 1024 * 1024)
                throw new InvalidOperationException(
                    "Image size must not exceed 2 MB.");

            var webRoot = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            var uploadFolder = Path.Combine(
                webRoot,
                "uploads",
                folderName
            );

            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
        }


    }
}
