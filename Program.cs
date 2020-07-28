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
                bool keepRunning = true;
                do
                {
                    keepRunning = Run(todoPath);
                } while (keepRunning);
            }
            else
            {
                Printer.PrintNoArgumentsPassed();
            }
        }

        private static bool Run(string todoPath, bool exit = false)
        {
            if (!exit)
            {
                Console.Clear();
                if (File.Exists(todoPath))
                {
                    var todos = ReadTodo(todoPath);
                    Printer.PrintTodoList(todos);
                    
                    string choice = Console.ReadLine();

                    if (string.IsNullOrEmpty(choice))
                    {
                        return false;
                    }
                    else if (IsSelection(choice, todos))
                    {
                        return RunSelection(todoPath, todos, choice);
                    }
                    else if (TodoExists(todos, choice))
                    {
                        return true;
                    }
                    else
                    {
                        todos.Add(new TodoEntry() { Description = choice, PriorityLevel = GetPriorityForNewEntry() });
                        SaveTodos(todos, todoPath);
                        return true;
                    }
                }
                else
                {
                    File.WriteAllText(todoPath, string.Empty, Encoding.Default);
                    return true;
                }
            }

            return false;
        }

        private static bool RunSelection(string todoPath, IList<TodoEntry> todos, string choice)
        {
            var todoSelection = GetSelection(Convert.ToInt32(choice), todos);
            string modify = GetModify(todoSelection);
            switch (modify)
            {
                case "d":
                    string newDescription = GetNewDescription(todoSelection, todos);
                    if (!string.IsNullOrEmpty(newDescription))
                    {
                        UpdateTodo(todos, todoSelection, new TodoEntry() { Description = newDescription, PriorityLevel = todoSelection.PriorityLevel });
                        SaveTodos(todos, todoPath);
                    }
                    break;
                case "p":
                    UpdateTodo(todos, todoSelection, new TodoEntry() { Description = todoSelection.Description, PriorityLevel = GetPriorityForNewEntry() });
                    SaveTodos(todos, todoPath);
                    break;
                case "r":
                    todos.Remove(todoSelection);
                    SaveTodos(todos, todoPath);
                    break;
            }

            return true;
        }

        #region User input

        private static Priority GetPriorityForNewEntry()
        {
            var priority = Priority.None;

            var priorities = Enum.GetNames(typeof(Priority)).ToList();
            
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

        private static string GetModify(TodoEntry todoSelection)
        {
            Console.Clear();
            Printer.PrintSelection(todoSelection);
            Console.WriteLine("\nPlease make a selection. Any other keypress will abort.");
            Console.WriteLine("(D)escription edit");
            Console.WriteLine("(P)riority edit");
            Console.WriteLine("(R)emove");
            return Console.ReadKey().KeyChar.ToString().ToLower();
        }

        private static string GetNewDescription(TodoEntry todoSelection, IList<TodoEntry> todos)
        {
            Console.WriteLine("\nWrite a new description for this todo. Just enter without typing anything will exit.");
            string newDescription = Console.ReadLine();
            if (!string.IsNullOrEmpty(newDescription) && TodoExists(todos, newDescription))
            {
                Printer.PrintTodoExists();
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

        private static IList<TodoEntry> ReadTodo(string todoPath)
        {
            var todos = new List<TodoEntry>();

            IList<string> todoLines = File.ReadAllLines(todoPath, Encoding.Default);
            int counter = 1;

            foreach (var item in todoLines)
                if (!string.IsNullOrEmpty(item) && item.Contains(','))
                {
                    var priorityTemp = Priority.None;
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

        #region Filter

        private static TodoEntry GetSelection(int choice, IList<TodoEntry> todos)
        {
            return todos.Where(x => x.Id == choice).Single();
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