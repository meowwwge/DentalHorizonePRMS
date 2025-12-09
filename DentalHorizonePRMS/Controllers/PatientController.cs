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

        [HttpGet("all-patients")]
        public async Task<ActionResult<List<Patient>>> GetAllAsync([FromQuery] int? month, [FromQuery] int? year)
        {
            if (month.HasValue || year.HasValue) 
            {
                var filtered = await _patientRepository.GetPatientsByDateAsync(month, year);
                return Ok(filtered);
            }

            var patients = await _patientRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id) 
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return NotFound();

            var patientDto = _mapper.Map<PatientDTO>(patient);
            return Ok(patientDto);
        }

        [HttpGet("inactive-patients")]
        public async Task<IActionResult> GetInactivePatients() 
        {
            var patients = await _patientRepository.GetInactivePatients();
            return Ok(patients);
        }

		[HttpGet("search")]
		public async Task<IActionResult> Search([FromQuery] string keyword, [FromQuery] string status)
		{
			var results = await _patientRepository.SearchPatientsAsync(keyword, status);
			return Ok(results);
		}


		[HttpPost("create-patient")]
        public async Task<ActionResult<int>> CreateAsync([FromBody] PatientCreateDTO createDto) 
        {
            var patient = _mapper.Map<Patient>(createDto);
            var patientId = await _patientRepository.AddPatientAsync(patient);
            return Ok(patientId);
        }

        [HttpPut("{id}/update-patient")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] PatientDTO patientDTO) 
        {
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var existing = await _patientRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(patientDTO, existing);
            existing.Id = id;

			var ok = await _patientRepository.UpdatePatientAsync(existing);
			if (!ok)
				return StatusCode(500, "UpdatePatientAsync returned false (no rows affected).");

			// Return 200 with the updated entity to make debugging easier
			return Ok(_mapper.Map<PatientDTO>(existing));
		}

        [HttpPut("{id}/soft-delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id) 
        {
            var softDelete = await _patientRepository.SoftDeletePatientAsync(id);
            if (softDelete)
            {
                return Ok(new { message = "Patient marked as inactive." });
            }
            else 
            {
                return NotFound(new { message = "Patient not found."});
            }
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

		[HttpPut("{id}/cancel")]
		public async Task<IActionResult> CancelAppointment(int id)
		{
			var success = await _patientRepository.CancelAppointmentAsync(id);
			return success ? Ok() : BadRequest("Unable to cancel appointment.");
		}


		[HttpGet("dashboard-totals")]
        public async Task<ActionResult<DashboardTotalsDTO>> GetTotalsAsync() 
        {
            var dashboardTotals = await _patientRepository.GetDashboardTotalsAsync();
            return Ok(dashboardTotals);
        }

        [HttpPut("{id}/reschedule")]
        public async Task<IActionResult> RescheduleAsync(int id, [FromBody] DateTime nextAppointment) 
        {
            var ok = await _patientRepository.ReschedulePatientAsync(id, nextAppointment);
            if (!ok) return NotFound(new { message = "Patient not found or update failed." });

            return Ok(new { message = "Appointment rescheduled successfully.", nextAppointment });
        }

        
	}
}
