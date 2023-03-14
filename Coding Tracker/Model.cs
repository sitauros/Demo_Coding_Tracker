using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace CodingTracker
{
    internal class Model
    {
        private string? ConnectionString { get; set; }
        private string? TableName { get; set; }   

        public Model(string? ConnectionString, string? TableName)
        {
            initialize_DB(ConnectionString, TableName);
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {this.TableName} (
                        ID              INTEGER     PRIMARY KEY AUTOINCREMENT NOT NULL,
                        StartTime       TEXT        NOT NULL,
                        EndTime         TEXT        NOT NULL,
                        Duration        TEXT        NOT NULL
                    )";
            var test = command.ExecuteNonQuery();
        }

       private void initialize_DB(string? ConnectionString, string? TableName)
        {
            if (String.IsNullOrEmpty(ConnectionString))
            {
                Console.WriteLine("Missing connection string in App.config XML file");
                Console.WriteLine("Press any key to exit program.");
                Console.ReadLine();
                Controller.ExitProgram();
            }
            else if (String.IsNullOrEmpty(TableName))
            {
                Console.WriteLine("Missing database name in App.config XML file");
                Console.WriteLine("Press any key to exit program.");
                Console.ReadLine();
                Controller.ExitProgram();
            }
            else
            {
                this.ConnectionString = ConnectionString;
                this.TableName = TableName;
            }
        }

        internal int GetRecordCount()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =$@"
                    SELECT COUNT(*) FROM {TableName}
                    ";

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count;
        }

        internal List<CodingSession> GetSessionByID(int ID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {TableName}
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
                            SELECT * FROM {TableName}
                            WHERE ID < $ID_offset
                            ORDER BY ID DESC 
                            LIMIT 5
                    ) AS SESSIONS
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
                    SELECT * FROM {TableName}
                    WHERE ID > $ID_offset
                    ORDER BY ID ASC
                    LIMIT 5;
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);
            using SqliteDataReader reader = command.ExecuteReader();
            List<CodingSession> records = RetrieveRecords(reader);
            return records;
        }

        internal List<CodingSession> AddSession(CodingSession session)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    INSERT INTO {TableName} (StartTime, EndTime, Duration) 
                    VALUES ($StartTime, $EndTime, $Duration
                    )";
            command.Parameters.AddWithValue("$StartTime", session.StartTime);
            command.Parameters.AddWithValue("$EndTime", session.EndTime);
            command.Parameters.AddWithValue("$Duration", session.Duration);
            command.ExecuteNonQuery();

            command.CommandText = $@"
                    SELECT * FROM {TableName}
                    WHERE StartTime=$StartTime
                    AND EndTime=$EndTime
                    AND Duration=$Duration
                    ORDER BY ID DESC
                    LIMIT 1
                    ";
            using SqliteDataReader reader = command.ExecuteReader();
            List<CodingSession> records = RetrieveRecords(reader);
            return records;
        }

        internal void DeleteSession(int ID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    DELETE FROM {TableName} 
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", ID);
            command.ExecuteNonQuery();
        }

        internal void UpdateSession(List<CodingSession> table)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    UPDATE {TableName}
                    SET StartTime=$StartTime, EndTime=$EndTime, Duration=$Duration
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", table[0].ID);
            command.Parameters.AddWithValue("$StartTime", table[0].StartTime);
            command.Parameters.AddWithValue("$EndTime", table[0].EndTime);
            command.Parameters.AddWithValue("$Duration", table[0].Duration);
            command.ExecuteNonQuery();
        }

        internal List<CodingSession> RetrieveRecords(SqliteDataReader reader)
        {
            List<CodingSession> records = new();
            while (reader.Read())
            {
                CodingSession row = new CodingSession((long)reader[0], (string)reader[1], (string)reader[2], (string)reader[3]);
                records.Add(row);
            }
            return records;
        }
    }
}