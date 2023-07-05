using System;
using System.Text;

namespace TODO
{
    public class Category
    {
        public int SortOrder { get; set; } = -1;
        public string Name { get; set; } = "None";
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;

        public override string ToString()
        {
            const char separator = ',';
            StringBuilder s = new StringBuilder();
            s.Append(this.Name);
            s.Append(separator);
            s.Append(this.Color);
            s.Append(separator);
            s.Append(this.SortOrder);
            return s.ToString();
        }

        public void FromString(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Contains(','))
            {
                string[] strParts = str.Split(',');

                if (strParts.Length >= 1)
                    this.Name = strParts[0];

                if (strParts.Length >= 2)
                    if (Enum.TryParse(strParts[1], out ConsoleColor color))
                        this.Color = color;

                if (strParts.Length >= 3)
                    this.SortOrder = Convert.ToInt32(strParts[2]);
            }
        }
    }
}
