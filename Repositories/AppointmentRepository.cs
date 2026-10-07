using DentalCareAPI.Data;
using DentalCareAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Repositories
{
    public class AppointmentRepository : Repository<Appointment> , IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<List<Appointment>> GetAppointmentsByDentistAndDateAsync(int dentistId,DateTime date)
        {
            return await _dbSet
              .Where(x =>
                  x.DentistId == dentistId &&
                  x.AppointmentDate.Date == date.Date &&
                  x.Status != "cancelled")
              .Select(x => new Appointment
              {
                  StartTime = x.StartTime,
                  EndTime = x.EndTime
              })
              .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int Id)
        {
            return await _dbSet
                .Include(a => a.Patient)
                   .ThenInclude(p => p!.User)
                .Include(a => a.Dentist)
                   .ThenInclude(d => d!.User)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }
        public IQueryable<Appointment> GetAppointmentByPatientId(int patientId)
        {
            return  _dbSet
                .Include(a => a.Patient)
                   .ThenInclude(p => p!.User)
                .Include(a => a.Dentist)
                   .ThenInclude(d => d!.User)
                .Include(a => a.Service)
                 .Where(x => x.PatientId == patientId)
                       .OrderByDescending(t => t.CreatedAt);
        }   

        public async Task<bool> HasConflictAsync(int dentistId,DateOnly appointmentDate,TimeOnly startTime,TimeOnly endTime)
         {
            return await _dbSet.AnyAsync(a =>
                 a.DentistId == dentistId &&
                 DateOnly.FromDateTime(a.AppointmentDate) == appointmentDate &&
                 a.Status.ToLower() != "cancelled" &&
                 a.StartTime < endTime &&
                 a.EndTime > startTime
             );
        }
        public IQueryable<Appointment> GetAllAppointmentsAsync(int? patientId, int? dentistId, string? status)
        {
            var query = _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Dentist)
                .Include(a => a.Service)
                .AsQueryable();

            if (patientId.HasValue)
            {
                query = query.Where(a => a.PatientId == patientId.Value);
            }

            if (dentistId.HasValue)
            {
                query = query.Where(a => a.DentistId == dentistId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status.ToLower() == status.ToLower());
            }
           
            return query.OrderByDescending(a => a.CreatedAt);
        }
    }
}
