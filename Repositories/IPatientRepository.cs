using DentalCareAPI.DTOs;
using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Patient-specific repository.
    /// Inherits from IRepository<Patient> which provides:
    ///   - AddAsync()        → used in AuthService.CreatePatientAsync()
    ///   - SaveChangesAsync() → used in AuthService.CreatePatientAsync()
    /// This interface adds patient-specific queries on top.
    /// </summary>
    public interface IPatientRepository : IRepository<Patient>
    {
        //---------------------------------------------------------- 
        //PATIENT-SPECIFIC QUERIES
        //These are additional to what IRepository<Patient> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets patient by userId 
        /// Used id : AuthService.LoginAsync()
        /// Internally Patient : base.FirstOrDefaultAsync
        /// </summary>
        Task<Patient?> GetPatientByUserIdAsync(int userId);
        Task<Patient?> GetPatientByIdAsync(int Id);
        IQueryable<PatientListResponseDto> GetAllPatientsAsync();
    }
}
