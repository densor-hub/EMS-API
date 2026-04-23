namespace WebApplication1.Helpers
{
    public static class StringExtensions
    {
        public static string GetInitials(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(nameParts.Select(x => char.ToUpper(x[0])));
        }
    }
}
