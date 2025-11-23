using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.Interfaces;
using DentalHorizonePRMS.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardRecordController : ControllerBase
    {
        private readonly IDashboardRecordRepository _dashboardRecordRepository;

        public DashboardRecordController(IDashboardRecordRepository dashboardRecordRepository)
        {
            _dashboardRecordRepository = dashboardRecordRepository;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDTO>> GetSummary() 
        {
            var dashboardSummary = new DashboardSummaryDTO()
            {
                TotalPatients = await _dashboardRecordRepository.GetTotalPatientsAsync(),
                UpcomingAppointments = await _dashboardRecordRepository.GetUpcomingAppointmentsCountAsync(),
                MissedAppointments = await _dashboardRecordRepository.GetMissedAppointmentsCountAsync()
            };

            return Ok(dashboardSummary);
        }

        [HttpGet("appointments/upcoming")]
        public async Task<ActionResult<List<DashboardRecordDTO>>> GetUpcomingAppointments() 
        {
            var upcomingRecords = await _dashboardRecordRepository.GetUpcomingAppointmentsAsync();
            return Ok(upcomingRecords);
        }

        [HttpGet("appointments/missed")]
        public async Task<ActionResult<List<DashboardRecordDTO>>> GetMissedAppointments() 
        {
            var missedRecords = await _dashboardRecordRepository.GetMissedAppointmentsAsync();
            return Ok(missedRecords);
        }

        [HttpGet("patients")]
        public async Task<ActionResult<List<DashboardRecordDTO>>> GetAllPatients() 
        {
            var allPatients = await _dashboardRecordRepository.GetAllPatientsAsync();
            return Ok(allPatients);
        }

    }
}
