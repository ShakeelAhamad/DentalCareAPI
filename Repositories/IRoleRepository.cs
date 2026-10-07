using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Role-specific repository.
    /// Inherits from IRepository<Role> which provides:
    ///  - GetRoleByNameAsync -> used in AuthService
    /// This interface adds role-specific queries on top.
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        //-----------------------------------------------------------
        //TASK-SPECIFIC QUERIES
        //These are additional to what IRepository<Role> provides
        //-----------------------------------------------------------
        ///<summary>
        /// Gets role by rolename 
        /// Used id : AuthService.LoginAsync()
        /// Internally uses : base.CreateDentistAsync
        /// </summary> 
        Task<Role?> GetRoleByNameAsync(string roleName);
    }
}
