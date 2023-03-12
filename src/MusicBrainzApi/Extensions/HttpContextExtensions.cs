using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace MusicBrainzApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static string MyDotUsername(this HttpContext context)
        {
            return context.GetFromRequest(s => s.Username);
        }

        public static Guid CdfGuid(this HttpContext context)
        {
            return context.GetFromRequest(s => s.CdfGuid);
        }

        public static Guid IdsUserGuid(this HttpContext context)
        {
            return context.GetFromRequest(s => s.IdsUserGuid);
        }

        public static string IdsUserId(this HttpContext context)
        {
            return context.GetFromRequest(s => s.IdsUserId);
        }

        public static int UserId(this HttpContext context)
        {
            return context.GetFromRequest(s => s.UserId);
        }

        public static IReadOnlyCollection<string> Roles(this HttpContext context)
        {
            return context.GetFromRequest(s => s.Roles);
        }

        public static bool IsServiceRequest(this HttpContext context)
        {
            return context.GetFromRequest(s => s.IsServiceRequest);
        }


        public static bool MatchesPath(this HttpContext context, string pathToCompareTo)
        {
            return context.Request.Path.ToUriComponent().EndsWith(pathToCompareTo, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsPath(this HttpContext context, string pathToCompareTo)
        {
            return context.Request.Path.ToUriComponent().Contains(pathToCompareTo);
        }
    }
}