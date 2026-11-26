using DentalCareAPI.DTOs;
using DentalCareAPI.Extensions;
using DentalCareAPI.Models;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;

        public PatientController(IPatientService patientService, IAppointmentService appointmentService, IWebHostEnvironment environment)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _environment = environment;
        }
        //POST : api/patient/create - create patients
        [HttpPost("create")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto patientDto)
        {
            try
            {
                if (patientDto.ProfileImageFile is { Length: > 0 } imageFile)
                {
                    var imagePath = await UploadProfileImageAsync(
                        imageFile,
                        "patitent"
                    );

                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        patientDto.ProfileImage = imagePath;
                    }
                }
                var patient = await _patientService.CreatePatientAsync(patientDto);
                return CreatedAtAction(
                    nameof(CreatePatient),
                    new
                    {
                        id = patient.Id
                    },
                    new
                    {
                        success = true,
                        message = "Account created successfully!",
                        data = patient
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

        //GET : api/patient/appointments?status=cancelled - get all patients
        [Authorize(Policy = "PatientOnly")]
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

        //GET : api/patient/appointments/{appointmentId} - get appointment by Id
        [Authorize(Policy = "PatientOnly")]
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

        //GET : api/patient/getTreatments - get all treatments for the current patient
        [Authorize(Policy = "PatientOnly")]
        [HttpGet("getTreatments")]
        public async Task<IActionResult> GetTreatments()
        {
            try
            {
                var userId = User.GetUserId();
                var treatments = await _patientService.GetTreatmentsAsync(userId);
                return Ok(new
                {
                    success = true,
                    message = "Treatments retrieved successfully.",
                    data = treatments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the treatments", details = ex.Message });
            }
        }

        private async Task<string?> UploadProfileImageAsync(IFormFile? file, string folderName = "profiles")
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
