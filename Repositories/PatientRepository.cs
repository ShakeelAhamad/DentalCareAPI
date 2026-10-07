using DentalCareAPI.Data;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Patient-specific repository implementation.
    ///
    /// FROM GENERIC REPOSITORY (inherited, no need to rewrite):
    ///   AddAsync()         → adds new patient in AuthService.CreatePatientAsync()
    ///   SaveChangesAsync() → saves patient in AuthService.CreatePatientAsync()
    /// </summary>
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        //--------------------------------------------------
        //PATIENT-SPECIFIC IMPLEMENTATIONS
        //These use base class methods as building blocks
        //--------------------------------------------------
        ///<summary>
        ///Gets Patient by userId
        ///Uses base : FirstOrDefaultAsync() with case-insensitive comparison.
        /// </summary>
        public async Task<Patient?> GetPatientByUserIdAsync(int userId)
        {
            //Include role because AuthService needs user.Role.Name for JWT claims
            return await _dbSet
                  .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Patient?> GetPatientByIdAsync(int Id)
        {
            //Include role because AuthService needs user.Role.Name for JWT claims
            return await _dbSet
                   .Include(p => p.User)
                  .FirstOrDefaultAsync(p => p.Id == Id);
        }

        ///<summary>
        /// Gets all Patient if belong to the specified user and role
        /// Used for : get all record
        /// </summary>
        public IQueryable<PatientListResponseDto> GetAllPatientsAsync()
        {
            return _dbSet
                       .Include(p => p.User)
                       .OrderByDescending(p => p.CreatedAt)
                       .Select(p => new PatientListResponseDto
                       {
                           Id                 = p.Id,
                           UserId             = p.UserId,
                           FullName           = p.FullName,
                           PhoneNumber        = p.PhoneNumber,
                           Email              = p.User != null ? p.User.Email : "",
                           DateOfBirth        = p.DateOfBirth.ToString("MMMM dd, yyyy"),
                           Gender             = p.Gender,
                           Address            = p.Address,
                           ProfileImage       = p.ProfileImage,
                           Status             = p.Status,
                           AppointmentCount   = _context.Appointments.Count(a => a.PatientId == p.Id),
                           TreatmentCount     = _context.Treatments.Count(t => t.PatientId == p.Id)
                       });
        }
    }
}
