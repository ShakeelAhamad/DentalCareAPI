using DentalCareAPI.Data;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// SERVICE-specific repository implementation.
    ///
    /// FROM GENERIC REPOSITORY (inherited, no need to rewrite):
    ///   AddAsync()         → adds new dintist in AdminService.CreateServiceAsync()
    ///   SaveChangesAsync() → saves dintist in AdminService.CreateServiceAsync()
    /// </summary>
    public class ServicesRepository : Repository<Service>, IServicesRepository 
    {
        public ServicesRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        //--------------------------------------------------
        //SERVICES-SPECIFIC IMPLEMENTATIONS 
        //These use base class methods as building blocks
        //--------------------------------------------------
        /// <summary>
        /// Gets a single service ONLY 
        /// This is the primary security enforcement method.
        /// Used for: get single, update, delete, 
        /// Cannot use base.GetServicesByIdAsync() because that has no Id filter.
        /// </summary>
        public async Task<Service?> GetServicesByIdAsync(int Id)
        {
            return await _dbSet
                   .FirstOrDefaultAsync(s => s.Id == Id);
        }

        /// <summary>
        /// Gets ALL service in the system for admin/dentist Patient access.
        /// No ID filter - this is intentional for admin/dentist Patient role.
        /// Only called from AdminService methods that have role-based authorization.
        /// Cannot use base.GetAllServiceAsync() because we need specific ordering.
        /// </summary>
        public IQueryable<Service> GetAllServiceAsync()
        {
            return _dbSet
                       .OrderByDescending(s => s.CreatedAt);
        }
    } 
}
