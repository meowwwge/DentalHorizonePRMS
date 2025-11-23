using Dapper;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DentalHorizonePRMS.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
       
        public async Task<User?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM [User] WHERE Id = @Id";
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
                return user;
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM [User] WHERE Username = @Username";
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = username });
                return user;
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM [User] WHERE Username = @Username and IsActive = 1";
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = username });

                if (user is null) return false;

                var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

                if (isValid) 
                {
                    var updateQuery = @"UPDATE [User] SET LastLogin = @Now WHERE Id = @Id";
                    await connection.ExecuteAsync(updateQuery, new { Now = DateTime.Now, Id = user.Id });
                } 

                return isValid;
            }
        }
    }
}
