namespace MenadzerWydarzen
{
    internal static class WydarzenieHelper
    {
        internal static TimeSpan? ParseTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var clean = text.Replace(":", "").Trim();
            if (clean.Length <= 2 && int.TryParse(clean, out int hour))
                return new TimeSpan(hour, 0, 0);
            if (clean.Length == 4
                && int.TryParse(clean[..2], out int h)
                && int.TryParse(clean[2..], out int m))
                return new TimeSpan(h, m, 0);
            return null;
        }

        internal static string? NullIfBlank(string? s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
    }
}
