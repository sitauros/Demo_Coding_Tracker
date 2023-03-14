using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace CodingTracker
{
    internal class Model
    {
        private string? ConnectionString { get; set; }
        private string? DatabaseName { get; set; }   

        public Model(string? DB_Path, string? DB_Name)
        {
            initialize_DB(DB_Path, DB_Name);
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {DatabaseName} (
                        ID              INTEGER     PRIMARY KEY AUTOINCREMENT NOT NULL,
                        StartTime       TEXT        NOT NULL,
                        EndTime         TEXT        NOT NULL,
                        Duration        TEXT        NOT NULL
                    )";
            var test = command.ExecuteNonQuery();
        }

       private void initialize_DB(string? DB_Path, string? DB_Name)
        {
            if (String.IsNullOrEmpty(DB_Path))
            {
                Console.WriteLine("Missing connection string in App.config XML file");
                Console.WriteLine("Press any key to exit program.");
                Console.ReadLine();
                Controller.ExitProgram();
            }
            else if (String.IsNullOrEmpty(DB_Name))
            {
                Console.WriteLine("Missing database name in App.config XML file");
                Console.WriteLine("Press any key to exit program.");
                Console.ReadLine();
                Controller.ExitProgram();
            }
            else
            {
                ConnectionString = DB_Path;
                DatabaseName = DB_Name;
            }
        }

        internal int GetRecordCount()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =$@"
                    SELECT COUNT(*) FROM {DatabaseName}
                    ";

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count;
        }

        internal DataTable GetSessionByID(int ID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {DatabaseName}
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", ID);
            using SqliteDataReader reader = command.ExecuteReader();
            List<CodingSession> records = RetrieveRecords(reader);
            return records;
        }

        internal List<CodingSession> RetrievePageBeforeID(int ID_offset)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM (
                            SELECT * FROM {DatabaseName}
                            WHERE ID < $ID_offset
                            ORDER BY ID DESC 
                            LIMIT 5
                    ) AS DATABASE
                    ORDER BY ID ASC
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);
            using SqliteDataReader reader = command.ExecuteReader();
            List<CodingSession> records = RetrieveRecords(reader);
            return records;
        }

        internal List<CodingSession> RetrievePageAfterID(int ID_offset)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {DatabaseName}
                    WHERE ID > $ID_offset
                    ORDER BY ID ASC
                    LIMIT 5;
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);

            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal DataTable AddSession(string StartTime, string EndTime, string Duration)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    INSERT INTO {DatabaseName} (StartTime, EndTime, Duration) 
                    VALUES ($StartTime, $EndTime, $Duration
                    )";
            command.Parameters.AddWithValue("$StartTime", StartTime);
            command.Parameters.AddWithValue("$EndTime", EndTime);
            command.Parameters.AddWithValue("$Duration", Duration);
            command.ExecuteNonQuery();

            command.CommandText = $@"
                    SELECT * FROM {DatabaseName}
                    WHERE StartTime=$StartTime
                    AND EndTime=$EndTime
                    AND Duration=$Duration
                    ORDER BY ID DESC
                    LIMIT 1
                    ";

            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal void DeleteSession(int ID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    DELETE FROM {DatabaseName} 
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", ID);
            command.ExecuteNonQuery();
        }

        internal void UpdateSession(DataTable table)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    UPDATE {DatabaseName}
                    SET StartTime=$StartTime, EndTime=$EndTime, Duration=$Duration
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", table.Rows[0]["ID"]);
            command.Parameters.AddWithValue("$StartTime", table.Rows[0]["StartTime"]);
            command.Parameters.AddWithValue("$EndTime", table.Rows[0]["EndTime"]);
            command.Parameters.AddWithValue("$Duration", table.Rows[0]["Duration"]);
            command.ExecuteNonQuery();
        }
    }
}