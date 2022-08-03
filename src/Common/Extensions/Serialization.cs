using Newtonsoft.Json;

namespace MusicBrainzApi.Extensions
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
