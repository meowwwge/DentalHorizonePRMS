using Dapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
    public class PatientMedicalHistoryRepository : IPatientMedicalHistoryRepository
    {
        private readonly string _connectionString;

        public PatientMedicalHistoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> AddAsync(PatientMedicalHistory patientMedicalHistory)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"INSERT INTO PatientMedicalHistory (PatientId, KnownAllergies, CurrentMedications, PrimaryPhysician, EmergencyContactName, EmergencyContactNumber)
                              VALUES (@PatientId, @KnownAllergies, @CurrentMedications, @PrimaryPhysician, @EmergencyContactName, @EmergencyContactNumber)
                              SELECT CAST(SCOPE_IDENTITY() AS INT)";

                return await connection.ExecuteScalarAsync<int>(query, patientMedicalHistory);
            }
        }

        public async Task<PatientMedicalHistory?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM PatientMedicalHistory WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<PatientMedicalHistory>(query, new { Id = id});
            }
        }

        public async Task<PatientMedicalHistory?> GetByPatientIdAsync(int patientId)
        {
          using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT * FROM PatientMedicalHistory WHERE PatientId = @PatientId";
                return await connection.QueryFirstOrDefaultAsync<PatientMedicalHistory>(query, new { PatientId = patientId });
            }
        }
    }
}
