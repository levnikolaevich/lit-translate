using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

namespace SDT.Helpers.Helpers
{
    public static class JsonSchemaGenerator
    {
        public static bool TryDeserializeJson<T>(string jsonResponse, out List<T> results)
        {
            try
            {
                results = JsonConvert.DeserializeObject<List<T>>(jsonResponse) ?? new List<T>();
                return results.Count > 0;
            }
            catch (JsonException)
            {
                results = new List<T>();
                return false;
            }
        }

        /// <summary>
        /// Генерация JSON схемы для OpenAI API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, object> GenerateAPIOpenAIJsonSchema<T>()
        {
            var schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = new Dictionary<string, Dictionary<string, object>>()
            };

            var requiredFields = new List<string>();

            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var propSchema = new Dictionary<string, object>();

                // Определяем, является ли поле nullable
                bool isNullable = IsNullableType(prop);

                // Определяем тип данных для JSON
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                // Проверка, является ли тип списком (List<T>)
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // Получаем тип элемента внутри списка
                    Type itemType = propType.GetGenericArguments()[0];

                    if (itemType.IsEnum)
                    {
                        // Если элемент Enum, создаём массив строк с возможными значениями
                        var enumValues = Enum.GetNames(itemType);
                        propSchema["type"] = "array";
                        propSchema["items"] = new Dictionary<string, object>
                        {
                            { "type", "string" },
                            { "enum", enumValues }
                        };
                    }
                    else
                    {
                        // Если элемент другого типа, указываем тип массива соответствующих типов
                        propSchema["type"] = "array";
                        propSchema["items"] = new Dictionary<string, object> { { "type", GetJsonType(itemType) } };
                    }

                    // Если поле nullable, то тип должен включать "null"
                    if (isNullable)
                    {
                        propSchema["type"] = new List<string> { "array", "null" };
                    }
                }
                else if (propType.IsEnum)
                {
                    // Если это Enum, добавляем тип "string" или "null" в зависимости от nullable
                    propSchema["type"] = isNullable ? new List<string> { "string", "null" } : "string";

                    // Получаем значения Enum и добавляем их в "enum"
                    var enumValues = Enum.GetNames(propType);
                    propSchema["enum"] = enumValues;
                }
                else
                {
                    // Если не список и не enum, получаем тип данных для JSON
                    string jsonType = GetJsonType(propType);
                    if (isNullable)
                    {
                        propSchema["type"] = new List<string> { jsonType, "null" };
                    }
                    else
                    {
                        propSchema["type"] = jsonType;
                    }
                }

                // Получаем название поля из JsonProperty
                var jsonPropertyAttribute = prop.GetCustomAttribute<JsonPropertyAttribute>();
                string propName = jsonPropertyAttribute?.PropertyName ?? prop.Name;

                // Получаем описание из DescriptionAttribute
                var descriptionAttribute = prop.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    propSchema["description"] = descriptionAttribute.Description;
                }

                requiredFields.Add(propName);

                // Добавляем в schema
                ((Dictionary<string, Dictionary<string, object>>)schema["properties"]).Add(propName, propSchema);
            }

            schema["required"] = requiredFields;
            schema["additionalProperties"] = false;

            return schema;
        }

        // Метод для определения nullable типа
        private static bool IsNullableType(PropertyInfo prop)
        {
            // Проверяем наличие модификатора required
            bool isRequired = prop.CustomAttributes.Any(a => a.AttributeType.Name == "RequiredMemberAttribute");

            // Если поле отмечено как required, то оно не nullable
            if (isRequired)
                return false;

            return !prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null;
        }

        private static string GetJsonType(Type type)
        {
            if (type == typeof(string)) return "string";
            if (type == typeof(int) || type == typeof(int?)) return "integer";
            if (type == typeof(bool) || type == typeof(bool?)) return "boolean";
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
            return "string"; // fallback type
        }
    }
}
