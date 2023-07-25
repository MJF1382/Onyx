namespace Onyx.Classes
{
    public static class StringExtensions
    {
        public static bool HasValue(this string input)
        {
            if (string.IsNullOrEmpty(input) == false && string.IsNullOrWhiteSpace(input) == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
