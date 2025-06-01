using System.Globalization;

namespace Gufel.Date.Base
{
    public interface IVDateSetting
    {
        string[] Days { get; }
        string[] Months { get; }
        string Am { get; }
        string Pm { get; }

        Calendar CurrentCalendar { get; }
        int MaxYear { get; }
        int MaxMonth { get; }
        int MaxDay { get; }
        int MinYear { get; }
        int MinMonth { get; }
        int MinDay { get; }

        Func<string, string> FormatString { get; }
    }
}
