using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;

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
            try
            {
                return Ok(await _patientService.GetAllUserAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Get patient by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var patient = await _patientService.GetByIdAsync(id);
                if (patient == null) return NotFound();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Update patient
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePatientDto dto)
        {
            try
            {
                var updated = await _patientService.UpdateAsync(
                    id, dto.Name, dto.Mobile, dto.Password);

                if (updated == null) return NotFound();

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Check mobile
        [HttpGet("check-mobile/{mobile}")]
        public async Task<IActionResult> CheckMobile(string mobile)
        {
            try
            {
                return Ok(new
                {
                    exists = await _patientService.MobileExistsAsync(mobile)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Add doctor
        [HttpPost("add-doctor")]
        public async Task<IActionResult> AddDoctor([FromBody] PatientAddDoctorDto dto)
        {
            try
            {
                if (dto.PatientId <= 0 || string.IsNullOrWhiteSpace(dto.DoctorIdentifier))
                    return BadRequest(false);

                var result = await _patientService.AddDoctorAsync(
                    dto.PatientId,
                    dto.DoctorIdentifier
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Get doctors by patient
        [HttpGet("{patientId}/doctors")]
        public async Task<IActionResult> GetDoctorsByPatient(int patientId)
        {
            try
            {
                var doctors = await _patientDoctorService.GetDoctorsByPatientIdAsync(patientId);
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Book appointment
        [HttpPost("bookAppointment")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(
                    request.DoctorId,
                    request.PatientId,
                    request.AppointmentDate,
                    request.StartTime
                );

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Get today's appointments for patient
        [HttpGet("{patientId}/today")]
        public async Task<IActionResult> GetPatientTodayAppointments(int patientId)
        {
            try
            {
                var result = await _appointmentService
                    .GetTodayAppointmentsForPatientAsync(patientId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Get upcoming appointments for patient
        [HttpGet("{patientId}/appointments/upcoming")]
        public async Task<IActionResult> GetTodayAndUpcomingAppointments(int patientId)
        {
            var appointments =
                await _appointmentService.GetUpcomingAppointmentsForPatientAsync(patientId);

            return Ok(appointments);
        }

        // 🔹 Update appointment for patient
        [HttpPut("appointments/update")]
        public async Task<IActionResult> UpdateAppointmentForPatient(
        [FromBody] UpdateAppointmentRequest request)
        {
            try
            {
                await _appointmentService.UpdateAppointmentForPatientAsync(request);

                return Ok(new
                {
                    message = "Appointment updated successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}