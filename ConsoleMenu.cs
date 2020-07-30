using System;
using System.Collections.Generic;
using System.Linq;

namespace TODO
{
    public class ConsoleMenu
    {
        public List<KeyValuePair<string, string>> Choices { get; set; } = new List<KeyValuePair<string, string>>();
        public ICollection<string> ValidInputs { get { return this.Choices.Select(x => x.Key).ToList(); } }

        public ConsoleColor IdWrapColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor IdColor { get; set; } = ConsoleColor.White;
        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public int SpacingCount { get; set; } = 1;
        public bool ClearConsole { get; set; } = false;
        public bool ReadKeyAtEnd { get; set; } = false;
        public bool PrintChoices { get; set; } = true;

        public virtual void SetNumberedChoices(IList<string> choices)
        {
            int counter = 1;
            foreach (var choice in choices)
                this.Choices.Add(new KeyValuePair<string, string>(counter++.ToString(), choice));
        }

        public virtual void OutputToConsole(string beforeMessage = null, ConsoleColor beforeMessageColor = ConsoleColor.Gray, string afterMessage = null, ConsoleColor afterMessageColor = ConsoleColor.Gray, bool skipBeforeMessage = false, bool skipAfterMessage = false)
        {
            if (!skipBeforeMessage && this.Choices != null && this.Choices.Count > 0)
            {
                Print.NewLine();
                Print.NewLine(!string.IsNullOrEmpty(beforeMessage) ? beforeMessage : "Menu\n----", beforeMessageColor);
            }

            foreach (var choice in this.Choices)
                OutputMenuChoiceToConsole(
                    choice.Value,
                    this.PrintChoices ? choice.Key.ToString() : null);

            if (!skipAfterMessage && this.Choices != null && this.Choices.Count > 0)
            {
                Print.NewLine();
                Print.NewLine(!string.IsNullOrEmpty(afterMessage) ? afterMessage : "Please make a selection. Just enter without typing anything will abort.", afterMessageColor);
            }
        }

        public virtual string GetUserInput()
        {
            string userInput;
            do
            {
                userInput = Console.ReadLine();
                if (this.ValidInputs != null)
                {
                    if (string.IsNullOrEmpty(userInput) || // Enter keypress
                        this.ValidInputs.Contains(userInput))
                        break;
                }
                else
                    break;
            } while (true);

            return userInput;
        }

        public virtual void OutputMenuChoiceToConsole(string text, string id = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Print.Line($"(", this.IdWrapColor);
                Print.Line(id, this.IdColor);
                Print.Line($")", this.IdWrapColor);
                Print.Line(new string(' ', this.SpacingCount));
            }
            Print.NewLine(text, this.TextColor);
        }
    }
}
