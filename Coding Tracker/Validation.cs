namespace CodingTracker
{
    internal class Validation
    {
        internal static int calculateDuration()
        {
            throw new NotImplementedException();
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

        internal static string ValidateUserString()
        {
            string? input = Console.ReadLine();

            while (String.IsNullOrEmpty(input))
            {
                Console.WriteLine("Invalid input. Please try again.");
                Console.WriteLine("Your input: ");
                input = Console.ReadLine();
            }

            return input;
        }
    }
}
