using System;

namespace TODO
{
    public class CharConsoleMenu : ConsoleMenu
    {
        public char AbortSwitch { get; set; }
        public CharConsoleMenu(char abortSwitch = (char)13)
        {
            this.AbortSwitch = abortSwitch;
        }

        public override string GetUserInput()
        {
            char userInput;
            do
            {
                userInput = Console.ReadKey().KeyChar;
                if (this.ValidInputs != null)
                {
                    if (userInput == this.AbortSwitch || // Enter keypress
                        this.ValidInputs.Contains(userInput.ToString()))
                        break;
                }
                else
                    break;
            } while (true);

            Console.WriteLine();

            return userInput.ToString();
        }
    }
}
