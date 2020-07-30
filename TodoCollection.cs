using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TODO
{
    class TodoCollection
    {
        public IList<Todo> Items { get; set; }
        public string TodoPath { get; set; }
        public TodoCollection(string todoPath, bool load = true)
        {
            this.TodoPath = todoPath;
            if (load)
                this.Load();
        }

        #region Read and Save

        public void Initialize()
        {
            File.WriteAllText(this.TodoPath, string.Empty, Encoding.Default);
        }

        public void Load()
        {
            this.Items = new List<Todo>();

            IList<string> todoLines = File.ReadAllLines(this.TodoPath, Encoding.Default);
            int counter = 1;

            foreach (var line in todoLines)
                if (!string.IsNullOrEmpty(line) && line.Contains(','))
                {
                    var todo = new Todo() { Id = counter++ };
                    todo.FromString(line);
                    this.Items.Add(todo);
                }
        }

        public void Save()
        {
            if (File.Exists(this.TodoPath))
            {
                this.Items = this.Items.OrderBy(x => x.PriorityLevel).ToList();

                var newLines = new List<string>();
                foreach (var item in this.Items)
                {
                    newLines.Add(item.ToString());
                }
                File.WriteAllLines(this.TodoPath, newLines, Encoding.Default);
            }
        }

        #endregion

        #region Print

        public void OutputToConsole()
        {
            foreach (var priority in Priority.GetSortOrder())
            {
                var todoEntries = this.Items.Where(x => x.PriorityLevel == priority).ToList();
                if (todoEntries != null && todoEntries.Count > 0)
                {
                    var menu = new ConsoleMenu();
                    
                    foreach (var entry in todoEntries)
                    {
                        menu.Choices.Add(new KeyValuePair<string, string>(entry.Id.ToString(), entry.Description));
                    }

                    menu.OutputToConsole(priority.ToString(), Priority.GetColor(priority), skipAfterMessage: true);
                }
            }
        }

        #endregion

        public bool IdExists(string id)
        {
            return
                int.TryParse(id, out _) ?
                this.IdExists(Convert.ToInt32(id)) :
                false;
        }

        public bool IdExists(int id)
        {
            return this.FindById(id) != null;
        }

        public bool DbExists()
        {
            return File.Exists(this.TodoPath);
        }

        public bool Exists(string description)
        {
            return this.Items.Where(x => x.Description == description).SingleOrDefault() != null;
        }

        public void Add(Todo todo)
        {
            this.Items.Add(todo);
        }

        public void Update(Todo oldTodo, Todo newTodo)
        {
            this.Items.Add(newTodo);
            this.Items.Remove(oldTodo);
        }

        internal void Remove(Todo todoSelection)
        {
            this.Items.Remove(todoSelection);
        }

        internal Todo FindById(string id)
        {
            return this.FindById(Convert.ToInt32(id));
        }

        internal Todo FindById(int id)
        {
            return this.Items.Where(x => x.Id == id).SingleOrDefault();
        }
    }
}
