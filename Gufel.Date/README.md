# Gufel.Date

Gufel.Date is a flexible and powerful date manipulation library for .NET that provides support for multiple calendar systems, including Persian (Jalali) and Hijri calendars.

## Features

- Multi-calendar support (Persian/Jalali, Hijri, Gregorian)
- Easy conversion between different calendar systems
- Date formatting with custom patterns
- Date comparison and calculations
- Age calculation
- Date span calculations
- Implicit and explicit type conversions
- String parsing

## Installation

Install the package via NuGet:

```bash
dotnet add package Gufel.Date
```

## Usage
### Basic Usage
```csharp
// Create a VDate from current date and time (default is Persian calendar)
var now = VDate.Now;

// Create a VDate from a specific date
var date = new VDate(1404, 3, 11); // Persian date (year, month, day)

// Convert to DateTime
DateTime dateTime = date.ToDateTime();

// Convert from DateTime to VDate
var vDate = new VDate(DateTime.Now);
```

### Working with Different Calendars
```csharp
// Create a date with Hijri calendar
var hijriDate = new VDate(VDateSettingFactory.GetSetting("ar"), DateTime.Now);

// Get year, month, day
Console.WriteLine($"Hijri Year: {hijriDate.Year}");
Console.WriteLine($"Hijri Month: {hijriDate.Month}");
Console.WriteLine($"Hijri Day: {hijriDate.Day}");
```

### Date Formatting
```csharp
var date = new VDate(1404, 3, 11, 14, 10, 23);

// Format as string
string formatted = date.ToString("f"); // Full date/time pattern
// Output: "یکشنبه, 11 خرداد 1404 14:10:23 ب.ظ"

// Custom format
string custom = date.ToString("$yyyy/$MM/$dd");
// Output: "1404/03/11"

### Date Comparison
var date1 = new VDate(1404, 3, 11);
var date2 = new VDate(1404, 3, 12);

bool isEqual = date1 == date2; // false
bool isLess = date1 < date2;   // true
bool isGreater = date1 > date2; // false
```

### Date Parsing
```csharp
// Parse from string
if (VDate.TryParse("1404/03/11", out var parsedDate))
{
    Console.WriteLine($"Year: {parsedDate.Year}, Month: {parsedDate.Month}, Day: {parsedDate.Day}");
}

### Type Conversions
// Convert from integer (YYYYMMDD format)
int dateInt = 14040311;
VDate dateFromInt = dateInt;

// Convert to integer
int intValue = dateFromInt; // 14040311

// Convert to string
string dateString = (string)dateFromInt; // "1404/03/11"

// Convert from string
VDate dateFromString = (VDate)"1404/03/11";
```

## Format Patterns
The ToString(string format) method supports the following patterns:
```code
- $dddd - Day name (e.g., "یکشنبه")
- $dd - Day of month with leading zero (e.g., "01")
- $d - Day of month (e.g., "1")
- $MMMM - Month name (e.g., "خرداد")
- $MM - Month with leading zero (e.g., "03")
- $M - Month (e.g., "3")
- $yyyy - 4-digit year (e.g., "1404")
- $yy - 2-digit year (e.g., "04")
- $HH - Hour with leading zero, 24-hour format (e.g., "14")
- $H - Hour, 24-hour format (e.g., "14")
- $hh - Hour with leading zero, 12-hour format (e.g., "02")
- $h - Hour, 12-hour format (e.g., "2")
- $mm - Minute with leading zero (e.g., "05")
- $m - Minute (e.g., "5")
- $ss - Second with leading zero (e.g., "09")
- $s - Second (e.g., "9")
- $g - AM/PM designator (e.g., "ب.ظ" for PM in Persian)
```
Predefined formats:
```code
- f or F - Full date/time pattern
- s or S - Sortable date/time pattern
```