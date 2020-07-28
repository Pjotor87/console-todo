using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                Console.Clear();
                if (File.Exists(todoPath))
                {
                    var todos = ReadTodos(todoPath);
                    PrintTodos(todos);
                    Print.NewLine("\nWrite a new todo to add it. A number to edit. Just enter without typing anything to exit.", ConsoleColor.White);

                    string choice = Console.ReadLine();
                    if (string.IsNullOrEmpty(choice))
                        exit = true;
                    else if (IsSelection(choice, todos))
                        RunSelection(todoPath, todos, choice);
                    else if (TodoExists(todos, choice))
                        exit = false;
                    else
                    {
                        todos.Add(new TodoEntry() { Description = choice, PriorityLevel = GetPriorityForNewEntry() });
                        SaveTodos(todos, todoPath);
                        exit = false;
                    }
                }
                else
                {
                    File.WriteAllText(todoPath, string.Empty, Encoding.Default);
                    exit = false;
                }
            }

            return !exit;
        }

        private static void PrintTodos(IList<TodoEntry> todos)
        {
            foreach (var priority in Priority.GetSortOrder())
            {
                var todoEntries = todos.Where(x => x.PriorityLevel == priority).ToList();
                if (todoEntries != null && todoEntries.Count > 0)
                {
                    Print.NewLine(priority.ToString(), Priority.GetColor(priority));
                    foreach (var entry in todoEntries)
                    {
                        Menu.PrintMenuChoice(entry.Description, entry.Id.ToString());
                    }
                }
            }
        }

        private static void RunSelection(string todoPath, IList<TodoEntry> todos, string choice)
        {
            var todoSelection = todos.Where(x => x.Id == Convert.ToInt32(choice)).Single();

            Console.Clear();

            Print.Line($"You selected: ");
            Print.NewLine(todoSelection.Description, todoSelection.Color);
            Print.Line($"Priority level is: ");
            Print.NewLine(todoSelection.PriorityLevel.ToString(), Priority.GetColor(todoSelection.PriorityLevel));

            var menu = new List<KeyValuePair<char, string>>() {
                new KeyValuePair<char, string>('d', "(D)escription edit"),
                new KeyValuePair<char, string>('p', "(P)riority edit"),
                new KeyValuePair<char, string>('r', "(R)emove"),
            };
            Menu.WithCharChoices(menu, printChoices: false);
            Print.NewLine("\nPlease make a selection. Just enter without typing anything will abort.");
            string modify = Menu.GetUserInput(menu.Select(x => x.Key).ToList()).ToString().ToLower();

            if (modify == menu[0].Key.ToString()) // d
            {
                string newDescription = GetNewDescription(todoSelection, todos);
                if (!string.IsNullOrEmpty(newDescription))
                {
                    UpdateTodo(todos, todoSelection, new TodoEntry() { Description = newDescription, PriorityLevel = todoSelection.PriorityLevel });
                    SaveTodos(todos, todoPath);
                }
            }
            else if (modify == menu[1].Key.ToString()) // p
            {
                UpdateTodo(todos, todoSelection, new TodoEntry() { Description = todoSelection.Description, PriorityLevel = GetPriorityForNewEntry() });
                SaveTodos(todos, todoPath);
            }
            else if (modify == menu[2].Key.ToString()) // r
            {
                todos.Remove(todoSelection);
                SaveTodos(todos, todoPath);
            }
        }

        #region User input

        private static PriorityLevel GetPriorityForNewEntry()
        {
            var priority = PriorityLevel.None;

            var priorities = Enum.GetNames(typeof(PriorityLevel)).ToList();
            
            int counter = 1;
            foreach (var item in priorities)
                Console.WriteLine($"({counter++}) {item}");

            Console.WriteLine($"Select priority level. Anything not in the list will select None.");
            string selection = Console.ReadKey().KeyChar.ToString();
            if (IsNumeric(selection.ToString()))
            {
                int selectionAsInt = Convert.ToInt32(selection);
                for (int i = 0; i < priorities.Count; i++)
                    if (i + 1 == selectionAsInt)
                        Enum.TryParse(priorities[i], out priority);
            }

            return priority;
        }

        #region Modify item

        private static string GetNewDescription(TodoEntry todoSelection, IList<TodoEntry> todos)
        {
            Print.NewLine("\nWrite a new description for this todo. Just enter without typing anything will exit.");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription) && TodoExists(todos, newDescription))
            {
                Print.NewLine("The entry already exists. Try again.", ConsoleColor.Red);
                return GetNewDescription(todoSelection, todos);
            }
            return newDescription;
        }

        private static void UpdateTodo(IList<TodoEntry> todos, TodoEntry oldTodo, TodoEntry newTodo)
        {
            todos.Add(newTodo);
            todos.Remove(oldTodo);
        }

        #endregion

        #endregion

        #region Read and Save

        private static IList<TodoEntry> ReadTodos(string todoPath)
        {
            var todos = new List<TodoEntry>();

            IList<string> todoLines = File.ReadAllLines(todoPath, Encoding.Default);
            int counter = 1;

            foreach (var item in todoLines)
                if (!string.IsNullOrEmpty(item) && item.Contains(','))
                {
                    var priorityTemp = PriorityLevel.None;
                    Enum.TryParse(item.Split(',')[1], out priorityTemp);
                    todos.Add(new TodoEntry() { Id = counter, Description = item.Split(',')[0], PriorityLevel = priorityTemp });
                    counter++;
                }

            return todos;
        }

        private static void SaveTodos(IList<TodoEntry> todosToSave, string todoPath)
        {
            if (File.Exists(todoPath))
            {
                todosToSave = todosToSave.OrderBy(x => x.PriorityLevel).ToList();

                var newLines = new List<string>();
                foreach (var item in todosToSave)
                {
                    newLines.Add(item.ToString());
                }
                File.WriteAllLines(todoPath, newLines, Encoding.Default);
            }
        }

        #endregion

        #region Validation

        private static bool IsNumeric(string str)
        {
            int isNumeric;
            return int.TryParse(str, out isNumeric);
        }

        private static bool IsSelection(string choice, IList<TodoEntry> todos)
        {
            if (IsNumeric(choice))
            {
                int selection = Convert.ToInt32(choice);
                if (todos.Where(x => x.Id == selection).SingleOrDefault() != null)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TodoExists(IList<TodoEntry> todos, string newDescription)
        {
            return todos.Where(x => x.Description == newDescription).SingleOrDefault() != null;
        }

        #endregion
    }
}