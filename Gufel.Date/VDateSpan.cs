using Gufel.Date.Properties;

namespace Gufel.Date
{
    public sealed record VDateSpan
    {
        public int Year { get; set; } = 0;
        public int Month { get; set; } = 0;
        public int Days { get; set; } = 0;

        public override string ToString()
        {
            var count = (Year > 0 ? 1 : 0) + (Month > 0 ? 1 : 0) + (Days > 0 ? 1 : 0);
            if (count == 0) return string.Empty;

            var parts = new string[count];
            var idx = 0;
            if (Year > 0) parts[idx++] = $"{Year} {Resources.YearText}";
            if (Month > 0) parts[idx++] = $"{Month} {Resources.MonthText}";
            if (Days > 0) parts[idx++] = $"{Days} {Resources.DayText}";

            return string.Join($" {Resources.Separator} ", parts);
        }
    }
}
