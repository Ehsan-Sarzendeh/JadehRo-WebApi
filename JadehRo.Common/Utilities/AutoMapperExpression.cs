using MD.PersianDateTime;
using System.Globalization;

namespace JadehRo.Common.Utilities;


public static class AutoMapperExpression
{
    public static string ToStringDateTime(this DateTime source)
    {
        var pDate = new PersianDateTime(source).ToString(CultureInfo.InvariantCulture).Split(' ');
        return pDate[3] == "00:00:00" ? pDate[0] : $"{pDate[3][..5]} {pDate[0]}";
    }

    public static string ToStringDate(this DateOnly source)
    {
        var date = source.ToDateTime(TimeOnly.Parse("00:00:00"));
        var pDate = new PersianDateTime(date).ToString(CultureInfo.InvariantCulture).Split(' ');
        return pDate[0];
    }

    public static string ToStringTime(this TimeSpan source)
    {
        return $"{source.Hours:D2}:{source.Minutes:D2}";
    }

    public static DateTime ToDateTime(this string source)
    {
        return string.IsNullOrEmpty(source)
            ? new DateTime()
            : PersianDateTime.Parse(source).ToDateTime();
    }

    public static TimeSpan ToTimeSpan(this string source)
    {
        if (string.IsNullOrEmpty(source)) source = "00:00";
        return TimeSpan.Parse(source);
    }

    public static IList<int> ToBinaries(this int source)
    {
        var result = new List<int>();

        while (source > 0)
        {
            var power = (int)Math.Log(source, 2);
            var res = (int)Math.Pow(2, power);

            result.Add(res);

            source -= res;
        }

        return result;
    }

    public static int ToInteger(this IList<int>? source)
    {
        if (source != null && source.Any())
            return source.Sum();
        return 0;
    }
}
