using System;
using System.Collections.Generic;

namespace TODO
{
    public static class CategoryDefaults
    {
        public static IList<Category> GetCategories()
        {
            int sortOrder = 1;
            var categories = new List<Category>();
            categories.Add(new Category() { Name = "Urgent", Color = ConsoleColor.Red, SortOrder = sortOrder++ });
            categories.Add(new Category() { Name = "High", Color = ConsoleColor.Yellow, SortOrder = sortOrder++ });
            categories.Add(new Category() { Name = "Medium", Color = ConsoleColor.Cyan, SortOrder = sortOrder++ });
            categories.Add(new Category() { Name = "Low", Color = ConsoleColor.Blue, SortOrder = sortOrder++ });
            categories.Add(new Category() { Name = "Sometime", Color = ConsoleColor.Green, SortOrder = sortOrder++ });
            
            var defaultCategory = GetDefaultCategory();
            defaultCategory.SortOrder = sortOrder++;
            categories.Add(defaultCategory);
            
            return categories;
        }

        public static Category GetDefaultCategory()
        {
            return new Category() { Name = "None", Color = ConsoleColor.Magenta, SortOrder = 1 };
        }
    }
}
