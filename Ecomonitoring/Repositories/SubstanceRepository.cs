using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring.Repositories
{
    public class SubstanceRepository
    {
        private readonly string connectionString;

        public List<Substance> Substances { get; set; }

        public SubstanceRepository(string connectionString)
        {
            this.connectionString = connectionString;
            Substances = new List<Substance>();
        }
        public void LoadSubstancesFromDatabase()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                Substances.Clear();

                connection.Open();

                string query = "SELECT * FROM substances";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Substance substance = new Substance
                            (
                                Convert.ToInt32(reader["substance_Id"]),
                                Convert.ToString(reader["substance_name"]),
                                Convert.ToDouble(reader["TLK"]),
                                Convert.ToDouble(reader["RfC"]),
                                Convert.ToDouble(reader["SF"]),
                                Convert.ToInt32(reader["cancerogenic"]),
                                Convert.ToString(reader["danger_class"]),
                                Convert.ToDouble(reader["roNorm"]),
                                Convert.ToDouble(reader["qv"]),
                                Convert.ToString(reader["taxType"]),
                                Convert.ToDouble(reader["taxAmmount"])
                            );

                            Substances.Add(substance);
                        }
                    }
                }
            }
        }

        public bool AddNewSubstanceToDB(Substance substance)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO substances (substance_Id, substance_name, TLK, RfC, SF, " +
                                       "cancerogenic, danger_class, roNorm, qv, taxType, taxAmmount) " +
                                       "VALUES (@substance_Id, @substanceName, @tLK, @RfC, @SF, " +
                                       "@cancerogenic, @dangerClass, @roNorm, @qv, @taxType, @taxAmmount)";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@substance_Id", substance.Substance_Id);
                            command.Parameters.AddWithValue("@substanceName", substance.name);
                            command.Parameters.AddWithValue("@tLK", substance.TLK);
                            command.Parameters.AddWithValue("@RfC", substance.RfC);
                            command.Parameters.AddWithValue("@SF", substance.SF);
                            command.Parameters.AddWithValue("@cancerogenic", substance.cancerogenic);
                            command.Parameters.AddWithValue("@dangerClass", substance.danger_class);
                            command.Parameters.AddWithValue("@roNorm", substance.roNorm);
                            command.Parameters.AddWithValue("@qv", substance.qv);
                            command.Parameters.AddWithValue("@taxType", substance.taxType);
                            command.Parameters.AddWithValue("@taxAmmount", substance.taxAmmount);

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

        public void UpdateSubstancesDB()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (Substance substance in Substances)
                        {
                            string query = "UPDATE substances " +
                                           "SET substance_name = @newName, " +
                                           "    TLK = @newTLK, " +
                                           "    RfC = @newRfC, " +
                                           "    SF = @newSF, " +
                                           "    cancerogenic = @newCancerogenic, " +
                                           "    danger_class = @newDangerClass, " +
                                           "    roNorm = @newRoNorm, " +
                                           "    qv = @newQv, " +
                                           "    taxType = @newTaxType, " +
                                           "    taxAmmount = @newTaxAmmount " +
                                           "WHERE substance_Id = @substanceId";

                            using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@newName", substance.name);
                                command.Parameters.AddWithValue("@newTLK", substance.TLK);
                                command.Parameters.AddWithValue("@newRfC", substance.RfC);
                                command.Parameters.AddWithValue("@newSF", substance.SF);
                                command.Parameters.AddWithValue("@newCancerogenic", substance.cancerogenic);
                                command.Parameters.AddWithValue("@newDangerClass", substance.danger_class);
                                command.Parameters.AddWithValue("@newRoNorm", substance.roNorm);
                                command.Parameters.AddWithValue("@newQv", substance.qv);
                                command.Parameters.AddWithValue("@newTaxType", substance.taxType);
                                command.Parameters.AddWithValue("@newTaxAmmount", substance.taxAmmount);
                                command.Parameters.AddWithValue("@substanceId", substance.Substance_Id);

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

        public void DeleteSubstanceFromDB(Substance substance)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "DELETE FROM substances WHERE substance_Id = @substanceId";

                        using (MySqlCommand command = new MySqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@substanceId", substance.Substance_Id);

                            command.ExecuteNonQuery();
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

        public bool SubstanceExists(string substanceName)
        {
            foreach (Substance sub in Substances)
            {
                if (sub.name.Equals(substanceName))
                {
                    return true;
                }
            }

            return false;
        }

        public int FindSubstanceId(string substanceName)
        {
            foreach (Substance sub in Substances)
            {
                if (sub.name == substanceName)
                {
                    return sub.Substance_Id;
                }
            }

            return -1;
        }

        public Substance GetSubstance(int id) { 
            foreach (Substance sub in Substances) {
                if (sub.Substance_Id == id) {
                    return sub;
                }
            }
            return null;
        }

        public void ClearSubstancesTable()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM substances";

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
