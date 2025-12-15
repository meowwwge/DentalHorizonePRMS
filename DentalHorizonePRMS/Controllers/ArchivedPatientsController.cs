using AutoMapper;
using DentalHorizonePRMS.DTOs.Patients;
using DentalHorizonePRMS.Interfaces;
using DentalHorizonePRMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ArchivedPatientsController : ControllerBase
	{
		private readonly IArchivedPatientRepository _archivedPatientRepository;
		private readonly IMapper _mapper;

		public ArchivedPatientsController(IArchivedPatientRepository archivedPatientRepository, IMapper mapper)
		{
			_archivedPatientRepository = archivedPatientRepository;
			_mapper = mapper;
		}

		[HttpPut("{id}/soft-delete")]
		public async Task<IActionResult> SoftDeletePatient(int id) 
		{
			await _archivedPatientRepository.SoftDeleteAsync(id);
			return Ok(new { message = "Patient archived successfully." });
		}

		[HttpPut("{id}/restore")]
		public async Task<IActionResult> RestorePatient(int id)
		{
			await _archivedPatientRepository.RestorePatientAsync(id);
			return Ok(new { message = "Patient restored successfully." });
		}

		[HttpGet("archived-patients")]
		public async Task<ActionResult<List<PatientDTO>>> GetArchivedAsync()
		{
			var archived = await _archivedPatientRepository.GetArchivedPatientsAsync();
			return Ok(archived);
		}

	}
}
