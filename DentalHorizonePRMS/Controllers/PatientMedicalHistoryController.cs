using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMedicalHistoryController : ControllerBase
    {
        private readonly IPatientMedicalHistoryRepository _patientMedicalHistoryRepository;

        public PatientMedicalHistoryController(IPatientMedicalHistoryRepository patientMedicalHistoryRepository)
        {
            _patientMedicalHistoryRepository = patientMedicalHistoryRepository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientMedicalHistory>> GetById(int id) 
        {
            var patientHistory = await _patientMedicalHistoryRepository.GetByIdAsync(id);
            if (patientHistory == null) return NotFound();
            return Ok(patientHistory);
        }

        [HttpPost("medical-history")]
        public async Task<ActionResult<int>> AddMedicalHistory(PatientMedicalHistory patientMedicalHistory) 
        {
            var newId = await _patientMedicalHistoryRepository.AddAsync(patientMedicalHistory);
            return CreatedAtAction(nameof(GetById), new { Id = newId }, newId);
        }
    }
}
