using Gufel.Date.Base;
using System.Globalization;

namespace Gufel.Date.Setting;

public sealed class VDateEnglishSetting() : VDateSetting(new GregorianCalendar())
{
    public override string[] Days => ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

    public override string[] Months =>
    ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
    ];

    public override string Am => "AM";

    public override string Pm => "PM";
}