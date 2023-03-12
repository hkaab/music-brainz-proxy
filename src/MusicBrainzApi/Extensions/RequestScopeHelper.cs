using System;
using Microsoft.AspNetCore.Http;

namespace MusicBrainzApi.Extensions
{
    public static class RequestScopeHelper
    {
        private const string Key = "RequestScope";

        public static void SaveToRequestScope(this HttpContext httpContext, Action<RequestScope> update)
        {
            if (httpContext.Items[Key] is RequestScope requestScope)
            {
                update(requestScope);
            }
            else
            {
                var reqSc = new RequestScope();
                update(reqSc);
                httpContext.Items.Add(Key, reqSc);
            }
        }

        public static T GetFromRequest<T>(this HttpContext httpContext, Func<RequestScope, T> getFunc)
        {
            if (httpContext != null && httpContext.Items[Key] is RequestScope rs)
            {
                return getFunc(rs);
            }

            return default;
        }
    }
}