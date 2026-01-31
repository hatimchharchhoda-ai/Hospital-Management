using backend.Data;
using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend.Reposetory
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Count how many patients already booked for this doctor/date/time slot
        public async Task<int> GetSlotCountAsync(int doctorId, DateOnly date, TimeOnly startTime)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.AppointmentDate == date &&
                            a.StartTime == startTime &&
                            a.Status == AppointmentStatus.Booked)
                .CountAsync();
        }

        // Check if same patient has already booked same doctor on the same date
        public async Task<bool> IsPatientAlreadyBookedAsync(int doctorId, int patientId, DateOnly date)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                               a.PatientId == patientId &&
                               a.AppointmentDate == date &&
                               a.Status == AppointmentStatus.Booked);
        }


        // Get the details of today appointment by doctor id
        public async Task<List<AppointmentListDto>> GetTodayAppointmentsByDoctorAsync(int doctorId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.AppointmentDate == today &&
                            a.Status == AppointmentStatus.Booked)
                .Include(a => a.Patient)
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name
                })
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        // Get the today appointment from the patient id
        public async Task<List<AppointmentListDto>> GetTodayAppointmentsByPatientAsync(int patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Appointments
                .Where(a => a.PatientId == patientId &&
                            a.AppointmentDate == today &&
                            a.Status == AppointmentStatus.Booked)
                .Include(a => a.Doctor)
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name
                })
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        // Get the booked appointments for the doctor
        public async Task<List<AppointmentListDto>> GetTodayAndUpcomingByDoctorAsync(int doctorId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.AppointmentDate >= today &&
                    a.Status == AppointmentStatus.Booked)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name
                })
                .ToListAsync();
        }

        // Get the booked appointments for the patient
        public async Task<List<AppointmentListDto>> GetTodayAndUpcomingByPatientAsync(int patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return await _context.Appointments
                .Where(a =>
                    a.PatientId == patientId &&
                    a.AppointmentDate >= today &&
                    a.Status == AppointmentStatus.Booked)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name
                })
                .ToListAsync();
        }

        // Get appointment by id
        public async Task<Appointment?> GetByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        // Update appointment
        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        // Add new appointment
        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        // Check if same patient has already booked same doctor on the same date excluding a specific appointment
        public async Task<bool> IsPatientAlreadyBookedExcludingAppointmentAsync(int appointmentId,int doctorId,int patientId,DateOnly appointmentDate)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.AppointmentId != appointmentId &&
                a.DoctorId == doctorId &&
                a.PatientId == patientId &&
                a.AppointmentDate == appointmentDate &&
                a.Status != AppointmentStatus.Cancelled
            );
        }

        // Count how many patients already booked for this doctor/date/time slot excluding a specific appointment
        public async Task<int> GetSlotCountExcludingAppointmentAsync(int appointmentId,int doctorId,DateOnly appointmentDate,TimeOnly startTime)
        {
            return await _context.Appointments.CountAsync(a =>
                a.AppointmentId != appointmentId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate == appointmentDate &&
                a.StartTime == startTime &&
                a.Status == AppointmentStatus.Booked
            );
        }

        // Get all appointments for a specific doctor
        public async Task<List<AppointmentListDto>> GetAllByDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId) // ✅ ONLY FILTER
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.Name
                })
                .ToListAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
