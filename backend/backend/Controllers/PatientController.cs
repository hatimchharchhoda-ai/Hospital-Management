using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IPatientDoctorService _patientDoctorService;
        private readonly IAppointmentService _appointmentService;
        private readonly IPrescriptionService _prescriptionService;

        public PatientController(
            IPatientService patientService,
            IPatientDoctorService patientDoctorService,
            IAppointmentService appointmentService,
            IPrescriptionService prescriptionService)
        {
            _patientService = patientService;
            _patientDoctorService = patientDoctorService;
            _appointmentService = appointmentService;
            _prescriptionService = prescriptionService;
        }

        // 🔹 Get all patients
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllUserAsync();
            return Ok(patients);
        }

        // 🔹 Get patient by id
        [Authorize(Roles = "Patient")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return Ok(patient);
        }

        // 🔹 Update patient
        [Authorize(Roles = "Patient")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePatientDto dto)
        {
            var updated = await _patientService.UpdateAsync(id, dto.Name, dto.Mobile, dto.Password);
            return Ok(updated);
        }

        // 🔹 Check mobile
        [AllowAnonymous]
        [HttpGet("check-mobile/{mobile}")]
        public async Task<IActionResult> CheckMobile(string mobile)
        {
            var exists = await _patientService.MobileExistsAsync(mobile);
            return Ok(new { exists });
        }

        // 🔹 Add doctor
        [Authorize(Roles = "Patient")]
        [HttpPost("add-doctor")]
        public async Task<IActionResult> AddDoctor([FromBody] PatientAddDoctorDto dto)
        {
            var result = await _patientService.AddDoctorAsync(dto.PatientId, dto.DoctorIdentifier);
            return Ok(result);
        }

        // 🔹 Get doctors by patient
        [Authorize(Roles = "Patient")]
        [HttpGet("{patientId}/doctors")]
        public async Task<IActionResult> GetDoctorsByPatient(int patientId)
        {
            var doctors = await _patientDoctorService.GetDoctorsByPatientIdAsync(patientId);
            return Ok(doctors);
        }

        // 🔹 Book appointment
        [Authorize(Roles = "Patient")]
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
        [Authorize(Roles = "Patient")]
        [HttpGet("{patientId}/today")]
        public async Task<IActionResult> GetPatientTodayAppointments(int patientId)
        {
            var result = await _appointmentService.GetTodayAppointmentsForPatientAsync(patientId);
            return Ok(result);
        }

        // 🔹 Get upcoming appointments for patient
        [Authorize(Roles = "Patient")]
        [HttpGet("{patientId}/appointments/upcoming")]
        public async Task<IActionResult> GetTodayAndUpcomingAppointments(int patientId)
        {
            var appointments = await _appointmentService.GetUpcomingAppointmentsForPatientAsync(patientId);
            return Ok(appointments);
        }

        // 🔹 Update appointment for patient
        [Authorize(Roles = "Patient")]
        [HttpPut("appointments/update")]
        public async Task<IActionResult> UpdateAppointmentForPatient([FromBody] UpdateAppointmentRequest request)
        {
            await _appointmentService.UpdateAppointmentForPatientAsync(request);
            return Ok(new { message = "Appointment updated successfully." });
        }

        // 🧑‍🦱 Get all prescriptions for this patient
        [Authorize(Roles = "Patient")]
        [HttpGet("{patientId}/prescriptions/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByPatient(int patientId, int doctorId)
        {
            var prescriptions = await _prescriptionService
                .GetByPatientIdAsync(patientId, doctorId);

            return Ok(prescriptions);
        }

        // 🧑‍🦱 Get single prescription
        [Authorize(Roles = "Patient")]
        [HttpGet("prescription/{prescriptionId}")]
        public async Task<IActionResult> GetPrescription(int prescriptionId)
        {
            var prescription = await _prescriptionService
                .GetByIdAsync(prescriptionId);

            return Ok(prescription);
        }
    }
}