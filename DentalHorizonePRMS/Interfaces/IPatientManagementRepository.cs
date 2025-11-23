using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IPatientManagementRepository
    {
        Task<List<PatientManagementDTO>> GetAllAsync();
        Task<PatientManagementDTO?> GetByIdAsync(int id);
        Task<int> AddAsync(PatientManagementDTO patientManagementDTO);
        Task<bool> UpdateAsync(PatientManagementDTO patientManagementDTO);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
        Task<PatientManagementDTO?> GetByPatientIdAsync(int patientId);
    }
}
