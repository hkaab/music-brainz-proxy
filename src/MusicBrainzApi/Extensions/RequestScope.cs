using System;
using System.Collections.Generic;

namespace MusicBrainzApi.Extensions
{
    public class RequestScope
    {
        public Guid IdsUserGuid
        {
            get
            {
                if (IsServiceRequest)
                {
                    throw new InvalidOperationException("IdsUserGuid is not available for service requests");
                }

                return Guid.Parse(IdsUserId);
            }
        }

        public string IdsUserId { get; set; }

        public Guid CdfGuid { get; set; }

        public string Username { get; set; }


        public bool IsServiceRequest { get; set; }


        public int UserId { get; set; }

        public IReadOnlyCollection<string> Roles { get; set; }

    }
}
