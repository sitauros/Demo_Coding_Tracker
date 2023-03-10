using System;
using System.Configuration;


namespace CodingTracker
{
    internal class Engine
    {
        static void Main(string[] args)
        {
            string? ConnectionString = ConfigurationManager.AppSettings.Get("DB Path");
            Console.WriteLine(ConnectionString);  
        }
    }
}