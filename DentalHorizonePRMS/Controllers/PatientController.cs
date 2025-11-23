using AutoMapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.DTOs.PatientMedicalHistory;
using DentalHorizonePRMS.DTOs.Patients;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using DentalHorizonePRMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IPatientManagementRepository _patientManagementRepository;
        private readonly IDashboardRecordRepository _dashboardRecordRepository;
        private readonly IPatientMedicalHistoryRepository _medicalHistoryRepository;
        private readonly IMapper _mapper;

        public PatientController(IPatientRepository patientRepository, IPatientManagementRepository patientManagementRepository, IDashboardRecordRepository dashboardRecordRepository, IPatientMedicalHistoryRepository medicalHistoryRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _patientManagementRepository = patientManagementRepository;
            _dashboardRecordRepository = dashboardRecordRepository;
            _medicalHistoryRepository = medicalHistoryRepository;
            _mapper = mapper;
        }

        [HttpGet("full/{id}")]
        public async Task<ActionResult<FullPatientResponseDTO>> GetFullPatientAsync(int id) 
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var patientManagement = await _patientManagementRepository.GetByPatientIdAsync(id);
            var dashboardRecord = await _dashboardRecordRepository.GetByPatientIdAsync(id);
            var medicalHistory = await _medicalHistoryRepository.GetByPatientIdAsync(id);


            var response = new FullPatientResponseDTO
            {
                Patient = _mapper.Map<PatientDTO>(patient),
                PatientManagement = _mapper.Map<PatientManagementDTO>(patientManagement),
                DashboardRecord = _mapper.Map<DashboardRecordDTO>(dashboardRecord),
                PatientMedicalHistory = _mapper.Map<PatientMedicalHistoryDTO>(medicalHistory)
            };

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<List<Patient>> GetPatient() 
        {
            var patient = await _patientRepository.GetAllAsync();
            return patient;
        }

        [HttpPost("create-patient")]
        public async Task<ActionResult<int>> CreatePatient([FromBody] FullPatientDetailsDTO createDTO) 
        {
            
		    var patient = _mapper.Map<Patient>(createDTO);
            var patientId = await _patientRepository.AddAsync(patient);

            var managePatient = _mapper.Map<PatientManagementDTO>(createDTO);
            managePatient.PatientId = patientId;
            await _patientManagementRepository.AddAsync(managePatient);

			return Ok(patientId);

		}
	}
}
