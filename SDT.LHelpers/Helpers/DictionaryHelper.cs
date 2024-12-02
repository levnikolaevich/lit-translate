namespace SDT.Bl.Helpers
{
    public static class DictionaryHelper
    {
        public static T GetValueOrDefault<T>(Dictionary<string, object?> settings, string key, T defaultValue = default)
        {
            return settings.TryGetValue(key, out var value) && value is T typedValue ? typedValue : defaultValue;
        }
    }
}