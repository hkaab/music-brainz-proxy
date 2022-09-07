using Newtonsoft.Json;

namespace Common.Extensions
{
    public static class Serialization
    {
        public static T FromJson<T>(this string json) where T : class
        {
            try
            {
                if (json == null)
                {
                    return default;
                }

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }

        }
    }
}
