using DentalCareAPI.Constants;
using DentalCareAPI.Data;
using DentalCareAPI.Repositories;
using DentalCareAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//------------------------------------------------------
//REPOSITORY REGISTRATION
//Order : Generic first , then specific repositories
//------------------------------------------------------
//Generic repository - available for any entity type if needed
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//User repository - used by AuthService
builder.Services.AddScoped<IUserRepository, UserRepository>();

//RefreshToken repository - used by AuthService
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

//Role repository - used by AuthService
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

//Dentist repository - used by AdminService
builder.Services.AddScoped<IDentistRepository, DentistRepository>();

//Service repository - used by AdminService
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();

//Working Hours repostory
builder.Services.AddScoped<IWorkHourRepository,WorkHoursRepository>();
//BlockedDate Repository
builder.Services.AddScoped<IBlockedDateRepository, BlockedDateRepository>();
//Appointment repository
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
//Patient repository - used by AuthService
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
//Treatment repository - used by AdminService
builder.Services.AddScoped<ITreatmentRepository, TreatmentRepository>();

//Register Auth Service
builder.Services.AddScoped<IAuthService, AuthService>();

//Register Admin Service
builder.Services.AddScoped<IAdminService, AdminService>();
//Register DentistService
builder.Services.AddScoped<IDentistService,DentistService>();

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();

//Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audienc"],
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        // User is not logged in / token missing or invalid
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized. Please login first."
            });
        },

        // User is logged in but doesn't have required role
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            string message;
            switch (userRole)
            {
                case RoleConstants.Admin:
                    message = "Access denied. This action is not available for Admin users.";
                    break;

                case RoleConstants.Dentist:
                    message = "Access denied. This action is only available to Dentist users.";
                    break;

                case RoleConstants.Patient:
                    message = "Access denied. This action is only available to Patient users.";
                    break;

                default:
                    message = "Access denied. You do not have permission to perform this action.";
                    break;
            }

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = message
            });
        }
    };
});

//Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    //Admin-only policies
    options.AddPolicy("AdminOnly",policy => policy.RequireRole(RoleConstants.Admin));

    //Dentist-only policies
    options.AddPolicy("DentistOnly", policy => policy.RequireRole(RoleConstants.Dentist));

    //Patient-only policies
    options.AddPolicy("PatientOnly", policy => policy.RequireRole(RoleConstants.Patient));

    //Dentist or Admin policies
    options.AddPolicy("DentistOrAdmin", policy => policy.RequireRole(RoleConstants.Dentist, RoleConstants.Admin));

    //Any authenticated user (Admin,Dentist or Patient)
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
