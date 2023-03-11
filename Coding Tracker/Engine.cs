using System;
using System.Configuration;

namespace CodingTracker
{
    internal class Engine
    {
        static void Main(string[] args)
        {
            string? ConnectionString = ConfigurationManager.AppSettings.Get("DB Path");
            string? DB_Name = ConfigurationManager.AppSettings.Get("DB Name");

            if (String.IsNullOrEmpty(ConnectionString))
            {
                Console.WriteLine("Missing connection string in App.config XML file");
            }
            else if (String.IsNullOrEmpty(DB_Name))
            {
                Console.WriteLine("Missing database name in App.config XML file");
            }
            else
            {
                Database CodingTrackerDB = new Database(ConnectionString, DB_Name);
                Console.WriteLine("HERE: ");
                Console.ReadLine();
            }
           
        }
    }
}