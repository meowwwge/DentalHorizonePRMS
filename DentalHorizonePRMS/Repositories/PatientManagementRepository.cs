using Dapper;
using DentalHorizonePRMS.DTOs.PatientManagement;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;

namespace DentalHorizonePRMS.Repositories
{
    public class PatientManagementRepository : IPatientManagementRepository
    {
        private readonly string _connectionString;

        public PatientManagementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
       
        public async Task<List<PatientManagementDTO>> GetAllAsync()
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT pm.Id, pm.PatientId, p.PatientName, p.Age, p.Occupation, p.Telephone, p.Address, p.Complaint,
                                     pm.NextAppointment, pm.PatientStatus, pm.Service,
                                     CASE
                                         WHEN pm.NextAppointment < CAST(GETDATE() AS DATE) THEN 'Missed'
                                         WHEN pm.NextAppointment = CAST(GETDATE() AS DATE) THEN 'Today'
                                         ELSE 'Upcoming'
                                     END AS VisitStatus
                               FROM PatientManagement pm
                               INNER JOIN Patient p ON pm.PatientId = p.Id";

                var result = await connection.QueryAsync<PatientManagementDTO>(query);
                return result.ToList();
            }
        }

        public async Task<PatientManagementDTO?> GetByIdAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT pm.Id, pm.PatientId, p.PatientName, p.Age, p.Occupation, p.Telephone, p.Address, p.Complaint,
                                     pm.NextAppointment, pm.PatientStatus, pm.Service,
                                     CASE
                                         WHEN pm.NextAppointment < CAST(GETDATE() AS DATE) THEN 'Missed'
                                         WHEN pm.NextAppointment = CAST(GETDATE() AS DATE) THEN 'Today'
                                         ELSE 'Upcoming'
                                     END AS VisitStatus
                               FROM PatientManagement pm
                               INNER JOIN Patient p ON pm.PatientId = p.Id
                               WHERE pm.Id = @Id";

                return await connection.QueryFirstOrDefaultAsync<PatientManagementDTO>(query, new { Id = id });
            }
        }

        public async Task<int> AddAsync(PatientManagementDTO patientManagementDTO)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"INSERT INTO PatientManagement (PatientId, LastVisit, NextAppointment, PatientStatus, Service)
                              VALUES (@PatientId, @LastVisit, @NextAppointment, @PatientStatus, @Service);
                              SELECT CAST(SCOPE_IDENTITY() AS INT)";

                return await connection.ExecuteScalarAsync<int>(query, patientManagementDTO);
            }
        }

        public async Task<bool> UpdateAsync(PatientManagementDTO patientManagementDTO)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"UPDATE PatientManagement
                              SET NextAppointment = @NextAppointment,
                                  PatientStatus = @PatientStatus
                              WHERE Id = @Id";

                var updated = await connection.ExecuteAsync(query, patientManagementDTO);
                return updated > 0;
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"UPDATE PatientManagement
                              SET PatientStatus = 'Inactive'
                              WHERE Id = @Id";

                var softDeleted = await connection.ExecuteAsync(query, new { Id = id });
                return softDeleted > 0;
            }
        }

        public async Task<bool> RestoreAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"UPDATE PatientManagement
                              SET PatientStatus = 'Active'
                              WHERE Id = @Id";

                var restored = await connection.ExecuteAsync(query, new { Id = id });
                return restored > 0;
            }
        }

        public async Task<PatientManagementDTO?> GetByPatientIdAsync(int patientId)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var query = @"SELECT * FROM PatientManagement WHERE PatientId = @PatientId";
                return await connection.QueryFirstOrDefaultAsync<PatientManagementDTO>(query, new { PatientId = patientId });
            }
        }
    }
}
