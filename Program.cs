using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace TODO
{
    class Program
    {
        private static string mmm = "mmm";
        private static string DefaultDbPath = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), "todo.txt")).AbsolutePath.ToString();

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
                string fileUsedWhenDebugging = DefaultDbPath;
                if (File.Exists(fileUsedWhenDebugging))
                    File.Delete(fileUsedWhenDebugging);
                todoPath = fileUsedWhenDebugging;
            }
            else if (args != null)
            {
                if (args.Length > 0)
                    todoPath = args[0];
                else if (File.Exists(DefaultDbPath))
                    todoPath = DefaultDbPath;
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
                }, ConsoleColor.Red);
                var menu = new CharConsoleMenu()
                {
                    Choices = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("y", "es"),
                        new KeyValuePair<string, string>("n", "o")
                    },
                    SpacingCount = 0,
                    IdWrapColor = ConsoleColor.Cyan
                };
                Print.NewLines(new string[] {
                    "Or, would you like to create a new db at the following path",
                    DefaultDbPath,
                    "?"
                }, ConsoleColor.Yellow);
                menu.OutputToConsole(skipBeforeMessage: true);
                string choice = menu.GetUserInput().ToLower();
                if (choice == "y")
                {
                    File.WriteAllText(DefaultDbPath, string.Empty, Encoding.Default);
                    RunLoop(DefaultDbPath);
                }
            }
        }

        private static bool Run(string todoPath, bool exit = false)
        {
            if (!exit)
            {
                var db = new InMemoryDb(todoPath, false);

                if (!db.DbExists())
                {
                    db.Initialize();
                    exit = false;
                }
                else
                {
                    Console.Clear();
                    db.Load();
                    OutputAppTitle("console-todo");
                    db.OutputTodosToConsole();
                    Print.NewLine("\nWrite a new todo to add it. A number to edit. 'c' to edit categories. Just enter without typing anything to exit.\n", ConsoleColor.White);
                    exit = GetMainMenuUserInput(db);
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

        private static bool GetMainMenuUserInput(InMemoryDb db)
        {
            bool exit = false;

            string choice = Console.ReadLine();
            if (string.IsNullOrEmpty(choice))
                exit = true;
            else if (db.TodoIdExists(choice))
                RunSelection(db, choice);
            else if (choice.ToLower() == "c")
                RunManageCategories(db);
            else if (!db.TodoExists(choice))
            {
                var category = GetCategoryForNewEntry(db);
                db.AddTodo(new Todo() { Description = choice, Category = category });
                db.Save();
            }
            else if (choice == mmm)
                MMM();

            return exit;
        }

        private static void RunManageCategories(InMemoryDb db)
        {
            bool exit = false;

            while (!exit)
            {
                db.OutputCategoriesToConsole();
                Print.NewLine("\nWrite a new category to add it. A number to edit. 'r' to remove all categories. Just enter without typing anything to exit back to todos.\n", ConsoleColor.White);
                string choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice))
                    exit = true;
                else if (choice.ToLower() == "r")
                {
                    RunDeleteAllCategories(db);
                    db.Save();
                }
                else if (!db.CategorySortOrderExists(choice))
                {
                    var category = new Category();
                    category.Name = choice;
                    category.Color = GetColorForCategoryFromUser(db);
                    category.SortOrder = GetSortOrderForCategoryFromUser(db);
                    db.AddCategory(category);
                    db.Save();
                }
                else if (choice == mmm)
                    MMM();
                else if (db.CategorySortOrderExists(choice))
                {
                    RunManageCategory(db, choice);
                    db.Save();
                }
            }
        }

        private static void RunManageCategory(InMemoryDb db, string categorySortOrder)
        {
            bool exit = false;

            var category = db.Categories.Single(x => x.SortOrder == Convert.ToInt32(categorySortOrder));

            while (!exit)
            {
                var menu = new CharConsoleMenu()
                {
                    Choices = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("n", "ame"),
                        new KeyValuePair<string, string>("c", "olor"),
                        new KeyValuePair<string, string>("s", "ort order"),
                    },
                    SpacingCount = 0,
                    IdWrapColor = ConsoleColor.Cyan,
                    TextColor = ConsoleColor.Yellow
                };
                menu.OutputToConsole(
                    afterMessage: "Just pressing enter will go back to categories.",
                    beforeMessage: "What would you like to edit?", beforeMessageColor: ConsoleColor.Green);
                var selection = Convert.ToChar(menu.GetUserInput());
                {
                    if (selection == '\r')
                        exit = true;
                    else if (selection == 'n')
                    {
                        db.OutputCategoriesToConsole();
                        Print.NewLine("\nWrite a new category name that is not already in use. Just enter without typing anything to exit back to editing the category.\n", ConsoleColor.White);
                        string choice = Console.ReadLine();
                        if (!string.IsNullOrEmpty(choice))
                            if (!db.Categories.Any(x => x.Name == choice))
                                category.Name = choice;
                            else
                                Print.NewLine("\nThat name is already in use. Try again.\n", ConsoleColor.Red);
                    }
                    else if (selection == 'c')
                        category.Color = GetColorForCategoryFromUser(db);
                    else if (selection == 's')
                        category.SortOrder = GetSortOrderForCategoryFromUser(db, category);
                }
            }
        }

        private static void RunDeleteAllCategories(InMemoryDb db)
        {
            var menu = new CharConsoleMenu()
            {
                Choices = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("y", "es"),
                    new KeyValuePair<string, string>("n", "o")
                },
                SpacingCount = 0,
                IdWrapColor = ConsoleColor.Cyan
            };
            menu.OutputToConsole(
                beforeMessage: "You are about to remove all categories. Are you sure? (Any existing todos will be placed in the category None)",
                beforeMessageColor: ConsoleColor.Red);
            string choice = menu.GetUserInput().ToLower();
            if (choice == "y")
                db.RemoveAllCategories();
        }

        private static int GetSortOrderForCategoryFromUser(InMemoryDb db, Category category = null)
        {
            var menu = new ConsoleMenu();
            menu.TextColor = ConsoleColor.Yellow;
            menu.OutputToConsole(
                afterMessage: "Just pressing enter will set it last.",
                beforeMessage: "Enter sort order", beforeMessageColor: ConsoleColor.Green,
                customChoicesOutputFunction: () => { db.OutputCategoriesToConsole(); } );
            var selection = menu.GetNumericUserInput();

            int sortOrder;
            {
                if (!string.IsNullOrEmpty(selection))
                    sortOrder = int.Parse(selection);
                else if (!db.Categories.Any())
                    sortOrder = 1;
                else
                {
                    var lastCategorySortOrder = db.Categories.OrderByDescending(x => x.SortOrder).First().SortOrder;
                    if (category == null || lastCategorySortOrder != category.SortOrder)
                        sortOrder = lastCategorySortOrder + 1;
                    else
                        sortOrder = category.SortOrder;
                }
            }

            if (db.CategorySortOrderExists(sortOrder))
                db.IncrementAllSortOrdersAbove(sortOrder);

            return sortOrder;
        }

        private static ConsoleColor GetColorForCategoryFromUser(InMemoryDb db)
        {
            ConsoleColor color;

            var colors = Enum.GetValues(typeof(ConsoleColor))
                 .Cast<ConsoleColor>()
                 .Select(color => color.ToString())
                 .ToList();
            
            var menu = new ConsoleMenu();
            menu.TextColor = ConsoleColor.Yellow;
            menu.SetNumberedChoices(colors);
            menu.OutputToConsole(
                afterMessage: "Just pressing enter will select Gray.",
                beforeMessage: "Select Color", beforeMessageColor: ConsoleColor.Green);
            var selection = menu.GetUserInput();
            {
                if (selection == '\r'.ToString())
                    color = ConsoleColor.Gray;
                else
                {
                    string colorName = menu.Choices.Single(x => x.Key == selection).Value;
                    Enum.TryParse(colorName, out color);
                }
            }

            return color;
        }

        private static void RunSelection(InMemoryDb db, string choice)
        {
            var todoSelection = db.FindTodoById(choice);

            Print.Line($"You selected: ", clearConsole: true);
            Print.NewLine(todoSelection.Description, todoSelection.Category.Color);
            Print.Line($"Priority level/Category is: ");
            Print.NewLine(todoSelection.Category.ToString(), todoSelection.Category.Color);

            var menu = new CharConsoleMenu()
            {
                Choices = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("d", "escription edit"),
                    new KeyValuePair<string, string>("p", "riority/category edit"),
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
                string newDescription = GetNewDescription(todoSelection, db);
                if (!string.IsNullOrEmpty(newDescription))
                {
                    db.UpdateTodo(todoSelection, new Todo() { Description = newDescription, Category = todoSelection.Category });
                    db.Save();
                }
            }
            else if (modify == menu.Choices[1].Key.ToString()) // p
            {
                var category = GetCategoryForNewEntry(db);
                db.UpdateTodo(todoSelection, new Todo() { Description = todoSelection.Description, Category = category });
                db.Save();
            }
            else if (modify == menu.Choices[2].Key.ToString()) // r
            {
                db.RemoveTodo(todoSelection);
                db.Save();
            }
        }

        #region User input

        private static Category GetCategoryForNewEntry(InMemoryDb db)
        {
            Category category;

            var menu = new CharConsoleMenu();
            menu.TextColor = ConsoleColor.Yellow;
            menu.SetNumberedChoices(db.Categories.Select(x => x.Name).ToList());
            menu.OutputToConsole(
                afterMessage: "Just pressing enter will select None.",
                beforeMessage: "Select Priority level/Category", beforeMessageColor: ConsoleColor.Green,
                customChoicesOutputFunction: () => { db.OutputCategoriesToConsole(false); });
            var selection = menu.GetUserInput();
            {
                if (selection == '\r'.ToString())
                    category = new Category();
                else
                {
                    string categoryName = menu.Choices.Single(x => x.Key == selection).Value;
                    category = db.Categories.Single(x => x.Name == categoryName);
                }
            }

            return category;
        }

        #region Modify item

        private static string GetNewDescription(Todo todoSelection, InMemoryDb db)
        {
            Print.NewLine("\nWrite a new description for this todo. Just enter without typing anything will exit.");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription) && db.TodoExists(newDescription))
            {
                Print.NewLine("The entry already exists. Try again.", ConsoleColor.Red);
                return GetNewDescription(todoSelection, db);
            }
            return newDescription;
        }

        #endregion

        #endregion
    }
}