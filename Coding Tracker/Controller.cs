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
           
            if (controller.Model.GetRecordCount() == 0)
            {
                generateSessionList(controller.Model);  
            }

            controller.View.PrintMainMenu();
        }

        internal static void generateSessionList(Model model)
        {
            Random randomizer = new Random();
            for (int i = 1; i <= 20; i++)
            {
                int offset_days = randomizer.Next(0, 365);
                int offset_hours = randomizer.Next(0, 24);
                int offset_minutes = randomizer.Next(0, 60);

                DateTime StartTime = DateTime.Now;
                DateTime EndTime = StartTime.Add(new TimeSpan(offset_days, offset_hours, offset_minutes, 0));
                string duration = Validation.CalculateDuration(StartTime, EndTime);
                model.AddSession(new CodingSession(i, StartTime.ToString("g"), EndTime.ToString("g"), duration));
            }
        }

        internal static void ExitProgram()
        {
            Environment.Exit(1);
        }
    }
}