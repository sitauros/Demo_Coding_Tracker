using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace CodingTracker
{
    internal static class Database
    {
        internal static string? ConnectionString { get; set; }

        internal static void CreateDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS JuniorUnicornFirms (
                        CompanyID       INTEGER     PRIMARY KEY AUTOINCREMENT NOT NULL,
                        Name            TEXT        NOT NULL,
                        DesiredSkill    TEXT        NOT NULL,
                        YearsOfExp      INTEGER     NOT NULL,
                        Perk            TEXT        NOT NULL 
                    )";
            command.ExecuteNonQuery();
        }

        internal static int GetRecordCount()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT COUNT(*) FROM JuniorUnicornFirms
                    ";

            int count = Convert.ToInt32(command.ExecuteScalar());

            return count;
        }

        internal static DataTable GetCompanyByID(int CompanyID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT * FROM JuniorUnicornFirms
                    WHERE CompanyID = $CompanyID
                    ";
            command.Parameters.AddWithValue("$CompanyID", CompanyID);
            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal static DataTable RetrievePageBeforeID(int ID_offset)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT * FROM JuniorUnicornFirms
                    WHERE CompanyID < $ID_offset
                    ORDER BY CompanyID DESC
                    LIMIT 5
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);

            using SqliteDataReader reader = command.ExecuteReader();
            // Rows returned are in descending order and require sorting
            DataTable resultSet = new DataTable();
            resultSet.Load(reader);
            resultSet.DefaultView.Sort = "CompanyID ASC";
            resultSet = resultSet.DefaultView.ToTable();

            return resultSet;
        }

        internal static DataTable RetrievePageAfterID(int ID_offset)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT * FROM JuniorUnicornFirms
                    WHERE CompanyID > $ID_offset
                    ORDER BY CompanyID ASC
                    LIMIT 5;
                    ";
            command.Parameters.AddWithValue("ID_offset", ID_offset);

            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal static DataTable AddNewCompany(string name, string skill, int yearsOfExp, string perk)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    INSERT INTO JuniorUnicornFirms (Name, DesiredSkill, YearsOfExp, Perk) 
                    VALUES ($name, $skill, $yearsOfExp, $perk
                    )";
            command.Parameters.AddWithValue("$name", name);
            command.Parameters.AddWithValue("$skill", skill);
            command.Parameters.AddWithValue("$yearsOfExp", yearsOfExp);
            command.Parameters.AddWithValue("$perk", perk);
            command.ExecuteNonQuery();

            command.CommandText = @"
                    SELECT * FROM JuniorUnicornFirms
                    WHERE Name=$name
                    AND DesiredSkill=$skill
                    AND YearsOfExp=$yearsOfExp
                    AND Perk=$perk
                    ";

            using SqliteDataReader reader = command.ExecuteReader();

            DataTable resultSet = new DataTable();
            resultSet.Load(reader);

            return resultSet;
        }

        internal static void DeleteCompany(int CompanyID)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    DELETE FROM JuniorUnicornFirms 
                    WHERE CompanyID = $CompanyID
                    ";
            command.Parameters.AddWithValue("$CompanyID", CompanyID);
            command.ExecuteNonQuery();
        }

        internal static void UpdateCompany(DataTable table)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    UPDATE JuniorUnicornFirms
                    SET Name=$name, DesiredSkill=$skill, YearsOfExp=$yearsOfExp, Perk=$perk
                    WHERE CompanyID = $CompanyID
                    ";
            command.Parameters.AddWithValue("$CompanyID", table.Rows[0]["CompanyID"]);
            command.Parameters.AddWithValue("$name", table.Rows[0]["Name"]);
            command.Parameters.AddWithValue("$skill", table.Rows[0]["DesiredSkill"]);
            command.Parameters.AddWithValue("$yearsOfExp", table.Rows[0]["YearsOfExp"]);
            command.Parameters.AddWithValue("$perk", table.Rows[0]["Perk"]);
            command.ExecuteNonQuery();
        }
    }
}