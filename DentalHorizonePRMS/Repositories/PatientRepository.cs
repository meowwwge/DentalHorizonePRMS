using Dapper;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> AddAsync(Patient patient)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"INSERT INTO Patient (PatientName, Address, Telephone, Age, Occupation, Status, Complaint)
                              VALUES (@PatientName, @Address, @Telephone, @Age, @Occupation, @Status, @Complaint)
                              SELECT CAST(SCOPE_IDENTITY() AS INT)";

                return await connection.ExecuteScalarAsync<int>(query, patient);
                                 
            }
        }

        public async Task<List<Patient>> GetAllActiveAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM Patient WHERE Status = 'Active'";
                var result = await connection.QueryAsync<Patient>(query);
                return result.ToList();
            }
        }

        public async Task<List<Patient>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM Patient";
                var result = await connection.QueryAsync<Patient>(query);
                return result.ToList();
            }
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM Patient WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<Patient>(query, new { Id = id});
            }
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"UPDATE Patient
                              SET PatientName = @PatientName,
                                  Address = @Address,
                                  Telephone = @Telephone,
                                  Age = @Age,
                                  Occupation = @Occupation,
                                  Status = @Status,
                                  Complaint = @Complaint
                              WHERE Id = @Id";
                var updated = await connection.ExecuteAsync(query, patient);
                return updated > 0;
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"UPDATE Patient
                              SET Status = 'Inactive'
                              WHERE Id = @Id";
                var softDeleted = await connection.ExecuteAsync(query, new { Id = id});
                return softDeleted > 0;
            }
        }

        public async Task<bool> RestoreAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"UPDATE Patient
                              SET Status = 'Active'
                              WHERE Id = @Id";
                var restored = await connection.ExecuteAsync(query, new { Id = id });
                return restored > 0;
            }
        }
    }
}
