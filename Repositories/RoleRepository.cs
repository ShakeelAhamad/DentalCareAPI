using DentalCareAPI.Data;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    public class RoleRepository : Repository<Role>,IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        //--------------------------------------------------
        //ROLE-SPECIFIC IMPLEMENTATIONS
        //These use base class methods as building blocks
        //--------------------------------------------------

        ///<summary>
        ///Gets role by name
        ///Uses base : FirstOrDefaultAsync() with case-insensitive comparison.
        /// </summary>
        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbSet
                   .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());
        }

    }
}
