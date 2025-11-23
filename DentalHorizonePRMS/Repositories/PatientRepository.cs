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

		private string DetermineStatus(DateTime? nextAppointment) 
		{
			if (nextAppointment == null) return "Active";
			if (nextAppointment > DateTime.Now) return "Upcoming";
			if (nextAppointment < DateTime.Now) return "Missed";
			return "Active";
		}
		public async Task<int> AddPatientAsync(Patient patient)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				patient.Status = DetermineStatus(patient.NextAppointment);

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
				var query = @"SELECT * FROM Patient WHERE Status = 'Active'";
				var result = await connection.QueryAsync<Patient>(query);

				return result.ToList();
			}
		}

		public async Task<List<Patient>> GetAllPatientsAsync()
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
				var query = @"UPDATE Patient SET Status = 'Inactive' WHERE Id = @Id";
				var softDeleted = await connection.ExecuteAsync(query, new { Id = id });
				return softDeleted > 0;
			}
		}

		public async Task<bool> RestorePatientAsync(int id)
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"UPDATE Patient SET Status = 'Active' WHERE Id = @Id";
				var restored = await connection.ExecuteAsync(query, new { Id = id });
				return restored > 0;
			}
		}

		public async Task<IEnumerable<PatientManagementDTO>> GetForManagementAsync()
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT 
								Id, PatientName, Telephone, Address, Complaint,
								NextAppointment, Service, VisitStatus, Status, PatientStatus
							  FROM Patient";

				return await connection.QueryAsync<PatientManagementDTO>(query);
;			}
		}
		public async Task<IEnumerable<PatientFinanceDTO>> GetForFinanceAsync()
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT
								Id, PatientName, Age, Occupation, Telephone, Address,
								DateOfVisit, Complaint, Service, Debit, Credit, Status
							  FROM Patient";

				return await connection.QueryAsync<PatientFinanceDTO>(query);
			}
		}
		public async Task<DashboardTotalsDTO> GetDashboardTotalsAsync()
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var totalPatientsQuery = @"SELECT COUNT(*) FROM Patient";
				var upcomingAppointmentsQuery = @"SELECT COUNT(*) FROM Patient WHERE NextAppointment > GETDATE()";
				var missedAppointmentsQuery = @"SELECT COUNT(*) FROM Patient WHERE NextAppointment < GETDATE() AND VisitStatus != 'Completed'";

				var totalPatients = await connection.ExecuteScalarAsync<int>(totalPatientsQuery);
				var upcomingAppointments = await connection.ExecuteScalarAsync<int>(upcomingAppointmentsQuery);
				var missedAppointments = await connection.ExecuteScalarAsync<int>(missedAppointmentsQuery);

				return new DashboardTotalsDTO
				{
					TotalPatients = totalPatients,
					UpcomingAppointments= upcomingAppointments,
					MissedAppointments= missedAppointments
				};
			}
		}

		public async Task<IEnumerable<UpcomingAppointmentsDTO>> GetUpcomingAppointmentsAsync()
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT
								Id, PatientName, Age, Telephone, NextAppointment, Service, Complaint, Status
							  FROM Patient
							  WHERE NextAppointment IS NOT NULL AND NextAppointment > GETDATE()";

				return await connection.QueryAsync<UpcomingAppointmentsDTO>(query);
			}
		}

		public async Task<IEnumerable<MissedAppointmentsDTO>> GetMissedAppointmentsAsync()
		{
			using (var connection = new SqlConnection(_connectionString)) 
			{
				var query = @"SELECT
								Id, PatientName, Age, Telephone, NextAppointment AS OriginalAppointmentDate, Service, Complaint, Status
							  FROM Patient
							  WHERE NextAppointment IS NOT NULL AND NextAppointment < GETDATE()
									AND VisitStatus != 'Completed'";

				return await connection.QueryAsync<MissedAppointmentsDTO>(query);
			}
		}
	}
}
