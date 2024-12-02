using System.ComponentModel;
using System.Reflection;

namespace SDT.Helpers.Helpers
{

    public static class EnumExtensions
    {
        /// <summary>
        /// Получить описание для значения перечисления.
        /// </summary>
        /// <param name="value">Значение перечисления.</param>
        /// <returns>Описание из атрибута Description или "Description Not Found", если атрибут отсутствует.</returns>
        public static string GetEnumDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            var descriptionAttribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute?.Description ?? "Description Not Found";
        }

        /// <summary>
        /// Получить список значений перечисления по указанным целочисленным значениям.
        /// </summary>
        /// <typeparam name="T">Тип перечисления.</typeparam>
        /// <param name="values">Коллекция целочисленных значений.</param>
        /// <returns>Список значений перечисления, соответствующих указанным значениям.</returns>
        public static List<T> GetListByRange<T>(IEnumerable<int> values) where T : struct, Enum
        {
            if (values == null || !values.Any()) return new List<T>();

            var enumValues = Enum.GetValues(typeof(T)).Cast<T>();
            return enumValues.Where(item => values.Contains(Convert.ToInt32(item))).ToList();
        }

        /// <summary>
        /// Преобразует строку значений, разделенных запятыми, в список значений перечисления.
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления.</typeparam>
        /// <param name="input">Строка значений, разделенных запятыми.</param>
        /// <returns>Список значений перечисления.</returns>
        public static List<TEnum> ParseEnumList<TEnum>(this string input) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(input)) return new List<TEnum>();

            return input.Split(',')
                        .Select(value => Enum.TryParse<TEnum>(value.Trim(), out var enumValue) ? enumValue : (TEnum?)null)
                        .Where(enumValue => enumValue.HasValue)
                        .Cast<TEnum>()
                        .ToList();
        }
    }
}
