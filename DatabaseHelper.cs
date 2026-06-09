using Gym.Core;
using Gym.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Gym.Data
{
    public class DatabaseHelper
    {
        private string _connectionString = "Data Source=gym_base.db;Version=3;";

        public DatabaseHelper()
        {
            if (!File.Exists("gym_base.db"))
            {
                SQLiteConnection.CreateFile("gym_base.db");
            }
            CreateTables();
        }

        private void CreateTables()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sqlClients = @"CREATE TABLE IF NOT EXISTS Clients (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT, LastName TEXT, Phone TEXT, Age INTEGER, SubEndDate TEXT);";

                string sqlTrainers = @"CREATE TABLE IF NOT EXISTS Trainers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT, LastName TEXT, Phone TEXT, Specialization TEXT, Experience INTEGER);";

                string sqlSchedule = @"CREATE TABLE IF NOT EXISTS Schedule (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ClientId INTEGER, TrainerId INTEGER, TrainingDate TEXT, Activity TEXT,
            FOREIGN KEY(ClientId) REFERENCES Clients(Id),
            FOREIGN KEY(TrainerId) REFERENCES Trainers(Id));";

                new SQLiteCommand(sqlClients, connection).ExecuteNonQuery();
                new SQLiteCommand(sqlTrainers, connection).ExecuteNonQuery();
                new SQLiteCommand(sqlSchedule, connection).ExecuteNonQuery();
            }
        }

        public void AddClient(Client client)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Clients (FirstName, LastName, Phone, Age, SubEndDate) " +
                                 "VALUES (@fn, @ln, @ph, @age, @date)";
                    var cmd = new SQLiteCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@fn", client.FirstName);
                    cmd.Parameters.AddWithValue("@ln", client.LastName);
                    cmd.Parameters.AddWithValue("@ph", client.Phone);
                    cmd.Parameters.AddWithValue("@age", client.Age);
                    cmd.Parameters.AddWithValue("@date", client.SubscriptionEndDate.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка при записі в БД: " + ex.Message);
            }
        }

        public List<Client> GetAllClients()
        {
            var list = new List<Client>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Clients";
                var cmd = new SQLiteCommand(sql, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Client(
                            Convert.ToInt32(reader["Id"]),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["Phone"].ToString(),
                            Convert.ToInt32(reader["Age"]),
                            DateTime.Parse(reader["SubEndDate"].ToString())
                        ));
                    }
                }
            }
            return list;
        }

        public void UpdateClient(Client client)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"UPDATE Clients 
                           SET FirstName=@fn, LastName=@ln, Phone=@ph, Age=@age, SubEndDate=@date 
                           WHERE Id=@id";
                    var cmd = new SQLiteCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@fn", client.FirstName);
                    cmd.Parameters.AddWithValue("@ln", client.LastName);
                    cmd.Parameters.AddWithValue("@ph", client.Phone);
                    cmd.Parameters.AddWithValue("@age", client.Age);
                    cmd.Parameters.AddWithValue("@date", client.SubscriptionEndDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@id", client.Id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка оновлення в БД: " + ex.Message);
            }
        }

        public void DeleteClient(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Clients WHERE Id = @id";
                    var cmd = new SQLiteCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка видалення з БД: " + ex.Message);
            }
        }

        public void AddTraining(TrainingSession session)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Schedule (ClientId, TrainerId, TrainingDate, Activity) VALUES (@cid, @tid, @date, @act)";
                var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@cid", session.ClientId);
                cmd.Parameters.AddWithValue("@tid", session.TrainerId);
                cmd.Parameters.AddWithValue("@date", session.Date.ToString("yyyy-MM-dd HH:mm"));
                cmd.Parameters.AddWithValue("@act", session.Activity);
                cmd.ExecuteNonQuery();
            }
        }

        public List<Trainer> GetAllTrainers()
        {
            var list = new List<Trainer>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Trainers", connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Trainer(
                            Convert.ToInt32(reader["Id"]),
                            reader["FirstName"].ToString(),
                            reader["LastName"].ToString(),
                            reader["Phone"].ToString(),
                            reader["Specialization"].ToString(),
                            Convert.ToInt32(reader["Experience"])
                        ));
                    }
                }
            }
            return list;
        }

        public List<TrainingSession> GetAllTrainings()
        {
            var list = new List<TrainingSession>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SQLiteCommand("SELECT * FROM Schedule", connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TrainingSession(
                            Convert.ToInt32(reader["Id"]),
                            Convert.ToInt32(reader["ClientId"]),
                            Convert.ToInt32(reader["TrainerId"]),
                            DateTime.Parse(reader["TrainingDate"].ToString()),
                            reader["Activity"].ToString()
                        ));
                    }
                }
            }
            return list;
        }
        public void AddTrainer(Trainer trainer)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "INSERT INTO Trainers (FirstName, LastName, Phone, Specialization, Experience) " +
                             "VALUES (@fn, @ln, @ph, @spec, @exp)";
                var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@fn", trainer.FirstName);
                cmd.Parameters.AddWithValue("@ln", trainer.LastName);
                cmd.Parameters.AddWithValue("@ph", trainer.Phone);
                cmd.Parameters.AddWithValue("@spec", trainer.Specialization);
                cmd.Parameters.AddWithValue("@exp", trainer.Experience);
                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteTrainer(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Trainers WHERE Id = @id";
                var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTraining(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Schedule WHERE Id = @id";
                var cmd = new SQLiteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

    }
}