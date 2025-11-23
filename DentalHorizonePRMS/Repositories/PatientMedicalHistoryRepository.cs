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

		public async Task<int> AddMedicalHistoryAsync(PatientMedicalHistory patientMedicalHistory)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"INSERT INTO PatientMedicalHistory (
									PatientId, KnownAllergies, CurrentMedications, ChronicConditions,
									PrimaryPhysicianOrDentist, EmergencyContactName, EmergencyContactNumber, CreatedAt)
							  VALUES (
									@PatientId, @KnownAllergies, @CurrentMedications, @ChronicConditions,
									@PrimaryPhysicianOrDentist, @EmergencyContactName, @EmergencyContactNumber, @CreatedAt);
							  SELECT CAST(SCOPE_IDENTITY() AS INT);";

				return await connection.ExecuteScalarAsync<int>(query, patientMedicalHistory);
			}
		}

		public async Task<List<PatientMedicalHistory>> GetByPatientIdAsync(int patientId)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT * FROM PatientMedicalHistory WHERE PatientId = @PatientId";
				var patient = await connection.QueryAsync<PatientMedicalHistory>(query, new { PatientId = patientId });

				return patient.ToList();
			}
		}
	}
}
