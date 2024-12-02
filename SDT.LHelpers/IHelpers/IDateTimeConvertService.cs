using NodaTime;
using System.Globalization;

namespace SDT.Bl.IHelpers
{
    public interface IDateTimeConvertService
    {
        public DateTime? ConvertDateTimeToUtc(DateTime? date);

        public DateTime ConvertStringToUtc(string date, DateTime defaultValue);

        public string? FormatStringDate(DateTime? dateTime, string timeZone);

        public string? FormatStringTime(DateTime? dateTime, string timeZone);

        public string FormatStringDateTime(DateTime? dateTime, string timeZone);
    }
}
