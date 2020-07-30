using System;
using System.Text;

namespace TODO
{
    class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public PriorityLevel PriorityLevel { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.Description);
            s.Append(',');
            s.Append(this.PriorityLevel.ToString());
            s.Append(',');
            s.Append(this.Color.ToString());
            return s.ToString();
        }

        public void FromString(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Contains(','))
            {
                string[] strParts = str.Split(',');

                if (strParts.Length >= 1)
                    this.Description = strParts[0];
                if (strParts.Length >= 2)
                {
                    var priority = PriorityLevel.None;
                    Enum.TryParse(strParts[1], out priority);
                    this.PriorityLevel = priority;
                }
                if (strParts.Length >= 2)
                {
                    var color = ConsoleColor.Gray;
                    Enum.TryParse(strParts[2], out color);
                    this.Color = color;
                }
            }
        }
    }
}
