using System.Globalization;
using AutoMapper;
using MD.PersianDateTime;
using Newtonsoft.Json;

namespace JadehRo.Common.Utilities;

public static class AutoMapperTypeConvertors
{

    #region DateTime <=> string
    public class DateNullableToStringTypeConvertor : ITypeConverter<DateTime?, string>
    {
        public string Convert(DateTime? source, string destination, ResolutionContext context)
        {
            if (!source.HasValue) return "";
            var pDate = new PersianDateTime(source).ToString(CultureInfo.InvariantCulture).Split(' ');
            if (pDate[3] == "00:00:00")
                return pDate[0];
            return $"{pDate[3][..5]} {pDate[0]}";


        }
    }

    public class StringToDateNullableTypeConvertor : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source)) return null;
            return PersianDateTime.Parse(source).ToDateTime();
        }
    }
    public class DateToStringTypeConvertor : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            var pDate = new PersianDateTime(source).ToString(CultureInfo.InvariantCulture).Split(' ');
            if (pDate[3] == "00:00:00")
                return pDate[0];
            return $"{pDate[3][..5]} {pDate[0]}";
        }
    }
    public class DateOnlyToStringTypeConvertor : ITypeConverter<DateOnly, string>
    {
        public string Convert(DateOnly source, string destination, ResolutionContext context)
        {
            var date = source.ToDateTime(TimeOnly.Parse("00:00:00"));

            var pDate = new PersianDateTime(date).ToString(CultureInfo.InvariantCulture).Split(' ');

            return pDate[0];
        }
    }
    public class StringToDateTypeConvertor : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source)) return new DateTime();
            return PersianDateTime.Parse(source).ToDateTime();

        }
    }
    #endregion DateTime <=> string

    #region Dictionary <=> String 
    public class DictionaryToStringTypeConvertor : ITypeConverter<Dictionary<string, string>, string>
    {
        public string Convert(Dictionary<string, string> source, string destination, ResolutionContext context)
        {

            return JsonConvert.SerializeObject(source);


        }
    }

    public class StringToDictionaryTypeConvertor : ITypeConverter<string, Dictionary<string, string>>
    {
        public Dictionary<string, string> Convert(string source, Dictionary<string, string> destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source)) return new Dictionary<string, string>();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(source);

        }
    }


    #endregion


    #region  TimeSpan <=> String


    public class TimeSpanToStringTypeConvertor : ITypeConverter<TimeSpan, string>
    {
        public string Convert(TimeSpan source, string destination, ResolutionContext context)
        {
            return $"{source.Hours:D2}:{source.Minutes:D2}";
        }
    }

    public class StringToTimeSpanTypeConvertor : ITypeConverter<string, TimeSpan>
    {
        public TimeSpan Convert(string source, TimeSpan destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source)) source = "00:00";
            //if (source.Length==5)source = source + ":00";
            return TimeSpan.Parse(source);
        }
    }

    public class CalculateAgeConvertor : IValueConverter<DateTime, int>
    {

        public int Convert(DateTime sourceMember, ResolutionContext context)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - sourceMember.Year;

            // Go back to the year the person was born in case of a leap year
            if (sourceMember.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }

    #endregion

    #region Integer <=> List of binaries

    public class IntegerToBinariesConvertor : ITypeConverter<int, IList<int>>
    {
        public IList<int> Convert(int source, IList<int> destination, ResolutionContext context)
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
    }

    public class BinariesToIntegerConvertor : ITypeConverter<IList<int>, int>
    {
        public int Convert(IList<int> source, int destination, ResolutionContext context)
        {
            if (source != null && source.Any())
            {
                return source.Sum();
                // return source.Aggregate(0, (result, id) => result | id);
            }
            return 0;
        }
    }

    #endregion
}