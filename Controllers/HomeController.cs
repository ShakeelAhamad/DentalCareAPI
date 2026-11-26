using DentalCareAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public HomeController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpGet("getService")]
        public async Task<IActionResult> GetService(int pageNumber = 1, int? pageSize = null)
        {
            try
            {
                var services = await _adminService.GetAllServiceAsync(
                        pageNumber,
                        pageSize
                    );
                var dentists = await _adminService.GetAllDentistsAsync(
                        pageNumber,
                        pageSize
                    );
                return Ok(new
                {
                    success  = true,
                    message  = "Retrieved successfully.",
                    services = services,
                    dentists = dentists
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
    }
}
