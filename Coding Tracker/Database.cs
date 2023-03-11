using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace CodingTracker
{
    internal class Database
    {
        private string ConnectionString { get; set; }
        private string DatabaseName { get; set; }   

        public Database(string DB_Path, string DB_Name)
        {
            this.ConnectionString = DB_Path;
            this.DatabaseName = DB_Name;

            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS {this.DatabaseName} (
                        ID              INTEGER     PRIMARY KEY AUTOINCREMENT NOT NULL,
                        StartTime       TEXT        NOT NULL,
                        EndTime         TEXT        NOT NULL,
                        Duration        INTEGER     NOT NULL
                    )";
            var test = command.ExecuteNonQuery();
        }

        internal int GetRecordCount()
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =$@"
                    SELECT COUNT(*) FROM {this.DatabaseName}
                    ";

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count;
        }

        internal DataTable GetCompanyByID(int ID)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {this.DatabaseName}
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", ID);
            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal DataTable RetrievePageBeforeID(int ID_offset)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {this.DatabaseName}
                    WHERE ID < $ID_offset
                    ORDER BY ID DESC
                    LIMIT 5
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);

            using SqliteDataReader reader = command.ExecuteReader();
            // Rows returned are in descending order and require sorting
            DataTable resultSet = new DataTable();
            resultSet.Load(reader);
            resultSet.DefaultView.Sort = "ID ASC";
            resultSet = resultSet.DefaultView.ToTable();

            return resultSet;
        }

        internal DataTable RetrievePageAfterID(int ID_offset)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    SELECT * FROM {this.DatabaseName}
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

        internal DataTable AddNewSession(string StartTime, string EndTime, int Duration)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    INSERT INTO {this.DatabaseName} (StartTime, EndTime, Duration) 
                    VALUES ($StartTime, $EndTime, $Duration
                    )";
            command.Parameters.AddWithValue("$StartTime", StartTime);
            command.Parameters.AddWithValue("$EndTime", EndTime);
            command.Parameters.AddWithValue("$Duration", Duration);
            command.ExecuteNonQuery();

            command.CommandText = $@"
                    SELECT * FROM {this.DatabaseName}
                    WHERE StartTime=$StartTime
                    AND EndTime=$EndTime
                    AND Duration=$Duration
                    ";

            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal void DeleteCompany(int ID)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    DELETE FROM {this.DatabaseName} 
                    WHERE ID = $ID
                    ";
            command.Parameters.AddWithValue("$ID", ID);
            command.ExecuteNonQuery();
        }

        internal void UpdateCompany(DataTable table)
        {
            using var connection = new SqliteConnection(this.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $@"
                    UPDATE {this.DatabaseName}
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