using System.Globalization;

namespace Gufel.Date.Base;

public abstract class VDateSetting : IVDateSetting
{
    protected VDateSetting(Calendar currentCalendar)
    {
        CurrentCalendar = currentCalendar;
        LoadCalendarValues();
    }

    public abstract string[] Days { get; }
    public abstract string[] Months { get; }
    public abstract string Am { get; }
    public abstract string Pm { get; }

    public Calendar CurrentCalendar { get; protected set; }
    public int MaxYear { get; protected set; }
    public int MaxMonth { get; protected set; }
    public int MaxDay { get; protected set; }
    public int MinYear { get; protected set; }
    public int MinMonth { get; protected set; }
    public int MinDay { get; protected set; }

    public virtual Func<string, string> FormatString => (str) => str;

    protected void LoadCalendarValues()
    {
        var dt = CurrentCalendar.MaxSupportedDateTime;
        MaxYear = CurrentCalendar.GetYear(dt);
        MaxMonth = CurrentCalendar.GetMonth(dt);
        MaxDay = CurrentCalendar.GetDayOfMonth(dt);

        dt = CurrentCalendar.MinSupportedDateTime;
        MinYear = CurrentCalendar.GetYear(dt);
        MinMonth = CurrentCalendar.GetMonth(dt);
        MinDay = CurrentCalendar.GetDayOfMonth(dt);
    }
}