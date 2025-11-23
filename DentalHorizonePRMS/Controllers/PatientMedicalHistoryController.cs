using AutoMapper;
using DentalHorizonePRMS.DTOs.PatientMedicalHistory;
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
        private readonly IPatientMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IMapper _mapper;

        public PatientMedicalHistoryController(IPatientMedicalHistoryRepository medicalHistoryRepository, IMapper mapper)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
            _mapper = mapper;
        }

        [HttpPost("add-history")]
        public async Task<ActionResult<int>> AddAsync([FromBody] PatientMedicalHistoryDTO historyDto) 
        {
            var history = _mapper.Map<PatientMedicalHistory>(historyDto);
            history.CreatedAt = DateTime.Now;

            var id = await _medicalHistoryRepository.AddMedicalHistoryAsync(history);
            return Ok(id);
		}


        [HttpGet("{patientId}")]
        public async Task<ActionResult<List<PatientMedicalHistory>>> GetByPatientIdAsync(int patientId) 
        {
            var history = await _medicalHistoryRepository.GetByPatientIdAsync(patientId);
            return Ok(history);
        }
    }
}
