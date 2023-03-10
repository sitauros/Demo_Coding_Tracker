using System.Configuration;


namespace CodingTracker
{
    internal class Console
    {
        static void Main(string[] args)
        {
            string? ConnectionString = ConfigurationManager.AppSettings.Get("DB Path");
        }
    }
}