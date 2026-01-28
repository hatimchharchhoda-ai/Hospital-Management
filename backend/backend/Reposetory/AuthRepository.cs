using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend.Reposetory
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------- Patients (FUTURE) ----------------
        public async Task<Patient> AddPatientAsync(Patient patient)
        {
            // Hash the password before saving
            patient.Password = BCrypt.Net.BCrypt.HashPassword(patient.Password);

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        public async Task<Patient?> GetPatientByMobileAsync(string mobile)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Mobile == mobile);

            if (patient == null) return null;

            // Return only safe info
            return new Patient
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Mobile = patient.Mobile,
                Password = patient.Password
            };
        }


        // ---------------- DOCTOR (FUTURE) ----------------

        
        public async Task<Doctor> RegisterDoctorAsync(Doctor doctor)
        {
            doctor.Password = BCrypt.Net.BCrypt.HashPassword(doctor.Password);

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return doctor;
        }

        public async Task<Doctor?> GetByEmailOrMobileAsync(string identifier)
        {
            return await _context.Doctors.FirstOrDefaultAsync(d =>
                d.Email == identifier || d.Mobile == identifier);
        }
        
    }
}
