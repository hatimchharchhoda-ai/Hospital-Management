using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IPatientDoctorService _patientDoctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPrescriptionService _prescriptionService;

        public DoctorController(
            IDoctorService doctorService,
            IPatientDoctorService patientDoctorService,
            IAppointmentService appointmentService,
            IPrescriptionService prescriptionService)
        {
            _doctorService = doctorService;
            _patientDoctorService = patientDoctorService;
            _appointmentService = appointmentService;
            _prescriptionService = prescriptionService;
        }

        // 🔹 Verify doctor
        [HttpGet("/verify")]
        public async Task<IActionResult> Verify([FromQuery] string email, [FromQuery] string mobile)
        {
            var canRegister = await _doctorService.VerifyDoctorCanRegisterAsync(email, mobile);
            return Ok(new { isAvailable = canRegister });
        }

        // 🔹 Get doctor id from email/mobile
        [HttpGet("lookup")]
        public async Task<IActionResult> GetDoctorId([FromQuery] string identifier)
        {
            var doctorId = await _doctorService.GetDoctorIdAsync(identifier);
            return Ok(new { doctorId });
        }

        // 🔹 Get all patients for a doctor
        [HttpGet("{doctorId}/patients")]
        public async Task<IActionResult> GetPatientsByDoctor(int doctorId)
        {
            var patients = await _patientDoctorService.GetPatientsByDoctorIdAsync(doctorId);
            return Ok(patients);
        }

        // 🔹 Get today's appointments for doctor
        [HttpGet("{doctorId}/today")]
        public async Task<IActionResult> GetDoctorTodayAppointments(int doctorId)
        {
            var result = await _appointmentService.GetTodayAppointmentsForDoctorAsync(doctorId);
            return Ok(result);
        }

        // 🔹 Get today's and upcoming appointments for doctor
        [HttpGet("{doctorId}/appointments/upcoming")]
        public async Task<IActionResult> GetTodayAndUpcomingAppointments(int doctorId)
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsForDoctorAsync(doctorId);
            return Ok(appointments);
        }

        // 🔹 Doctor update appointment
        [HttpPut("appointments/update")]
        public async Task<IActionResult> DoctorUpdateAppointment([FromBody] DoctorUpdateAppointmentRequest request)
        {
            await _appointmentService.DoctorUpdateAppointmentAsync(request);
            return Ok(new { message = "Appointment updated successfully." });
        }

        // 🔹 Create prescription
        [HttpPost("createPrescription")]
        public async Task<IActionResult> CreatePrescription(
           [FromForm] CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prescription = await _prescriptionService.CreateAsync(dto);

            return Ok(prescription);
        }

        // 👨‍⚕️ Get all prescriptions created by this doctor of particular patient
        [HttpGet("{doctorId}/prescriptions/{patientId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctor(int doctorId, int patientId )
        {
            var prescriptions = await _prescriptionService
                .GetByDoctorIdAsync(doctorId, patientId);

            return Ok(prescriptions);
        }

        // 👨‍⚕️ Get single prescription
        [HttpGet("prescription/{prescriptionId}")]
        public async Task<IActionResult> GetPrescription(int prescriptionId)
        {
            var prescription = await _prescriptionService
                .GetByIdAsync(prescriptionId);

            return Ok(prescription);
        }

        // 🔹 Get all appointments for a doctor
        [HttpGet("{doctorId}/appointments/all")]
        public async Task<IActionResult> GetAllAppointmentsByDoctor(int doctorId)
        {
            var appointments = await _appointmentService.GetDoctorAppointmentsAsync(doctorId);
            return Ok(appointments);
        }
    }
}