using System;
using System.Configuration;

namespace CodingTracker
{
    internal class Controller
    {
        private Model model { get; set; }
        private View view { get; set; }

        public Controller(string? DB_Path, string? DB_Name)
        {
            model = new Model(DB_Path, DB_Name);
            view = new View(model);
        }

        static void Main(string[] args)
        {
            Controller controller = new Controller(ConfigurationManager.AppSettings.Get("DB Path"), ConfigurationManager.AppSettings.Get("DB Name"));
            controller.view.PrintMainMenu();
        }

        internal static void ExitProgram()
        {
            Environment.Exit(1);
        }
    }
}