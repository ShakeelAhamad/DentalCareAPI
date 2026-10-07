using DentalCareAPI.Data;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Dentist-specific repository implementation.
    ///
    /// FROM GENERIC REPOSITORY (inherited, no need to rewrite):
    ///   AddAsync()         → adds new dintist in AuthService.CreateDentistAsync()
    ///   SaveChangesAsync() → saves dintist in AuthService.CreateDentistAsync()
    /// </summary>
    public class DentistRepository : Repository<Dentist>,IDentistRepository 
    {
        public DentistRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        //--------------------------------------------------
        //DENTIST-SPECIFIC IMPLEMENTATIONS
        //These use base class methods as building blocks
        //--------------------------------------------------
        ///<summary>
        ///Gets Dentist by userId
        ///Uses base : FirstOrDefaultAsync() with case-insensitive comparison.
        /// </summary>
        public async Task<Dentist?> GetDentistByIdAsync(int userId)
        {
            //Include role because AuthService needs user.Role.Name for JWT claims
            return await _dbSet
                  .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        /// <summary>
        /// Gets a single task ONLY if it belongs to the specified user.
        /// This is the primary security enforcement method.
        /// Used for: get single, update, delete, 
        /// Cannot use base.GetDentistByUserIdAsync() because that has no userId filter.
        /// </summary>

        public async Task<Dentist?> GetDentistByUserIdAsync(int userId)
        {
            // Both conditions required:
            // d.UserId == userId → ensure it belongs to the requesting user
            // If either fails, returns null → controller returns 404
            return await _dbSet
                        .FirstOrDefaultAsync(d => d.UserId == userId);
        }

        ///<summary>
        /// Gets all Dentist if belong to the specified user and role
        /// Used for : get all record
        /// </summary>
        public IQueryable<Dentist> GetAllDentistsAsync()
        {
             return _dbSet
                        .Include(d => d.User)
                        .OrderByDescending(d => d.CreatedAt);
        }
    }
}
