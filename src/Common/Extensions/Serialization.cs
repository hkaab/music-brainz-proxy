using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

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
                    return default(T);
                }

                JObject @object = JObject.Parse(json);
                var schema = GenerateJsonSchema<T>();
                if (@object.IsValid(schema))
                    return JsonConvert.DeserializeObject<T>(json);
                else
                    return default(T);
            }
            catch
            {
                return default(T);
            }

        }

        private static JSchema GenerateJsonSchema<T>() where T : class
        {
            JSchemaGenerator generator = new JSchemaGenerator();

            JSchema schema = generator.Generate(typeof(T));
            return schema;
        }
    }
}
