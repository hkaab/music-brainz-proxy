using System;

namespace MusicBrainzApi.Extensions
{
    public static class StringComparisonExtensions
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public static string TryGetValueOr(this string a, string b)
        {
            return !string.IsNullOrEmpty(a) ? a : b;
        }
    }
}