using DentalHorizonePRMS.Entities;

namespace DentalHorizonePRMS.Interfaces
{
    public interface IUserRepository
    {
        
        Task<User?> GetByIdAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<User?> GetByUsernameAsync(string username);


    }
}
