namespace TODO
{
    public class Validator
    {
        public static bool IsNumeric(string str)
        {
            int isNumeric;
            return int.TryParse(str, out isNumeric);
        }
    }
}
