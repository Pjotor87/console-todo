using System;
using System.Collections.Generic;
using System.Linq;

namespace TODO
{
    public static class Printer
    {
        internal static void PrintNoArgumentsPassed()
        {
            Print.NewLines(new string[] {
                "This program requires one argument passed to it.",
                "Pass a path to an empty .txt file, then try running the program again."
            }, ConsoleColor.Yellow);
        }

        #region Todo List

        internal static void PrintTodoList(IList<TodoEntry> todos)
        {
            PrintPriorityTodoList(todos, Priority.Urgent);
            PrintPriorityTodoList(todos, Priority.High);
            PrintPriorityTodoList(todos, Priority.Medium);
            PrintPriorityTodoList(todos, Priority.Low);
            PrintPriorityTodoList(todos, Priority.Sometime);
            PrintPriorityTodoList(todos, Priority.None);
            Print.NewLine("\nWrite a new todo to add it. A number to edit. Just enter without typing anything to exit.", ConsoleColor.White);
        }

        internal static void PrintPriorityTodoList(IList<TodoEntry> todos, Priority priority)
        {
            var todoEntries = todos.Where(x => x.PriorityLevel == priority).ToList();
            if (todoEntries != null && todoEntries.Count > 0)
            {
                Print.NewLine(priority.ToString(), GetPriorityColor(priority));
                foreach (var entry in todoEntries)
                {
                    PrintMenuChoice(entry);
                }
            }
        }

        private static void PrintMenuChoice(TodoEntry entry)
        {
            Console.Write($"(");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{entry.Id}");
            Console.ResetColor();
            Console.Write($") ");
            Console.ForegroundColor = entry.Color;
            Console.WriteLine($"{entry.Description}");
            Console.ResetColor();
        }

        #endregion

        #region Selection

        internal static void PrintSelection(TodoEntry todoSelection)
        {
            Console.Write($"You selected: ");
            PrintDescription(todoSelection);
            Console.Write($"Priority level is: ");
            PrintPriority(todoSelection);
        }

        #endregion

        internal static void PrintPriority(TodoEntry todoSelection)
        {
            Print.NewLine(todoSelection.PriorityLevel.ToString(), GetPriorityColor(todoSelection.PriorityLevel));
        }

        internal static ConsoleColor GetPriorityColor(Priority priority)
        {
            switch (priority)
            {
                case Priority.Urgent:
                    return ConsoleColor.Red;
                case Priority.High:
                    return ConsoleColor.Yellow;
                case Priority.Medium:
                    return ConsoleColor.Cyan;
                case Priority.Low:
                    return ConsoleColor.Blue;
                case Priority.Sometime:
                    return ConsoleColor.Green;
                case Priority.None:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.Gray;
            }
        }

        internal static void PrintDescription(TodoEntry todo)
        {
            Print.NewLine(todo.Description, todo.Color);
        }

        internal static void PrintTodoExists()
        {
            Print.NewLine("The entry already exists. Try again.", ConsoleColor.Red);
        }

        internal static void PrintPressAnyKeyToExit()
        {
            Print.NewLine("\nPress the any key to exit...", ConsoleColor.White, readKeyAtEnd: true);
        }
    }
}
