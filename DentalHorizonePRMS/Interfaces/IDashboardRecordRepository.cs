using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IDashboardRecordRepository
    {
        Task<int> GetTotalPatientsAsync();
        Task<int> GetUpcomingAppointmentsCountAsync();
        Task<int> GetMissedAppointmentsCountAsync();
        Task<List<DashboardRecordDTO>> GetUpcomingAppointmentsAsync();
        Task<List<DashboardRecordDTO>> GetMissedAppointmentsAsync();
        Task<List<DashboardRecordDTO>> GetAllPatientsAsync();
        Task<DashboardRecordDTO?> GetByPatientIdAsync(int patientId);
    }
}
