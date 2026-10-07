using BCrypt.Net;
using DentalCareAPI.Constants;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using DentalCareAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentalCareAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDentistRepository _dentistRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IConfiguration _configuration;
        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IRoleRepository roleRepository,
            IDentistRepository dentistRepository,
            IPatientRepository patientRepository,
            IConfiguration configuration
            )
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _roleRepository = roleRepository;
            _dentistRepository = dentistRepository;
            _patientRepository = patientRepository;
            _configuration = configuration;
        }

        //Create New Admin 
        public async Task<AdminResponseDto> CreateAdminAsync(AdminDto adminDto)
        {
            var email = adminDto.Email.Trim().ToLower();
            var username = adminDto.Username.Trim().ToLower();
            //Check Email 
            if (await _userRepository.EmailExistsAsync(email))
            {
                throw new InvalidOperationException("User with this email already exists");
            }
            //Check Username
            if (await _userRepository.UsernameExistsAsync(username))
            {
                throw new InvalidOperationException("User with this username already exists");
            }
            var roleName = !string.IsNullOrEmpty(adminDto.Role) ? adminDto.Role : RoleConstants.Admin;
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                throw new InvalidOperationException("Default user role not found. Please ensure roles are seeded");
            }
            //Has the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(adminDto.Password);
            //Create New User
            var user = new User
            {
                Username = adminDto.Username,
                Email = adminDto.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                Status = true,
                CreatedAt = DateTime.UtcNow,
            };
            //Add to Database
            //Uses base generic AddAsync() from Repository<User>
            await _userRepository.AddAsync(user);
            //Uses base generic SaveChangesAsync() from Repository<User>
            await _userRepository.SaveChangesAsync();
            var userWithRole = await _userRepository.GetUserWithRoleAsync(user.Id);

            return new AdminResponseDto
            {
                Id = userWithRole!.Id,
                Username = userWithRole.Username,
                Email = user.Email,
                Status = userWithRole.Status,
                Role = userWithRole.Role.Name,
                CreatedAt = userWithRole.CreatedAt,
                UpdatedAt = userWithRole.UpdatedAt
            };
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string ipAddress)
        {
            //Get User by Email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            //If Check User Exists
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            //Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            //Check password
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            //Generate JWT Token
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            //Save refresh token to database
            refreshToken.UserId = user.Id;
            //Uses base generic AddAsync() from Repository<RefreshToke>
            await _refreshTokenRepository.AddAsync(refreshToken);
            //Remove old/incative refresh tokens for this users (optional cleanup)
            //RefreshTokenRepository.RemoveOldTokensAsync()
            //Uses base.RemoveRange() internally for cleanup
            await _refreshTokenRepository.RemoveOldTokensAsync(user.Id);
            //Uses base generic SaveChangeAsync() from Repository<RefreshToke>
            await _refreshTokenRepository.SaveChangesAsync();
            var Profile  = "";
            var FullName = user.Username!;
            if(user.Role.Name.ToLower() == RoleConstants.Patient.ToLower())
            {
                var patient = await _patientRepository.GetPatientByUserIdAsync(user.Id);
                if (patient != null)
                {
                    FullName = patient.FullName;
                    Profile = patient.ProfileImage;
                }
            }
            else if (user.Role.Name.ToLower() == RoleConstants.Dentist.ToLower())
            {
                var dentist = await _dentistRepository.GetDentistByIdAsync(user.Id);
                if (dentist != null)
                {
                    FullName = dentist.FullName;
                    Profile = dentist.ProfileImage;
                }
            }

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:AccessTokenExpiryMinutes"]!));

            //Return login Response
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                Details = new AdminResponseDto
                {
                    Id = user.Id,
                    Username = user.Username!,
                    Email = user.Email!,
                    CreatedAt = user.CreatedAt,
                    Role = user.Role.Name, //Include the role
                    FullName = FullName,
                    Profile  = Profile
                },
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string token, string ipAddresh)
        {
            //UserRepository.GetUserWitheRefreshTokenAsync()
            //Custome Query - needs the user + Role + RefreshTokens loaded
            var user = await _userRepository.GetUserWithRefreshTokensAsync(token);
            if(user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token");
            }

            //Generate New tokens
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToke = GenerateRefreshToken(ipAddresh);
            newRefreshToke.UserId = user.Id;
            //Mark old refresh token as replaced
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddresh;
            refreshToken.ReplacedByToken = newRefreshToke.Token;
            //Uses base generic  Update() from Repository<RefreshToken>
            _refreshTokenRepository.Update(refreshToken);
            //Save New Refresh Token
            //Uses base generic AddAsync() from Repository<RefreshToken>
            await _refreshTokenRepository.AddAsync(newRefreshToke);

            //Remove old refreh token
            //RefreshTokenRepository.RemoveOldTokensAsync()
            // Use base.RemoveRange() internally for cleanup
            await _refreshTokenRepository.RemoveOldTokensAsync(user.Id);
            //Uses base generic SaveChangesAsync() from Repository<RefreshToken>
            await _refreshTokenRepository.SaveChangesAsync();
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:AccessTokenExpiryMinutes"]!));
            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToke.Token,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = newRefreshToke.ExpiresAt
            };
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddresh)
        {
            //UserRepository.GetUserWithRefreshTokensAsync()
            //Custom query - needs User + RefreshTokens loaded
            var user = await _userRepository.GetUserWithRefreshTokensAsync(token);
            if(user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
            {
                return false;
            }

            //Remove Token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddresh;
            //Uses base generic Update() from Repository<RefreshToken>
            _refreshTokenRepository.Update(refreshToken);
            //Uses base generic SaveChangesAsync() from Repository<RefreshToken>
            await _refreshTokenRepository.SaveChangesAsync();
            return true;
        }

        private string GenerateAccessToken(User user)
        {
            //Get JWT Settings from configuration
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audienc"];
            var expiryMinutes = double.Parse(_configuration["JwtSettings:AccessTokenExpiryMinutes"]!);

            //Create security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Define claims (user information to include in token)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email!),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.Username!),
                new Claim(ClaimTypes.Role,user.Role.Name),//Add role claim
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };

            //Create Token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
                );

            //Return Token as string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshTokenExpiry = double.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!);
            //Generate cryptographically secure random token
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);
            return new RefreshToken
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiry),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

        }
    }
}
