using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IPatientMedicalHistoryRepository
    {
        Task<int> AddMedicalHistoryAsync(PatientMedicalHistory patientMedicalHistory);
        Task<List<PatientMedicalHistory>> GetByPatientIdAsync(int patientId);
    }
}
