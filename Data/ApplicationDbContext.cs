using BCrypt.Net;
using DentalCareAPI.DTOs;
using DentalCareAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DentalCareAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
         
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<WorkingHour> WorkHours { get; set; }

        public DbSet<BlockedDate> BlockedDates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Configure Role-Users relationship
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade); //Don`t allow deleting role if users exit
            //Configure Dentist-Users relationship
            modelBuilder.Entity<Dentist>()
                .HasOne(d => d.User)
                .WithOne(u => u.Dentist)
                .HasForeignKey<Dentist>(d => d.UserId);

            //Configure Dentist-Users relationship
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(p => p.UserId);


            // Configure Dentist - WorkingHours relationship
            modelBuilder.Entity<WorkingHour>()
                .HasOne(w => w.Dentist)
                .WithMany(d => d.WorkingHours)
                .HasForeignKey(w => w.DentistId);

            //Configure BlockedDate - Dentist relationship
            modelBuilder.Entity<BlockedDate>()
                .HasOne(b => b.Dentist)
                .WithMany()
                .HasForeignKey(b => b.DentistId);

            //Configure Treatment - Patient relationship
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId);

            //Configure Treatment - Dentist relationship
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Dentist)
                .WithMany()
                .HasForeignKey(t => t.DentistId);

            //Configure Treatment - Appointment relationship
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Appointment)
                .WithMany()
                .HasForeignKey(t => t.AppointmentId);

            //Configure Appointment - Patient relationship
            modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

            //Configure Appointment - Dentist relationship
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Dentist)
                .WithMany()
                .HasForeignKey(a => a.DentistId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure Appointment - Service relationship
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);


            //Configure indexs for better performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name) //Role name must be unique
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<Service>()
                .HasIndex(se => se.Name) //service name must be unique
                .IsUnique();
            modelBuilder.Entity<Appointment>()
                .HasIndex(ap => new { ap.PatientId, ap.DentistId , ap.ServiceId });

            modelBuilder.Entity<Dentist>()
                .HasIndex(d => d.UserId);

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.UserId);
            modelBuilder.Entity<Treatment>()
                .HasIndex(t => new { t.PatientId, t.AppointmentId, t.DentistId });
            modelBuilder.Entity<WorkingHour>()
                .HasIndex(w => w.DentistId);
            
            modelBuilder.Entity<BlockedDate>()
                .HasIndex(d => d.DentistId);


            //Seed Default roles
            modelBuilder.Entity<Role>().HasData(
                   new Role
                   { 
                       Id = 1,
                       Name = "Admin",
                       Description = "Full system access - can manage Patient and Dentist , Appointment, service",
                       CreatedAt = new DateTime(2026, 9, 18, 10, 30, 0),
                       UpdatedAt = new DateTime(2026, 9, 18, 10, 30, 0)
                   },
                   new Role
                   {
                       Id = 2,
                       Name = "Dentist",
                       Description = "Full system access - can manage Patient and Appointment, WorkingHour,Treatment",
                       CreatedAt = new DateTime(2026, 9, 18, 10, 30, 0),
                       UpdatedAt = new DateTime(2026, 9, 18, 10, 30, 0)
                   },
                    new Role
                    {
                        Id = 3,
                        Name = "Patient",
                        Description = "Book the Appointment manage profile all access",
                        CreatedAt = new DateTime(2026, 9, 18, 10, 30, 0),
                        UpdatedAt = new DateTime(2026, 9, 18, 10, 30, 0)
                    }
                );

            //Seed Default Admin role user create
            //var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin@123");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "Admin",
                    Email = "admin@admin.com",
                    PasswordHash = "$2a$11$9O6ich5lYfWOoVt2RIHzS.W/nkZ5.gQjwvNi.3s8CVl4uVzEfv6ba",
                    RoleId = 1,
                    Status = true,
                    CreatedAt = new DateTime(2026, 9, 22, 10, 30, 0),
                    UpdatedAt = new DateTime(2026, 9, 22, 10, 30, 0)
                }
                );


        }

    }
}
