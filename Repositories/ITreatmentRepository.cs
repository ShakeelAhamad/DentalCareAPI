using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    /// <summary>
    /// Treatment-specific repository.
    /// Inherits from IRepository<Treatment> which provides:
    ///   - AddAsync()        → used in AdminService.CreateTreatmentAsync()
    ///   - SaveChangesAsync() → used in AdminService.CreateTreatmentAsync()
    /// This interface adds treatment-specific queries on top.
    /// </summary>
    public interface ITreatmentRepository : IRepository<Treatment>
    {
        //----------------------------------------------------------
        //TREATMENT-SPECIFIC QUERIES
        //These are additional to what IRepository<Treatment> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets treatments by ID 
        /// Used id : AdminService.CreateTreatmentAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        Task<Treatment?> GetTreatmentByIdAsync(int Id);

        //----------------------------------------------------------
        //TREATMENT-SPECIFIC QUERIES
        //These are additional to what IRepository<Treatment> provides
        //----------------------------------------------------------
        /// <summary>
        /// Gets treatments by Patient ID
        /// Used in : AdminService.CreateTreatmentAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        IQueryable<Treatment> GetTreatmentByPatientIdAsync(int patientId);

        // <summary>
        /// Gets All Treatments 
        /// Used id : AdminService.GetAllTreatmentsAsync()
        /// Internally Dentist : base.FirstOrDefaultAsync
        /// </summary>
        IQueryable<Treatment> GetAllTreatmentsAsync(int dentistId);
    }
}
