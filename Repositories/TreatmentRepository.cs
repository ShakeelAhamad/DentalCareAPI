using DentalCareAPI.Data;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// TREATMENT-specific repository implementation.
    ///
    /// FROM GENERIC REPOSITORY (inherited, no need to rewrite):
    ///   AddAsync()         → adds new treatment in AdminService.CreateTreatmentAsync()
    ///   SaveChangesAsync() → saves treatment in AdminService.CreateTreatmentAsync()
    /// </summary>
    public class TreatmentRepository : Repository<Treatment>, ITreatmentRepository
    {
        public TreatmentRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        //--------------------------------------------------
        //TREATMENTS-SPECIFIC IMPLEMENTATIONS 
        //These use base class methods as building blocks
        //--------------------------------------------------
        /// <summary>
        /// Gets a single treatment ONLY 
        /// This is the primary security enforcement method.
        /// Used for: get single, update, delete, 
        /// Cannot use base.GetTreatmentsByIdAsync() because that has no Id filter.
        /// </summary>
        public async Task<Treatment?> GetTreatmentByIdAsync(int Id)
        {
            return await _dbSet
                    .Include(t => t.Patient)
                    .Include(t => t.Dentist)
                    .Include(t => t.Appointment)
                   .FirstOrDefaultAsync(t => t.Id == Id);
        }

        /// <summary>
        /// Gets a single treatment ONLY 
        /// This is the primary security enforcement method.
        /// Used for: get single, update, delete, 
        /// Cannot use base.GetTreatmentsByPatientIdAsync() because that has no Id filter.
        /// </summary>
        public IQueryable<Treatment> GetTreatmentByPatientIdAsync(int patientId)
        {
            return _dbSet
                       .Include(t => t.Patient)
                       .Include(t => t.Dentist)
                       .Include(t => t.Appointment)
                       .Where(t => t.PatientId == patientId)
                       .OrderByDescending(t => t.CreatedAt);
        }


        /// <summary>
        /// Gets ALL treatments in the system for admin/dentist Patient access.
        /// No ID filter - this is intentional for admin/dentist Patient role.
        /// Only called from AdminService methods that have role-based authorization.
        /// Cannot use base.GetAllTreatmentsAsync() because we need specific ordering.
        /// </summary>

        public IQueryable<Treatment> GetAllTreatmentsAsync(int dentistId)
        {
            return _dbSet
                       .Include(t => t.Patient)
                       .Include(t => t.Dentist)
                       .Include(t => t.Appointment)
                       .Where(t => t.DentistId == dentistId)
                       .OrderByDescending(t => t.CreatedAt);
        }
    }
}
