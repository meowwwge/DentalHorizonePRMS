using Dapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
    public class DashboardRecordRepository : IDashboardRecordRepository
    {
        private readonly string _connectionString;

        public DashboardRecordRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> GetMissedAppointmentsCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT COUNT(*)
                              FROM DashboardRecord
                              WHERE DateOfVist >= CAST(GETDATE() AS DATE)";

                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<int> GetTotalPatientsAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT COUNT(*)
                              FROM DashboardRecord
                              WHERE STATUS = 'Active'";

                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<List<DashboardRecordDTO>> GetAllPatientsAsync() 
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT dr.Id, dr.PatientId, p.PatientName, p.Age, p.Address, p.Telephone,
                                     dr.DateOfVisit, p.Complaint, dr.Debit, dr.Credit, dr.Balance, p.Status
                            FROM DashboardRecord dr
                            INNER JOIN Patient p ON dr.PatientId = p.Id
                            WHERE p.Status = 'Active'";

                var result = await connection.QueryAsync<DashboardRecordDTO>(query);
                return result.ToList();
            }
        }

        public Task<int> GetUpcomingAppointmentsCountAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT COUNT(*)
                              FROM DashboardRecord
                              WHERE DateOfVisit < CAST(GETDATE() AS DATE)
                              AND Credit = 0";

                return connection.ExecuteScalarAsync<int>(query);
                                    
            }
        }

        public async Task<List<DashboardRecordDTO>> GetUpcomingAppointmentsAsync() 
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT dr.Id, dr.PatientId,
                                     p.PatientName, p.Age, p.Address, p.Telephone
                                     dr.DateOfVisit, p.Complaint, dr.Debit, dr.Credit, dr.Balance, p.Status
                            FROM DashboardRecord dr
                            INNER JOIN Patient p ON dr.PatientId = p.Id
                            WHERE dr.DateOfVisit >= CAST(GETDATE() AS DATE)";

                var upcoming = await connection.QueryAsync<DashboardRecordDTO>(query);
                return upcoming.ToList();
            }
        }

        public async Task<List<DashboardRecordDTO>> GetMissedAppointmentsAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT dr.Id, dr.PatientId, p.PatientName, p.Age, p.Address, p.Telephone,
                                     dr.DateOfVisit, p.Complaint, dr.Debit, dr.Credit, dr.Balance, p.Status
                              FROM DashboardRecord dr
                              INNER JOIN Patient p ON dr.PatientId = p.Id
                              WHERE dr.DateOfVisit >= CAST(GETDATE() AS DATE)";

                var missed = await connection.QueryAsync<DashboardRecordDTO>(query);
                return missed.ToList();
            }

        }

        public async Task<DashboardRecordDTO?> GetByPatientIdAsync(int patientId)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM DashboardRecord WHERE PatientId = @PatientId";
                return await connection.QueryFirstOrDefaultAsync<DashboardRecordDTO>(query, new { PatientId = patientId });
            }
        }
    }
}
