using DentalCareAPI.DTOs;
using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Dentist-specific repository.
    /// Inherits from IRepository<Dentist> which provides:
    ///   - AddAsync()        → used in AuthService.CreateDentistAsync()
    ///   - SaveChangesAsync() → used in AuthService.CreateDentistAsync()
    /// This interface adds dentist-specific queries on top.
    /// </summary>
    public interface IDentistRepository : IRepository<Dentist>
    {
        //---------------------------------------------------------- 
        //DENTIST-SPECIFIC QUERIES
        //These are additional to what IRepository<Dentist> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets dentist by userId 
        /// Used id : AuthService.LoginAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        Task<Dentist?> GetDentistByIdAsync(int userId);
        Task<Dentist?> GetDentistByUserIdAsync(int userId);
        IQueryable<Dentist> GetAllDentistsAsync();
    }
}
