using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Services-specific repository.
    /// Inherits from IRepository<Dentist> which provides:
    ///   - AddAsync()        → used in AdminService.CreateDentistAsync()
    ///   - SaveChangesAsync() → used in AdminService.CreateDentistAsync()
    /// This interface adds dentist-specific queries on top.
    /// </summary>
    public interface IServicesRepository : IRepository<Service>
    {

        //----------------------------------------------------------
        //SERVICES-SPECIFIC QUERIES
        //These are additional to what IRepository<Service> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets services by ID 
        /// Used id : AdminService.CreateServiceAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        Task<Service?> GetServicesByIdAsync(int Id);
        // <summary>
        /// Gets All Services 
        /// Used id : AdminService.GetAllServiceAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        IQueryable<Service> GetAllServiceAsync();
    }
}
