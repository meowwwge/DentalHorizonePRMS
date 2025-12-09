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
        Task<bool> SoftDeletePatientAsync(int id);
        Task<bool> RestorePatientAsync(int id);
        Task<Patient?> GetByIdAsync(int id);
        Task<List<Patient>> GetAllPatientsAsync();
        Task<List<Patient>> GetAllActivePatientsAsync();
        Task<DashboardTotalsDTO> GetDashboardTotalsAsync();
        Task<IEnumerable<UpcomingAppointmentsDTO>> GetUpcomingAppointmentsAsync();
        Task<IEnumerable<MissedAppointmentsDTO>> GetMissedAppointmentsAsync();
        Task<bool> CancelAppointmentAsync(int id);
		Task<IEnumerable<Patient>> GetPatientsByDateAsync(int? month, int? year);
        Task<IEnumerable<Patient>> GetInactivePatients();
        Task<bool> ReschedulePatientAsync(int id, DateTime nextAppointment);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string keyword, string status);

	}
}
