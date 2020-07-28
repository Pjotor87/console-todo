using System;
using System.Collections.Generic;

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

        public static void WithCharChoices(IList<KeyValuePair<char, string>> choices, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, int spacingCount = 1, bool clearConsole = false, bool readKeyAtEnd = false, bool printChoices = true)
        {
            foreach (var choice in choices)
                PrintMenuChoice(
                    choice.Value, 
                    printChoices ? choice.Key.ToString() : null, 
                    idWrapColor,
                    idColor,
                    textColor,
                    spacingCount);
        }

        public static IDictionary<int, string> PrintMenuChoices(IList<string> menuChoices, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, bool clearConsole = false, bool readKeyAtEnd = false, int spacingCount = 1)
        {
            var dict = new Dictionary<int, string>();
            int counter = 1;
            foreach (var choiceText in menuChoices)
            {
                PrintMenuChoice(choiceText, counter.ToString(), idWrapColor, idColor, textColor, spacingCount);
                dict.Add(counter++, choiceText);
            }
            return dict;
        }

        public static void PrintMenuChoice(string text, string id = null, ConsoleColor idWrapColor = ConsoleColor.Gray, ConsoleColor idColor = ConsoleColor.White, ConsoleColor textColor = ConsoleColor.White, int spacingCount = 1)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Console.ForegroundColor = idWrapColor;
                Console.Write($"(");
                Console.ForegroundColor = idColor;
                Console.Write(id);
                Console.ForegroundColor = idWrapColor;
                Console.Write($")");
                Console.Write(new string(' ', spacingCount));
            }
            Console.ForegroundColor = textColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
