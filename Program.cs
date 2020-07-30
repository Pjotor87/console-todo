using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TODO
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            RunLoop(GetTodoPath(args, debug));
        }

        private static string GetTodoPath(string[] args, bool debug)
        {
            string todoPath = string.Empty;
            if (debug)
            {
                string fileUsedWhenDebugging = "todo.txt";
                if (File.Exists(fileUsedWhenDebugging))
                    File.Delete(fileUsedWhenDebugging);
                todoPath = fileUsedWhenDebugging;
            }
            else if (args != null)
            {
                todoPath = args[0];
            }

            return todoPath;
        }

        private static void RunLoop(string todoPath)
        {
            if (!string.IsNullOrEmpty(todoPath))
            {
                bool keepRunning;
                do
                {
                    keepRunning = Run(todoPath);
                } while (keepRunning);
            }
            else
            {
                Print.NewLines(new string[] {
                    "This program requires one argument passed to it.",
                    "Pass a path to an empty .txt file, then try running the program again."
                }, ConsoleColor.Yellow);
            }
        }

        private static bool Run(string todoPath, bool exit = false)
        {
            if (!exit)
            {
                var todoCollection = new TodoCollection(todoPath, false);

                if (!todoCollection.DbExists())
                {
                    todoCollection.Initialize();
                    exit = false;
                }
                else
                {
                    Console.Clear();
                    todoCollection.Load();
                    OutputAppTitle("console-todo");
                    todoCollection.OutputToConsole();
                    Print.NewLine("\nWrite a new todo to add it. A number to edit. Just enter without typing anything to exit.\n", ConsoleColor.White);
                    exit = GetMainMenuUserInput(todoCollection);
                }
            }

            return !exit;
        }

        private static void OutputAppTitle(string title)
        {
            var colors = new List<ConsoleColor>
            {
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.Cyan,
                ConsoleColor.White,
                ConsoleColor.Gray
            };
            var titleColor = ConsoleColor.Cyan;
            colors = colors.OrderBy(a => Guid.NewGuid()).ToList();
            int colorCounter = 0;

            int lineCount = 3;
            int borderWidth = 2;
            int leftRighttitlePadding = 2;
            int lineLength = title.Length + (2 * borderWidth) + (2 * leftRighttitlePadding);
            int totalCharCount = lineLength * lineCount;

            bool titleLineSwitch = false;
            char borderChar = '¤';
            char paddingChar = ' ';
            
            int charCounter = 0;

            for (int i = 0; i < totalCharCount; i++)
            {
                char charToPrint;
                if (!titleLineSwitch)
                    charToPrint = borderChar;
                else
                {
                    if (charCounter < borderWidth)
                        charToPrint = borderChar;
                    else if (charCounter < borderWidth + leftRighttitlePadding)
                        charToPrint = paddingChar;
                    else if (charCounter < borderWidth + leftRighttitlePadding + title.Length)
                        charToPrint = title[charCounter - leftRighttitlePadding - borderWidth];
                    else if (charCounter < borderWidth + leftRighttitlePadding + title.Length + leftRighttitlePadding)
                        charToPrint = paddingChar;
                    else
                        charToPrint = borderChar;
                    charCounter++;
                }

                ConsoleColor charColor = charToPrint != borderChar ? titleColor : colors[colorCounter];
                colorCounter = colorCounter == colors.Count - 1 ? 0 : colorCounter + 1;

                bool newLine = i == totalCharCount - 1;
                if (!newLine)
                    for (int j = 1; j < lineCount; j++)
                        if (i == (j * lineLength) - 1)
                        {
                            newLine = true;
                            break;
                        }
                if (!newLine)
                {
                    Print.Line(charToPrint.ToString(), charColor);
                }
                else
                {
                    Print.NewLine(charToPrint.ToString(), charColor);
                    titleLineSwitch = !titleLineSwitch;
                }
            }
        }

        private static void MMM()
        {
            Console.Clear();
            for (int i = 0; i < 50; i++)
            {
                OutputAppTitle("Multicolored menu mode activated!!! Here we go! Whooooooo!!!");
                Thread.Sleep(100);
            }
        }

        private static bool GetMainMenuUserInput(TodoCollection todoCollection)
        {
            bool exit = false;

            string choice = Console.ReadLine();
            if (string.IsNullOrEmpty(choice))
                exit = true;
            else if (todoCollection.IdExists(choice))
                RunSelection(todoCollection, choice);
            else if (!todoCollection.Exists(choice))
            {
                todoCollection.Add(new Todo() { Description = choice, PriorityLevel = GetPriorityForNewEntry(), Color = ConsoleColor.White });
                todoCollection.Save();
            }
            else if (choice == "mmm")
                MMM();

            return exit;
        }

        private static void RunSelection(TodoCollection todoCollection, string choice)
        {
            var todoSelection = todoCollection.FindById(choice);

            Print.Line($"You selected: ", clearConsole: true);
            Print.NewLine(todoSelection.Description, todoSelection.Color);
            Print.Line($"Priority level is: ");
            Print.NewLine(todoSelection.PriorityLevel.ToString(), Priority.GetColor(todoSelection.PriorityLevel));

            var menu = new CharConsoleMenu()
            {
                Choices = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("d", "escription edit"),
                    new KeyValuePair<string, string>("p", "riority edit"),
                    new KeyValuePair<string, string>("r", "emove")
                },
                SpacingCount = 0,
                IdWrapColor = ConsoleColor.Yellow,
                TextColor = ConsoleColor.Blue
            };

            menu.OutputToConsole("Please make a selection", ConsoleColor.Green);
            string modify = menu.GetUserInput().ToLower();

            if (modify == menu.Choices[0].Key.ToString()) // d
            {
                string newDescription = GetNewDescription(todoSelection, todoCollection);
                if (!string.IsNullOrEmpty(newDescription))
                {
                    todoCollection.Update(todoSelection, new Todo() { Description = newDescription, PriorityLevel = todoSelection.PriorityLevel });
                    todoCollection.Save();
                }
            }
            else if (modify == menu.Choices[1].Key.ToString()) // p
            {
                todoCollection.Update(todoSelection, new Todo() { Description = todoSelection.Description, PriorityLevel = GetPriorityForNewEntry() });
                todoCollection.Save();
            }
            else if (modify == menu.Choices[2].Key.ToString()) // r
            {
                todoCollection.Remove(todoSelection);
                todoCollection.Save();
            }
        }

        #region User input

        private static PriorityLevel GetPriorityForNewEntry()
        {
            var priority = PriorityLevel.None;

            var menu = new CharConsoleMenu();
            menu.TextColor = ConsoleColor.Yellow;
            menu.SetNumberedChoices(Enum.GetNames(typeof(PriorityLevel)).ToList());
            menu.OutputToConsole(
                afterMessage: "Just pressing enter will select None.", 
                beforeMessage: "Select priority level", beforeMessageColor: ConsoleColor.Green);
            var selection = menu.GetUserInput();
            {
                if (selection != '\r'.ToString())
                    Enum.TryParse(menu.Choices.Where(x => x.Key == selection).Single().Value, out priority);
            }

            return priority;
        }

        #region Modify item

        private static string GetNewDescription(Todo todoSelection, TodoCollection todoCollection)
        {
            Print.NewLine("\nWrite a new description for this todo. Just enter without typing anything will exit.");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription) && todoCollection.Exists(newDescription))
            {
                Print.NewLine("The entry already exists. Try again.", ConsoleColor.Red);
                return GetNewDescription(todoSelection, todoCollection);
            }
            return newDescription;
        }

        #endregion

        #endregion
    }
}