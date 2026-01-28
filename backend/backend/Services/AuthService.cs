using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<string> LoginPatientAsync(LoginPatientDto dto);
        Task<Patient> RegisterPatientAsync(Patient patient);

        // DOCTOR (FUTURE)
        Task<string> LoginDoctorAsync(LoginDoctorDto dto);
        Task<Doctor> RegisterDoctorAsync(Doctor doctor);
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

        public async Task<string> LoginPatientAsync(LoginPatientDto dto)
        {
            var patient = await _authRepository.GetPatientByMobileAsync(dto.Mobile);
            if (patient == null)
                throw new AppException("Patient not found with the provided mobile number.");

            bool validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, patient.Password);
            if (!validPassword)
                throw new AppException("Invalid password.");

            return GenerateJwt(patient.PatientId.ToString(), patient.Name, "Patient");
        }

        public async Task<Patient> RegisterPatientAsync(Patient patient)
        {
            var existingPatient = await _authRepository.GetPatientByMobileAsync(patient.Mobile);
            if (existingPatient != null)
                throw new AppException("Mobile number is already registered.");

            return await _authRepository.AddPatientAsync(patient);
        }

        // ---------------- DOCTOR (FUTURE) ----------------

        public async Task<string> LoginDoctorAsync(LoginDoctorDto dto)
        {
            var doctor = await _authRepository.GetByEmailOrMobileAsync(dto.Identifier);
            if (doctor == null)
                throw new AppException("Doctor not found with the provided identifier.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, doctor.Password))
                throw new AppException("Invalid password.");

            return GenerateJwt(doctor.DoctorId.ToString(), doctor.FullName, "Doctor");
        }

        public async Task<Doctor> RegisterDoctorAsync(Doctor doctor)
        {
            var existingDoctor = await _authRepository.GetByEmailOrMobileAsync(doctor.Email ?? doctor.Mobile);
            if (existingDoctor != null)
                throw new AppException("Email or mobile is already registered.");

            return await _authRepository.RegisterDoctorAsync(doctor);
        }

        // ---------------- JWT GENERATION ----------------

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