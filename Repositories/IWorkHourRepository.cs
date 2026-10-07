using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// WorkingHour-specific repository.
    /// Inherits from IRepository<WorkingHour> which provides:
    ///   - AddAsync()        → used in DentistService.CreateWorkingAsync()
    ///   - SaveChangesAsync() → used in DentistService.CreateWorkingAsync()
    /// This interface adds dentist-specific queries on top.
    /// </summary>
    public interface IWorkHourRepository : IRepository<WorkingHour>
    {
        //----------------------------------------------------------
        //WorkingHour-SPECIFIC QUERIES
        //These are additional to what IRepository<WorkingHour> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets Working Hour by dentistId and  dayOfWeek
        /// Used id : DentistService.CreateWorkingAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        Task<WorkingHour?> GetWorkingHourByDentistIdAndDayOfWeekAsync(int dentistId,string dayOfWeek);
        /// <summary>
        /// Gets Working hour by ID 
        /// Used id : DentistService.DeleteWorkingAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        Task<WorkingHour?> GetWorkingHourByIdAsync(int Id);
        // <summary>
        /// Gets All Services 
        /// Used id : AdminService.GetAllWorkHoursAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        IQueryable<WorkingHour> GetAllWorkHoursAsync(int dentistId);
    }
}
