using DentalCareAPI.Models;

namespace DentalCareAPI.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        //-----------------------------------------------------------------
        //APPOINTMENT-SPECIFIC QUERIES
        //These are additional to what IRepository<Appointment> provides
        //-----------------------------------------------------------------
        Task<List<Appointment>> GetAppointmentsByDentistAndDateAsync(int dentistId,DateTime date);
        Task<Appointment?> GetAppointmentByIdAsync(int Id);
        IQueryable<Appointment> GetAppointmentByPatientId(int patientId);
        Task<bool> HasConflictAsync(int dentistId,DateOnly appointmentDate,TimeOnly startTime,TimeOnly endTime);
        IQueryable<Appointment> GetAllAppointmentsAsync(int? patientId,int? dentistId, string? status);

    }
}
