using Gufel.Date.Base;
using System.Globalization;

namespace Gufel.Date.Setting;

public sealed class VDateArabicSetting : VDateSetting
{
    public VDateArabicSetting(int? adjustDate = null) : base(new HijriCalendar())
    {
        Days = ["اِلأَحَّد", "اِلأِثنين", "اِثَّلاثا", "اِلأَربِعا", "اِلخَميس", "اِجُّمعَة", "اِسَّبِت"];
        Months = ["محرم", "صفر", "ربيع الاول", "ربيع الثاني", "جمادي الاول", "جمادي الثاني", "رجب", "شعبان", "رمضان", "شوال", "ذي القعدة", "ذي الحجة"
        ];

        if (adjustDate.HasValue)
            ((HijriCalendar)CurrentCalendar).HijriAdjustment = adjustDate.Value;

    }

    public override string[] Days { get; }

    public override string[] Months { get; }

    public override string Am => "ص";

    public override string Pm => "م";
}