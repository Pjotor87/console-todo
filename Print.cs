using System;
using System.Collections.Generic;

namespace TODO
{
    public class Print
    {
        public static void Line(string line, ConsoleColor color, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            Lines(new string[] { line }, color, clearConsole, readKeyAtEnd);
        }

        public static void Lines(IEnumerable<string> lines, ConsoleColor color, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            WriteColoredLines(lines, color, readKeyAtEnd, readKeyAtEnd: clearConsole);
        }

        public static void NewLine(string line, ConsoleColor color, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            NewLines(new string[] { line }, color, clearConsole, readKeyAtEnd);
        }

        public static void NewLines(IEnumerable<string> lines, ConsoleColor color, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            WriteColoredLines(lines, color, true, clearConsole, readKeyAtEnd);
        }

        private static void WriteColoredLines(IEnumerable<string> lines, ConsoleColor color, bool newLine = false, bool clearConsole = false, bool readKeyAtEnd = false)
        {
            if (clearConsole)
                Console.Clear();

            Console.ForegroundColor = color;
            foreach (var line in lines)
                if (!newLine)
                    Console.Write(line); 
                else
                    Console.WriteLine(line);
            Console.ResetColor();

            if (readKeyAtEnd)
                Console.ReadKey();
        }
    }
}
