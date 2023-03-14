using System.Configuration;

namespace CodingTracker
{
    internal class Controller
    {
        private Model Model { get; set; }
        private View View { get; set; }

        public Controller(string? DB_Path, string? DB_Name)
        {
            Model = new Model(DB_Path, DB_Name);
            View = new View(Model);
        }

        static void Main(string[] args)
        {
            Controller controller = new Controller(ConfigurationManager.AppSettings.Get("DB Path"), ConfigurationManager.AppSettings.Get("DB Name"));
            controller.View.PrintMainMenu();
        }

        internal static void ExitProgram()
        {
            Environment.Exit(1);
        }
    }
}