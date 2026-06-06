using System.Text;
using System.Text.RegularExpressions;
using Gufel.Date.Base;
using Gufel.Date.Properties;

namespace Gufel.Date
{
    public sealed partial record VDate : IComparable<VDate>
    {
        private int _year, _day, _month, _hour, _minute, _seconds, _milliSecond;
        public VDateSetting Setting { get; }

        private void SetDate(DateTime dt)
        {
            _year = Setting.CurrentCalendar.GetYear(dt);
            _month = Setting.CurrentCalendar.GetMonth(dt);
            _day = Setting.CurrentCalendar.GetDayOfMonth(dt);
            DayOfWeek = (int)Setting.CurrentCalendar.GetDayOfWeek(dt);
            _hour = dt.Hour;
            _minute = dt.Minute;
            _seconds = dt.Second;
            MilliSeconds = dt.Millisecond;
        }
        private static void ChangePropertyByValue(ref int value, int propertyUnit, ref int property)
        {
            while (value < 0)
            {
                value += propertyUnit;
                property--;
            }

            while (value >= propertyUnit)
            {
                value -= propertyUnit;
                property++;
            }
        }
        private void CheckYearRange(int year)
        {
            if (year > Setting.MaxYear || year < Setting.MinYear)
            {
                throw new DateException(Resources.InvalidYear);
            }
        }
        private void ChangeYearByMonth(ref int year, ref int month)
        {
            var monthInYear = Setting.CurrentCalendar.GetMonthsInYear(year);
            while (month < 1)
            {
                year--;
                month += monthInYear;
                monthInYear = Setting.CurrentCalendar.GetMonthsInYear(year);
            }

            while (month > monthInYear)
            {
                year++;
                month -= monthInYear;
                monthInYear = Setting.CurrentCalendar.GetMonthsInYear(year);
            }

            CheckYearRange(year);
        }
        private void ChangeMonthByDay(ref int year, ref int month, ref int day)
        {
            var maxDay = Setting.CurrentCalendar.GetDaysInMonth(year, month);
            while (day < 1)
            {
                month--;
                day += maxDay;
                maxDay = Setting.CurrentCalendar.GetDaysInMonth(year, month);
            }

            while (day > maxDay)
            {
                month++;
                day -= maxDay;
                maxDay = Setting.CurrentCalendar.GetDaysInMonth(year, month);
            }
        }
        private void ReCalculateProperties(ref int year, ref int month, ref int day,
            ref int hour, ref int minute, ref int second, ref int millisecond)
        {
            ChangePropertyByValue(ref millisecond, 1000, ref second);
            ChangePropertyByValue(ref second, 60, ref minute);
            ChangePropertyByValue(ref minute, 60, ref hour);
            ChangePropertyByValue(ref hour, 24, ref day);
            ChangeMonthByDay(ref year, ref month, ref day);
            ChangeYearByMonth(ref year, ref month);

            DayOfWeek = (int)Setting.CurrentCalendar.GetDayOfWeek(ToDateTime());
        }


        private string Format(string expression)
        {
            string result;
            switch (expression)
            {
                case "F":
                case "f":
                    result = Format("$dddd, $d $MMMM $yyyy $HH:$mm:$ss $g");
                    break;
                case "S":
                case "s":
                    result = Format("$yyyy/$MM/$dd $HH:$mm:$ss $g");
                    break;
                default:
                    var sb = new StringBuilder(expression);

                    sb.Replace("$dddd", Setting.Days[DayOfWeek]);
                    sb.Replace("$MMMM", Setting.Months[Month - 1]);
                    sb.Replace("$yyyy", Year.ToString());
                    sb.Replace("$yy", TwoDigitYear.ToString());
                    sb.Replace("$HH", Hour.ToString("D2"));
                    sb.Replace("$hh", Hour12.ToString("D2"));
                    sb.Replace("$mm", Minute.ToString("D2"));
                    sb.Replace("$ss", Seconds.ToString("D2"));
                    sb.Replace("$dd", Day.ToString("D2"));
                    sb.Replace("$MM", Month.ToString("D2"));
                    sb.Replace("$d", Day.ToString());
                    sb.Replace("$M", Month.ToString());
                    sb.Replace("$H", Hour.ToString());
                    sb.Replace("$h", Hour12.ToString());
                    sb.Replace("$m", Minute.ToString());
                    sb.Replace("$s", Seconds.ToString());
                    sb.Replace("$g", Daylight);

                    result = sb.ToString();
                    break;
            }

            return Setting.FormatString(result);
        }

        #region Constructors
        public VDate(int year, int month, int day)
            : this(year, month, day, 0, 0, 0, 0, VDateSettingFactory.GetSetting("fa"))
        {
        }
        public VDate(int year, int month, int day, VDateSetting setting)
            : this(year, month, day, 0, 0, 0, 0, setting)
        {
        }
        public VDate(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, VDateSettingFactory.GetSetting("fa"))
        {
        }
        public VDate(int year, int month, int day, int hour, int minute, int second, int milliSecond, VDateSetting setting)
        {
            Setting = setting;
            _year = year;
            _month = month;
            _day = day;
            _hour = hour;
            _minute = minute;
            _seconds = second;
            _milliSecond = milliSecond;

            ReCalculateProperties(
                ref _year,
                ref _month,
                ref _day,
                ref _hour,
                ref _minute,
                ref _seconds,
                ref _milliSecond);
        }
        public VDate()
            : this(VDateSettingFactory.GetSetting("fa"), DateTime.Now)
        {
        }
        public VDate(DateTime dt)
            : this(VDateSettingFactory.GetSetting("fa"), dt)
        {
        }
        public VDate(VDateSetting setting)
            : this(setting, DateTime.Now)
        {
        }
        public VDate(VDateSetting setting, DateTime dt)
        {
            Setting = setting;
            SetDate(dt);
        }
        #endregion

        #region Public Properties

        private bool IsLastDayOfMonth { get; set; }
        public int Day
        {
            get => _day;
            set
            {
                IsLastDayOfMonth = Setting.CurrentCalendar.GetDaysInMonth(Year, Month) == Day;
                _day = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
            }
        }
        public int DayOfWeek { get; private set; }

        public int Month
        {
            get => _month;
            set
            {
                _month = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
                IsLastDayOfMonth = Setting.CurrentCalendar.GetDaysInMonth(Year, Month) == Day;
            }
        }
        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
                IsLastDayOfMonth = Setting.CurrentCalendar.GetDaysInMonth(Year, Month) == Day;
            }
        }
        public int Hour
        {
            get => _hour;
            set
            {
                _hour = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
            }
        }
        public int Hour12
        {
            get
            {
                if (_hour > 12)
                    return _hour - 12;
                else
                    return _hour;
            }
        }
        public int Minute
        {
            get => _minute;
            set
            {
                _minute = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
            }
        }
        public int Seconds
        {
            get => _seconds;
            set
            {
                _seconds = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
            }
        }
        public int MilliSeconds
        {
            get => _milliSecond;
            set
            {
                _milliSecond = value;
                ReCalculateProperties(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milliSecond);
            }
        }
        public bool IsLeapYear => Setting.CurrentCalendar.IsLeapYear(Year);
        public bool IsLeapMonth => Setting.CurrentCalendar.IsLeapMonth(Year, Month);

        public int TwoDigitYear => Year % 100;

        public string Daylight => Hour > 12 ? Setting.Pm : Setting.Am;
        public int MonthLength => Setting.CurrentCalendar.GetDaysInMonth(Year, Month);

        #endregion

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Seconds, MilliSeconds, Setting.CurrentCalendar);
        }

        public override string ToString()
        {
            return Format("s");
        }

        public string ToString(string format)
        {
            return Format(format);
        }

        public static VDate Now => new();
        public static VDate Today => new(DateTime.Today);

        [GeneratedRegex(@"\A(\d{4})[/\-](\d{1,2})[/\-](\d{1,2})\z", RegexOptions.CultureInvariant, 1000)]
        private static partial Regex RegexDate();

        [GeneratedRegex(@"\A(\d{4})[/\-](\d{1,2})[/\-](\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})\z", RegexOptions.CultureInvariant, 1000)]
        private static partial Regex RegexDateTime();

        [GeneratedRegex(@"\A(\d{4})/(\d{1,2})/(\d{1,2})-(\d{1,2}):(\d{1,2})\z", RegexOptions.CultureInvariant, 1000)]
        private static partial Regex RegexDateTimeFactor();

        public static bool TryParse(string dt, out VDate? result)
        {
            result = null;

            if (TryMatch(RegexDate().Match(dt), 3, p => new VDate(p[0], p[1], p[2]), out result)) return true;

            if (TryMatch(RegexDateTime().Match(dt), 6, p => new VDate(p[0], p[1], p[2], p[3], p[4], p[5], 0), out result)) return true;

            if (TryMatch(RegexDateTimeFactor().Match(dt), 5, p => new VDate(p[0], p[1], p[2], p[3], p[4], 0, 0), out result)) return true;

            return false;

            static bool TryMatch(Match match, int groupCount, Func<int[], VDate> constructor, out VDate? date)
            {
                date = null;
                if (!match.Success || match.Groups.Count <= groupCount)
                    return false;

                try
                {
                    var parts = new int[groupCount];
                    for (var i = 0; i < groupCount; i++)
                        parts[i] = int.Parse(match.Groups[i + 1].Value);

                    date = constructor(parts);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static implicit operator int(VDate input)
        {
            return input.Year * 10000 + input.Month * 100 + input.Day;
        }

        public static implicit operator VDate(int input)
        {
            var year = input / 10000;
            var month = (input / 100) % 100;
            var day = input % 100;

            if (year >= 1 && month >= 1 && month <= 12 && day >= 1 && day <= 31)
                return new VDate(year, month, day);

            throw new ArgumentException("Invalid date format int");
        }

        public static implicit operator DateTime(VDate input)
        {
            return input.ToDateTime();
        }

        public static implicit operator VDate(DateTime input)
        {
            return new VDate(input);
        }

        public static explicit operator string(VDate input)
        {
            return input.ToString("$yyyy/$MM/$dd");
        }

        public static explicit operator VDate(string input)
        {
            if (TryParse(input, out var dateResult))
                return dateResult!;

            throw new ArgumentException("Invalid date format string");
        }

        public int CompareTo(VDate? other)
        {
            if (other is null) return 1;

            int result;
            if ((result = Year.CompareTo(other.Year)) != 0) return result;
            if ((result = Month.CompareTo(other.Month)) != 0) return result;
            if ((result = Day.CompareTo(other.Day)) != 0) return result;
            if ((result = Hour.CompareTo(other.Hour)) != 0) return result;
            if ((result = Minute.CompareTo(other.Minute)) != 0) return result;
            if ((result = Seconds.CompareTo(other.Seconds)) != 0) return result;
            return MilliSeconds.CompareTo(other.MilliSeconds);
        }

        public static bool operator <(VDate left, VDate right) => left.CompareTo(right) < 0;
        public static bool operator >(VDate left, VDate right) => left.CompareTo(right) > 0;
        public static bool operator <=(VDate left, VDate right) => left.CompareTo(right) <= 0;
        public static bool operator >=(VDate left, VDate right) => left.CompareTo(right) >= 0;
    }
}
