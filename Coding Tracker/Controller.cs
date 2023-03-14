using System.Configuration;

namespace CodingTracker
{
    internal class Controller
    {
        private Model Model { get; set; }
        private View View { get; set; }

        public Controller(string? ConnectionString, string? TableName)
        {
            Model = new Model(ConnectionString, TableName);
            View = new View(Model);
        }

        static void Main(string[] args)
        {
            Controller controller = new Controller(ConfigurationManager.AppSettings.Get("ConnectionString"), ConfigurationManager.AppSettings.Get("TableName"));
            controller.View.PrintMainMenu();
        }

        internal static void ExitProgram()
        {
            Environment.Exit(1);
        }
    }
}