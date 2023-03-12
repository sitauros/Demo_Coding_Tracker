using System;
using System.Data;
using System.Diagnostics.Metrics;
using CodingTracker;
using ConsoleTableExt;

namespace CodingTracker
{
    internal class View
    {
        private Model model { get; set; }

        internal View(Model model)
        {
            this.model = model;
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
            int totalRecords = model.GetRecordCount();
            int numPages = CalculateNumPages(totalRecords);

            var resultSet = model.RetrievePageAfterID(ID_offset);
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
                        ID_offset = Convert.ToInt32(resultSet.Rows[0]["ID"]); // First record in result set
                        resultSet = model.RetrievePageBeforeID(ID_offset);
                        FormatPage(resultSet, numPages, currentPage, totalRecords);
                        break;
                    case 1:
                        continueLoop = false;
                        break;
                    case 2:
                        currentPage = currentPage + 1;
                        ID_offset = Convert.ToInt32(resultSet.Rows[4]["ID"]); // Last record in result set
                        resultSet = model.RetrievePageAfterID(ID_offset);
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
Add a new company to the Coding Tracker Database
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
1) Enter your session's starting time in military format (YYYY-MM-DD-HH:MM): ");
            string StartTime = Validation.ValidateUserString();
            Console.WriteLine("2) Enter your session's ending time in military format (YYYY-MM-DD-HH:MM): ");
            string EndTime = Validation.ValidateUserString();
            int Duration = Validation.calculateDuration();

            var resultSet = model.AddSession(StartTime, EndTime, Duration);
            FormatTable(resultSet);
            BackToMainMenu("New session added.");
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
                var resultSet = model.GetSessionByID(SessionID);

                if (resultSet.Rows.Count == 1)
                {
                    FormatTable(resultSet);
                    Console.WriteLine("Enter a number below: \n");
                    Console.WriteLine(@"1) Update session start time.
2) Update session end time.

Your input: ");

                    var result = Validation.ValidateIntegerRange(1, 4);
                    string return_message = "";

                    switch (result)
                    {
                        case 1:
                            Console.WriteLine("Enter your session's starting time in military format (YYYY-MM-DD-HH:MM): ");
                            string StartTime = Validation.ValidateUserString();
                            resultSet.Rows[0]["StartTime"] = StartTime;
                            return_message = "Updated session start time to: " + StartTime;
                            break;
                        case 2:
                            Console.WriteLine("Enter your session's ending time in military format (YYYY-MM-DD-HH:MM): ");
                            string EndTime = Validation.ValidateUserString();
                            resultSet.Rows[0]["EndTime"] = EndTime;
                            return_message = "Updated session start time to: " + EndTime;
                            break;
                    }

                    model.UpdateSession(resultSet);
                    resultSet = model.GetSessionByID(SessionID);
                    FormatTable(resultSet);
                    BackToMainMenu(return_message);
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
Enter the company's ID: ");

            int SessionID;
            var input = Console.ReadLine();

            if (Int32.TryParse(input, out SessionID))
            {
                var resultSet = model.GetSessionByID(SessionID);

                if (resultSet.Rows.Count == 1)
                {
                    model.DeleteSession(SessionID);
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

        private void FormatPage(DataTable resultSet, int numPages, int currentPage, int totalRecords)
        {
            Console.Clear();
            Console.WriteLine(@"=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
Showing sessions listed in the Coding Tracker Database
Now viewing page " + currentPage + " of " + numPages +
"\nTotal: " + totalRecords + "\n=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=" + "\n");

            FormatTable(resultSet);
        }

        private void FormatTable(DataTable resultSet)
        {
            ConsoleTableBuilder
               .From(resultSet)
               .WithColumn("Id", "StartTime", "EndTime", "Duration")
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