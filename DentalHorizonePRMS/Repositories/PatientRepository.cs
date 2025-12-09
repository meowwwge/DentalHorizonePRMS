using Dapper;
using DentalHorizonePRMS.DTOs.Dashboard;
using DentalHorizonePRMS.DTOs.Patients;
using DentalHorizonePRMS.Entities;
using DentalHorizonePRMS.Interfaces;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

namespace DentalHorizonePRMS.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _connectionString;

        public PatientRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

		private string DetermineStatus(DateTime? nextAppointment, bool isCancelled = false)
		{
			if (isCancelled) return "Cancelled";
			if (nextAppointment == null) return "No appointment"; // ✅ clearer than "-"
			if (nextAppointment < DateTime.Now) return "Missed";
			return "Upcoming";
		}


		//private string DetermineVisitStatus(DateTime dateOfVisit, string status)
		//{
		//	// If the visit date has passed and it's not missed, mark as Completed
		//	if (dateOfVisit <= DateTime.Now && status != "Missed") return "Completed";
		//	return "Pending";
		//}

		public async Task<int> AddPatientAsync(Patient patient)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				patient.Status = DetermineStatus(patient.NextAppointment);
				//patient.VisitStatus = DetermineVisitStatus(patient.DateOfVisit, patient.Status);

				var query = @"INSERT INTO Patient(
                        PatientName, Address, Telephone, Age, Occupation, Complaint,
                        DateOfVisit, NextAppointment, Service, VisitStatus, Status,
                        PatientStatus, Debit, Credit)
                      VALUES(
                        @PatientName, @Address, @Telephone, @Age, @Occupation, @Complaint,
                        @DateOfVisit, @NextAppointment, @Service, @VisitStatus, @Status,
                        @PatientStatus, @Debit, @Credit);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);";

				return await connection.ExecuteScalarAsync<int>(query, patient);
			}
		}

		public async Task<List<Patient>> GetAllActivePatientsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"SELECT * FROM Patient WHERE PatientStatus = 'Active'";
				var result = await connection.QueryAsync<Patient>(query);

				return result.ToList();
			}
		}

		public async Task<List<Patient>> GetAllPatientsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"SELECT *,
                            CASE
								WHEN Status = 'Cancelled' THEN 'Cancelled'
								WHEN NextAppointment IS NULL THEN 'No appointment'
								WHEN CAST(NextAppointment AS DATE) < CAST(GETDATE() AS DATE) THEN 'Missed'
								WHEN CAST(NextAppointment AS DATE) >= CAST(GETDATE() AS DATE) THEN 'Upcoming'
								ELSE Status
							END AS ComputedStatus

                      FROM Patient";

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

		public async Task<bool> UpdatePatientAsync(Patient patient)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				patient.Status = DetermineStatus(patient.NextAppointment);

				var query = @"UPDATE PATIENT
							  SET
								 PatientName = @PatientName,
								 Address = @Address,
								 Telephone = @Telephone,
								 Age = @Age,
								 Occupation = @Occupation,
								 Complaint = @Complaint,
								 DateOfVisit = @DateOfVisit,
								 NextAppointment = @NextAppointment,
								 Service = @Service,
								 VisitStatus = @VisitStatus,
								 Status = @Status,
								 PatientStatus = @PatientStatus,
								 Debit = @Debit,
								 Credit = @Credit
							WHERE Id = @Id";

				var patientUpdated = await connection.ExecuteAsync(query, patient);
				return patientUpdated > 0;
			}
		}

		public async Task<bool> SoftDeletePatientAsync(int id)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"UPDATE Patient SET PatientStatus = 'Inactive' WHERE Id = @Id";
				var softDeleted = await connection.ExecuteAsync(query, new { Id = id });
				return softDeleted > 0;
			}
		}

		public async Task<bool> RestorePatientAsync(int id)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"UPDATE Patient SET PatientStatus = 'Active' WHERE Id = @Id";
				var restored = await connection.ExecuteAsync(query, new { Id = id });
				return restored > 0;
			}
		}

		public async Task<DashboardTotalsDTO> GetDashboardTotalsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var totalPatientsQuery = @"SELECT COUNT(*) FROM Patient";

				var upcomingAppointmentsQuery = @"
												SELECT COUNT(*) 
												FROM Patient 
												WHERE NextAppointment IS NOT NULL
												  AND CAST(NextAppointment AS DATE) >= CAST(GETDATE() AS DATE)
												  AND VisitStatus <> 'Completed'
												  AND Status <> 'Cancelled'";

				var missedAppointmentsQuery = @"
											  SELECT COUNT(*) 
											  FROM Patient 
											  WHERE NextAppointment IS NOT NULL
												  AND CAST(NextAppointment AS DATE) < CAST(GETDATE() AS DATE)
												  AND VisitStatus <> 'Completed'
												  AND Status <> 'Cancelled'";

				var cancelledAppointmentsQuery = @"
												 SELECT COUNT(*) 
												 FROM Patient 
												 WHERE Status = 'Cancelled'";

				var noAppointmentsQuery = @"
										  SELECT COUNT(*) 
										  FROM Patient 
										  WHERE NextAppointment IS NULL 
										   AND Status <> 'Cancelled'";

				var totalPatients = await connection.ExecuteScalarAsync<int>(totalPatientsQuery);
				var upcomingAppointments = await connection.ExecuteScalarAsync<int>(upcomingAppointmentsQuery);
				var missedAppointments = await connection.ExecuteScalarAsync<int>(missedAppointmentsQuery);
				var cancelledAppointments = await connection.ExecuteScalarAsync<int>(cancelledAppointmentsQuery);
				var noAppointments = await connection.ExecuteScalarAsync<int>(noAppointmentsQuery);

				return new DashboardTotalsDTO
				{
					TotalPatients = totalPatients,
					UpcomingAppointments = upcomingAppointments,
					MissedAppointments = missedAppointments,
					CancelledAppointments = cancelledAppointments,
					NoAppointments = noAppointments
				};
			}
		}

		public async Task<IEnumerable<UpcomingAppointmentsDTO>> GetUpcomingAppointmentsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"
							SELECT Id, PatientName, Age, Telephone, NextAppointment, Service, Complaint, Status
							FROM Patient
							WHERE NextAppointment IS NOT NULL
							  AND CAST(NextAppointment AS DATE) >= CAST(GETDATE() AS DATE)
							  AND Status <> 'Cancelled'";

				return await connection.QueryAsync<UpcomingAppointmentsDTO>(query);
			}
		}

		public async Task<IEnumerable<MissedAppointmentsDTO>> GetMissedAppointmentsAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"
							SELECT Id, PatientName, Age, Telephone, NextAppointment AS OriginalAppointmentDate, Service, Complaint,
							CASE 
								WHEN VisitStatus = 'Completed' THEN 'Completed'
								WHEN CAST(NextAppointment AS DATE) < CAST(GETDATE() AS DATE) THEN 'Missed'
								ELSE 'Upcoming'
							END AS Status
							FROM Patient
							WHERE NextAppointment IS NOT NULL
							  AND CAST(NextAppointment AS DATE) < CAST(GETDATE() AS DATE)
							  AND Status <> 'Cancelled'";

				return await connection.QueryAsync<MissedAppointmentsDTO>(query);
			}
		}

		public async Task<bool> CancelAppointmentAsync(int id)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"UPDATE Patient SET Status = 'Cancelled' WHERE Id = @Id";
				var result = await connection.ExecuteAsync(query, new { Id = id });
				return result > 0;
			}
		}

		public async Task<IEnumerable<Patient>> GetPatientsByDateAsync(int? month, int? year)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT *,
										CASE
											WHEN NextAppointment < GETDATE() AND VisitStatus <> 'Completed' THEN 'Missed'
											WHEN NextAppointment >= GETDATE() THEN 'Upcoming'
											ELSE Status
										END AS Status
							  FROM Patient 
							  WHERE 1=1";

				if (month.HasValue && year.HasValue)
				{
					query += " AND MONTH(DateOfVisit) = @Month AND YEAR(DateOfVisit) = @Year";
				}
				else if (year.HasValue) 
				{
					query += " AND YEAR(DateOfVisit) = @Year";
				}

				return await connection.QueryAsync<Patient>(query, new { Month = month, Year = year });
			}
		}

		public async Task<IEnumerable<Patient>> GetInactivePatients() 
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT * FROM Patient WHERE PatientStatus = 'Inactive'";
				var result = await connection.QueryAsync<Patient>(query);
				return result.ToList();
				
			}
		}

		public async Task<bool> ReschedulePatientAsync(int id, DateTime nextAppointment)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"
							UPDATE Patient
							SET NextAppointment = @NextAppointment,
								Status = CASE
											WHEN @NextAppointment <= GETDATE() THEN 'Missed'
											ELSE 'Upcoming'
										  END
							WHERE Id = @Id";

				var rowsAffected = await connection.ExecuteAsync(query, new { Id = id, NextAppointment = nextAppointment });
				return rowsAffected > 0;
			}
		}

		public async Task<IEnumerable<Patient>> SearchPatientsAsync(string keyword, string status)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var query = @"SELECT * FROM Patient 
                      WHERE PatientStatus = 'Active'
                      AND Status = @Status
                      AND PatientName LIKE '%' + @Keyword + '%'";

				return await connection.QueryAsync<Patient>(query, new { Keyword = keyword, Status = status });
			}
		}
	}
}
