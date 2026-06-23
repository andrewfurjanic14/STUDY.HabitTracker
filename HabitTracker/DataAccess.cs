using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace HabitTracker
{
    internal class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["HabitTrackerDB"].ConnectionString;

            string sql = "CREATE TABLE IF NOT EXISTS Entries (" +
                            "Id INTEGER PRIMARY KEY," +
                            "HabitsId INTEGER," +
                            "Date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                            "Quantity REAL);" +
                            "\n" +
                         "CREATE TABLE IF NOT EXISTS Habits (" +
                            "Id INTEGER PRIMARY KEY," +
                            "Name TEXT NOT NULL," +
                            "Measurement TEXT NOT NULL);";
            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void TestConnection()
        {
            try
            {
                var con = new SqliteConnection(_connectionString);
                con.Open();
                con.Close();
                Console.WriteLine("Connection Successful");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CreateHabit(Habit habit)
        {
            string sql = "INSERT INTO Habits (Name, Measurement) " +
                            "VALUES (@name, @measurement);";
            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@name", habit.Name);
                cmd.Parameters.AddWithValue("@measurement", habit.Measurement);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CreateEntry(Entry entry)
        {
            string sql;
            if (entry.Date is null)
            {
                sql = "INSERT INTO Entries (HabitsId, Quantity) " +
                        "VALUES (@habitsId, @qty);";
            }
            else
            {
                sql = "INSERT INTO Entries (HabitsId, Date, Quantity) " +
                        "VALUES (@habitsId, @date, @qty);";
            }

            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@habitsId", entry.HabitId);
                cmd.Parameters.AddWithValue("@qty", entry.Qty);
                if (entry.Date is not null)
                    cmd.Parameters.AddWithValue("@date", entry.Date);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<Entry> GetHabitEntries(Habit habit)
        {
            List<Entry> result = new List<Entry>();
            string sql = "SELECT * FROM Entries " +
                         "WHERE HabitsId = @habitId;";
            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@habitId", habit.Id);
                using var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Entry entry = new Entry();
                    entry.Id = Convert.ToInt32(rdr["Id"]);
                    entry.HabitId = Convert.ToInt32(rdr["HabitsId"]);
                    entry.Date = Convert.ToDateTime(rdr["Date"]);
                    entry.Qty = Convert.ToSingle(rdr["Quantity"]);
                    result.Add(entry);
                } 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public List<Habit> GetHabits()
        {
            List<Habit> result = new List<Habit>();
            string sql = "SELECT * FROM Habits;";

            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Habit habit = new Habit();
                    habit.Id = Convert.ToInt32(rdr["Id"]);
                    habit.Name = Convert.ToString(rdr["Name"]);
                    habit.Measurement = Convert.ToString(rdr["Measurement"]);
                    result.Add(habit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        public string GetEntryMeasurement(Entry entry)
        {
            string result = "";
            string sql = "SELECT * From Habits " +
                         "WHERE Id = @habitId " +
                         "LIMIT 1;";
            try
            {
                using var con = new SqliteConnection(_connectionString);
                con.Open();
                using var cmd = new SqliteCommand(sql, con);
                cmd.Parameters.AddWithValue("@habitId", entry.HabitId);
                using var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    result = Convert.ToString(rdr["Measurement"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
