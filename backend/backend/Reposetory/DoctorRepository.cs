using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend.Reposetory
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailOrMobileAsync(string email, string mobile)
        {
            return await _context.Doctors
                .AnyAsync(d => d.Email == email || d.Mobile == mobile);
        }

        public async Task<Doctor?> GetDoctorByEmailOrMobileAsync(string identifier)
        {
            return await _context.Doctors
            .Where(d => d.Email == identifier || d.Mobile == identifier)
            .FirstOrDefaultAsync();
        }
    }
}
