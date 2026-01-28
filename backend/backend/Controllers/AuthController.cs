using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IDoctorService _doctorService;

        public AuthController(IAuthService authService, IDoctorService doctorService)
        {
            _authService = authService;
            _doctorService = doctorService;
        }

        // ---------------- PATIENT ----------------
        [HttpPost("patient/login")]
        [AllowAnonymous]
        public async Task<IActionResult> PatientLogin([FromBody] LoginPatientDto dto)
        {
            var token = await _authService.LoginPatientAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("patient/register")]
        [AllowAnonymous]
        public async Task<IActionResult> PatientRegister([FromBody] Patient patient)
        {
            var created = await _authService.RegisterPatientAsync(patient);
            return Ok(created);
        }

        // ---------------- DOCTOR ----------------
        [HttpPost("doctor/login")]
        [AllowAnonymous]
        public async Task<IActionResult> DoctorLogin([FromBody] LoginDoctorDto dto)
        {
            var token = await _authService.LoginDoctorAsync(dto);
            return Ok(new { token });
        }

        [HttpPost("doctor/register")]
        [AllowAnonymous]
        public async Task<IActionResult> DoctorRegister([FromBody] Doctor doctor)
        {
            // This call will throw AppException if doctor cannot register
            var canRegister = await _doctorService.VerifyDoctorCanRegisterAsync(doctor.Email, doctor.Mobile);

            var created = await _authService.RegisterDoctorAsync(doctor);
            return Ok(created);
        }
    }
}