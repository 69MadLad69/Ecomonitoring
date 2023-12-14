using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.Repositories
{
    public class PollutantRepository
    {
        string connectionString;
        public List<Pollutant> Pollutants = new List<Pollutant>();
        public PollutantRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void LoadPollutantsFromDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                Pollutants.Clear();

                connection.Open();

                string query = "SELECT * FROM pollutants";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Pollutant pollutant = new Pollutant
                            (
                                Convert.ToInt32(reader["pollutant_Id"]),
                                Convert.ToString(reader["pollutant_name"]),
                                Convert.ToString(reader["activity"]),
                                Convert.ToString(reader["ownership"]),
                                Convert.ToString(reader["address"]),
                                Convert.ToDouble(reader["Kf"]),
                                Convert.ToDouble(reader["Knas"])
                            );

                            Pollutants.Add(pollutant);
                        }
                    }
                }
            }
        }

        public bool AddNewPollutantToDB(Pollutant pollutant)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO pollutants (pollutant_Id, pollutant_name, activity, ownership, address, Kf, Knas) " +
                                       "VALUES (@pollutant_Id, @pollutantName, @activity, @ownership, @address, @Kf, @Knas)";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@pollutant_Id", pollutant.Pollutant_Id);
                            command.Parameters.AddWithValue("@pollutantName", pollutant.name);
                            command.Parameters.AddWithValue("@activity", pollutant.activity);
                            command.Parameters.AddWithValue("@ownership", pollutant.ownership);
                            command.Parameters.AddWithValue("@address", pollutant.address);
                            command.Parameters.AddWithValue("@Kf", pollutant.Kf);
                            command.Parameters.AddWithValue("@Knas", pollutant.Knas);

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

        public void UpdatePollutantsDB()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Pollutant pollutant in Pollutants)
                        {
                            string query = "UPDATE pollutants " +
                                           "SET pollutant_name = @newName, " +
                                           "    activity = @newActivity, " +
                                           "    ownership = @newOwnership, " +
                                           "    address = @newAddress, " +
                                           "    Kf = @newKf, " +
                                           "    Knas = @newKnas " +
                                           "WHERE pollutant_Id = @pollutantId";

                            using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@newName", pollutant.name);
                                command.Parameters.AddWithValue("@newActivity", pollutant.activity);
                                command.Parameters.AddWithValue("@newOwnership", pollutant.ownership);
                                command.Parameters.AddWithValue("@newAddress", pollutant.address);
                                command.Parameters.AddWithValue("@newKf", pollutant.Kf);
                                command.Parameters.AddWithValue("@newKnas", pollutant.Knas);
                                command.Parameters.AddWithValue("@pollutantId", pollutant.Pollutant_Id);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void DeletePollutantFromDB(Pollutant pollutant)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM pollutants WHERE pollutant_Id = @pollutantId";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@pollutantId", pollutant.Pollutant_Id);

                            command.ExecuteNonQuery();

                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public bool PollutantExists(string pollutantName)
        {
            foreach (Pollutant poll in Pollutants)
            {
                if (poll.name.Equals(pollutantName))
                {
                    return true;
                }
            }

            return false;
        }

        public int FindPollutantId(string pollutantName)
        {
            foreach (Pollutant poll in Pollutants)
            {
                if (poll.name == pollutantName)
                {
                    return poll.Pollutant_Id;
                }
            }

            return -1;
        }

        public Pollutant GetPollutant(int id)
        {
            foreach (Pollutant pol in Pollutants)
            {
                if (pol.Pollutant_Id == id)
                {
                    return pol;
                }
            }
            return null;
        }

        public void ClearPollutantsTable(MySqlConnection connection)
            {
                try
                {
                    using (connection) 
                    {

                        string query = "DELETE FROM pollutants";

                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
    }
}
