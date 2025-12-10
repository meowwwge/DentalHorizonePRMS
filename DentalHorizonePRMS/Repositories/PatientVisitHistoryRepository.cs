using Dapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
    public class PatientVisitHistoryRepository : IPatientVisitHistory
    {
        private readonly string _connectionString;

        public PatientVisitHistoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

	}
}
