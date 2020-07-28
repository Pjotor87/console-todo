using System;
using System.Collections.Generic;

namespace TODO
{
    public enum PriorityLevel
    {
        Urgent,
        High,
        Medium,
        Low,
        Sometime,
        None
    }

    public static class Priority
    {
        public static List<PriorityLevel> GetSortOrder()
        {
            return new List<PriorityLevel>
            {
                PriorityLevel.Urgent,
                PriorityLevel.High,
                PriorityLevel.Medium,
                PriorityLevel.Low,
                PriorityLevel.Sometime,
                PriorityLevel.None
            };
        }

        public static ConsoleColor GetColor(PriorityLevel priority)
        {
            switch (priority)
            {
                case PriorityLevel.Urgent:
                    return ConsoleColor.Red;
                case PriorityLevel.High:
                    return ConsoleColor.Yellow;
                case PriorityLevel.Medium:
                    return ConsoleColor.Cyan;
                case PriorityLevel.Low:
                    return ConsoleColor.Blue;
                case PriorityLevel.Sometime:
                    return ConsoleColor.Green;
                case PriorityLevel.None:
                    return ConsoleColor.Magenta;
                default:
                    return ConsoleColor.Gray;
            }
        }
    }
}
