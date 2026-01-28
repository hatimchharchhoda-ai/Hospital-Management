using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IPatientDoctorService _patientDoctorService;
        private readonly IAppointmentService _appointmentService;

        public PatientController(
            IPatientService patientService,
            IPatientDoctorService patientDoctorService,
            IAppointmentService appointmentService)
        {
            _patientService = patientService;
            _patientDoctorService = patientDoctorService;
            _appointmentService = appointmentService;
        }

        // 🔹 Get all patients
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllUserAsync();
            return Ok(patients);
        }

        // 🔹 Get patient by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return Ok(patient);
        }

        // 🔹 Update patient
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePatientDto dto)
        {
            var updated = await _patientService.UpdateAsync(id, dto.Name, dto.Mobile, dto.Password);
            return Ok(updated);
        }

        // 🔹 Check mobile
        [HttpGet("check-mobile/{mobile}")]
        public async Task<IActionResult> CheckMobile(string mobile)
        {
            var exists = await _patientService.MobileExistsAsync(mobile);
            return Ok(new { exists });
        }

        // 🔹 Add doctor
        [HttpPost("add-doctor")]
        public async Task<IActionResult> AddDoctor([FromBody] PatientAddDoctorDto dto)
        {
            var result = await _patientService.AddDoctorAsync(dto.PatientId, dto.DoctorIdentifier);
            return Ok(result);
        }

        // 🔹 Get doctors by patient
        [HttpGet("{patientId}/doctors")]
        public async Task<IActionResult> GetDoctorsByPatient(int patientId)
        {
            var doctors = await _patientDoctorService.GetDoctorsByPatientIdAsync(patientId);
            return Ok(doctors);
        }

        // 🔹 Book appointment
        [HttpPost("bookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(
                request.DoctorId,
                request.PatientId,
                request.AppointmentDate,
                request.StartTime
            );
            return Ok(appointment);
        }

        // 🔹 Get today's appointments for patient
        [HttpGet("{patientId}/today")]
        public async Task<IActionResult> GetPatientTodayAppointments(int patientId)
        {
            var result = await _appointmentService.GetTodayAppointmentsForPatientAsync(patientId);
            return Ok(result);
        }

        // 🔹 Get upcoming appointments for patient
        [HttpGet("{patientId}/appointments/upcoming")]
        public async Task<IActionResult> GetTodayAndUpcomingAppointments(int patientId)
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(patientId);
            return Ok(appointments);
        }

        // 🔹 Update appointment for patient
        [HttpPut("appointments/update")]
        public async Task<IActionResult> UpdateAppointmentForPatient([FromBody] UpdateAppointmentRequest request)
        {
            await _appointmentService.UpdateAppointmentForPatientAsync(request);
            return Ok(new { message = "Appointment updated successfully." });
        }
    }
}