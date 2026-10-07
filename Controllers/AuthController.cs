using DentalCareAPI.DTOs;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace DentalCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AdminDto adminDto)
        {
            try
            {
                var user = await _authService.CreateAdminAsync(adminDto);
                return CreatedAtAction(nameof(Create), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var response = await _authService.LoginAsync(loginDto,ipAddress);
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {

                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
            }
        }

       
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get refresh token from HttpOnly cookie
                var refreshToken = Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return BadRequest(new
                    {
                        message = "Refresh token not found."
                    });
                }

                var ipAddress = GetIpAddress();

                // Revoke refresh token in database
                var success = await _authService.RevokeTokenAsync(
                    refreshToken,
                    ipAddress
                );

                // Remove refresh token cookie
                Response.Cookies.Delete(
                    "refreshToken",
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Path = "/api/auth/refresh-token"
                    }
                );

                if (!success)
                {
                    return BadRequest(new
                    {
                        message = "Token is invalid or already revoked."
                    });
                }

                return Ok(new
                {
                    message = "Logout successful."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred during logout.",
                    details = ex.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenResponseDto request)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var response  = await _authService.RefreshTokenAsync(request.RefreshToken,ipAddress);
                SetRefreshTokenCookie(response.RefreshToken);//Optional : update cookie
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
            }
        }

        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenResponseDto request)
        {
            try
            {
                var ipAddresh = GetIpAddress();
                var success = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddresh);
                if (!success)
                {
                    return BadRequest(new { message = "Token is invalid or already revoked" });
                }
                return Ok(new { message = "Token revoked successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {

                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", details = ex.Message });
            }
        }

        private string GetIpAddress()
        {
            //Get Ip Address from request
            if (Request.Headers.ContainsKey("X-Forwarded-for"))
            {
                return Request.Headers["X-Forwarded-for"].ToString();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,//Cannot be accessed by JavaScript
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,// Only sent over HTTPS
                SameSite = SameSiteMode.Strict,//CSRF Protection
                Path = "/api/auth/refresh-token"  //Only sent ot refresh endpoint
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
