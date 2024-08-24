using Newtonsoft.Json;

namespace CarChanger.Common
{
    internal static class JsonExtensions
    {
        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public static string ToJson<T>(this T obj)
            where T : class
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, SerializerSettings);
        }

        public static T FromJson<T>(this string? json, T defaultValue)
            where T : class
        {
            if (string.IsNullOrEmpty(json)) return defaultValue;

            return JsonConvert.DeserializeObject<T>(json!)!;
        }

        public static void FromJson<T>(this string? json, ref T value)
            where T : class
        {
            if (!string.IsNullOrEmpty(json))
            {
                value = JsonConvert.DeserializeObject<T>(json!)!;
            }
        }
    }
}
