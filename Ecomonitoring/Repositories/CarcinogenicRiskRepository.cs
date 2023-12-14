using Ecomonitoring.RisksCalasses.CarcinogenicRisk;
using Ecomonitoring.RisksCalasses.NonCarcinogenicRisk;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.Repositories
{
    public class CarcinogenicRiskRepository
    {
        private readonly string connectionString;

        public List<CarcinogenicRisk> CarcinogenicRisks { get; set; }

        public CarcinogenicRiskRepository(string connectionString)
        {
            this.connectionString = connectionString;
            CarcinogenicRisks = new List<CarcinogenicRisk>();
        }

        public void LoadCarcinogenicRisksFromDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                CarcinogenicRisks.Clear();

                connection.Open();

                string query = "SELECT * FROM CarcinogenicRisks";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarcinogenicRisk carcinogenicRisk = new CarcinogenicRisk
                            (
                                Convert.ToInt32(reader["CR_Id"]),
                                Convert.ToInt32(reader["pollution_Id"]),
                                Convert.ToDouble(reader["CR_TimeOut"]),
                                Convert.ToDouble(reader["CR_TimeIn"]),
                                Convert.ToDouble(reader["Vout"]),
                                Convert.ToDouble(reader["Vin"]),
                                Convert.ToDouble(reader["EF"]),
                                Convert.ToDouble(reader["ED"]),
                                Convert.ToDouble(reader["BW"]),
                                Convert.ToDouble(reader["LADD"]),
                                Convert.ToDouble(reader["CR"])
                            );

                            CarcinogenicRisks.Add(carcinogenicRisk);
                        }
                    }
                }
            }
        }

        public bool AddNewCarcinogenicRiskToDB(CarcinogenicRisk carcinogenicRisk)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO CarcinogenicRisks (CR_Id, pollution_Id, CR_TimeOut, CR_TimeIn, Vout, Vin, EF, ED, BW, LADD, CR) " +
                                       "VALUES (@riskId, @pollution_Id, @CR_TimeOut, @CR_TimeIn, @Vout, @Vin, @EF, @ED, @BW, @LADD, @CR)";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@riskId", carcinogenicRisk.riskId);
                            command.Parameters.AddWithValue("@pollution_Id", carcinogenicRisk.pollution_Id);
                            command.Parameters.AddWithValue("@CR_TimeOut", carcinogenicRisk.TimeOut);
                            command.Parameters.AddWithValue("@CR_TimeIn", carcinogenicRisk.TimeIn);
                            command.Parameters.AddWithValue("@Vout", carcinogenicRisk.Vout);
                            command.Parameters.AddWithValue("@Vin", carcinogenicRisk.Vin);
                            command.Parameters.AddWithValue("@EF", carcinogenicRisk.EF);
                            command.Parameters.AddWithValue("@ED", carcinogenicRisk.ED);
                            command.Parameters.AddWithValue("@BW", carcinogenicRisk.BodyWeight);
                            command.Parameters.AddWithValue("@LADD", carcinogenicRisk.LADD);
                            command.Parameters.AddWithValue("@CR", carcinogenicRisk.CR);

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

        public void ClearCarcinogenicRisksTable()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM CarcinogenicRisks";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    // Handle exception
                }
            }
        }

        public bool UpdateCarcinogenicRiskInDB(CarcinogenicRisk carcinogenicRisk)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE CarcinogenicRisks " +
                                       "SET CR_TimeOut = @CR_TimeOut, " +
                                       "    CR_TimeIn = @CR_TimeIn, " +
                                       "    Vout = @Vout, " +
                                       "    Vin = @Vin, " +
                                       "    EF = @EF, " +
                                       "    ED = @ED, " +
                                       "    BW = @BW, " +
                                       "    LADD = @LADD, " +
                                       "    CR = @CR " +
                                       "WHERE CR_Id = @CR_Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CR_Id", carcinogenicRisk.riskId);
                            command.Parameters.AddWithValue("@CR_TimeOut", carcinogenicRisk.TimeOut);
                            command.Parameters.AddWithValue("@CR_TimeIn", carcinogenicRisk.TimeIn);
                            command.Parameters.AddWithValue("@Vout", carcinogenicRisk.Vout);
                            command.Parameters.AddWithValue("@Vin", carcinogenicRisk.Vin);
                            command.Parameters.AddWithValue("@EF", carcinogenicRisk.EF);
                            command.Parameters.AddWithValue("@ED", carcinogenicRisk.ED);
                            command.Parameters.AddWithValue("@BW", carcinogenicRisk.BodyWeight);
                            command.Parameters.AddWithValue("@LADD", carcinogenicRisk.LADD);
                            command.Parameters.AddWithValue("@CR", carcinogenicRisk.CR);

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

        public bool DeleteCarcinogenicRiskFromDB(int riskId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM CarcinogenicRisks WHERE CR_Id = @CR_Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@CR_Id", riskId);

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

        public CarcinogenicRisk GetCarcinogenicRisk(int riskId)
        {
            return CarcinogenicRisks.FirstOrDefault(risk => risk.riskId == riskId);
        }

        public List<CarcinogenicRisk> findCRsWithPollutionID(int pollutionId) {
            List<CarcinogenicRisk> risks = new List<CarcinogenicRisk>();

            foreach (CarcinogenicRisk CR in CarcinogenicRisks)
            {
                if (CR.pollution_Id == pollutionId)
                {
                    risks.Add(CR);
                }
            }

            return risks;
        }
    }
}
