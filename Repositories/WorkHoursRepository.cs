using DentalCareAPI.Data;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// WorkHours-specific repository implementation.
    ///
    /// FROM GENERIC REPOSITORY (inherited, no need to rewrite):
    ///   AddAsync()         → adds new dintist in AdminService.CreateWorkingAsync()
    ///   SaveChangesAsync() → saves dintist in AdminService.CreateWorkingAsync()
    /// </summary>
    public class WorkHoursRepository : Repository<WorkingHour>,IWorkHourRepository
    {
        public WorkHoursRepository(ApplicationDbContext context) : base(context)
        {

        }

        //--------------------------------------------------
        //WorkHours-SPECIFIC IMPLEMENTATIONS
        //These use base class methods as building blocks
        //--------------------------------------------------

        /// <summary>
        /// Gets a single WorkHours ONLY 
        /// This is the primary security enforcement method.
        /// Used for: get single, update, delete, 
        /// Cannot use base.GetWorkingHourByDentistIdAndDayOfWeekAsync() because that has no Id filter.
        /// </summary>
        public async Task<WorkingHour?> GetWorkingHourByDentistIdAndDayOfWeekAsync(int dentistId, string dayOfWeek)
        {
            return await _dbSet
                   .Include(w => w.Dentist)
                   .FirstOrDefaultAsync(wh => wh.DentistId == dentistId && wh.DayOfWeek.ToLower() == dayOfWeek.ToLower());
        }

        /// <summary>
        /// Gets ALL working hours in the system for admin/dentist Patient access.
        /// No ID filter - this is intentional for admin/dentist Patient role.
        /// Only called from AdminService methods that have role-based authorization.
        /// Cannot use base.GetAllWorkHoursAsync() because we need specific ordering.
        /// </summary>
        public IQueryable<WorkingHour> GetAllWorkHoursAsync(int dentistId)
        {
            return _dbSet
                       .Include(w => w.Dentist)
                       .Where(wh => wh.DentistId == dentistId)
                       .OrderByDescending(s => s.CreatedAt);
        }

        public async Task<WorkingHour?> GetWorkingHourByIdAsync(int Id)
        {
            return await _dbSet
                   .Include(wh => wh.Dentist)
                   .FirstOrDefaultAsync(wh => wh.Id == Id);
        }
    }
}
