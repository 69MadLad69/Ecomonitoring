using Ecomonitoring.RisksCalasses.NonCarcinogenicRisk;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.Repositories
{
    public class NonCarcinogenicRiskRepository
    {
        private readonly string connectionString;

        public List<NonCarcinogenicRisk> NonCarcinogenicRisks { get; set; }

        public NonCarcinogenicRiskRepository(string connectionString)
        {
            this.connectionString = connectionString;
            NonCarcinogenicRisks = new List<NonCarcinogenicRisk>();
        }

        public void LoadNonCarcinogenicRisksFromDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                NonCarcinogenicRisks.Clear();

                connection.Open();

                string query = "SELECT * FROM NonCarcinogenicRisks";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            NonCarcinogenicRisk nonCarcinogenicRisk = new NonCarcinogenicRisk
                            (
                                Convert.ToInt32(reader["NCR_Id"]),
                                Convert.ToInt32(reader["pollution_Id"]),
                                Convert.ToDouble(reader["HQ"])
                            );

                            NonCarcinogenicRisks.Add(nonCarcinogenicRisk);
                        }
                    }
                }
            }
        }

        public bool AddNewNonCarcinogenicRiskToDB(NonCarcinogenicRisk nonCarcinogenicRisk)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO NonCarcinogenicRisks (NCR_Id, pollution_Id, HQ) " +
                                       "VALUES (@riskId, @pollution_Id, @HQ)";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@riskId", nonCarcinogenicRisk.riskId);
                            command.Parameters.AddWithValue("@pollution_Id", nonCarcinogenicRisk.pollution_Id);
                            command.Parameters.AddWithValue("@HQ", nonCarcinogenicRisk.HQ);

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

        public bool UpdateNonCarcinogenicRiskInDB(NonCarcinogenicRisk nonCarcinogenicRisk)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "UPDATE NonCarcinogenicRisks " +
                                       "SET pollution_Id = @pollution_Id, " +
                                       "    HQ = @HQ " +
                                       "WHERE NCR_Id = @NCR_Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@pollution_Id", nonCarcinogenicRisk.pollution_Id);
                            command.Parameters.AddWithValue("@HQ", nonCarcinogenicRisk.HQ);
                            command.Parameters.AddWithValue("@NCR_Id", nonCarcinogenicRisk.riskId);

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

        public bool DeleteNonCarcinogenicRiskFromDB(NonCarcinogenicRisk nonCarcinogenicRisk)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM NonCarcinogenicRisks WHERE NCR_Id = @NCR_Id";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@NCR_Id", nonCarcinogenicRisk.riskId);

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

        public NonCarcinogenicRisk GetNCR(int id)
        {
            foreach (NonCarcinogenicRisk NCR in NonCarcinogenicRisks)
            {
                if (NCR.riskId == id)
                {
                    return NCR;
                }
            }
            return null;
        }

        public List<NonCarcinogenicRisk> findNCRsWithPollutionID(int pollutionId) {
            List<NonCarcinogenicRisk> risks = new List<NonCarcinogenicRisk> ();

            foreach (NonCarcinogenicRisk NCR in NonCarcinogenicRisks) {
                if (NCR.pollution_Id == pollutionId) {
                    risks.Add(NCR);
                }
            }

            return risks;
        }
    }
}
