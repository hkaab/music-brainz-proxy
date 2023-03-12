using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MusicBrainzApi.Extensions
{
    public static class JsonSanitizer
    {
        public static JToken RemoveFields(this JToken token, params string[] fields)
        {
            if (!(token is JContainer container))
            {
                return token;
            }

            var removeList = new List<JToken>();
            foreach (var el in container.Children())
            {
                if (el is JProperty p && fields.Any(name => name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    removeList.Add(el);
                }

                el.RemoveFields(fields);
            }

            foreach (var el in removeList)
            {
                el.Remove();
            }

            return token;
        }
    }
}