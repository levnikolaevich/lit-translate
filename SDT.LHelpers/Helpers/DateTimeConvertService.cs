using SDT.Bl.IHelpers;
using NodaTime;
using System.Globalization;

namespace SDT.Bl.Helpers
{
    public class DateTimeConvertService : IDateTimeConvertService
    {        
        private static ZonedDateTime? ConvertToZonedDateTime(DateTime? dateTime, string timeZoneId)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }

            // Получаем часовой пояс
            var timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

            // Преобразуем DateTime? в Instant. Предполагается, что DateTime? в UTC.
            var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc));

            // Преобразуем Instant в ZonedDateTime с использованием заданного часового пояса
            var zonedDateTime = instant.InZone(timeZone);

            return zonedDateTime;
        }

        public DateTime? ConvertDateTimeToUtc(DateTime? date)
        {
            if (date == null)
                return null;

            // If the date is not specified as UTC, convert it to UTC
            if (date.Value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);

            return date;
        }

        public string? FormatStringDate(DateTime? dateTime, string timeZone)
        {
            return dateTime.HasValue
                ? ConvertToZonedDateTime(dateTime.Value, timeZone)?.Date.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                : null;
        }

        public string? FormatStringTime(DateTime? dateTime, string timeZone)
        {
            return dateTime.HasValue
                ? ConvertToZonedDateTime(dateTime.Value, timeZone)?.TimeOfDay.ToString(@"HH\:mm\:ss", CultureInfo.InvariantCulture)
                : null;
        }

        public string FormatStringDateTime(DateTime? dateTime, string timeZone)
        {
            return ConvertToZonedDateTime(dateTime, timeZone)?.ToString("dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public DateTime ConvertStringToUtc(string date, DateTime defaultValue)
        {
            const string dateFormat = "dd-MM-yyyy";

            // Попытка преобразовать строку в дату с учетом конкретного формата "dd-MM-yyyy" и UTC
            if (!DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
            {
                // Если не удалось преобразовать строку в дату, устанавливаем значение по умолчанию
                parsedDate = DateTime.SpecifyKind(defaultValue, DateTimeKind.Utc);
            }
            else
            {
                // Убедитесь, что преобразованная дата также имеет Kind = Utc
                parsedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
            }

            return parsedDate;
        }
    }
}