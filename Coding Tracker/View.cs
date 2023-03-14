using ConsoleTableExt;

namespace CodingTracker
{
    internal class View
    {
        private Model Model { get; set; }

        internal View(Model Model)
        {
            this.Model = Model;
        }

        private Dictionary<HeaderCharMapPositions, char> CharMapPositions = new Dictionary<HeaderCharMapPositions, char> {
            {HeaderCharMapPositions.TopLeft, '╒' },
            {HeaderCharMapPositions.TopCenter, '═' },
            {HeaderCharMapPositions.TopRight, '╕' },
            {HeaderCharMapPositions.BottomLeft, '╞' },
            {HeaderCharMapPositions.BottomCenter, '╤' },
            {HeaderCharMapPositions.BottomRight, '╡' },
            {HeaderCharMapPositions.BorderTop, '═' },
            {HeaderCharMapPositions.BorderRight, '│' },
            {HeaderCharMapPositions.BorderBottom, '═' },
            {HeaderCharMapPositions.BorderLeft, '│' },
            {HeaderCharMapPositions.Divider, ' ' },
        };

        private Dictionary<int, TextAligntment> TextAlignments = new Dictionary<int, TextAligntment> {
            {0, TextAligntment.Center},
            {1, TextAligntment.Center},
            {2, TextAligntment.Center},
            {3, TextAligntment.Center},
            {4, TextAligntment.Center}
        };

        internal void PrintMainMenu()
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Welcome to the Coding Session Tracker Logger. 
Record your development hours with this C# console application.
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

Enter a number below:

1) View all sessions in the Coding Tracker database.
2) Create a new session in the Coding Tracker database.
3) Update an entry in the Coding Tracker database.
4) Delete an entry in the Coding Tracker database.
0) Quit program.

Your input: ");

            var result = Validation.ValidateIntegerRange(0, 4);

            switch (result)
            {
                case 1:
                    ListAllSessions();
                    break;
                case 2:
                    AddSession();
                    break;
                case 3:
                    UpdateSession();
                    break;
                case 4:
                    DeleteSession();
                    break;
                case 0:
                    Controller.ExitProgram();
                    break;
            }
        }

        private void ListAllSessions()
        {
            bool continueLoop = true;
            int ID_offset = 0;
            int currentPage = 1;
            int totalRecords = Model.GetRecordCount();
            int numPages = CalculateNumPages(totalRecords);

            var resultSet = Model.RetrievePageAfterID(ID_offset);
            FormatPage(resultSet, numPages, currentPage, totalRecords);

            while (continueLoop)
            {
                int minValue = 1;
                int maxValue = 1;
                Console.WriteLine("Enter a number below: ");

                if (currentPage > 1)
                {
                    Console.WriteLine("0) Return to previous page. ");
                    minValue = 0;
                }

                Console.WriteLine("1) Return to main menu.");

                if (currentPage < numPages)
                {
                    Console.WriteLine("2) Advance to next page. ");
                    maxValue = 2;
                }

                Console.WriteLine("\nYour input: ");
                var result = Validation.ValidateIntegerRange(minValue, maxValue);

                switch (result)
                {
                    case 0:
                        currentPage = currentPage - 1;
                        ID_offset = Convert.ToInt32(resultSet[0].ID); // First record in result set
                        resultSet = Model.RetrievePageBeforeID(ID_offset);
                        FormatPage(resultSet, numPages, currentPage, totalRecords);
                        break;
                    case 1:
                        continueLoop = false;
                        break;
                    case 2:
                        currentPage = currentPage + 1;
                        ID_offset = Convert.ToInt32(resultSet[4].ID); // Last record in result set
                        resultSet = Model.RetrievePageAfterID(ID_offset);
                        FormatPage(resultSet, numPages, currentPage, totalRecords);
                        break;
                }
            }

            PrintMainMenu();
        }

        private void AddSession()
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Add a new session to the Coding Tracker Database
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
1) Enter start time in military hours (MM/DD/YYYY HH:MM): ");
            DateTime StartTime = Validation.ValidateDateTime();
            Console.WriteLine("2) Enter end time in military hours (MM/DD/YYYY HH:MM): ");
            DateTime EndTime = Validation.ValidateDateTime();

            string duration = Validation.CalculateDuration(StartTime, EndTime);
            
            if (duration == "N/A")
            {
                BackToMainMenu("End time is prior to start time.");
            }
            else
            {
                // Stores dates in MM/DD/YYYY HH:MM format
                CodingSession session = new CodingSession(-1, StartTime.ToString("g"), EndTime.ToString("g"), duration);
                var resultSet = Model.AddSession(session);
                FormatTable(resultSet);
                BackToMainMenu("New session added.");
            }        
        }

        private void UpdateSession()
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Update an entry in the Coding Tracker database
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Enter the session ID: ");

            int SessionID;
            var input = Console.ReadLine();

            if (Int32.TryParse(input, out SessionID))
            {
                var resultSet = Model.GetSessionByID(SessionID);

                if (resultSet.Count == 1)
                {
                    FormatTable(resultSet);
                    Console.WriteLine("Enter a number below: \n");
                    Console.WriteLine(@"1) Update session start time.
2) Update session end time.

Your input: ");

                    var result = Validation.ValidateIntegerRange(1, 2);
                    string return_message = "";

                    switch (result)
                    {
                        case 1:
                            Console.WriteLine("Enter start time in military hours (MM/DD/YYYY HH:MM): ");
                            DateTime StartTime = Validation.ValidateDateTime();
                            resultSet[0].StartTime = StartTime.ToString("g");
                            resultSet[0].Duration = Validation.CalculateDuration(StartTime, DateTime.Parse(resultSet[0].EndTime));
                            return_message = "Updated start time to: " + StartTime;
                            break;
                        case 2:
                            Console.WriteLine("Enter end time in military hours (MM/DD/YYYY HH:MM): ");
                            DateTime EndTime = Validation.ValidateDateTime();
                            resultSet[0].EndTime = EndTime.ToString("g");
                            resultSet[0].Duration = Validation.CalculateDuration(DateTime.Parse(resultSet[0].StartTime), EndTime);
                            return_message = "Updated end time to: " + EndTime;
                            break;
                    }

                    if (resultSet[0].Duration == "N/A")
                    {
                        BackToMainMenu("End time is prior to start time.");
                    }
                    else
                    {
                        Model.UpdateSession(resultSet);
                        resultSet = Model.GetSessionByID(SessionID);
                        FormatTable(resultSet);
                        BackToMainMenu(return_message);
                    }
                }
                else
                {
                    BackToMainMenu("Session not found with ID value: " + input);
                }
            }
            else
            {
                BackToMainMenu("Input is not a number: " + input);
            }
        }

        private void DeleteSession()
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Delete an entry in the Coding Tracker database
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Enter the session ID: ");

            int SessionID;
            var input = Console.ReadLine();

            if (Int32.TryParse(input, out SessionID))
            {
                var resultSet = Model.GetSessionByID(SessionID);

                if (resultSet.Count == 1)
                {
                    Model.DeleteSession(SessionID);
                    FormatTable(resultSet);
                    BackToMainMenu("Session deleted with ID: " + SessionID);
                }
                else
                {
                    BackToMainMenu("Session not found with ID value: " + input);
                }
            }
            else
            {
                BackToMainMenu("Input is not a number: " + input);
            }
        }

        private void BackToMainMenu(string message)
        {
            Console.WriteLine("\n" + "-----------------------------------------------------------------------------------");
            Console.WriteLine(message);
            Console.WriteLine("Press any key to return to main menu.");
            Console.WriteLine("-----------------------------------------------------------------------------------");
            Console.ReadLine();
            PrintMainMenu();
        }

        private void FormatPage(List<CodingSession> resultSet, int numPages, int currentPage, int totalRecords)
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Showing sessions listed in the Coding Tracker Database
Now viewing page " + currentPage + " of " + numPages +
"\nTotal: " + totalRecords + "\n=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=" + "\n");

            FormatTable(resultSet);
        }

        private void FormatTable(List<CodingSession> resultSet)

        {
            ConsoleTableBuilder
               .From(resultSet)
               .WithColumn(new List<String> { "ID", "StartTime", "EndTime", "Duration"} )
               .WithTextAlignment(TextAlignments)
               .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
               .WithCharMapDefinition(CharMapDefinition.FramePipDefinition, CharMapPositions)
               .ExportAndWriteLine(TableAligntment.Center); 
        }

        private int CalculateNumPages(int totalRecords)
        {
            int numPages;

            if (totalRecords == 0)
                numPages = 1;
            else if (totalRecords % 5 == 0)
                numPages = totalRecords / 5;
            else
                numPages = 1 + totalRecords / 5;

            return numPages;
        }

    }
}