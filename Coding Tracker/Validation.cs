using System.Text.RegularExpressions;

namespace CodingTracker
{
    internal class Validation
    {
        internal static string calculateDuration(DateTime startTime, DateTime endTime)
        {
            string duration = "N/A";

            if (DateTime.Compare(endTime, startTime) >= 0) //endTime occurred after startTime
            {
                TimeSpan difference = endTime - startTime;
                string days = difference.Days == 1 ? difference.Days + " day, " : difference.Days + " days, ";
                string hours = difference.Hours == 1 ? difference.Hours + " hour, and " : difference.Hours + " hours, and ";
                string minutes = difference.Minutes == 1 ? difference.Minutes + " minute" : difference.Hours + " minutes";
                duration = days + hours + minutes;  
            }

            return duration;
        }

        internal static int ValidateIntegerRange(int minValue, int maxValue)
        {
            int result;
            var input = Console.ReadLine();
            bool isValidInput = int.TryParse(input, out result);

            while (!isValidInput || result < minValue || result > maxValue)
            {
                Console.WriteLine("Invalid input. Please try again.");
                Console.WriteLine("Your input: ");
                isValidInput = int.TryParse(Console.ReadLine(), out result);
            }

            return result;
        }

        internal static DateTime ValidateDateTime()
        {
            DateTime dateTime = new DateTime();
            bool isValidInput = false;
            Regex dateTimeRegEx = new Regex("^[0-9]{2}/[0-9]{2}/[0-9]{4}\\s[0-9]{2}:[0-9]{2}$");

            string? input = Console.ReadLine();

            while (!isValidInput)
            {
                if (String.IsNullOrEmpty(input))
                {
                    Console.WriteLine("No input detected. Please try again.");
                    Console.WriteLine("Your input: ");
                    input = Console.ReadLine();
                }
                else
                {
                    input = input.Trim();

                    if (!dateTimeRegEx.IsMatch(input))
                    {
                        Console.WriteLine("Datetime not entered in (MM/DD/YYYY HH:MM) format. Please try again.");
                        Console.WriteLine("Your input: ");
                        input = Console.ReadLine();
                    }
                    else
                    {
                        string[] words = input.Split('/', ' ', ':');
                        int[] numbers = Array.ConvertAll(words, s => int.Parse(s));

                        if (numbers[0] < 1 || numbers[0] > 12)
                        {
                            Console.WriteLine("Month must be between 1 and 12 inclusive. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else if (numbers[1] < 1 || numbers[1] > 31)
                        {
                            Console.WriteLine("Day must be between 1 and 31 inclusive. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else if (numbers[2] < 1 || numbers[2] > 9999)
                        {
                            Console.WriteLine("Year must be between 1 and 9999 inclusive. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else if (numbers[3] < 0 || numbers[3] > 23)
                        {
                            Console.WriteLine("Hour must be between 0 and 23 inclusive. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else if (numbers[4] < 0 || numbers[4] > 59)
                        {
                            Console.WriteLine("Minute must be between 0 and 59 inclusive. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else if (DateTime.TryParse(input, out dateTime) == false) // Leap year mismatch
                        {
                            Console.WriteLine("Date entered does not match calendar year. Please try again.");
                            Console.WriteLine("Your input: ");
                            input = Console.ReadLine();
                        }
                        else
                        {
                            isValidInput = true;  
                        }
                    }
                }
            }
            return dateTime;
        }
    }
}