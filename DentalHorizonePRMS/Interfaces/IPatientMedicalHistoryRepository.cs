using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IPatientMedicalHistoryRepository
    {
        Task<int> AddAsync(PatientMedicalHistory patientMedicalHistory);
        Task<PatientMedicalHistory?> GetByIdAsync(int id);
        Task<PatientMedicalHistory?> GetByPatientIdAsync(int id);
    }
}
