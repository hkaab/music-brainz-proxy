using Newtonsoft.Json;

namespace Common.Extensions
{
    public static class Serialization
    {
        public static T FromJson<T>(this string json) where T : class
        {
            if (json == null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
