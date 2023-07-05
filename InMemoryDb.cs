using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TODO
{
    class InMemoryDb
    {
        public IList<Category> Categories { get; set; }
        public IList<Todo> Todos { get; set; }
        public string FilePath { get; set; }
        public string SectionDelimeter { get { return "########"; } }
        public InMemoryDb(string todoPath, bool load = true)
        {
            this.FilePath = todoPath;
            if (load)
                this.Load();
        }

        #region Read and Save

        public void Initialize()
        {
            File.WriteAllText(this.FilePath, string.Empty, Encoding.Default);
        }

        public void Load()
        {
            var categories = new List<Category>();
            var todos = new List<Todo>();

            var sections = GetSections();
            var sectionStrings = GetSectionStrings(sections);

            IList<string> lines = File.ReadAllLines(this.FilePath, Encoding.Default);

            string currentSection = string.Empty;
            int idCounter = 1;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (IsSectionString(line))
                {
                    string sectionString = line.Trim();

                    if (sectionStrings.Contains(sectionString))
                        currentSection = GetSection(sectionString);

                    idCounter = 1;

                    continue;
                }
                
                if (currentSection == sections[0])
                {
                    var item = new Category();
                    item.FromString(line);
                    
                    categories.Add(item);
                }
                else if (currentSection == sections[1])
                {
                    var item = new Todo() { Id = idCounter++ };
                    item.FromString(line);
                    todos.Add(item);
                }
            }

            if (categories.Count == 0)
                categories = CategoryDefaults.GetCategories().ToList();
            this.Categories = categories.OrderBy(x => x.SortOrder).ToList();
            this.Todos = todos;
        }

        public void Save()
        {
            if (File.Exists(this.FilePath))
            {
                var newLines = new List<string>();

                var sections = GetSections();
                int sectionCounter = 0;

                this.Categories = this.Categories.OrderBy(x => x.SortOrder).ToList();
                newLines.Add(GetSectionString(sections[sectionCounter++]));
                foreach (var item in this.Categories)
                    newLines.Add(item.ToString());

                this.Todos = this.Todos.OrderBy(x => x.Category.SortOrder).ToList();
                newLines.Add(GetSectionString(sections[sectionCounter++]));
                foreach (var item in this.Todos)
                    newLines.Add(item.ToString());

                File.WriteAllLines(this.FilePath, newLines, Encoding.Default);
            }
        }

        private IList<string> GetSections()
        {
            return new List<string>() { "Categories", "Todos" };
        }

        private IList<string> GetSectionStrings(IList<string> sections)
        {
            var sectionStrings = new List<string>();

            foreach (var section in sections)
                sectionStrings.Add(GetSectionString(section));

            return sectionStrings;
        }

        private string GetSectionString(string section)
        {
            return $"{SectionDelimeter} {section} {SectionDelimeter}";
        }

        private string GetSection(string sectionString)
        {
            return sectionString.Replace(SectionDelimeter, "").Trim();
        }

        private bool IsSectionString(string str)
        {
            return str.StartsWith(SectionDelimeter) && str.TrimEnd().EndsWith(SectionDelimeter);
        }

        #endregion

        #region Print

        public void OutputTodosToConsole()
        {
            foreach (var category in this.Categories)
            {
                var todosForCategory = this.Todos.Where(x => x.Category.Name == category.Name).ToList();
                if (todosForCategory != null && todosForCategory.Count > 0)
                {
                    var menu = new ConsoleMenu();
                    
                    foreach (var entry in todosForCategory)
                        menu.Choices.Add(new KeyValuePair<string, string>(entry.Id.ToString(), entry.Description));

                    menu.OutputToConsole(category.Name, category.Color, skipAfterMessage: true);
                }
            }
        }

        public void OutputCategoriesToConsole(bool useSortOrderForSelection = true)
        {
            int selectionValueCounter = 1;
            foreach (var category in this.Categories.OrderBy(x => x.SortOrder))
            {
                Print.Line($"(", category.Color);
                Print.Line(!useSortOrderForSelection ? selectionValueCounter.ToString() : category.SortOrder.ToString(), category.Color);
                Print.Line($")", category.Color);
                Print.Line(new string(' ', 1));
                Print.NewLine(category.Name, category.Color);
                selectionValueCounter++;
            }
        }

        #endregion

        public bool TodoIdExists(string id)
        {
            return
                int.TryParse(id, out _) ?
                this.TodoIdExists(Convert.ToInt32(id)) :
                false;
        }

        public bool CategorySortOrderExists(string sortOrder)
        {
            return
                int.TryParse(sortOrder, out _) ?
                this.CategorySortOrderExists(Convert.ToInt32(sortOrder)) :
                false;
        }

        public bool TodoIdExists(int id)
        {
            return this.FindTodoById(id) != null;
        }

        public bool CategorySortOrderExists(int sortOrder)
        {
            return this.FindCategoryBySortOrder(sortOrder) != null;
        }

        public bool DbExists()
        {
            return File.Exists(this.FilePath);
        }

        public bool TodoExists(string description)
        {
            return this.Todos.Where(x => x.Description == description).SingleOrDefault() != null;
        }

        public void AddTodo(Todo todo)
        {
            this.Todos.Add(todo);
        }

        public void UpdateTodo(Todo oldTodo, Todo newTodo)
        {
            this.Todos.Add(newTodo);
            this.Todos.Remove(oldTodo);
        }

        internal void RemoveTodo(Todo todoSelection)
        {
            this.Todos.Remove(todoSelection);
        }

        internal Todo FindTodoById(string id)
        {
            return this.FindTodoById(Convert.ToInt32(id));
        }

        internal Todo FindTodoById(int id)
        {
            return this.Todos.Where(x => x.Id == id).SingleOrDefault();
        }

        internal Category FindCategoryBySortOrder(int sortOrder)
        {
            return this.Categories.Where(x => x.SortOrder == sortOrder).SingleOrDefault();
        }

        internal void AddCategory(Category category)
        {
            this.Categories.Add(category);
        }

        internal int GetNextSortOrderValue(int incrementBy = 1)
        {
            return this.Categories.Max(x => x.SortOrder) + incrementBy;
        }

        internal void IncrementAllSortOrdersAbove(int sortOrder)
        {
            var categoriesWithSortOrdersAbove = this.Categories.Where(x => x.SortOrder >= sortOrder).OrderBy(x => x.SortOrder);

            int lastSortOrder = sortOrder;
            foreach (var category in categoriesWithSortOrdersAbove)
                if (category.SortOrder != lastSortOrder)
                    break;
                else
                {
                    category.SortOrder++;
                    lastSortOrder++;
                }
        }

        internal void RemoveAllCategories()
        {
            this.Categories.Clear();

            var defaultCategory = CategoryDefaults.GetDefaultCategory();
            this.Categories.Add(defaultCategory);

            foreach (var todo in this.Todos)
                todo.Category = defaultCategory;
        }
    }
}
