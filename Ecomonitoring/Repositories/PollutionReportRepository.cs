using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.Repositories
{
    public class PollutionReportRepository
    {
        private readonly string connectionString;

        public List<Pollution_Report> PollutionReports { get; set; }

        public PollutionReportRepository(string connectionString)
        {
            this.connectionString = connectionString;
            PollutionReports = new List<Pollution_Report>();
        }
        public void LoadPollutionReportsFromDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PollutionReports.Clear();

                connection.Open();

                string query = "SELECT * FROM pollution";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pollution_Report pollutionReport = new Pollution_Report
                            (
                                Convert.ToInt32(reader["pollution_Id"]),
                                Convert.ToInt32(reader["pollutant_Id"]),
                                Convert.ToInt32(reader["substance_Id"]),
                                Convert.ToDouble(reader["ammount"]),
                                Convert.ToDouble(reader["avrage_concentration"]),
                                Convert.ToString(reader["report_year"]),
                                Convert.ToDouble(reader["T"]),
                                Convert.ToDouble(reader["excessive_mass"]),
                                Convert.ToDouble(reader["compensation_ammount"]),
                                Convert.ToDouble(reader["taxYearAmmount"]),
                                Convert.ToDouble(reader["roi"])
                            );

                            PollutionReports.Add(pollutionReport);
                        }
                    }
                }
            }
        }

        public bool AddNewPollutionReportToDB(Pollution_Report pollutionReport)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO pollution (pollution_Id, pollutant_Id, substance_Id, ammount, avrage_concentration, report_year, T, excessive_mass, compensation_ammount, taxYearAmmount, roi) " +
                                       "VALUES (@pollution_Id, @pollutant_Id, @substance_Id, @ammount, @avrage_concentration, @report_year, @T, @excessive_mass, @compensation_ammount, @taxYearAmmount, @roi)";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@pollution_Id", pollutionReport.Pollution_Id);
                            command.Parameters.AddWithValue("@pollutant_Id", pollutionReport.Pollutant_Id);
                            command.Parameters.AddWithValue("@substance_Id", pollutionReport.Substance_Id);
                            command.Parameters.AddWithValue("@ammount", pollutionReport.ammount);
                            command.Parameters.AddWithValue("@avrage_concentration", pollutionReport.CA);
                            command.Parameters.AddWithValue("@report_year", pollutionReport.report_year);
                            command.Parameters.AddWithValue("@T", pollutionReport.T);
                            command.Parameters.AddWithValue("@excessive_mass", pollutionReport.excessiveMass);
                            command.Parameters.AddWithValue("@compensation_ammount", pollutionReport.compensationAmmount);
                            command.Parameters.AddWithValue("@taxYearAmmount", pollutionReport.taxYearAmmount);
                            command.Parameters.AddWithValue("@roi", pollutionReport.roi);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool UpdatePollutionReportsInDB()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Pollution_Report pollutionReport in PollutionReports)
                        {
                            string query = "UPDATE pollution " +
                                           "SET pollutant_Id = @pollutant_Id, " +
                                           "    substance_Id = @substance_Id, " +
                                           "    ammount = @ammount, " +
                                           "    avrage_concentration = @avrage_concentration, " +
                                           "    report_year = @report_year, " +
                                           "    T = @T, " +
                                           "    excessive_mass = @excessive_mass, " +
                                           "    compensation_ammount = @compensation_ammount, " +
                                           "    taxYearAmmount = @taxYearAmmount, " +
                                           "    roi = @roi " +
                                           "WHERE pollution_Id = @pollution_Id";

                            using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@pollution_Id", pollutionReport.Pollution_Id);
                                command.Parameters.AddWithValue("@pollutant_Id", pollutionReport.Pollutant_Id);
                                command.Parameters.AddWithValue("@substance_Id", pollutionReport.Substance_Id);
                                command.Parameters.AddWithValue("@ammount", pollutionReport.ammount);
                                command.Parameters.AddWithValue("@avrage_concentration", pollutionReport.CA);
                                command.Parameters.AddWithValue("@report_year", pollutionReport.report_year);
                                command.Parameters.AddWithValue("@T", pollutionReport.T);
                                command.Parameters.AddWithValue("@excessive_mass", pollutionReport.excessiveMass);
                                command.Parameters.AddWithValue("@compensation_ammount", pollutionReport.compensationAmmount);
                                command.Parameters.AddWithValue("@taxYearAmmount", pollutionReport.taxYearAmmount);
                                command.Parameters.AddWithValue("@roi", pollutionReport.roi);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }


        public bool DeletePollutionReportFromDB(Pollution_Report pollutionReport)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM pollution WHERE pollution_Id = @pollution_Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@pollution_Id", pollutionReport.Pollution_Id);

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public Pollution_Report GetPollution_Report(int pollutionId) {
            foreach (Pollution_Report polRep in PollutionReports) {
                if (polRep.Pollution_Id == pollutionId) { 
                    return polRep;
                }
            }

            return null;
        }

        public List<Pollution_Report> findPollutionRepotsWithPolId(int polId) {
            List<Pollution_Report> polReps = new List<Pollution_Report>();
            foreach (Pollution_Report polRep in PollutionReports) {
                if (polRep.Pollutant_Id == polId) {
                    polReps.Add(polRep);
                }
            }

            return polReps;
        }

        public List<Pollution_Report> findPollutionReportsWithSubId(int subId)
        {
            List<Pollution_Report> polReps = new List<Pollution_Report>();
            foreach (Pollution_Report polRep in PollutionReports)
            {
                if (polRep.Substance_Id == subId)
                {
                    polReps.Add(polRep);
                }
            }

            return polReps;
        }

        public void ClearPollutionReportsTable()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM pollution";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
