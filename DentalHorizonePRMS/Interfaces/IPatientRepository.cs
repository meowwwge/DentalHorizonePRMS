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
        Task<IEnumerable<PatientManagementDTO>> GetForManagementAsync();
        Task<IEnumerable<PatientFinanceDTO>> GetForFinanceAsync();
        Task<DashboardTotalsDTO> GetDashboardTotalsAsync();
        Task<IEnumerable<UpcomingAppointmentsDTO>> GetUpcomingAppointmentsAsync();
        Task<IEnumerable<MissedAppointmentsDTO>> GetMissedAppointmentsAsync();
    }
}
