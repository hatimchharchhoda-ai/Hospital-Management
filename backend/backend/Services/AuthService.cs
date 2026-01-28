using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<string?> LoginPatientAsync(LoginPatientDto dto);
        Task<Patient?> RegisterPatientAsync(Patient patient);

        // DOCTOR (FUTURE)
        Task<string?> LoginDoctorAsync(LoginDoctorDto dto);
        Task<Doctor?> RegisterDoctorAsync(Doctor doctor);
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
        }

        // ---------------- PATIENT ----------------

        public async Task<string?> LoginPatientAsync(LoginPatientDto dto)
        {
            var patient = await _authRepository.GetPatientByMobileAsync(dto.Mobile);
            if (patient == null) return null;

            // Verify password
            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, patient.Password);
            if (!validPassword) return null;

            return GenerateJwt(patient.PatientId.ToString(), patient.Name, "Patient");
        }

        public async Task<Patient?> RegisterPatientAsync(Patient patient)
        {
            return await _authRepository.AddPatientAsync(patient);
        }

        // ---------------- DOCTOR (FUTURE) ----------------
        
        public async Task<string?> LoginDoctorAsync(LoginDoctorDto dto)
        {
            // Fetch doctor by email OR mobile
            var doctor = await _authRepository.GetByEmailOrMobileAsync(dto.Identifier);

            if (doctor == null) return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, doctor.Password))
                return null;

            // Generate JWT (use FullName, not Name)
            return GenerateJwt(
                doctor.DoctorId.ToString(),
                doctor.FullName,
                "Doctor"
    );
        }

        public async Task<Doctor?> RegisterDoctorAsync(Doctor doctor)
        {
            return await _authRepository.RegisterDoctorAsync(doctor);
        }
        

        private string GenerateJwt(string id, string name, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role)
                }),     
                Expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpireMinutes"])
                ),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
