using DentalCareAPI.DTOs;
using DentalCareAPI.Extensions;
using DentalCareAPI.Models;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Policy = "DentistOnly")]//Enter controller requred only Dentist role
    public class DentistController : ControllerBase
    {
        private readonly IDentistService _dentistService;
        private readonly IAppointmentService _appointmentService;
        public DentistController(IDentistService dentistService, IAppointmentService appointmentService)
        {
            _dentistService = dentistService;
            _appointmentService = appointmentService;
        }

        //POST : api/dentist/schedule - create schedule
        [Authorize(Policy = "DentistOnly")]
        [HttpPost("schedule")]
        public async Task<IActionResult> Create([FromBody] WorkingHourDto workingHourDto)
        {
            try
            {
                //Get the current user`s Id from the JWT toke
                var userId = User.GetUserId();
                var workingHour = await _dentistService.CreateWorkingHoursAsync(workingHourDto, userId);

                return CreatedAtAction(
                    nameof(Create),
                    new
                    {
                        id = workingHour.Id
                    },
                    new
                    {
                        success = true,
                        message = "Schedule updated successfully.",
                        data = workingHour
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

        //DELETE : api/dentist/delete/{Id}
        [Authorize(Policy = "DentistOnly")]
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteWorkingHour(int Id)
        {
            try
            {
                var success = await _dentistService.DeleteAnyWorkingHourAsync(Id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Working hours not found." });
                }
                return Ok(new { success = true, message = "Working hours deleted successfully." });
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

        //GET : api/dentist
        [Authorize(Policy = "DentistOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllWorkingHours(int pageNumber = 1, int? pageSize = null)
        {
            try
            {
                //Get the current user`s Id from the JWT toke
                var userId = User.GetUserId();
                var result = await _dentistService.GetAllWorkingHourAsync(userId,pageNumber,pageSize);

                return Ok(new
                {
                    success = true,
                    message = "Working hours retrieved successfully.",
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

        //GET : api/dentist/appointments?status=cancelled 
        [Authorize(Policy = "DentistOnly")]
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

        //GET : api/dentist/appointments/{appointmentId} - get appointment by Id
        [Authorize(Policy = "DentistOnly")]
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

        //PUT : api/dentist/appointments/{appointmentId}/status?status=confirmed - update appointment status
        [Authorize(Policy = "DentistOnly")]
        [HttpPut("appointments/{appointmentId}/status")]
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

        //POST : api/dentist/appointments/{appointmentId}/create - add treatment to appointment
        [Authorize(Policy = "DentistOnly")]
        [HttpPost("appointments/{appointmentId}/create")]
        public async Task<IActionResult> AddTreatmentToAppointment([FromRoute] int appointmentId, [FromBody] TreatmentDto treatmentDto)
        {
            try
            {
                var userId = User.GetUserId();
                var treatment = await _dentistService.CreateTreatmentAsync(treatmentDto, userId);
                return CreatedAtAction(nameof(AddTreatmentToAppointment), new { id = treatment.Id }, new { success = true, message = "Treatment added successfully.", data = treatment });
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
                return StatusCode(500, new { success = false, message = "An error occurred while adding the treatment", details = ex.Message });
            }
        }

        //GET  : api/dentist/treatments - get all treatments
        [Authorize(Policy = "DentistOnly")]
        [HttpGet("treatments")]
        public async Task<IActionResult> GetAllTreatments([FromQuery] int pageNumber = 1, [FromQuery] int? pageSize = null)
        {
            try
            {
                var userId = User.GetUserId();
                var treatments = await _dentistService.GetAllTreatmentsAsync(userId, pageNumber, pageSize);
                return Ok(new { success = true, message = "Treatments retrieved successfully.", data = treatments });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the treatments", details = ex.Message });
            }
        }

        //GET  : api/dentist/treatments/{treatmentId} - get treatment by Id
        [Authorize(Policy = "DentistOnly")]
        [HttpGet("treatments/{treatmentId}")]
        public async Task<IActionResult> GetTreatmentById([FromRoute] int treatmentId)
        {
            try
            {
                var userId = User.GetUserId();
                var treatment = await _dentistService.GetTreatmentByIdAsync(treatmentId);
                return Ok(new { success = true, message = "Treatment retrieved successfully.", data = treatment });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the treatment", details = ex.Message });
            }
        }

    }
}
