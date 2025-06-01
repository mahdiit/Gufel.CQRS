using Gufel.Date.Properties;

namespace Gufel.Date
{
    public record VDateSpan
    {
        public int Year { get; set; } = 0;
        public int Month { get; set; } = 0;
        public int Days { get; set; } = 0;

        public override string ToString()
        {
            var result = new List<string>();
            if (Year > 0)
                result.Add($"{Year} {Resources.YearText}");

            if (Month > 0)
                result.Add($"{Month} {Resources.MonthText}");

            if (Days > 0)
                result.Add($"{Days} {Resources.DayText}");

            return string.Join(" " + Resources.Separator + " ", result.ToArray());
        }
    }
}
