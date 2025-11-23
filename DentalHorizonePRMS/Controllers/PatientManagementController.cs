using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientManagementController : ControllerBase
    {
        private readonly IPatientManagementRepository _patientManagementRepository;

        public PatientManagementController(IPatientManagementRepository patientManagementRepository)
        {
            _patientManagementRepository = patientManagementRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientManagementDTO>>> GetAllPatients() 
        {
            var patients = await _patientManagementRepository.GetAllAsync();
            if (patients is null) return NotFound();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientManagementDTO>> GetById(int id) 
        {
            var patientId = await _patientManagementRepository.GetByIdAsync(id);
            if (patientId == null) return NotFound();
            return Ok(patientId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, PatientManagementDTO patientManagementDTO) 
        {
            if (id != patientManagementDTO.Id) return BadRequest();
            var success = await _patientManagementRepository.UpdateAsync(patientManagementDTO);
            return success ? Ok(success) : BadRequest();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestorePatient(int id) 
        {
            var success = await _patientManagementRepository.RestoreAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeletePatient(int id) 
        {
            var success = await _patientManagementRepository.SoftDeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
