using Dapper;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
	public class ArchivedPatientRepository : IArchivedPatientRepository
	{
		private readonly string _connectionString;
		public ArchivedPatientRepository(IConfiguration configuration) 
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")!;
		}

		public async Task SoftDeleteAsync(int patientId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"UPDATE Patient
                      SET PatientStatus = 'Inactive',
                          Status = 'Cancelled'
                      WHERE Id = @Id";

				await connection.ExecuteAsync(query, new { Id = patientId });
			}
		}

		public async Task<IEnumerable<Patient>> GetArchivedPatientsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var sql = @"SELECT * FROM Patient 
							WHERE PatientStatus = 'Inactive' AND Status = 'Cancelled'";
				return await connection.QueryAsync<Patient>(sql);
			}
		}

		public async Task RestorePatientAsync(int patientId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"UPDATE Patient
                      SET PatientStatus = 'Active',
                          Status = 'Upcoming'
                      WHERE Id = @Id";

				var rows = await connection.ExecuteAsync(query, new { Id = patientId });
			}
		}

	}
}
