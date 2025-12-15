using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.Patients;
using DentalHorizonePRMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IPatientRepository
    {
        Task<int> AddPatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<Patient?> GetByIdAsync(int id);
        Task<List<Patient>> GetAllPatientsAsync();
        Task<IEnumerable<Patient>> GetAllActivePatientsAsync();
        Task<DashboardTotalsDTO> GetDashboardTotalsAsync();
        Task<IEnumerable<UpcomingAppointmentsDTO>> GetUpcomingAppointmentsAsync();
        Task<IEnumerable<MissedAppointmentsDTO>> GetMissedAppointmentsAsync();
        Task<bool> CancelAppointmentAsync(int id);
		Task<IEnumerable<Patient>> GetPatientsByDateAsync(int? month, int? year);
        Task<List<int>> GetAvailableYearsAsync();
        Task<bool> ReschedulePatientAsync(int id, DateTime nextAppointment);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string keyword, string status);
        Task<IEnumerable<Patient>> GetArchivedPatientsAsync();
	}
}
