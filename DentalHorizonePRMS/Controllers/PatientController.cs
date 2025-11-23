using AutoMapper;
using DentalHorizonePRMS.DTOs.Dashboard;
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
        private readonly IMapper _mapper;

        public PatientController(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        [HttpGet("patient-management")]
        public async Task<ActionResult<IEnumerable<PatientManagementDTO>>> GetPatientManagementAsync()
        {
            var patients = await _patientRepository.GetForManagementAsync();
            return Ok(patients);
        }

        [HttpGet("finance")]
        public async Task<ActionResult<IEnumerable<PatientFinanceDTO>>> GetPatientFinanceAsync()
        {
            var patients = await _patientRepository.GetForFinanceAsync();
            return Ok(patients);
        }

        [HttpGet("all-patients")]
        public async Task<ActionResult<List<Patient>>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpPost("create-patient")]
        public async Task<ActionResult<int>> CreateAsync([FromBody] PatientCreateDTO createDto) 
        {
            var patient = _mapper.Map<Patient>(createDto);
            var patientId = await _patientRepository.AddPatientAsync(patient);
            return Ok(patientId);
        }

        [HttpPut("{id}/update-patient")]
        public async Task<IActionResult> UpdateAsync([FromBody] PatientDTO patientDTO, int id) 
        {
            var existing = await _patientRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(patientDTO, existing);
            existing.Id = id;

            var ok = await _patientRepository.UpdatePatientAsync(existing);
            return ok ? NoContent() : StatusCode(500);
        }

        [HttpPost("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id) 
        {
            var softDelete = await _patientRepository.SoftDeletePatientAsync(id);
            return softDelete ? NoContent() : NotFound();
        }

        [HttpPost("{id}/restore-patient")]
        public async Task<IActionResult> RestoreAsync(int id) 
        {
            var restore = await _patientRepository.RestorePatientAsync(id);
            return restore ? NoContent() : NotFound();
        }

        [HttpGet("upcoming-appointments")]
        public async Task<ActionResult<IEnumerable<UpcomingAppointmentsDTO>>> GetUpcomingAsync() 
        {
            var upcomingAppointments = await _patientRepository.GetUpcomingAppointmentsAsync();
            return Ok(upcomingAppointments);
        }

		[HttpGet("missed-appointments")]
		public async Task<ActionResult<IEnumerable<MissedAppointmentsDTO>>> GetMissedAsycn()
		{
			var missedAppointments = await _patientRepository.GetMissedAppointmentsAsync();
			return Ok(missedAppointments);
		}

        [HttpGet("dashboard-totals")]
        public async Task<ActionResult<DashboardTotalsDTO>> GetTotalsAsync() 
        {
            var dashboardTotals = await _patientRepository.GetDashboardTotalsAsync();
            return Ok(dashboardTotals);
        }
	}
}
