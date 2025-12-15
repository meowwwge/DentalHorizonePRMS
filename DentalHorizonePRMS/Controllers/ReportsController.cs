using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ReportsController : ControllerBase
	{
		private readonly IReportRepository _reportRepository;

		public ReportsController(IReportRepository reportRepository)
		{
			_reportRepository = reportRepository;	
		}

		[HttpGet("monthly-patients")]
		public async Task<ActionResult<IEnumerable<Patient>>> GetMonthlyPatients([FromQuery] int month, [FromQuery] int year)
		{
			var patients = await _reportRepository.GetPatientsByMonthYearAsync(month, year);
			return Ok(patients);
		}
	}
}
