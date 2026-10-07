using DentalCareAPI.DTOs;
using DentalCareAPI.Extensions;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Authorize(Policy = "AuthenticatedUser")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Policy = "AuthenticatedUser")]
        [HttpPost("available-slots")]
        public async Task<IActionResult> AvailableSlots([FromBody] AvailableSlotsRequestDto request)
        {
            try
            {
                var result =
                await _appointmentService
                    .GetAvailableSlotsAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Available slots retrieved successfully.",
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

        //POST : api/appointment/{appointmentId}/cancel - cancel appointment
        [Authorize(Policy = "PatientOnly")]
        [HttpPut("{appointmentId}/cancel")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                //Get the current user`s Id from the JWT toke
                var userId = User.GetUserId();
                await _appointmentService.CancelAppointmentAsync(appointmentId,userId);
                return Ok(new { success = true, message = "Appointment canceled successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while canceling the appointment.", details = ex.Message });
            }
        }

        //POST : api/appointment/create - create appointment
        [Authorize(Policy = "PatientOnly")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentDto appointmentDto)
        {
            try
            {
                //Get the current user`s Id from the JWT toke
                var userId = User.GetUserId();
                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto, userId);
                return CreatedAtAction(
                    nameof(CreateAppointment),
                    new
                    {
                        id = appointment.Id
                    },
                    new
                    {
                        success = true,
                        message = "Appointment created successfully.",
                        data = appointment
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the appointment.", details = ex.Message });
            }
        }


    }
}
