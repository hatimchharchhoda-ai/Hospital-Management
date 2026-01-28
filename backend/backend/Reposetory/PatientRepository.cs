using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Reposetory
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<Patient?> UpdateAsync(
            int id, string name, string mobile, string password)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return null;

            patient.Name = name;
            patient.Mobile = mobile;
            patient.Password = password;

            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> MobileExistsAsync(string mobile)
        {
            return await _context.Patients.AnyAsync(p => p.Mobile == mobile);
        }


    }
}
