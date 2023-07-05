using System.Text;

namespace TODO
{
    class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; } = new Category();

        public override string ToString()
        {
            const char separator = ',';
            StringBuilder s = new StringBuilder();
            s.Append(this.Description);
            s.Append(separator);
            s.Append(this.Category.Name);
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
                    this.Category = new Category() { Name = strParts[1] };
            }
        }
    }
}
