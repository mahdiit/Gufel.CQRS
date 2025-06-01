using Gufel.Date.Base;
using System.Globalization;

namespace Gufel.Date.Setting;

public sealed class VDatePersianSetting() : VDateSetting(new PersianCalendar())
{
    public override string[] Days => ["یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"];

    public override string[] Months =>
    ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    ];

    public override string Am => "ق.ظ";

    public override string Pm => "ب.ظ";
}