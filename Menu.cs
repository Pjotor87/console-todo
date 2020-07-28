using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TODO
{
    public class Menu
    {
        public static char GetUserInput(ICollection<char> validInputs = null, char abortSwitch = (char)13)
        {
            char userInput;
            do
            {
                userInput = Console.ReadKey().KeyChar;
                if (validInputs != null)
                {
                    if (userInput == abortSwitch || // Enter keypress
                        validInputs.Contains(userInput))
                        break;
                }
                else
                    break;
            } while (true);

            return userInput;
        }

        public static string GetUserInput(ICollection<string> validInputs = null, string abortSwitch = null)
        {
            string userInput;
            do
            {
                userInput = Console.ReadLine();
                if (validInputs != null)
                {
                    if (string.IsNullOrEmpty(userInput) || // Enter keypress
                        validInputs.Contains(userInput))
                        break;
                }
                else
                    break;
            } while (true);

            return userInput;
        }

        public static IDictionary<int, string> WithNumberedChoices(IList<string> lines, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, int spacingCount = 1, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            return PrintMenuChoices(lines, idWrapColor, idColor, textColor, clearConsole, readKeyAtEnd, spacingCount);
        }

        public static void WithCharChoices(IList<KeyValuePair<char, string>> choices, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, int spacingCount = 1, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            foreach (var choice in choices)
                PrintMenuChoice(choice.Key.ToString(), choice.Value, idWrapColor, idColor, textColor, spacingCount);
        }

        private static IDictionary<int, string> PrintMenuChoices(IList<string> menuChoices, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, bool clearConsole = false, bool readKeyAtEnd = false, int spacingCount = 1)
        {
            var dict = new Dictionary<int, string>();
            int counter = 1;
            foreach (var choiceText in menuChoices)
            {
                PrintMenuChoice(counter.ToString(), choiceText, idWrapColor, idColor, textColor, spacingCount);
                dict.Add(counter++, choiceText);
            }
            return dict;
        }

        private static void PrintMenuChoice(string id, string text, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, int spacingCount = 1)
        {
            Console.ForegroundColor = idWrapColor;
            Console.Write($"(");
            Console.ForegroundColor = idColor;
            Console.Write(id);
            Console.ForegroundColor = idWrapColor;
            Console.Write($")");
            Console.Write(new string(' ', spacingCount));
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
