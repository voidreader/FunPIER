namespace UMP.Services.Helpers
{
    internal static class Text
    {
        public static string StringBetween(string prefix, string suffix, string parent)
        {
            var start = parent.IndexOf(prefix) + prefix.Length;

            if (start < prefix.Length)
                return string.Empty;

            var end = parent.IndexOf(suffix, start);

            if (end == -1)
                end = parent.Length;

            return parent.Substring(start, end - start);
        }

        public static int SkipWhitespace(this string text, int start)
        {
            var result = start;

            while (char.IsWhiteSpace(text[result]))
                result++;

            return result;
        }
    }
}
