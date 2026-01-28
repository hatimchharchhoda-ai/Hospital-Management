using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientDoctorController : ControllerBase
    {
        private readonly IPatientDoctorService _service;

        public PatientDoctorController(IPatientDoctorService service)
        {
            _service = service;
        }

        /// Link a patient with a doctor
        [HttpPost("link")]
        public async Task<IActionResult> LinkPatientDoctor([FromBody] PatientDoctorLinkDto dto)
        {
            if (dto.PatientId <= 0 || dto.DoctorId <= 0)
                return BadRequest("PatientId and DoctorId are required");

            try
            {
                var mapping = await _service.CreateMappingAsync(dto.PatientId, dto.DoctorId);
                return Ok(new
                {
                    mapping.PatientDoctorId,
                    mapping.PatientId,
                    mapping.DoctorId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
