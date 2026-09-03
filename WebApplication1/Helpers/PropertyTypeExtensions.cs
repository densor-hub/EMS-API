namespace WebApplication1.Helpers
{
    public static class PropertyTypeExtensions
    {
        
        public static string ToAlphanumericDate(this DateTime? date, string format = "Standard", bool includeTime = false)
        {
            if (!date.HasValue || date.Value == DateTime.MinValue)
                return string.Empty;

            return date.Value.ToAlphanumericDate(format, includeTime);
        }

        public static string GetInitials(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(nameParts.Select(x => char.ToUpper(x[0])));
        }

        public static T ToEnum<T>(this string value, bool ignoreCase = true, T defaultValue = default) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            if (Enum.TryParse<T>(value, ignoreCase, out var result))
                return result;

            return defaultValue;
        }


        public static string ToAlphanumericDate(this DateTime date, string format = "Standard", bool includeTime = false)
        {
            if (date == null || date == DateTime.MinValue)
                return string.Empty;

            // Month names array
            var monthNames = new[]
            {
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

            var fullMonthNames = new[]
            {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

            var day = date.Day;
            var month = date.Month - 1; // 0-based for array
            var year = date.Year;

            switch (format.ToLower())
            {
                case "standard":
                    // "24 Jan 2025" or "24 Jan 2025 14:30"
                    var result = $"{day:D2} {monthNames[month]} {year}";
                    if (includeTime)
                        result += $" {date:HH:mm}";
                    return result;

                case "full":
                    // "24th January 2025"
                    return $"{GetOrdinalSuffix(day)} {fullMonthNames[month]} {year}";

                case "compact":
                    // "24Jan2025"
                    return $"{day:D2}{monthNames[month]}{year}";

                case "monthyear":
                    // "Jan 2025"
                    return $"{monthNames[month]} {year}";

                case "fullmonthyear":
                    // "January 2025"
                    return $"{fullMonthNames[month]} {year}";

                case "daymonth":
                    // "24 Jan"
                    return $"{day} {monthNames[month]}";

                case "alphanumeric":
                    // "24-Jan-2025"
                    return $"{day:D2}-{monthNames[month]}-{year}";

                case "alphanumericfull":
                    // "24-January-2025"
                    return $"{day:D2}-{fullMonthNames[month]}-{year}";

                case "invoice":
                    // "INV-2025-01-24"
                    return $"INV-{year:0000}-{date.Month:D2}-{day:D2}";

                default:
                    return $"{day:D2} {monthNames[month]} {year}";
            }
        }

        /// <summary>
        /// Gets the ordinal suffix for a number (1st, 2nd, 3rd, 4th, etc.)
        /// </summary>
        private static string GetOrdinalSuffix(int day)
        {
            if (day >= 11 && day <= 13)
                return day + "th";

            return day switch
            {
                1 => day + "st",
                2 => day + "nd",
                3 => day + "rd",
                _ => day + "th"
            };
        }
    }
}
