using DentalCareAPI.DTOs;
using DentalCareAPI.Extensions;
using DentalCareAPI.Models;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;
        public PatientController(IPatientService patientService, IAppointmentService appointmentService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
        }
        //POST : api/patient/create - create patients
        [HttpPost("create")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientDto patientDto)
        {
            try
            {
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
                        message = "Patient added successfully.",
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
    }
}
