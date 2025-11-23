using DentalHorizonePRMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetAllAsync();
        Task<List<Patient>> GetAllActiveAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<int> AddAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
    }
}
