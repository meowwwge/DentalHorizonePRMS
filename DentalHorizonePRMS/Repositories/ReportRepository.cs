using Dapper;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Contracts;

namespace DentalHorizonePRMS.Repositories
{
	public class ReportRepository : IReportRepository
	{
		private readonly string _connectionString;

		public ReportRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")!;
		}

		public async Task<IEnumerable<Patient>> GetPatientsByMonthYearAsync(int month, int year)
		{
			using var connection = new SqlConnection(_connectionString);
			var sql = @"
					  SELECT * FROM Patient
					  WHERE MONTH(DateOfVisit) = @Month 
						AND YEAR(DateOfVisit) = @Year
						AND PatientStatus = 'Active'"; 

			return await connection.QueryAsync<Patient>(sql, new { Month = month, Year = year });
		}

	}
}
