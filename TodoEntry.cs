using System;
using System.Collections.Generic;
using System.Text;

namespace TODO
{
    class TodoEntry
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Priority PriorityLevel { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.Description);
            s.Append(',');
            s.Append(this.PriorityLevel.ToString());
            return s.ToString();
        }
    }
}
